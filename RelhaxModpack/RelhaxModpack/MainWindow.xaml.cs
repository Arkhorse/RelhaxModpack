﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RelhaxModpack.Windows;
using System.Xml;
using System.Diagnostics;
using Ionic.Zip;

namespace RelhaxModpack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon RelhaxIcon;
        private Stopwatch stopwatch = new Stopwatch();
        
        /// <summary>
        /// Creates the instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TheMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();
            //load please wait thing
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                Message = Translations.GetTranslatedString("loadingTranslations"),
                ProgressMinimum = 0,
                ProgressMaximum = 4
            };
            progressIndicator.Show();
            progressIndicator.UpdateProgress(0);
            //load translation hashes and set default language
            Translations.SetLanguage(Languages.English);
            Translations.LoadTranslations();
            //apply translations to this window
            Translations.LocalizeWindow(this,true);
            //create tray icons and menus
            CreateTray();
            //load and apply modpack settings
            progressIndicator.UpdateProgress(2, Translations.GetTranslatedString("loadingSettings"));
            ModpackSettings.LoadSettings();
            //apply settings to UI elements
            UISettings.LoadSettings(true);
            UISettings.ApplyUIColorSettings(this);
            //check command line settings
            CommandLineSettings.ParseCommandLineConflicts();
            //apply third party settings
            ThirdPartySettings.LoadSettings();
            //verify folder paths
            progressIndicator.UpdateProgress(3, Translations.GetTranslatedString("folderStructure"));
            //verify folder stucture for all folders in the directory
            VerifyApplicationFolderStructure();
            //set the application appData direcotry
            Settings.AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Wargaming.net", "WorldOfTanks");
            if(!Directory.Exists(Settings.AppDataFolder))
            {
                Logging.WriteToLog(string.Format("AppDataFolder does not exist at {0}, creating it",Settings.AppDataFolder),
                    Logfiles.Application,LogLevel.Warning);
                Directory.CreateDirectory(Settings.AppDataFolder);
            }
            //Build application macros TODO

            //check for updates to database and application
            progressIndicator.UpdateProgress(4, Translations.GetTranslatedString("checkForUpdates"));
            CheckForApplicationUpdates();
            CheckForDatabaseUpdates(false, true);
            //dispose of please wait here
            progressIndicator.Close();
            progressIndicator = null;
            Show();
        }

        private void TheMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logging.WriteToLog("Saving settings");
            if (ModpackSettings.SaveSettings())
                Logging.WriteToLog("Settings saved");
            Logging.WriteToLog("Disposing tray icon");
            if(RelhaxIcon != null)
            {
                RelhaxIcon.Dispose();
                RelhaxIcon = null;
            }
        }

        #region Tray code
        private void CreateTray()
        {
            //create base tray icon
            RelhaxIcon = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                Icon = Properties.Resources.modpack_icon,
                Text = Title
            };
            //create menu options
            //RelhaxMenustrip
            System.Windows.Forms.ContextMenuStrip RelhaxMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            //MenuItemRestore
            System.Windows.Forms.ToolStripMenuItem MenuItemRestore = new System.Windows.Forms.ToolStripMenuItem();
            MenuItemRestore.Name = nameof(MenuItemRestore);
            //MenuItemCheckUpdates
            System.Windows.Forms.ToolStripMenuItem MenuItemCheckUpdates = new System.Windows.Forms.ToolStripMenuItem();
            MenuItemCheckUpdates.Name = nameof(MenuItemCheckUpdates);
            //MenuItemAppClose
            System.Windows.Forms.ToolStripMenuItem MenuItemAppClose = new System.Windows.Forms.ToolStripMenuItem();
            MenuItemAppClose.Name = nameof(MenuItemAppClose);
            //build it
            RelhaxMenuStrip.Items.Add(MenuItemRestore);
            RelhaxMenuStrip.Items.Add(MenuItemCheckUpdates);
            RelhaxMenuStrip.Items.Add(MenuItemAppClose);
            RelhaxIcon.ContextMenuStrip = RelhaxMenuStrip;
            //setup the right click option
            RelhaxIcon.MouseClick += OnIconMouseClick;
            //setup each even option
            MenuItemRestore.Click += OnMenuItemRestoreClick;
            MenuItemCheckUpdates.Click += OnMenuClickChekUpdates;
            MenuItemAppClose.Click += OnMenuItemCloseClick;
        }

        private void OnMenuItemCloseClick(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            Close();
        }

        private void OnMenuClickChekUpdates(object sender, EventArgs e)
        {
            //make and show progress indicator
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                Message = Translations.GetTranslatedString("checkForUpdates"),
                ProgressMinimum = 0,
                ProgressMaximum = 1
            };
            progressIndicator.Show();
            CheckForDatabaseUpdates(false,false);
            //clean up progress inicaogr
            progressIndicator.Close();
            progressIndicator = null;
        }

        private void OnMenuItemRestoreClick(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Normal)
                WindowState = WindowState.Normal;
            //https://stackoverflow.com/questions/257587/bring-a-window-to-the-front-in-wpf
            this.Activate();
        }

        private void OnIconMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Right:
                    //apply translations for each sub menu option
                    foreach(System.Windows.Forms.ToolStripMenuItem item in RelhaxIcon.ContextMenuStrip.Items)
                    {
                        item.Text = Translations.GetTranslatedString(item.Name);
                    }
                    break;
                case System.Windows.Forms.MouseButtons.Left:
                    //if the application is not displayed on the screen (minimized, for example), then show it.
                    if (WindowState != WindowState.Normal)
                        WindowState = WindowState.Normal;
                    //https://stackoverflow.com/questions/257587/bring-a-window-to-the-front-in-wpf
                    this.Activate();
                    break;
            }
        }
        #endregion

        #region Update Code
        private void CheckForDatabaseUpdates(bool refreshManagerInfo, bool init)
        {
            Logging.WriteToLog("Checking for database updates");
            //TODO: consider just getting it from online?
            if(refreshManagerInfo)
            {
                //delete the last one and download a new one
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        if (File.Exists(Settings.ManagerInfoDatFile))
                            File.Delete(Settings.ManagerInfoDatFile);
                        client.DownloadFile("http://wotmods.relhaxmodpack.com/RelhaxModpack/managerInfo.dat", Settings.ManagerInfoDatFile);

                    }
                    catch (Exception e)
                    {
                        Logging.WriteToLog(string.Format("Failed to check for updates: \n{0}", e), Logfiles.Application, LogLevel.ApplicationHalt);
                        Application.Current.Shutdown();
                        Close();
                        return;
                    }
                }
            }
            //get the version info string
            string xmlString = Utils.GetStringFromZip(Settings.ManagerInfoDatFile, "manager_version.xml");
            if (string.IsNullOrEmpty(xmlString))
            {
                Logging.WriteToLog("Failed to get get xml string from managerInfo.dat", Logfiles.Application, LogLevel.ApplicationHalt);
                return;
            }
            //load the document info
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            //get new DB update version
            string databaseNewVersion = XMLUtils.GetXMLStringFromXPath(doc, "//version/database");
            if(init)
            {
                //auto apply and don't annouce
                Settings.DatabaseVersion = databaseNewVersion;
            }
            else
            {
                Logging.WriteToLog(string.Format("Comparing database versions, old={0}, new={1}", Settings.DatabaseVersion, databaseNewVersion));
                //compare and annouce if not equal
                if (!Settings.DatabaseVersion.Equals(databaseNewVersion))
                {
                    Logging.WriteToLog("new version of database applied");
                    Settings.DatabaseVersion = databaseNewVersion;
                    DatabaseVersionLabel.Text = Translations.GetTranslatedString("databaseVersion") + " " + Settings.DatabaseVersion;
                    MessageBox.Show(Translations.GetTranslatedString("newDBApplied"));
                }
                else
                    Logging.WriteToLog("database versions are the same");
            }
            Logging.WriteToLog("Checking for database updates complete");
        }

        private void CheckForApplicationUpdates()
        {
            //check if skipping updates
            Logging.WriteToLog("Started check for application updates");
            if(CommandLineSettings.SkipUpdate && ModpackSettings.DatabaseDistroVersion != DatabaseVersions.Test)
            {
                MessageBox.Show(Translations.GetTranslatedString("skipUpdateWarning"));
                Logging.WriteToLog("Skipping updates", Logfiles.Application, LogLevel.Warning);
                return;
            }
            //delete the last one and download a new one
            using (WebClient client = new WebClient())
            {
                try
                {
                    if (File.Exists(Settings.ManagerInfoDatFile))
                        File.Delete(Settings.ManagerInfoDatFile);
                    client.DownloadFile("http://wotmods.relhaxmodpack.com/RelhaxModpack/managerInfo.dat", Settings.ManagerInfoDatFile);

                }
                catch (Exception e)
                {
                    Logging.WriteToLog(string.Format("Failed to check for updates: \n{0}", e), Logfiles.Application, LogLevel.ApplicationHalt);
                    Application.Current.Shutdown();
                    Close();
                    return;
                }
            }
            //get the version info string
            string xmlString = Utils.GetStringFromZip(Settings.ManagerInfoDatFile, "manager_version.xml");
            if(string.IsNullOrEmpty(xmlString))
            {
                Logging.WriteToLog("Failed to get get xml string from managerInfo.dat", Logfiles.Application, LogLevel.ApplicationHalt);
                Application.Current.Shutdown();
                Close();
                return;
            }
            //load the document info
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            //if the request distro version is alpha, correct it to stable
            if (ModpackSettings.ApplicationDistroVersion == ApplicationVersions.Alpha)
            {
                Logging.WriteToLog(nameof(ModpackSettings.ApplicationDistroVersion) + "is Alpha, setting to stable for safety",
                    Logfiles.Application, LogLevel.Debug);
                ModpackSettings.ApplicationDistroVersion = ApplicationVersions.Stable;
            }
            //make a copy of the curent application version and set it to stable if alphs
            ApplicationVersions version = Settings.ApplicationVersion;
            if (version == ApplicationVersions.Alpha)
            {
                Logging.WriteToLog("temp version of " + nameof(Settings.ApplicationVersion) + " is Alpha, setting to stable for safety",
                    Logfiles.Application, LogLevel.Debug);
                version = ApplicationVersions.Stable;
            }
            //4 possibilities:
            //stable->stable (update check)
            //stable->beta (auto out of date)
            //beta->stable (auto out of date)
            //beta->beta (update check)
            bool outOfDate = false;
            //declare these out hereso the logger can access them
            string applicationBuildVersion = Utils.GetApplicationVersion();
            //if current application build does not equal requestion distribution channel
            if (version != ModpackSettings.ApplicationDistroVersion)
            {
                outOfDate = true;//can assume out of date
                Logging.WriteToLog(string.Format("Current build is {0} ({1}), online build is NA (changing distro version {2}->{3})",
                    applicationBuildVersion, version.ToString(), version.ToString(), ModpackSettings.ApplicationDistroVersion.ToString()));
            }
            else
            {
                //actually compare the bulid of the application of the requested distribution channel
                string applicationOnlineVersion = (ModpackSettings.ApplicationDistroVersion == ApplicationVersions.Stable) ?
                    XMLUtils.GetXMLStringFromXPath(doc, "//version/manager_v2") ://stable
                    XMLUtils.GetXMLStringFromXPath(doc, "//version/manager_beta_v2");//beta
                if (!(applicationBuildVersion.Equals(applicationOnlineVersion)))
                    outOfDate = true;
                Logging.WriteToLog(string.Format("Current build is {0} ({1}), online build is {2} ({3})",
                    applicationBuildVersion, version.ToString(), applicationOnlineVersion, ModpackSettings.ApplicationDistroVersion.ToString()));
            }
            if(!outOfDate)
            {
                Logging.WriteToLog("Application up to date");
                return;
            }
            Logging.WriteToLog("Application is out of date, display update window");
            VersionInfo versionInfo = new VersionInfo();
            versionInfo.ShowDialog();
            if(versionInfo.ConfirmUpdate)
            {
                //check for any other running instances
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                    if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                    {
                        MessageBoxResult result = MessageBox.Show(Translations.GetTranslatedString("closeInstanceRunningForUpdate"), Translations.GetTranslatedString("critical"), MessageBoxButton.OKCancel);
                        if (result != MessageBoxResult.OK)
                        {
                            Logging.WriteToLog("User canceled update, because he does not want to end the parallel running Relhax instance.");
                            Application.Current.Shutdown();
                            Close();
                            return;
                        }
                    }
                    else
                        break;
                }
                using (WebClient client = new WebClient())
                {
                    //start download of new version
                    client.DownloadProgressChanged += OnUpdateDownloadProgresChange;
                    client.DownloadFileCompleted += OnUpdateDownloadCompleted;
                    //set the UI for a download
                    ResetUI();
                    stopwatch.Reset();
                    //check to make sure this window is displayed for progress
                    if (WindowState != WindowState.Normal)
                        WindowState = WindowState.Normal;
                    //download the file
                    string modpackURL = (ModpackSettings.ApplicationDistroVersion == ApplicationVersions.Stable) ?
                        Settings.ApplicationUpdateURL :
                        Settings.ApplicationBetaUpdateURL;
                    //make sure to delte it if it's currently three
                    if (File.Exists(Settings.ApplicationUpdateFileName))
                        File.Delete(Settings.ApplicationUpdateFileName);
                    client.DownloadFileAsync(new Uri(modpackURL), Settings.ApplicationUpdateFileName);
                }
            }
            else
            {
                Logging.WriteToLog("User pressed x or said no");
                Application.Current.Shutdown();
                Close();
                return;
            }
        }

        private void ResetUI()
        {
            ChildProgressBar.Value = ParentProgressBar.Value = TotalProgressBar.Value = 0;
            InstallProgressTextBox.Text = string.Empty;
        }
        
        private void OnUpdateDownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //stop the timer
            stopwatch.Reset();
            if(e.Error != null)
            {
                Logging.WriteToLog("Failed to download application update\n" + e.Error.ToString(), Logfiles.Application, LogLevel.ApplicationHalt);
                MessageBox.Show(Translations.GetTranslatedString("cantDownloadNewVersion"));
                Application.Current.Shutdown();
            }
            //try to extract the update
            try
            {
                using (ZipFile zip = ZipFile.Read(Settings.ApplicationUpdateFileName))
                {
                    zip.ExtractAll(Settings.ApplicationStartupPath);
                }
            }
            catch (ZipException zipex)
            {
                Logging.WriteToLog("Failed to extract update zip file\n" + zipex.ToString(), Logfiles.Application, LogLevel.ApplicationHalt);
                MessageBox.Show(Translations.GetTranslatedString("failedToExtractUpdateArchive"));
                Application.Current.Shutdown();
            }
            //extract the batch script to update the application
            string batchScript = Utils.GetStringFromZip(Settings.ManagerInfoDatFile, "RelicCopyUpdate.txt");
            File.WriteAllText(Settings.RelicBatchUpdateScript, batchScript);
            //try to start the update script
            try
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = Path.Combine(Settings.ApplicationStartupPath,Settings.RelicBatchUpdateScript),
                    Arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1).ToArray())
                };
                Process installUpdate = new Process
                {
                    StartInfo = info
                };
                installUpdate.Start();
            }
            catch (Exception e3)
            {
                Logging.WriteToLog("Failed to start " + Settings.RelicBatchUpdateScript + "\n" + e3.ToString(),
                    Logfiles.Application, LogLevel.ApplicationHalt);
                MessageBox.Show(Translations.GetTranslatedString("cantStartNewApp"));
            }
            Application.Current.Shutdown();
        }

        private void OnUpdateDownloadProgresChange(object sender, DownloadProgressChangedEventArgs e)
        {
            //if it's in instant extraction mode, don't show download progress
            if (ModpackSettings.DownloadInstantExtraction)
                return;
            //if it's not running, start it
            if (!stopwatch.IsRunning)
                stopwatch.Start();
            //set the update progress bar
            ChildProgressBar.Value = e.ProgressPercentage;
            float MBDownloaded = (float)e.BytesReceived / (float)Utils.BYTES_TO_MBYTES;
            float MBTotal = (float)e.TotalBytesToReceive / (float)Utils.BYTES_TO_MBYTES;
            MBDownloaded = (float)Math.Round(MBDownloaded,2);
            MBTotal = (float)Math.Round(MBTotal,2);
            string downloadMessage = string.Format("{0} {1}MB {2} {3}MB", Translations.GetTranslatedString("downloadingUpdate"),
                MBDownloaded, Translations.GetTranslatedString("of"), MBTotal);
            InstallProgressTextBox.Text = downloadMessage;
        }
        #endregion

        #region All the dumb events for all the changing of settings
        private void OnSelectionViewChanged(object sender, RoutedEventArgs e)
        {
            //selection view code for each new view goes here
            if ((bool)SelectionDefault.IsChecked)
                ModpackSettings.ModSelectionView = SelectionView.DefaultV2;
            else if ((bool)SelectionLegacy.IsChecked)
                ModpackSettings.ModSelectionView = SelectionView.Legacy;
        }

        private void OnMulticoreExtractionChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.MulticoreExtraction = (bool)MulticoreExtractionCB.IsChecked;
        }

        private void OnCreateShortcutsChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.CreateShortcuts = (bool)CreateShortcutsCB.IsChecked;
        }

        private void OnSaveUserDataChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.SaveUserData = (bool)SaveUserDataCB.IsChecked;
        }

        private void OnClearWoTCacheChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.ClearCache = (bool)ClearCacheCB.IsChecked;
        }

        private void OnClearLogFilesChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.DeleteLogs = (bool)ClearLogFilesCB.IsChecked;
        }

        private void OnCleanInstallChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.CleanInstallation = (bool)CleanInstallCB.IsChecked;
        }

        private void OnImmidateExtarctionChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.DownloadInstantExtraction = (bool)InstantExtractionCB.IsChecked;
        }

        private void OnShowInstallCompleteWindowChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.ShowInstallCompleteWindow = (bool)ShowInstallCompleteWindowCB.IsChecked;
        }

        private void OnBackupModsChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.BackupModFolder = (bool)BackupModsCB.IsChecked;
        }

        private void OnPreviewLoadingImageChange(object sender, RoutedEventArgs e)
        {
            if ((bool)ThirdGuardsLoadingImageRB.IsChecked)
                ModpackSettings.GIF = LoadingGifs.ThirdGuards;
            else if ((bool)StandardImageRB.IsChecked)
                ModpackSettings.GIF = LoadingGifs.Standard;
        }

        private void OnForceManuelGameDetectionChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.ForceManuel = (bool)ForceManuelGameDetectionCB.IsChecked;
        }

        private void OnInformIfNoNewDatabaseChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.NotifyIfSameDatabase = (bool)NotifyIfSameDatabaseCB.IsChecked;
        }

        private void OnSaveLastInstallChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.SaveLastConfig = (bool)SaveLastInstallCB.IsChecked;
        }

        private void OnUseBetaAppChanged(object sender, RoutedEventArgs e)
        {
            if ((bool)UseBetaApplicationCB.IsChecked)
                ModpackSettings.ApplicationDistroVersion = ApplicationVersions.Beta;
            else if (!(bool)UseBetaApplicationCB.IsChecked)
                ModpackSettings.ApplicationDistroVersion = ApplicationVersions.Stable;
        }

        private void OnUseBetaDatabaseChanged(object sender, RoutedEventArgs e)
        {
            if ((bool)UseBetaDatabaseCB.IsChecked)
                ModpackSettings.DatabaseDistroVersion = DatabaseVersions.Beta;
            else if (!(bool)UseBetaDatabaseCB.IsChecked)
                ModpackSettings.DatabaseDistroVersion = DatabaseVersions.Stable;
        }

        private void OnDefaultBordersV2Changed(object sender, RoutedEventArgs e)
        {
            ModpackSettings.EnableBordersDefaultV2View = (bool)EnableBordersDefaultV2CB.IsChecked;
        }

        private void OnDefaultSelectColorChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.EnableColorChangeDefaultV2View = (bool)EnableColorChangeDefaultV2CB.IsChecked;
        }

        private void OnLegacyBordersChanged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.EnableBordersLegacyView = (bool)EnableBordersLegacyCB.IsChecked;
        }

        private void OnLegacySelectColorChenged(object sender, RoutedEventArgs e)
        {
            ModpackSettings.EnableColorChangeLegacyView = (bool)EnableColorChangeLegacyCB.IsChecked;
        }

        private void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Translations.SetLanguage((Languages)LanguagesSelector.SelectedIndex);
            Translations.LocalizeWindow(this, true);
        }
        #endregion

        private void InstallModpackButton_Click(object sender, RoutedEventArgs e)
        {
            //toggle buttons and reset UI
            ResetUI();
            ToggleUIButtons(false);
            //settings for export mode
            if(ModpackSettings.ExportMode)
            {
                //TODO
            }
            //parse WoT root directory
            Logging.WriteToLog("started looking for WoT root directory", Logfiles.Application, LogLevel.Debug);
            if(!Utils.AutoFindWoTDirectory(ref Settings.WoTDirectory) || ModpackSettings.ForceManuel)
            {
                Logging.WriteToLog("auto detect failed or user requests manual", Logfiles.Application, LogLevel.Debug);
                Microsoft.Win32.OpenFileDialog manualWoTFind = new Microsoft.Win32.OpenFileDialog()
                {
                    InitialDirectory = string.IsNullOrWhiteSpace(Settings.WoTDirectory) ? Settings.ApplicationStartupPath : Settings.WoTDirectory,
                    AddExtension = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Filter = "WorldOfTanks.exe|WorldOfTanks.exe",
                    Multiselect = false,
                    RestoreDirectory = true,
                    ValidateNames = true
                };
                if((bool)manualWoTFind.ShowDialog())
                {
                    Settings.WoTDirectory = manualWoTFind.FileName;
                }
                else
                {
                    Logging.WriteToLog("User Canceled installation");
                    ToggleUIButtons(true);
                    return;
                }
            }
            Settings.WoTDirectory = Path.GetDirectoryName(Settings.WoTDirectory);
            Logging.WriteToLog("Wot root directory parsed as " + Settings.WoTDirectory);
            //check to make sure the application is not in the same directory as the WoT install
            if (Settings.WoTDirectory.Equals(Settings.ApplicationStartupPath))
            {
                //display error and abort
                MessageBox.Show(Translations.GetTranslatedString("moveOutOfTanksLocation"));
                ToggleUIButtons(true);
                return;
            }
            //get the version of tanks in the format
            //of the res_mods version folder i.e. 0.9.17.0.3
            string versionTemp = XMLUtils.GetXMLStringFromXPath(Path.Combine(Settings.WoTDirectory, "version.xml"), "//version.xml/version");
            Settings.WoTClientVersion = versionTemp.Split('#')[0].Trim().Substring(2);
            //determine if current detected version of the game is supported

        }

        private void UninstallModpackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DiagnosticUtilitiesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ViewNewsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DumpColorSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                OverwritePrompt = true,
                RestoreDirectory = true,
                DefaultExt = "xml",
                Title = Translations.GetTranslatedString("ColorDumpSaveFileDialog"),
                Filter = "XML Documents|*.xml"
            };
            bool result = (bool)saveFileDialog.ShowDialog();
            if(result)
            {
                Logging.WriteToLog("Saving color settings dump to " + saveFileDialog.FileName);
                UISettings.DumpAllWindowColorSettingsToFile(saveFileDialog.FileName);
                Logging.WriteToLog("Color settings saved");
            }
        }

        private bool VerifyApplicationFolderStructure()
        {
            Logging.WriteToLog("Verifying folder structure");
            foreach(string s in Settings.FoldersToCheck)
            {
                try
                {
                    if (!Directory.Exists(s))
                        Directory.CreateDirectory(s);
                }
                catch(Exception e)
                {
                    Logging.WriteToLog("Failed to check application folder sturcture\n" + e.ToString(), Logfiles.Application, LogLevel.ApplicationHalt);
                    return false;
                }
            }
            Logging.WriteToLog("Structure verified");
            return true;
        }

        private void ToggleUIButtons(bool toggle)
        {
            List<FrameworkElement> controlsToToggle = Utils.GetAllWindowComponentsLogical(this, false);
            //any to remove here
            if (controlsToToggle.Contains(CancelDownloadButton))
                controlsToToggle.Remove(CancelDownloadButton);
            foreach (FrameworkElement control in controlsToToggle)
            {
                if (control is Button || control is CheckBox || control is RadioButton)
                    control.IsEnabled = toggle;
            }
            //any to include here
            AutoSyncFrequencyTexbox.IsEnabled = toggle;
            AutoSyncSelectionFileTextBox.IsEnabled = toggle;
        }
    }
}
