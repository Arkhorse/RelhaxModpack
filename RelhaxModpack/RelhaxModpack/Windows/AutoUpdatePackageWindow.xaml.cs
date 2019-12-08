﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;
using Ionic.Zip;
using System.Xml;
using RelhaxModpack.DatabaseComponents;
using System.Reflection;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace RelhaxModpack.Windows
{
    /// <summary>
    /// Interaction logic for AutoUpdatePackageWindow.xaml
    /// </summary>
    public partial class AutoUpdatePackageWindow : RelhaxWindow
    {

        /// <summary>
        /// Get or set the list of packages to display in the package update window
        /// </summary>
        public List<DatabasePackage> Packages
        {
            get
            {
                return packages;
            }
            set
            {
                packages.Clear();
                packages.AddRange(value);
            }
        }

        /// <summary>
        /// Gets or sets the current directory where the window will download and upload files to/from
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// The absolute path inside the zip file to the download instructions xml
        /// </summary>
        public const string AutoUpdateDownloadInstructionsXml = "_autoUpdate/download.xml";

        /// <summary>
        /// The absolute path inside the zip file to the file list instructions xml
        /// </summary>
        public const string AutoUpdateFileInstructionsXml = "_autoUpdate/files.xml";

        private List<DatabasePackage> packages = new List<DatabasePackage>();
        private int CurrentUpdateStep = 1;
        private WebClient client = new WebClient();
        private string UpdateOutputDirectory = string.Empty;

        /// <summary>
        /// Create an instance of the AutoUpdatePackageWindow window
        /// </summary>
        public AutoUpdatePackageWindow()
        {
            InitializeComponent();
        }

        private void RelhaxWindow_Loaded(object sender, RoutedEventArgs e)
        {
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            Logging.Editor("Checking if registry key is set for IE11 for this application");
            using (System.Windows.Forms.WebBrowser bro = new System.Windows.Forms.WebBrowser())
                SetRegistryKey(System.Diagnostics.Process.GetCurrentProcess().ProcessName, bro.Version.Major);

            Logging.Editor("Attaching datasources", LogLevel.Debug);
            PackageNamesListbox.ItemsSource = Packages;

            //set autoupdate output directory
            Logging.Editor("Setting update output directory", LogLevel.Debug);
            UpdateOutputDirectory = Path.Combine(WorkingDirectory, "Output");
            if (!Directory.Exists(UpdateOutputDirectory))
                Directory.CreateDirectory(UpdateOutputDirectory);

            //attach logfile reporting
            LogfileTextbox.Clear();
            Logging.OnLoggingUIThreadReport += Logging_OnLoggingUIThreadReport;
        }

        private void Logging_OnLoggingUIThreadReport(string message)
        {
            LogfileTextbox.AppendText(message + Environment.NewLine);
        }

        private void RelhaxWindow_Closed(object sender, EventArgs e)
        {
            client.Dispose();
            client = null;
        }

        private void StartContinueUpdateProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if(PackageNamesListbox.SelectedItems.Count == 0)
            {
                MessageBox.Show("No items selected");
                return;
            }
            ResetUpdateProcessButton.IsEnabled = true;
            StartContinueUpdateProcessButton.IsEnabled = false;
            switch(CurrentUpdateStep)
            {
                case 1:
                    UpdateProcessStep1();
                    break;
                case 2:
                    UpdateProcessStep2();
                    break;
                case 3:
                    UpdateProcessStep3();
                    break;
            }
            StartContinueUpdateProcessButton.IsEnabled = true;
            CurrentUpdateStep++;
            if (CurrentUpdateStep >= 4)
            {
                StartContinueUpdateProcessButton.Content = "Start";
                CurrentUpdateStep = 1;
                ResetUpdateProcessButton.IsEnabled = false;
            }
            else
            {
                StartContinueUpdateProcessButton.Content = string.Format("Continue from step {0}", CurrentUpdateStep);
            }
        }

        private async void UpdateProcessStep1()
        {
            Logging.Editor("Starting update process step 1");

            //check if update directory exists
            Logging.Editor("Checking if {0} exists", LogLevel.Info, WorkingDirectory);
            if (!Directory.Exists(WorkingDirectory))
            {
                Logging.Editor("Does not exist, creating");
                Directory.CreateDirectory(WorkingDirectory);
            }
            else
                Logging.Editor("Exists");

            DatabasePackage package = PackageNamesListbox.SelectedItems[0] as DatabasePackage;

            string downloadDir = Path.Combine(WorkingDirectory, package.PackageName);
            if (!Directory.Exists(downloadDir))
                Directory.CreateDirectory(downloadDir);

            string downloadPathCurrent = Path.Combine(WorkingDirectory, package.PackageName, package.ZipFile);
            package.DownloadInstructions = new DownloadInstructions() { DownloadedDatabaseZipFileLocation = downloadPathCurrent };
            bool downloadNeeded = false;
            if (File.Exists(downloadPathCurrent))
            {
                Logging.Editor("Current filename already exists, hashing for version");
                string hash = await Utils.CreateMD5HashAsync(downloadPathCurrent);
                if (hash.Equals(package.CRC))
                {
                    Logging.Editor("Hash matches, no need to download");
                }
                else
                {
                    Logging.Editor("Hash not match, setting for download");
                    downloadNeeded = true;
                }
            }
            else
                downloadNeeded = true;

            if(downloadNeeded)
            {
                Logging.Editor("Download needed, starting");
                string completeDownloadURL = package.StartAddress.Replace("{onlineFolder}", Settings.WoTModpackOnlineFolderVersion) + package.ZipFile + package.EndAddress;
                await client.DownloadFileTaskAsync(completeDownloadURL, downloadPathCurrent);
                Logging.Editor("Download completed");
                AutoUpdateProgressBar.Value = AutoUpdateProgressBar.Minimum;
            }

            //check inside zip file for download instructions xml file
            using (ZipFile currentZip = new ZipFile(downloadPathCurrent))
            {
                ZipEntry downloadxml = currentZip[AutoUpdateDownloadInstructionsXml];
                ZipEntry filesxml = currentZip[AutoUpdateFileInstructionsXml];
                if (downloadxml == null)
                {
                    Logging.Editor("This zip file does not support auto update, needs xml instructions (download)");
                    return;
                }
                if (filesxml == null)
                {
                    Logging.Editor("This zip file does not support auto update, needs xml instructions (files)");
                    return;
                }
                //extraction in step 1 allows in verbose mode for modifications to be made to the files before step 2
                downloadxml.Extract(Path.Combine(WorkingDirectory, package.PackageName),ExtractExistingFileAction.OverwriteSilently);
                filesxml.Extract(Path.Combine(WorkingDirectory, package.PackageName), ExtractExistingFileAction.OverwriteSilently);
            }

            Logging.Editor("Finished update process step 1");
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            AutoUpdateProgressBar.Maximum = e.TotalBytesToReceive;
            AutoUpdateProgressBar.Minimum = 0;
            AutoUpdateProgressBar.Value = e.BytesReceived;
        }

        private async void UpdateProcessStep2()
        {
            Logging.Editor("Starting update process step 2");

            DatabasePackage package = PackageNamesListbox.SelectedItems[0] as DatabasePackage;
            //parse download instructions xml files
            XmlDocument downloadDocument = XmlUtils.LoadXmlDocument(Path.Combine(WorkingDirectory, package.PackageName, "_autoUpdate", "download.xml"), XmlLoadType.FromFile);

            if (downloadDocument == null)
            {
                Logging.Editor("Failed to parse download xml document");
                return;
            }

            //parse to class objects
            package.DownloadInstructions = ParseDownloadInstructions(downloadDocument, package.DownloadInstructions.DownloadedDatabaseZipFileLocation);

            //get download URL string based on download instructions type
            string directDownloadURL = string.Empty;
            Logging.Editor("Getting download URL");
            switch(package.DownloadInstructions.DownloadType)
            {
                case DownloadTypes.StaticLink:
                    directDownloadURL = package.DownloadInstructions.UpdateURL;
                    break;
                case DownloadTypes.WgMods:
                    directDownloadURL = await GetWGmodsDownloadLink(package.DownloadInstructions.UpdateURL);
                    break;
            }

            //check that download URL is valid
            if (string.IsNullOrWhiteSpace(directDownloadURL))
            {
                Logging.Editor("Download URL is blank", LogLevel.Error);
                return;
            }
            else
                Logging.Editor("Download URL is valid, attempting to download file");

            //create download string and download the file
            string downloadLocation = Path.Combine(WorkingDirectory, package.PackageName, package.DownloadInstructions.DownloadFilename);
            package.DownloadInstructions.DownloadedFileLocation = downloadLocation;

            if (File.Exists(downloadLocation))
                File.Delete(downloadLocation);

            await client.DownloadFileTaskAsync(directDownloadURL, downloadLocation);
            AutoUpdateProgressBar.Value = AutoUpdateProgressBar.Minimum;
            Logging.Editor("File downloaded, finished update process step 2");
        }

        private async void UpdateProcessStep3()
        {
            Logging.Editor("Starting update process step 3: Loading files xml instructions");
            DatabasePackage package = PackageNamesListbox.SelectedItems[0] as DatabasePackage;
            //parse download instructions xml files
            XmlDocument filesDocument = XmlUtils.LoadXmlDocument(Path.Combine(WorkingDirectory, package.PackageName, "_autoUpdate", "files.xml"), XmlLoadType.FromFile);

            if (filesDocument == null)
            {
                Logging.Editor("Failed to parse files xml document");
                return;
            }

            //parse to class objects
            package.UpdateInstructions = ParseUpdateInstructions(filesDocument);

            Logging.Editor("Starting update zip file process");
            bool processUpdate = false;
            switch(package.UpdateInstructions.UpdateType)
            {
                case UpdateTypes.wotmod:
                    processUpdate = await ProcessWotmodUpdate(package);
                    break;
            }

            //output (move) updated zip file if successful creation
            if (processUpdate)
            {
                Logging.Editor("Update process complete, moving new zip to output directory");
                string locationToMoveTo = Path.Combine(UpdateOutputDirectory, Path.GetFileName(package.DownloadInstructions.DownloadedDatabaseZipFileLocation));
                File.Move(package.DownloadInstructions.DownloadedDatabaseZipFileLocation, locationToMoveTo);

                //change the name if the end is in the pattern yyyy-mm-dd.zip
                Logging.Editor("Changing date of filename to today");
                string regexPattern = @"\d\d\d\d[-_]\d\d[-_]\d\d.zip$";
                string currentFileName = Path.GetFileName(locationToMoveTo);
                Logging.Editor("Current filename: {0}", LogLevel.Info, currentFileName);

                if (Regex.IsMatch(currentFileName,regexPattern))
                {
                    string newFileNameMatch = string.Format("{0}.zip", DateTime.Now.ToString("yyyy-MM-dd"));
                    string newFilename = Regex.Replace(currentFileName, regexPattern, newFileNameMatch);
                    Logging.Editor("New filename:     {0}", LogLevel.Info, newFilename);
                    string newFileLocation = Path.Combine(Path.GetDirectoryName(locationToMoveTo), newFilename);
                    if(File.Exists(newFileLocation))
                    {
                        Logging.Editor("File already exists, overwriting",LogLevel.Warning);
                        File.Delete(newFileLocation);
                    }
                    File.Move(locationToMoveTo, newFileLocation);
                    Logging.Editor("New output file exists at {0}", LogLevel.Info, newFileLocation);
                }
                else
                {
                    Logging.Editor("Failed to process new filename (is not correct format?)",LogLevel.Error);
                }
            }

            Logging.Editor("Finished update process step 3");
        }

        private async Task<bool> ProcessWotmodUpdate(DatabasePackage package)
        {
            Logging.Editor("Processing wotmod update");

            DownloadInstructions downloadInstructions = package.DownloadInstructions;
            UpdateInstructions updateInstructions = package.UpdateInstructions;

            //verify that only one wotmod file exists in database file and get crc
            Logging.Editor("Checking for only 1 .wotmod file in the database zip file");
            using (ZipFile databaseZip = new ZipFile(downloadInstructions.DownloadedDatabaseZipFileLocation))
            {
                ZipEntry wotmodEntry = null;
                foreach (ZipEntry entry in databaseZip)
                {
                    if(entry.FileName.Contains(".wotmod"))
                    {
                        Logging.Editor("Found entry {0}", LogLevel.Info, entry.FileName);
                        if (wotmodEntry != null)
                        {
                            Logging.Editor("Entry for wotmod processing already exists and will be overriden!", LogLevel.Error);
                        }
                        wotmodEntry = entry;
                        updateInstructions.WotmodOldFilenameInZip = entry.FileName;
                    }
                }
                updateInstructions.WotmodDatabaseMD5 = await Utils.CreateMD5HashAsync(wotmodEntry.OpenReader());
            }

            //compare md5 of file in database zip to md5 of downloaded file
            updateInstructions.WotmodDownloadedMD5 = await Utils.CreateMD5HashAsync(downloadInstructions.DownloadedFileLocation);
            Logging.Editor("MD5 of downloaded wotmod: {0}", LogLevel.Info, updateInstructions.WotmodDownloadedMD5);
            Logging.Editor("MD5 of database wotmod:   {0}", LogLevel.Info, updateInstructions.WotmodDatabaseMD5);

            if(updateInstructions.WotmodDownloadedMD5.Equals(updateInstructions.WotmodDatabaseMD5))
            {
                Logging.Editor("MD5 files match, no need to update package");
                //return false;
            }

            //update wotmod file in zip
            Logging.Editor("MD5s don't match, updating wotmod in database zip with downloaded one");
            
            using (ZipFile databaseZip = new ZipFile(downloadInstructions.DownloadedDatabaseZipFileLocation))
            {
                //remove current entry and add new entry
                await Task.Run(() =>
                {
                    databaseZip.RemoveEntry(databaseZip[updateInstructions.WotmodOldFilenameInZip]);
                    databaseZip.AddEntry(updateInstructions.WotmodFilenameInZip, File.ReadAllBytes(downloadInstructions.DownloadedFileLocation));
                });

                //process patch instructions
                Logging.Editor("Processing patches");
                int patchesCount = 0;
                Utils.AllowUIToUpdate();
                foreach (PatchUpdate patchUpdate in updateInstructions.PatchUpdates)
                {
                    Logging.Editor("Processing patch {0} of {1}", LogLevel.Info, ++patchesCount, updateInstructions.PatchUpdates.Count);
                    //locate via zip files list regex search
                    //for each found, extract, load, xpath, search, replace, update

                    Utils.AllowUIToUpdate();
                }

                //save zip changes to disk
                Logging.Editor("Saving zip file changes to disk");
                databaseZip.SaveProgress += DatabaseZip_SaveProgress;
                await Task.Run(() =>
                {
                    databaseZip.Save();
                });
            }

            Logging.Editor("Save complete");
            AutoUpdateProgressBar.Value = AutoUpdateProgressBar.Minimum;
            return true;
        }

        private void DatabaseZip_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                AutoUpdateProgressBar.Maximum = e.TotalBytesToTransfer;
                AutoUpdateProgressBar.Minimum = 0;
                AutoUpdateProgressBar.Value = e.BytesTransferred;
            });
        }

        private DownloadInstructions ParseDownloadInstructions(XmlDocument doc, string databaseZipFileDownloadLocation)
        {
            DownloadInstructions instructions = new DownloadInstructions() { DownloadedDatabaseZipFileLocation = databaseZipFileDownloadLocation };
            string formatVersion = doc.DocumentElement.Attributes["formatVersion"].Value;
            instructions.InstructionsVersion = formatVersion;
            switch(formatVersion)
            {
                case "1.0":
                    ParseDownloadInstructionsV1(instructions, doc);
                    break;
            }

            return instructions;
        }

        private DownloadInstructions ParseDownloadInstructionsV1(DownloadInstructions instructions, XmlDocument doc)
        {
            //public string InstructionsVersion { get; set; } (already got)
            //public string ModVersion { get; set; }
            //public string ClientVersion { get; set; }
            //public DownloadTypes DownloadType { get; set; }
            //public string UpdateURL { get; set; }
            foreach(XmlNode node in doc.ChildNodes[1].ChildNodes)
            {
                switch(node.Name)
                {
                    case "ModVersion":
                        instructions.ModVersion = node.InnerText;
                        break;
                    case "ClientVersion":
                        instructions.ClientVersion = node.InnerText;
                        break;
                    case "DownloadType":
                        instructions.DownloadType = (DownloadTypes)Enum.Parse(instructions.DownloadType.GetType(), node.InnerText);
                        break;
                    case "UpdateURL":
                        instructions.UpdateURL = node.InnerText;
                        break;
                    case "DownloadFilename":
                        instructions.DownloadFilename = node.InnerText;
                        break;
                }
            }
            return instructions;
        }

        private UpdateInstructions ParseUpdateInstructions(XmlDocument doc)
        {
            UpdateInstructions instructions = new UpdateInstructions();
            string formatVersion = doc.DocumentElement.Attributes["formatVersion"].Value;
            instructions.InstructionsVersion = formatVersion;
            switch (formatVersion)
            {
                case "1.0":
                    ParseUpdateInstructionsV1(instructions, doc);
                    break;
            }

            return instructions;
        }

        private UpdateInstructions ParseUpdateInstructionsV1(UpdateInstructions instructions, XmlDocument doc)
        {
            //public string InstructionsVersion { get; set; } (already got)
            //public UpdateTypes UpdateType { get; set; }
            //public string WotmodFilenameInZip { get; set; }
            //public string WotmodMD5 { get; set; }
            foreach (XmlNode node in doc.ChildNodes[1].ChildNodes)
            {
                switch (node.Name)
                {
                    case nameof(instructions.WotmodFilenameInZip):
                        instructions.WotmodFilenameInZip = node.InnerText;
                        break;
                    case nameof(instructions.UpdateType):
                        instructions.UpdateType = (UpdateTypes)Enum.Parse(instructions.UpdateType.GetType(), node.InnerText);
                        break;
                    case nameof(instructions.PatchUpdates):
                        instructions.PatchUpdates = ParsePatchUpdates(node);
                        break;
                }
            }
            return instructions;
        }

        private List<PatchUpdate> ParsePatchUpdates(XmlNode patchNodee)
        {
            List<PatchUpdate> patchUpdates = new List<PatchUpdate>();
            foreach(XmlNode node in patchNodee.ChildNodes)
            {
                PatchUpdate patchUpdate = new PatchUpdate();
                foreach(XmlNode patchNode in node.ChildNodes)
                {
                    switch(node.Name)
                    {
                        case nameof(patchUpdate.PatchesToUpdate):
                            patchUpdate.PatchesToUpdate = patchNode.InnerText;
                            break;
                        case nameof(patchUpdate.XPath):
                            patchUpdate.XPath = patchNode.InnerText;
                            break;
                        case nameof(patchUpdate.Search):
                            XmlAttribute singleReturnAttribute = node.Attributes["single"];
                            if(singleReturnAttribute != null)
                            {
                                patchUpdate.SearchReturnFirst = bool.Parse(singleReturnAttribute.InnerText);
                                Logging.Editor("Search single attribute found and processed as {0}", LogLevel.Debug, patchUpdate.SearchReturnFirst);
                            }
                            patchUpdate.Search = patchNode.InnerText;
                            break;
                        case nameof(patchUpdate.Replace):
                            patchUpdate.Replace = patchNode.InnerText;
                            break;
                    }
                }
                patchUpdates.Add(patchUpdate);
            }
            return patchUpdates;
        }

        private async Task<string> GetWGmodsDownloadLink(string wgmodsBaseUrl)
        {
            bool browserLoaded = false;
            int browserLoadHits = 0;
            browser.LoadCompleted += (sendahh, endArgs) =>
            {
                if(++browserLoadHits >= 1)
                    browserLoaded = true;
            };
            
            //https://stackoverflow.com/questions/1298255/how-do-i-suppress-script-errors-when-using-the-wpf-webbrowser-control
            dynamic activeX = this.browser.GetType().InvokeMember("ActiveXInstance",
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, this.browser, new object[] { });
            activeX.Silent = true;
            
            browser.Navigate(wgmodsBaseUrl);

            while (!browserLoaded)
                await Task.Delay(500);

            //get the entire loaded html document as a string
            var doc = browser.Document as mshtml.HTMLDocument;
            string s = doc.body.outerHTML;

            //load string into html document
            //http://blog.olussier.net/2010/03/30/easily-parse-html-documents-in-csharp/
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(s);
            HtmlNode node = document.DocumentNode;

            //attempt to get client version text and download link text
            //https://stackoverflow.com/questions/1390568/how-can-i-match-on-an-attribute-that-contains-a-certain-string
            HtmlNodeCollection clientVersionNode = node.SelectNodes(@"//div[contains(@class, 'ModDetails_label')]");
            string version = string.Empty;
            HtmlNode downloadUrlNode = node.SelectSingleNode(@"//a[contains(@class, 'ModDetails_hidden')]");
            string downloadURL = string.Empty;

            //parse html nodes into string values
            if (clientVersionNode != null && clientVersionNode.Count >= 4)
            {
                HtmlNode nodeTest = clientVersionNode[3];
                HtmlNode versionNode = nodeTest.ChildNodes[0].ChildNodes[1];
                version = versionNode.InnerText;
            }
            if(downloadUrlNode != null)
            {
                downloadURL = downloadUrlNode.Attributes["href"].Value;
            }

            //display to user
            Logging.Editor(string.Format("For client: {0}, download link: {1}",
                string.IsNullOrEmpty(version)? "(null)" : version,
                string.IsNullOrEmpty(downloadURL) ? "(null)" : downloadURL));

            //check for empty string parsed values
            if(string.IsNullOrEmpty(version))
            {
                Logging.Editor("clientVersionNode is incorrect format (count = {0}), possibly HTML did not completely load?", LogLevel.Warning,
                    clientVersionNode == null ? "null" : clientVersionNode.Count.ToString());
            }
            if(string.IsNullOrEmpty(downloadURL))
            {
                Logging.Editor("downloadUrlNode is null, possibly HTML did not completely load?", LogLevel.Error);
                return null;
            }

            return downloadURL;
        }

        private void ResetUpdateProcessButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUpdateProcessButton.IsEnabled = false;
            CurrentUpdateStep = 1;
        }

        private void DetailedChangesWindow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PackageNamesListbox_Selected(object sender, RoutedEventArgs e)
        {
            SelectedPackagesStackPanel.Children.Clear();
            foreach(DatabasePackage package in PackageNamesListbox.SelectedItems)
            {
                TextBlock block = new TextBlock()
                {
                    Text = string.Format("Zipfile: {0}, CRC: {1}, Last Updated: {2}", package.ZipFile, package.CRC, package.Timestamp)
                };
                SelectedPackagesStackPanel.Children.Add(block);
            }
        }

        private void SetRegistryKey(string exeName, int IEVersion)
        {
            //https://weblog.west-wind.com/posts/2011/May/21/Web-Browser-Control-Specifying-the-IE-Version#Using-the-X--UA--Compatible-HTML-Meta-Tag
            //https://stackoverflow.com/questions/17922308/use-latest-version-of-internet-explorer-in-the-webbrowser-control

            int registryToSet = 0;
            int currentRegistryValue = 0;

            if (IEVersion >= 11)
                registryToSet = 11001;
            else if (IEVersion == 10)
                registryToSet = 10001;
            else if (IEVersion == 9)
                registryToSet = 9999;
            else if (IEVersion == 8)
                registryToSet = 8888;
            else
                registryToSet = 7000;

            using (RegistryKey Key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (Key.GetValue(exeName + ".exe") != null)
                {
                    currentRegistryValue = (int)Key.GetValue(exeName + ".exe");
                }
                else
                {
                    currentRegistryValue = -1;
                }

                Logging.Editor("IEVersion: {0} -> RegistryCurrent:{1}, RegistryToSet: {2}", LogLevel.Info, IEVersion, currentRegistryValue, registryToSet);
                if(currentRegistryValue != registryToSet)
                {
                    Logging.Editor("Values are not same, update registry");
                    Key.SetValue(exeName + ".exe", registryToSet, RegistryValueKind.DWord);
                    Logging.Editor("Registry updated");
                }
                else
                {
                    Logging.Editor("Values are same, continue");
                }
            }
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            LogfileTextbox.Clear();
        }

        private void LogfileTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogfileTextbox.ScrollToEnd();
        }
    }
}
