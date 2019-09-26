﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using RelhaxModpack.UIComponents;
using System.IO;
using System.Xml;
using System.Windows;
using Ionic.Zip;
using System.Text.RegularExpressions;
using System.Threading;

namespace RelhaxModpack.InstallerComponents
{
    #region Event stuff
    /// <summary>
    /// Possible points at which the installer can fail
    /// </summary>
    public enum InstallerExitCodes
    {
        /// <summary>
        /// No fail
        /// </summary>
        Success = 0,

        /// <summary>
        /// Error with downloading mods
        /// </summary>
        DownloadModsError,

        /// <summary>
        /// Error with backup of mods to the RelhaxBackup folder
        /// </summary>
        BackupModsError,

        /// <summary>
        /// Error with backing up of user cache data to temporary folder
        /// </summary>
        BackupDataError,

        /// <summary>
        /// Error with clearing WoT app data cache
        /// </summary>
        ClearCacheError,

        /// <summary>
        /// Error with clearing game and mod logs
        /// </summary>
        ClearLogsError,

        /// <summary>
        /// Error with cleaning mods and res_mods folders
        /// </summary>
        CleanModsError,

        /// <summary>
        /// Error with mods extraction/installation
        /// </summary>
        ExtractionError,

        /// <summary>
        /// Error with user mods extraction
        /// </summary>
        UserExtractionError,

        /// <summary>
        /// Error with restoring user cache data from temporary folder
        /// </summary>
        RestoreUserdataError,

        /// <summary>
        /// Error with copying/extracting and unpacking binary xml files
        /// </summary>
        XmlUnpackError,

        /// <summary>
        /// Error with patching configuration files
        /// </summary>
        PatchError,

        /// <summary>
        /// Error with creating shortcuts
        /// </summary>
        ShortcutsError,

        /// <summary>
        /// Error with creating the contour icon atlas files
        /// </summary>
        ContourIconAtlasError,

        /// <summary>
        /// Error with installing fonts (starting the fontReg process)
        /// </summary>
        FontInstallError,

        /// <summary>
        /// Error with deleting old download files from the RelhaxDownloads folder
        /// </summary>
        TrimDownloadCacheError,

        /// <summary>
        /// Error with cleanup of temporary and leftover files
        /// </summary>
        CleanupError,

        /// <summary>
        /// An unknown error has occurred
        /// </summary>
        UnknownError
    }

    /// <summary>
    /// Possible points at which the uninstaller can fail
    /// </summary>
    public enum UninstallerExitCodes
    {
        /// <summary>
        /// Error with getting the file lists (from folder scan and/or from log file)
        /// </summary>
        GettingFilelistError,

        /// <summary>
        /// Error with deleting of files
        /// </summary>
        UninstallError,

        /// <summary>
        /// Error with deleting of empty folders
        /// </summary>
        ProcessingEmptyFolders,

        /// <summary>
        /// Error with cleanup of temporary and leftover files
        /// </summary>
        PerformFinalClearup,

        /// <summary>
        /// No error occurred
        /// </summary>
        Success
    }

    /// <summary>
    /// Event arguments for when the installer finishes or is ended prematurely
    /// </summary>
    public class RelhaxInstallFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// The exit code from the installer thread
        /// </summary>
        public InstallerExitCodes ExitCode;

        /// <summary>
        /// The error message description
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// Reference to list of parsed categories
        /// </summary>
        public List<Category> ParsedCategoryList;

        /// <summary>
        /// Reference to list of parsed dependencies
        /// </summary>
        public List<Dependency> Dependencies;

        /// <summary>
        /// Reference to list of dependencies
        /// </summary>
        public List<DatabasePackage> GlobalDependencies;
    }

    /// <summary>
    /// A wrapper class around the Ionic.Zip.Zipfile with the purpose of saving the thread ID that the zip file belongs to
    /// </summary>
    public class RelhaxZipFile : ZipFile
    {
        /// <summary>
        /// The ID number of the thread that the zip file belongs to
        /// </summary>
        public int ThreadID;

        /// <summary>
        /// Constructor for making a RelhaxZipFile
        /// </summary>
        /// <param name="fileName">The name of the file to send to the base constructor. File must already exist.</param>
        public RelhaxZipFile(string fileName) : base(fileName) { }
    }
    #endregion

    /// <summary>
    /// The install engine is the root component to the entire installation process. It manages install tasks, threading, and resource usage from start to finish.
    /// </summary>
    public class InstallEngine : IDisposable
    {
        #region Instance Variables
        /// <summary>
        /// List of packages that have zip files to install and are enabled (and checked if selectable) and ordered into installGroups
        /// </summary>
        public List<DatabasePackage>[] OrderedPackagesToInstall;

        /// <summary>
        /// List of packages that have zip files to install and are enabled (and checked if selectable)
        /// </summary>
        public List<DatabasePackage> PackagesToInstall;

        /// <summary>
        /// Flat list of all selectable packages
        /// </summary>
        public List<SelectablePackage> FlatListSelectablePackages;

        /// <summary>
        /// List of user packages placed in the RelhaxUserMods folder and selected for installation
        /// </summary>
        public List<SelectablePackage> UserPackagesToInstall;

        //for passing back to application (DO NOT WRITE TO)
        /// <summary>
        /// A reference for the list of parsed categories
        /// </summary>
        public List<Category> ParsedCategoryList;

        /// <summary>
        /// A reference for the list of parsed dependencies
        /// </summary>
        public List<Dependency> Dependencies;

        /// <summary>
        /// A reference for the list of parsed globally installed dependencies
        /// </summary>
        public List<DatabasePackage> GlobalDependencies;

        //names of triggers
        /// <summary>
        /// The event name for starting the contour icon atlas building
        /// </summary>
        public const string TriggerContouricons = "build_contour_icons";

        /// <summary>
        /// The event name for starting the installation of fonts
        /// </summary>
        public const string TriggerInstallFonts = "install_fonts";

        /// <summary>
        /// The event name for starting the creation of shortcuts
        /// </summary>
        public const string TriggerCreateShortcuts = "create_shortcuts";

        /// <summary>
        /// A list of all current trigger event names
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly")]
        public static readonly string[] CompleteTriggerList = new string[]
        {
            TriggerContouricons,
            TriggerInstallFonts,
            TriggerCreateShortcuts
        };

        /// <summary>
        /// A list of all current trigger event objects. For more information, see the trigger class
        /// </summary>
        public static List<Trigger> Triggers = new List<Trigger>
        {
            new Trigger(){ Fired = false, Name = TriggerContouricons,    NumberProcessed = 0, Total = 0, TriggerTask = null },
            new Trigger(){ Fired = false, Name = TriggerInstallFonts,    NumberProcessed = 0, Total = 0, TriggerTask = null },
            new Trigger(){ Fired = false, Name = TriggerCreateShortcuts, NumberProcessed = 0, Total = 0, TriggerTask = null }
        };

        //other
        private Stopwatch InstallStopWatch = new Stopwatch();
        private TimeSpan OldTime;
        private RelhaxInstallFinishedEventArgs InstallFinishedArgs = new RelhaxInstallFinishedEventArgs();
        private IProgress<RelhaxInstallerProgress> Progress = null;
        private RelhaxInstallerProgress Prog = null;
        private string XvmFolderName = string.Empty;
        private Dictionary<string, string> OriginalPatchNames = new Dictionary<string, string>();
        /// <summary>
        /// The token used for handling and checking for cancellation requests
        /// </summary>
        public CancellationToken CancellationToken;

        //async progress reporters
        private RelhaxInstallerProgress ProgPatch = null;
        private RelhaxInstallerProgress ProgShortcuts = null;
        private RelhaxInstallerProgress ProgAtlas = null;
        private RelhaxInstallerProgress ProgFonts = null;

        //tasks
        private Task PatchTask = null;
        private Task CreateShortcutsTask = null;
        private Task[] AtlasTasks = null;
        private Task CreateFontsTask = null;

        //task holder when each one is created and active
        private List<Task> CreatedChildTasks = new List<Task>();
        //flag for if installing or installing
        private bool Installing = true;
        private Task AtlasDisposeTask;

        private object AtlasBuilderLockerObject = new object();
        private object duplicatePatchNameObjectLocker = new object();

        /// <summary>
        /// Flag for if the install engine should honor the user setting or if installing user packages, disable triggers anyways
        /// </summary>
        public bool DisableTriggersForInstall = true;
        #endregion

        /// <summary>
        /// Creates an instance of the installation engine. Properties should be assigned at this step
        /// </summary>
        public InstallEngine()
        {
            //init the install engine
        }

        #region Installer entry points

        /// <summary>
        /// Run an asynchronous installation
        /// </summary>
        /// <param name="progress">The progress reporter object</param>
        /// <returns>A RelhaxInstallFinishedEventArgs object contain installation data for if the installation succeed or ended prematurely</returns>
        public Task<RelhaxInstallFinishedEventArgs> RunInstallationAsync(IProgress<RelhaxInstallerProgress> progress)
        {
            //make the progress report objects
            Prog = new RelhaxInstallerProgress();
            Progress = progress;

            //set flag
            Installing = true;

            Task<RelhaxInstallFinishedEventArgs> task = Task.Run(() =>
            {
                RelhaxInstallFinishedEventArgs t = RunInstallation();

                InstallFinishedArgs = t;
                //use a continue with so that it acts as a way to swallow the exception of the last task not finishing completely
                //note that it will get to this if it completes successfully or error
            }).ContinueWith(taskk =>
            {
                //check for cancellation
                CheckForCancel();

                //stop the log file if it was started
                if (Logging.IsLogOpen(Logfiles.Installer))
                    Logging.DisposeLogging(Logfiles.Installer);

                //check if the task status failed to log it to the installer
                if (taskk.IsFaulted)
                    Logging.Exception(taskk.Exception.ToString());
                return InstallFinishedArgs;
            });
            return task;
        }

        /// <summary>
        /// Run an asynchronous uninstallation
        /// </summary>
        /// <param name="progress">The progress reporter object</param>
        /// <returns>A RelhaxInstallFinishedEventArgs object contain uninstallation data for if the uninstallation succeed or ended prematurely</returns>
        public Task<RelhaxInstallFinishedEventArgs> RunUninstallationAsync(IProgress<RelhaxInstallerProgress> progress)
        {
            //make the progress report object
            Prog = new RelhaxInstallerProgress();
            Progress = progress;

            //set flag
            Installing = false;

            Task<RelhaxInstallFinishedEventArgs> task = Task.Run(() =>
            {
                RelhaxInstallFinishedEventArgs t = RunUninstallation();
                InstallFinishedArgs = t;
            }).ContinueWith(taskk =>
            {
                CheckForCancel();
                if (Logging.IsLogOpen(Logfiles.Uninstaller))
                    Logging.DisposeLogging(Logfiles.Uninstaller);

                //check if the task status failed to log it to the installer
                if (taskk.IsFaulted)
                    Logging.Exception(taskk.Exception.ToString());
                return InstallFinishedArgs;
            });
            return task;
        }

        private RelhaxInstallFinishedEventArgs RunUninstallation()
        {
            Logging.Info("Uninstall process starts on new thread with mode {0}", ModpackSettings.UninstallMode.ToString());

            //by default, set the exitCode to error, therefore it only updates success at the end (if it makes it)
            InstallFinishedArgs.ExitCode = InstallerExitCodes.UnknownError;

            //run the uninstall methods
            bool success = true;
            switch (ModpackSettings.UninstallMode)
            {
                case UninstallModes.Default:
                    success = UninstallModsDefault(true);
                    break;
                case UninstallModes.Quick:
                    success = UninstallModsQuick(true);
                    break;
            }

            if (success)
                InstallFinishedArgs.ExitCode = InstallerExitCodes.Success;
            else
                InstallFinishedArgs.ExitCode = InstallerExitCodes.UnknownError;

            InstallFinishedArgs.ParsedCategoryList = ParsedCategoryList;
            InstallFinishedArgs.Dependencies = Dependencies;
            InstallFinishedArgs.GlobalDependencies = GlobalDependencies;

            return InstallFinishedArgs;
        }
        #endregion

        #region Main Install method
        private RelhaxInstallFinishedEventArgs RunInstallation()
        {
            //rookie mistake checks
            if(OrderedPackagesToInstall == null || OrderedPackagesToInstall.Count() == 0)
                throw new BadMemeException("WOW you really suck at programming");

            Logging.Info("Installation starts now from RunInstallation() in Install Engine");
            //do more stuff here I'm sure like init log files
            //also check enabled just to be safe
            List<SelectablePackage> selectedPackages = FlatListSelectablePackages.Where(package => package.Checked && package.Enabled).ToList();

            //do any list processing here
            if(!DisableTriggersForInstall)
            {
                //process any packages that have triggers
                //reset the internal list first
                foreach (Trigger trig in Triggers)
                {
                    if (trig.Total != 0)
                        trig.Total = 0;
                    if (trig.NumberProcessed != 0)
                        trig.NumberProcessed = 0;
                    trig.Fired = false;
                    trig.TriggerTask = null;
                }
                foreach (DatabasePackage package in PackagesToInstall)
                {
                    if (package.Triggers.Count > 0)
                    {
                        foreach (string triggerFromPackage in package.Triggers)
                        {
                            //in theory, each database package trigger is unique in each package AND in installer
                            Trigger match = Triggers.Find(search => search.Name.ToLower().Equals(triggerFromPackage.ToLower()));
                            if (match == null)
                            {
                                Logging.Debug("trigger match is null (no match!) {0}", triggerFromPackage);
                                continue;
                            }
                            match.Total++;
                        }
                    }
                }
            }

            //process packages with data (cache) for cache backup and restore
            List<SelectablePackage> packagesWithData = selectedPackages.Where(package => package.UserFiles.Count > 0).ToList();

            //and reset the stopwatch
            InstallStopWatch.Restart();

            //step 1 on install: backup user mods
            OldTime = InstallStopWatch.Elapsed;
            //unknown error is last step in ints
            Prog.TotalTotal = (int)InstallerExitCodes.UnknownError;
            Prog.TotalCurrent = 1;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.BackupModsError;
            Prog.InstallStatus = InstallerExitCodes.BackupModsError;
            Progress.Report(Prog);

            Logging.Info("Backup of mods, current install time = 0 msec");
            if (ModpackSettings.BackupModFolder && !ModpackSettings.ExportMode)
            {
                if (! BackupMods())
                {
                    return InstallFinishedArgs;
                }
                Logging.Info(string.Format("Backup of mods complete, took {0} msec",(int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds)));
            }
            else
                Logging.Info("...skipped");

            //step 2: backup data
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent++;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.BackupDataError;
            Prog.InstallStatus = InstallerExitCodes.BackupDataError;
            Progress.Report(Prog);

            Logging.Info(string.Format("Backup of user data, current install time = {0} msec",
                (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.SaveUserData && !ModpackSettings.ExportMode)
            {
                if(!BackupData(packagesWithData))
                {
                    return InstallFinishedArgs;
                }
                Logging.Info("Back of user data complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
            }
            else
                Logging.Info("...skipped");

            //step 3: clear cache
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent++;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.ClearCacheError;
            Prog.InstallStatus = InstallerExitCodes.ClearCacheError;
            Progress.Report(Prog);

            Logging.Info(string.Format("Cleaning of cache folders, current install time = {0} msec",
                (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.ClearCache && !ModpackSettings.ExportMode)
            {
                if(!ClearCache())
                {
                    return InstallFinishedArgs;
                }
                Logging.Info("Cleaning of cache complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
            }
            else
                Logging.Info("...skipped");

            //step 4: clear logs
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent++;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.ClearLogsError;
            Prog.InstallStatus = InstallerExitCodes.ClearLogsError;
            Progress.Report(Prog);

            Logging.Info(string.Format("Cleaning of logs, current install time = {0} msec",
                (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.DeleteLogs && !ModpackSettings.ExportMode)
            {
                if(!ClearLogs())
                {
                    return InstallFinishedArgs;
                }
                Logging.Info("Cleaning of Logs complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
            }
            else
                Logging.Info("...skipped");

            //step 5: clean mods folders
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent++;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.CleanModsError;
            Prog.InstallStatus = InstallerExitCodes.CleanModsError;
            Progress.Report(Prog);

            Logging.Info(string.Format("Cleaning of mods folders, current install time = {0} msec",
                (int)InstallStopWatch.Elapsed.TotalMilliseconds));

            if (ModpackSettings.CleanInstallation || ModpackSettings.ExportMode || ModpackSettings.AutoInstall || !string.IsNullOrEmpty(CommandLineSettings.AutoInstallFileName))
            {
                if (!ClearModsFolders())
                {
                    return InstallFinishedArgs;
                }
                Logging.Info("Cleaning of mods folders complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
            }
            else
                Logging.Info("...skipped");

            //backup the last installed log file
            string logsFilepath = Path.Combine(Settings.WoTDirectory, "logs");
            string backupInstallLogfile = Path.Combine(logsFilepath, Logging.InstallLogFilenameBackup);
            string installLogfile = Path.Combine(logsFilepath, Logging.InstallLogFilename);
            if (!Directory.Exists(logsFilepath))
                Directory.CreateDirectory(logsFilepath);
            if (File.Exists(backupInstallLogfile))
                Utils.FileDelete(backupInstallLogfile);
            if (File.Exists(installLogfile))
                File.Move(installLogfile, backupInstallLogfile);

            //start the logfile for the installer
            Logging.Init(Logfiles.Installer, installLogfile);

            //step 6: extract mods
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent++;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.ExtractionError;
            Prog.InstallStatus = InstallerExitCodes.ExtractionError;
            Progress.Report(Prog);

            Logging.Info(string.Format("Extracting mods, current install time = {0} msec",
                (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            if(!ExtractFilesAsyncSetup())
            {
                return InstallFinishedArgs;
            }
            Logging.Info("Extraction complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));

            //step 7: extract usermods
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent++;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.UserExtractionError;
            Prog.InstallStatus = InstallerExitCodes.UserExtractionError;
            Progress.Report(Prog);

            Logging.Info("Extracting usermods, current install time = {0} msec", (int)InstallStopWatch.Elapsed.TotalMilliseconds);
            Logging.Info("UserPackages to install: {0}", UserPackagesToInstall.Count);
            if(UserPackagesToInstall.Count > 0)
            {
                StringBuilder userModsBuilder = new StringBuilder();
                userModsBuilder.AppendLine("/*   User Mods   */");
                int counter = 0;
                foreach(DatabasePackage userPackage in UserPackagesToInstall)
                {
                    Logging.Info("Extraction started of user zipfile {0}", Path.GetFileName(userPackage.ZipFile));
                    userModsBuilder.AppendLine(string.Format("/*   {0}   */", Path.GetFileName(userPackage.ZipFile)));
                    Unzip(userPackage, 9, userModsBuilder);
                    Logging.Info("Extraction finished of user zipfile {0}", Path.GetFileName(userPackage.ZipFile));
                    Logging.Info("Completed {0} of {1} extractions", ++counter, UserPackagesToInstall.Count);
                }
                Logging.Installer(userModsBuilder.ToString());
            }
            Logging.Info("Extracting usermods complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));

            //if export mode, this is where we stop
            if(ModpackSettings.ExportMode)
            {
                //report to log install is finished
                OldTime = InstallStopWatch.Elapsed;
                InstallFinishedArgs.ExitCode = InstallerExitCodes.Success;
                Prog.InstallStatus = InstallerExitCodes.Success;
                Logging.Info("Export finished, total install time = {0} msec", (int)InstallStopWatch.Elapsed.TotalMilliseconds);
                InstallStopWatch.Stop();
                return InstallFinishedArgs;
            }

            //step 7: restore data
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent++;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.RestoreUserdataError;
            Prog.InstallStatus = InstallerExitCodes.RestoreUserdataError;
            Progress.Report(Prog);

            Logging.Info(string.Format("Restore of user data, current install time = {0} msec", (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.SaveUserData)
            {
                if (packagesWithData.Count > 0)
                {
                    StringBuilder restoreDataBuilder = new StringBuilder();
                    restoreDataBuilder.AppendLine("/*   Restored data   */");
                    if (!RestoreData(packagesWithData, restoreDataBuilder))
                    {
                        return InstallFinishedArgs;
                    }
                    Logging.Installer(restoreDataBuilder.ToString());
                }
                Logging.Info("Restore of data complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
            }
            else
                Logging.Info("...skipped");

            //step 8: unpack xml files
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent++;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.XmlUnpackError;
            Prog.InstallStatus = InstallerExitCodes.XmlUnpackError;
            Prog.ChildTotal = 1;
            Prog.ParrentTotal = 1;
            Prog.ParrentCurrent = Prog.ChildCurrent = 0;
            Prog.Filename = string.Empty;
            Progress.Report(Prog);

            Logging.Info(string.Format("Unpack of xml files, current install time = {0} msec", (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            List<XmlUnpack> xmlUnpacks = MakeXmlUnpackList();
            if (xmlUnpacks.Count > 0)
            {
                //progress
                Prog.ParrentTotal = xmlUnpacks.Count;

                StringBuilder unpackBuilder = new StringBuilder();
                unpackBuilder.AppendLine("/*   Unpack xml files   */");
                foreach (XmlUnpack xmlUnpack in xmlUnpacks)
                {
                    Prog.ParrentCurrent++;
                    Prog.Filename = xmlUnpack.FileName;
                    Progress.Report(Prog);

                    XmlUtils.UnpackXmlFile(xmlUnpack, unpackBuilder);
                }
                Logging.Installer(unpackBuilder.ToString());
                Logging.Info("Unpack of xml files complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
            }
            else
                Logging.Info("...skipped (no XmlUnpack entries parsed");

            //step 9: patch files (async option)
            OldTime = InstallStopWatch.Elapsed;
            Logging.Info(string.Format("Patching of files, current install time = {0} msec",
                (int)InstallStopWatch.Elapsed.TotalMilliseconds));

            //reset Prog for the next async progreses
            Prog.ChildTotal = 1;
            Prog.ParrentTotal = 1;
            Prog.ParrentCurrent = Prog.ChildCurrent = 0;
            Prog.Filename = string.Empty;
            Prog.ParrentCurrentProgress = string.Empty;
            Prog.ChildCurrentProgress = string.Empty;

            List<Patch> pathces = MakePatchList();
            if (pathces.Count > 0)
            {
                //no need to installer log patches, since it's operating on files that already exist
                PatchTask = Task.Run(() =>
                {
                    ProgPatch = CopyProgress(Prog);
                    ProgPatch.ParrentTotal = pathces.Count;
                    ProgPatch.InstallStatus = InstallerExitCodes.PatchError;
                    LockProgress();

                    foreach (Patch patch in pathces)
                    {
                        ProgPatch.Filename = patch.File;
                        ProgPatch.ParrentCurrent++;
                        LockProgress();

                        PatchUtils.RunPatch(patch);
                    }
                    Logging.Info("Patching of files complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
                    ProgPatch.TotalCurrent = (int)InstallerExitCodes.PatchError;
                    InstallFinishedArgs.ExitCode = InstallerExitCodes.PatchError;
                    ProgPatch.InstallStatus = InstallerExitCodes.PatchError;
                    LockProgress();
                    ProgPatch = null;
                });
            }
            else
                Logging.Info("...skipped (no patch entries parsed)");

            //step 10: create shortcuts (async option)
            if(DisableTriggersForInstall)
            {
                CreateShortcuts();
            }

            //step 11: create atlases (async option)
            if (DisableTriggersForInstall)
            {
                CreateAtlases();
            }

            //step 12: install fonts (async operation)
            if (DisableTriggersForInstall)
            {
                InstallFonts();
            }

            //barrier goes here to make sure cleanup is the last thing to do
            List<Task> concurrentTasksAfterMainExtractoin = new List<Task>()
            {
                PatchTask,
                CreateShortcutsTask,
                CreateFontsTask
            };
            if (AtlasTasks != null)
                concurrentTasksAfterMainExtractoin.AddRange(AtlasTasks);

            Task.WaitAll(concurrentTasksAfterMainExtractoin.Where(task => task != null).ToArray());
            Logging.Info("All async operations after extraction complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));

            //step 12: trim download cache folder
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent = (int)InstallerExitCodes.TrimDownloadCacheError;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.TrimDownloadCacheError;
            Prog.InstallStatus = InstallerExitCodes.TrimDownloadCacheError;
            Progress.Report(Prog);

            Logging.Info(string.Format("Trim download cache, current install time = {0} msec", (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            if(ModpackSettings.DeleteCacheFiles)
            {
                if (!TrimDownloadCache())
                {
                    return InstallFinishedArgs;
                }
                Logging.Info("Trim download cache complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
            }
            else
                Logging.Info("...skipped (ModpackSettings.DeleteCacheFiles = false)");


            //step 13: cleanup
            OldTime = InstallStopWatch.Elapsed;
            Prog.TotalCurrent++;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.CleanupError;
            Prog.InstallStatus = InstallerExitCodes.CleanupError;
            Progress.Report(Prog);

            Logging.Info(string.Format("Cleanup, current install time = {0} msec", (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            if(!ModpackSettings.ExportMode)
            {
                if (!Cleanup())
                    return InstallFinishedArgs;
                Logging.Info("Cleanup complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
            }
            else
                Logging.Info("...skipped (ModpackSettings.ExportMode = true)");


            if (!DisableTriggersForInstall)
            {
                //check if any triggers are still running
                foreach (Trigger trigger in Triggers)
                {
                    if (trigger.TriggerTask != null)
                    {
                        Logging.Debug("Start waiting for task {0} to complete at time {1}", trigger.Name, (int)InstallStopWatch.Elapsed.TotalMilliseconds);
                        trigger.TriggerTask.Wait();
                        Logging.Debug("Task {0} finished or was already done at time {1}", trigger.Name, (int)InstallStopWatch.Elapsed.TotalMilliseconds);
                    }
                    else
                        Logging.Debug("Trigger task {0} is null, skipping", trigger.Name);
                }
            }

            //report to log install is finished
            OldTime = InstallStopWatch.Elapsed;
            InstallFinishedArgs.ExitCode = InstallerExitCodes.Success;
            Prog.InstallStatus = InstallerExitCodes.Success;
            Logging.Info("Install finished, total install time = {0} msec", (int)InstallStopWatch.Elapsed.TotalMilliseconds);
            InstallStopWatch.Stop();
            return InstallFinishedArgs;
        }
        #endregion

        #region Main Uninstall methods
        private bool UninstallModsQuick(bool logIt)
        {
            Prog.UninstallStatus = UninstallerExitCodes.GettingFilelistError;
            Progress.Report(Prog);

            //get a list of all files and folders in mods and res_mods
            List<string> ListOfAllItems = new List<string>();
            if (Directory.Exists(Path.Combine(Settings.WoTDirectory, "res_mods")))
                ListOfAllItems.AddRange(Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, "res_mods"), SearchOption.AllDirectories,true).ToList());
            CancellationToken.ThrowIfCancellationRequested();
            if (Directory.Exists(Path.Combine(Settings.WoTDirectory, "mods")))
                ListOfAllItems.AddRange(Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, "mods"), SearchOption.AllDirectories,true).ToList());
            CancellationToken.ThrowIfCancellationRequested();

            //combine with a list of any installer engine created folders
            foreach (string folder in Settings.FoldersToCleanup)
            {
                CancellationToken.ThrowIfCancellationRequested();
                string folderPath = Path.Combine(Settings.WoTDirectory, folder);
                if (Directory.Exists(folderPath))
                    ListOfAllItems.AddRange(Utils.DirectorySearch(folderPath, SearchOption.AllDirectories,true));
            }

            ListOfAllItems.Sort();
            ListOfAllItems.Reverse();

            //split off into files and folders and shortcuts
            List<string> ListOfAllDirectories = ListOfAllItems.Where(item => Directory.Exists(item)).ToList();
            List<string> ListOfAllFiles = ListOfAllItems.Where(item => File.Exists(item)).ToList();

            //start init progress reporting. for uninstall, only use child current, total and filename
            Prog.ChildTotal = ListOfAllItems.Count;
            Prog.ChildCurrent = 0;
            Progress.Report(Prog);

            if(logIt)
            {
                string backupUninstallLogfile = Path.Combine(Settings.WoTDirectory, "logs", Logging.UninstallLogFilenameBackup);
                string uninstallLogfile = Path.Combine(Settings.WoTDirectory, "logs", Logging.UninstallLogFilename);
                if (File.Exists(backupUninstallLogfile))
                    Utils.FileDelete(backupUninstallLogfile);
                if (File.Exists(uninstallLogfile))
                    File.Move(uninstallLogfile, backupUninstallLogfile);

                //create the uninstall logfile and write header info
                if (!Logging.Init(Logfiles.Uninstaller, uninstallLogfile))
                {
                    Logging.Error("Failed to init the uninstall logfile");
                    return false;
                }
                else
                {
                    Logging.WriteToLog(string.Format(@"/*  Date: {0:yyyy-MM-dd HH:mm:ss}  */", DateTime.Now), Logfiles.Uninstaller, LogLevel.Info);
                    Logging.WriteToLog(string.Format("/* Uninstall Method: {0} */", ModpackSettings.UninstallMode.ToString()), Logfiles.Uninstaller, LogLevel.Info);
                    Logging.WriteToLog(@"/*  files and folders deleted  */", Logfiles.Uninstaller, LogLevel.Info);
                }
            }
            CancellationToken.ThrowIfCancellationRequested();

            //delete all files
            bool success = true;
            Prog.UninstallStatus = UninstallerExitCodes.UninstallError;
            foreach(string file in ListOfAllFiles)
            {
                CancellationToken.ThrowIfCancellationRequested();
                Prog.ChildCurrent++;
                Prog.Filename = file;
                Progress.Report(Prog);
                if (!Utils.FileDelete(file))
                    success = false;
                else
                {
                    if(logIt)
                        Logging.WriteToLog(file, Logfiles.Uninstaller, LogLevel.Info);
                }
            }

            //delete all folders
            foreach(string folder in ListOfAllDirectories)
            {
                CancellationToken.ThrowIfCancellationRequested();
                Prog.ChildCurrent++;
                Prog.Filename = folder;
                Progress.Report(Prog);
                if (!Utils.ProcessEmptyDirectories(folder, false))
                    success = false;
                else
                {
                    if (logIt)
                        Logging.WriteToLog(folder, Logfiles.Uninstaller, LogLevel.Info);
                }
            }

            //final wipe of the folders
            Prog.UninstallStatus = UninstallerExitCodes.PerformFinalClearup;
            Progress.Report(Prog);
            if (Directory.Exists(Path.Combine(Settings.WoTDirectory, "res_mods")))
                if (!Utils.DirectoryDelete(Path.Combine(Settings.WoTDirectory, "res_mods"), true))
                    success = false;
            if (Directory.Exists(Path.Combine(Settings.WoTDirectory, "mods")))
                if (!Utils.DirectoryDelete(Path.Combine(Settings.WoTDirectory, "mods"), true))
                    success = false;
            CancellationToken.ThrowIfCancellationRequested();

            //re-create the folders at the end
            Directory.CreateDirectory(Path.Combine(Settings.WoTDirectory, "res_mods", Settings.WoTClientVersion));
            Directory.CreateDirectory(Path.Combine(Settings.WoTDirectory, "mods", Settings.WoTClientVersion));

            //if we are logging, we need to dispose of the uninstall log
            if (logIt)
            {
                Logging.DisposeLogging(Logfiles.Uninstaller);
            }

            if (success)
                Prog.UninstallStatus = UninstallerExitCodes.Success;
            else
                Prog.UninstallStatus = UninstallerExitCodes.UninstallError;

            InstallFinishedArgs.ParsedCategoryList = ParsedCategoryList;
            InstallFinishedArgs.Dependencies = Dependencies;
            InstallFinishedArgs.GlobalDependencies = GlobalDependencies;

            Prog.ChildCurrent = Prog.ChildTotal;
            Progress.Report(Prog);
            return success;
        }

        private bool UninstallModsDefault(bool logIt)
        {
            //setup progress
            Prog.UninstallStatus = UninstallerExitCodes.GettingFilelistError;
            Progress.Report(Prog);

            //get a list of all files and folders in the install log (assuming checking for logfile already done and exists)
            Logging.Debug("creating list of files to delete from reading uninstall logfile");
            string regularlogfilePath = Path.Combine(Settings.WoTDirectory, "logs", Logging.InstallLogFilename);
            string backuplogfilePath = Path.Combine(Settings.WoTDirectory, "logs", Logging.InstallLogFilenameBackup);
            List<string> ListOfAllItems = new List<string>();

            //if the original log exists, then use it
            if(File.Exists(regularlogfilePath))
            {
                Logging.Debug("using regular install log");
                ListOfAllItems.AddRange(File.ReadAllLines(regularlogfilePath));
            }
            //else if the backup exists, then use it
            else if (File.Exists(backuplogfilePath))
            {
                Logging.Warning("regular install log does not exist, but backup does. previous install or uninstall failure?");
                ListOfAllItems.AddRange(File.ReadAllLines(backuplogfilePath));
            }
            //else we can't use one
            else
            {
                Logging.Warning("regular and backup install log files do not exist, first run or used WoT cleaner tool?");
            }

            //combine with a list of all files and folders in mods and res_mods
            Logging.Debug("adding any files in res_mods and mods by scanning the folders if they aren't on the list already");
            if(Directory.Exists(Path.Combine(Settings.WoTDirectory, "res_mods")))
                ListOfAllItems.AddRange(Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, "res_mods"), SearchOption.AllDirectories,true).ToList());
            CancellationToken.ThrowIfCancellationRequested();
            if (Directory.Exists(Path.Combine(Settings.WoTDirectory, "mods")))
                ListOfAllItems.AddRange(Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, "mods"), SearchOption.AllDirectories,true).ToList());
            CancellationToken.ThrowIfCancellationRequested();

            //combine with a list of any installer engine created folders
            Logging.Debug("adding any installer created folders if they exist");
            foreach(string folder in Settings.FoldersToCleanup)
            {
                CancellationToken.ThrowIfCancellationRequested();
                string folderPath = Path.Combine(Settings.WoTDirectory, folder);
                if (Directory.Exists(folderPath))
                {
                    Logging.Debug("adding installer created folder {0}", folder);
                    ListOfAllItems.AddRange(Utils.DirectorySearch(folderPath, SearchOption.AllDirectories, true));
                }
            }

            //merge then sort and then reverse
            Logging.Debug("merging and sorting lists");
            ListOfAllItems = ListOfAllItems.Distinct().ToList();
            ListOfAllItems.Sort();
            ListOfAllItems.Reverse();

            //split off into files and folders and shortcuts
            List<string> ListOfAllDirectories = ListOfAllItems.Where(item => Directory.Exists(item)).ToList();
            List<string> ListOfAllFiles = ListOfAllItems.Where(item => File.Exists(item)).ToList();
            List<string> ListOfAllShortcuts = ListOfAllFiles.Where(file => Path.GetExtension(file).Equals(".lnk")).ToList();
            ListOfAllFiles = ListOfAllFiles.Except(ListOfAllShortcuts).ToList();

            //start init progress reporting. for uninstall, only use child current, total and filename
            Prog.ChildTotal = ListOfAllItems.Count;
            Prog.ChildCurrent = 0;
            Progress.Report(Prog);

            if(logIt)
            {
                //backup old uninstall logfile
                Logging.Debug("backing up old uninstall logfile and creating new");
                string backupUninstallLogfile = Path.Combine(Settings.WoTDirectory, "logs", Logging.UninstallLogFilenameBackup);
                string uninstallLogfile = Path.Combine(Settings.WoTDirectory, "logs", Logging.UninstallLogFilename);
                if (File.Exists(backupUninstallLogfile))
                    Utils.FileDelete(backupUninstallLogfile);
                if (File.Exists(uninstallLogfile))
                    File.Move(uninstallLogfile, backupUninstallLogfile);

                //create the uninstall logfile and write header info
                if (!Logging.Init(Logfiles.Uninstaller, uninstallLogfile))
                {
                    Logging.Error("Failed to init the uninstall logfile");
                }
                else
                {
                    Logging.WriteToLog(string.Format(@"/*  Date: {0:yyyy-MM-dd HH:mm:ss}  */", DateTime.Now), Logfiles.Uninstaller, LogLevel.Info);
                    Logging.WriteToLog(string.Format("/* Uninstall Method: {0} */", ModpackSettings.UninstallMode.ToString()), Logfiles.Uninstaller, LogLevel.Info);
                    Logging.WriteToLog(@"/*  files and folders deleted  */", Logfiles.Uninstaller, LogLevel.Info);
                }
            }
            CancellationToken.ThrowIfCancellationRequested();

            //delete all files (not shortcuts)
            Logging.Debug("deleting all files from list, not including shortcuts");
            bool success = true;
            Prog.UninstallStatus = UninstallerExitCodes.UninstallError;
            foreach (string file in ListOfAllFiles)
            {
                CancellationToken.ThrowIfCancellationRequested();
                Prog.ChildCurrent++;
                Prog.Filename = file;
                Progress.Report(Prog);
                if (File.Exists(file))
                {
                    if (!Utils.FileDelete(file))
                    {
                        success = false;
                        Logging.Error("failed to delete the file {0}", file);
                    }
                    else
                    {
                        if (logIt)
                            Logging.WriteToLog(file, Logfiles.Uninstaller, LogLevel.Info);
                    }
                }
            }

            //deal with shortcuts
            //if settings.createShortcuts, then don't delete them (at least here, for now)
            //otherwise delete them
            if (!ModpackSettings.CreateShortcuts)
            {
                Logging.Debug("Deleting shortcuts");
                foreach (string file in ListOfAllShortcuts)
                {
                    CancellationToken.ThrowIfCancellationRequested();
                    if (File.Exists(file))
                    {
                        if (!Utils.FileDelete(file))
                        {
                            success = false;
                            Logging.Error("failed to delete the file {0}", file);
                        }
                        else
                        {
                            if (logIt)
                                Logging.WriteToLog(file, Logfiles.Uninstaller, LogLevel.Info);
                        }
                    }
                }
            }

            //final wipe of the folders
            Logging.Debug("performing final wipe of res_mods and mods folders");
            Prog.UninstallStatus = UninstallerExitCodes.PerformFinalClearup;
            Progress.Report(Prog);
            if (Directory.Exists(Path.Combine(Settings.WoTDirectory, "res_mods")))
                if (!Utils.DirectoryDelete(Path.Combine(Settings.WoTDirectory, "res_mods"), true))
                    success = false;
            CancellationToken.ThrowIfCancellationRequested();
            if (Directory.Exists(Path.Combine(Settings.WoTDirectory, "mods")))
                if (!Utils.DirectoryDelete(Path.Combine(Settings.WoTDirectory, "mods"), true))
                    success = false;
            CancellationToken.ThrowIfCancellationRequested();

            //delete all empty folders
            Logging.Debug("processing empty folders");
            Prog.UninstallStatus = UninstallerExitCodes.ProcessingEmptyFolders;
            Progress.Report(Prog);
            foreach (string folder in ListOfAllDirectories)
            {
                CancellationToken.ThrowIfCancellationRequested();
                if (Directory.Exists(folder))
                {
                    if (!Utils.ProcessEmptyDirectories(folder, false))
                        success = false;
                    if(logIt)
                        Logging.WriteToLog(folder, Logfiles.Uninstaller, LogLevel.Info);
                }
            }

            //re-create the folders at the end
            Logging.Debug("re-creating res_mods and mods folders including wot version folder number");
            Directory.CreateDirectory(Path.Combine(Settings.WoTDirectory, "res_mods", Settings.WoTClientVersion));
            Directory.CreateDirectory(Path.Combine(Settings.WoTDirectory, "mods", Settings.WoTClientVersion));

            //if we are logging, we need to dispose of the uninstall log
            Logging.Debug("disposing of logging and cleanup");
            if(logIt)
            {
                Logging.DisposeLogging(Logfiles.Uninstaller);
            }

            if (success)
                Prog.UninstallStatus = UninstallerExitCodes.Success;
            else
                Prog.UninstallStatus = UninstallerExitCodes.UninstallError;

            Prog.ChildCurrent = Prog.ChildTotal;
            Progress.Report(Prog);
            return success;
        }
        #endregion

        #region Installer methods
        private bool BackupMods()
        {
            //setup prog reporting
            Prog.ParrentTotal = Prog.ChildTotal = 1;
            Prog.ParrentCurrent = Prog.ChildCurrent = 0;
            Prog.Filename = string.Empty;
            Progress.Report(Prog);

            //check first if the modpack backup directory exists first
            if (!Directory.Exists(Settings.RelhaxModBackupFolderPath))
                Directory.CreateDirectory(Settings.RelhaxModBackupFolderPath);

            //create the directory for this version to backup to
            string zipFileName = string.Format("{0:yyyy-MM-dd-HH-mm-ss}_{1}.zip", DateTime.Now,Settings.WoTClientVersion);
            string zipFileFullPath = Path.Combine(Settings.RelhaxModBackupFolderPath, zipFileName);
            Logging.Debug("started backupMods(), making zipfile {0}", zipFileFullPath);

            //make a zip file of the mods and res_mods and appdata
            using (ZipFile backupZip = new ZipFile(zipFileFullPath))
            {
                //set the compression level to be normal
                backupZip.CompressionLevel = Ionic.Zlib.CompressionLevel.Default;

                //report progress of gettings mods to save into the zip file
                Prog.ParrentCurrent = 0;
                Prog.ParrentTotal = 1;
                Prog.ChildCurrent = 0;
                Prog.ChildTotal = 1;
                Prog.ParrentCurrentProgress = Translations.GetTranslatedString("scanningModsFolders");
                Progress.Report(Prog);

                //get the list of mods to add to the zip
                List<string> filesToAdd = Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, "mods"), SearchOption.AllDirectories, false, "*", 5, 3, false).ToList();
                filesToAdd.AddRange(Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, "res_mods"), SearchOption.AllDirectories, false, "*", 5, 3, false).ToList());

                //add them to the zip. also get the string to be the path in the zip file, meaning that the root path in the zip file starts at "World_of_Tanks"
                foreach(string file in filesToAdd)
                {
                    backupZip.AddFile(file, Path.GetDirectoryName(file.Substring(Settings.WoTDirectory.Length + 1)));
                }

                //clear the list and repeat the process for the appDataFolder
                filesToAdd.Clear();
                filesToAdd.AddRange(Utils.DirectorySearch(Settings.AppDataFolder, SearchOption.AllDirectories, false, "*", 5, 3, false).ToList());
                foreach (string file in filesToAdd)
                {
                    backupZip.AddFile(file, Path.GetDirectoryName(Path.Combine("appData", file.Substring(Settings.AppDataFolder.Length + 1))));
                }

                //save the file. all the time to wait is in this method, so add the event handler here
                backupZip.SaveProgress += BackupZip_SaveProgress;
                Prog.ParrentCurrentProgress = string.Empty;
                backupZip.Save();
            }
            Logging.Debug("finished backupMods()");
            return true;
        }

        private void BackupZip_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            //we only want entry bytes read for report (let's *try* to be efficient here)
            switch (e.EventType)
            {
                case ZipProgressEventType.Saving_EntryBytesRead:
                    Prog.ChildCurrent = (int)e.BytesTransferred;
                    Prog.ChildTotal = (int)e.TotalBytesToTransfer;
                    Progress.Report(Prog);
                    break;
                case ZipProgressEventType.Saving_AfterWriteEntry:
                case ZipProgressEventType.Saving_BeforeWriteEntry:
                    Prog.ParrentCurrent = e.EntriesSaved;
                    Prog.ParrentTotal = e.EntriesTotal;
                    Prog.EntryFilename = e.CurrentEntry.FileName;
                    Progress.Report(Prog);
                    break;
            } 
        }

        private bool BackupData(List<SelectablePackage> packagesWithData)
        {
            //setup progress reporting
            //total is backup data operation (not touched)
            //parent is number of packages with data
            //child is the file being backed up in this package
            //using file parameter
            Prog.ParrentTotal = packagesWithData.Count;

            //setup prog reporting
            Prog.ChildTotal = 1;
            Prog.ParrentCurrent = Prog.ChildCurrent = 0;
            Prog.Filename = string.Empty;
            Progress.Report(Prog);

            foreach (SelectablePackage package in packagesWithData)
            {
                Logging.Info("Backup data of package {0} starting", package.PackageName);
                Prog.ParrentCurrent++;
                Prog.ParrentCurrentProgress = package.NameFormatted;
                Progress.Report(Prog);
                foreach(UserFile files in package.UserFiles)
                {
                    //clear the list of files_saved, just in case
                    files.Files_saved.Clear();

                    //use the search parameter to get the actual files to move
                    //remove the mistake I made over a year ago of the double slashes
                    string searchPattern = files.Pattern.Replace(@"\\", @"\");

                    //legacy compatibility: the path will either start with a macro or with the raw path
                    //examples:
                    //new: {appData}\Roaming\Wargaming.net\WorldOfTanks\xvm\users\*
                    //old: \\mods\\configs\\promod\\artylog\\modCache.json
                    //new: {app}\autoequip.json
                    //old: \res_mods\mods\shared_resources\xvm\res\clanicons\CT\clan\*.png
                    //old: mods\configs\battle_assistant\mod_battle_assistant.txt
                    //they need to be treated differently
                    //string root_directory = "";
                    //string actual_search = "";

                    //build the entire directory path, still including the search
                    char startChar = searchPattern[0];
                    if (startChar.Equals('{'))
                    {
                        //it does not have the macro, so add it. (assume {app} macro)
                        Logging.Debug("pattern starts with \"{\", continue");
                    }
                    else if (startChar.Equals('\\'))
                    {
                        Logging.Debug("pattern starts with \"\\\", adding macro and continue");
                        searchPattern = @"{app}" + searchPattern;
                    }
                    else
                    {
                        Logging.Debug("pattern starts with folder name, adding macro and folder slash and continue");
                        searchPattern = @"{app}\" + searchPattern;
                    }
                    Logging.Debug("path with macro: {0}", searchPattern);

                    //replace the macro to make the complete path
                    searchPattern = Utils.MacroReplace(searchPattern, ReplacementTypes.FilePath);
                    string directorySearchpath = Path.GetDirectoryName(searchPattern);
                    searchPattern = Path.GetFileName(searchPattern);
                    Logging.Debug("search term (or file): {0}", searchPattern);

                    //if the directory to search does not exist, then make it, just in case
                    if (!Directory.Exists(directorySearchpath))
                        Directory.CreateDirectory(directorySearchpath);

                    //get the list of files to replace
                    string[] filesToSave = Utils.DirectorySearch(directorySearchpath, SearchOption.AllDirectories, false, searchPattern, 5, 3, false);

                    //check if we have files to move
                    if(filesToSave.Count() == 0)
                    {
                        Logging.Info("no files found to backup");
                        continue;
                    }

                    //update progress
                    Prog.ChildTotal = filesToSave.Count();

                    //make the temp directory to place the files based on this package
                    string tempFolderPath = Path.Combine(Settings.RelhaxTempFolderPath, package.PackageName);
                    if(!Directory.Exists(tempFolderPath))
                        Directory.CreateDirectory(tempFolderPath);

                    //move each file
                    foreach(string file in filesToSave)
                    {
                        Prog.ChildCurrent++;
                        Prog.Filename = file;
                        Progress.Report(Prog);
                        string destination = Path.Combine(tempFolderPath, Path.GetFileName(file));

                        //check if destinatino exists first before replace
                        if (File.Exists(destination))
                            File.Delete(destination);
                        File.Move(file, destination);
                        files.Files_saved.Add(file);
                    }
                }
                Logging.Info("backup data of {0} finished", package.PackageName);
            }
            return true;
        }

        private bool ClearCache()
        {
            //setup prog reporting
            Prog.ParrentTotal = Prog.ChildTotal = 1;
            Prog.ParrentCurrent = Prog.ChildCurrent = 0;
            Prog.Filename = string.Empty;
            Progress.Report(Prog);

            //make sure that the app data folder exists
            //if it does not, then it does not need to run this
            if (!Directory.Exists(Settings.AppDataFolder))
            {
                Logging.Info("Appdata folder does not exist, creating");
                Directory.CreateDirectory(Settings.AppDataFolder);
                return true;
            }
            Logging.Info("Appdata folder exists, backing up user settings and clearing cache");

            //make the temp folder if it does not already exist
            string AppPathTempFolder = Path.Combine(Settings.RelhaxTempFolderPath, "AppDataBackup");
            //delete if possibly from previous install
            if (Directory.Exists(AppPathTempFolder))
                Utils.DirectoryDelete(AppPathTempFolder, true);
            //and make the folder at the end
            Directory.CreateDirectory(AppPathTempFolder);

            //backup files and folders that should be kept that aren't cache
            string[] fileNames = { "preferences.xml", "preferences_ct.xml", "modsettings.dat" };
            string[] folderNames = { "xvm", "pmod" };

            //check if the directories are files or folders
            //if files they can move directly
            //if folders they have to be re-created on the destination and files moved manually
            Logging.WriteToLog("Starting clearing cache step 1 of 3: backing up old files", Logfiles.Application, LogLevel.Debug);
            foreach(string file in fileNames)
            {
                Logging.WriteToLog("Processing cache file/folder to move: " + file, Logfiles.Application, LogLevel.Debug);
                if(File.Exists(Path.Combine(Settings.AppDataFolder, file)))
                {
                    try { File.Move(Path.Combine(Settings.AppDataFolder, file), Path.Combine(AppPathTempFolder, file)); }
                    catch (Exception ex)
                    {
                        Logging.Exception(ex.ToString());
                        return false;
                    }
                } 
                else
                {
                    Logging.Info("File does not exist in step clearCache: {0}", file);
                }
            }

            foreach(string folder in folderNames)
            {
                if (Directory.Exists(Path.Combine(Settings.AppDataFolder, folder)))
                {
                    Utils.DirectoryMove(Path.Combine(Settings.AppDataFolder, folder), Path.Combine(AppPathTempFolder, folder), true);
                }
                else
                {
                    Logging.Info("Folder does not exist in step clearCache: {0}", folder);
                }
            }

            //now delete the temp folder
            Logging.WriteToLog("Starting clearing cache step 2 of 3: actually clearing cache", Logfiles.Application, LogLevel.Debug);
            Utils.DirectoryDelete(Settings.AppDataFolder, true);

            //then put the above files back
            Logging.WriteToLog("Starting clearing cache step 3 of 3: restoring old files", Logfiles.Application, LogLevel.Debug);
            Directory.CreateDirectory(Settings.AppDataFolder);
            foreach (string file in fileNames)
            {
                Logging.WriteToLog("Processing cache file/folder to move: " + file, Logfiles.Application, LogLevel.Debug);
                if (File.Exists(Path.Combine(AppPathTempFolder, file)))
                {
                    try { File.Move(Path.Combine(AppPathTempFolder, file), Path.Combine(Settings.AppDataFolder, file)); }
                    catch (Exception ex)
                    {
                        Logging.Exception(ex.ToString());
                        return false;
                    }
                }
                else
                {
                    Logging.Info("File does not exist in step clearCache: {0}", file);
                }
            }

            foreach (string folder in folderNames)
            {
                if (Directory.Exists(Path.Combine(AppPathTempFolder, folder)))
                {
                    Utils.DirectoryMove(Path.Combine(AppPathTempFolder, folder), Path.Combine(Settings.AppDataFolder, folder), true);
                }
                else
                {
                    Logging.Info("Folder does not exist in step clearCache: {0}", folder);
                }
            }
            return true;
        }

        private bool ClearLogs()
        {
            //setup prog reporting
            Prog.ParrentTotal = Prog.ChildTotal = 1;
            Prog.ParrentCurrent = Prog.ChildCurrent = 0;
            Prog.Filename = string.Empty;
            Progress.Report(Prog);
            string[] logsToDelete = new string[]
            {
                Path.Combine(Settings.WoTDirectory, "python.log"),
                Path.Combine(Settings.WoTDirectory, "xvm.log"),
                Path.Combine(Settings.WoTDirectory, "pmod.log"),
                Path.Combine(Settings.WoTDirectory, "WoTLauncher.log"),
                Path.Combine(Settings.WoTDirectory, "cef.log")
            };
            Prog.ParrentTotal = logsToDelete.Count();
            Progress.Report(Prog);
            foreach(string s in logsToDelete)
            {
                Logging.WriteToLog("Processing log file (if exists) " + s, Logfiles.Application, LogLevel.Info);
                Prog.ParrentCurrent++;
                Prog.Filename = s;
                Progress.Report(Prog);
                if (File.Exists(s))
                {
                    if (!Utils.FileDelete(s))
                    {
                        Logging.Warning("Unable to delete the logfile {0}", Path.GetFileName(s));
                    }
                }
            }
            return true;
        }

        private bool ClearModsFolders()
        {
            if (ModpackSettings.ExportMode)
            {
                Logging.Info("Running uninstall method quick for export mode");
                return UninstallModsQuick(false);
            }
            else if (ModpackSettings.UninstallMode == UninstallModes.Default)
            {
                Logging.Info("Running uninstall modes method Default");
                return UninstallModsDefault(false);
            }
            else if (ModpackSettings.UninstallMode == UninstallModes.Quick)
            {
                Logging.Info("Running uninstall modes method Quick (Advanced)");
                return UninstallModsQuick(false);
            }
            else
                throw new BadMemeException(":thinking:");
        }

        private bool ExtractFilesAsyncSetup()
        {
            //progress reporting
            Prog.ChildCurrent = Prog.ParrentCurrent = 0;
            Prog.ChildTotal = Prog.ParrentTotal = 1;
            Prog.Filename = string.Empty;
            Progress.Report(Prog);

            //this is the only really new one. for each install group, spawn a bunch of threads to start the install process
            //get the number of threads we will use for each of the install steps
            int numThreads = ModpackSettings.MulticoreExtraction ? Settings.NumLogicalProcesors : 1;
            Prog.TotalThreads = (uint)numThreads;

            Logging.Info(string.Format("Number of threads to use for install is {0}, (MulticoreExtraction={1}, LogicalProcesosrs={2}",
                numThreads, ModpackSettings.MulticoreExtraction, Settings.NumLogicalProcesors));

            //setup progress reporting for parent
            Prog.ParrentTotal = PackagesToInstall.Count();
            Prog.TotalInstallGroups = (uint)OrderedPackagesToInstall.Count();

            for (int i = 0; i < OrderedPackagesToInstall.Count(); i++)
            {
                Logging.Info("Install Group " + i + " starts now");
                Prog.InstallGroup = (uint)i;
                Progress.Report(Prog);

                //clear the list of child tasks
                CreatedChildTasks.Clear();

                //get the list of packages to install
                //this list represents all the packages in this install group that can be installed at the same time
                //i.e. there are NO conflicting zip file paths in ALL of the files (all the entries in all zip files are mutually exclusive)
                List<DatabasePackage> packages = new List<DatabasePackage>(OrderedPackagesToInstall[i]);

                //if a group does not have any packages in it, then we can skip
                Logging.Debug("number of packages in this group: {0}", packages.Count);
                if(packages.Count > 0)
                {
                    //set it for the progress report
                    Prog.CompletedThreads = 0;

                    //get the size of any packages where the size is invalid before sorting
                    foreach (DatabasePackage packa in packages.Where(pack => pack.Size == 0 && !string.IsNullOrWhiteSpace(pack.ZipFile)))
                    {
                        Logging.Debug("package {0} has size 0 and zipfile entry, getting size", packa.PackageName);
                        string zipFile = Path.Combine(Settings.RelhaxDownloadsFolderPath, packa.ZipFile);
                        if (File.Exists(zipFile))
                            packa.Size = (ulong)Utils.GetFilesize(zipFile);
                        Logging.Debug("size parsed to {0}", packa.Size.ToString());
                    }

                    //then sort the packages by the size parameter (largest files on top)
                    //https://stackoverflow.com/questions/3309188/how-to-sort-a-listt-by-a-property-in-the-object
                    packages = packages.OrderByDescending(pack => pack.Size).ToList();
                    //for not just go with the packages as they are, they should already be in alphabetical order

                    //make a list of packages again, but size is based on number of logical processors and/or multi-core install mods
                    //if a user has 8 cores, then make 8 lists of packages to install
                    List<DatabasePackage>[] packageThreads = new List<DatabasePackage>[numThreads];

                    //new up the lists before we can assign to them
                    for (int j = 0; j < packageThreads.Count(); j++)
                    {
                        packageThreads[j] = new List<DatabasePackage>();
                    }

                    //assign each package one at a time into a package thread
                    Logging.Info("starting package assignment to each thread");
                    for (int j = 0; j < packages.Count; j++)
                    {
                        int threadSelector = j % numThreads;
                        packageThreads[threadSelector].Add(packages[j]);
                        Logging.Info("j index = {0}, package {1} has been assigned to packageThread {2}", j, packages[j].PackageName, threadSelector);
                    }

                    //now the fun starts. these all can run at once. yeah.
                    //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming
                    Task[] tasks = new Task[numThreads];
                    if (tasks.Count() != packageThreads.Count())
                        throw new BadMemeException("ohhhhhhhhh, NOW you f*cked UP!");

                    bool valueLocked = false;

                    //start the threads
                    Logging.Debug("Starting {0} threads", tasks.Count());
                    //setup progress reporting for threads
                    Prog.CompletedPackagesOfAThread = new uint[tasks.Count()];
                    Prog.TotalPackagesofAThread = new uint[tasks.Count()];
                    Prog.EntriesTotalOfAThread = new uint[tasks.Count()];
                    Prog.EntriesProcessedOfAThread = new uint[tasks.Count()];
                    Prog.EntryFilenameOfAThread = new string[tasks.Count()];
                    Prog.BytesProcessedOfAThread = new long[tasks.Count()];
                    Prog.BytesTotalOfAThread = new long[tasks.Count()];

                    for (int k = 0; k < tasks.Count(); k++)
                    {
                        //if number of threads to use > number of packages, then skip making threads that won't be used
                        if (packageThreads[k].Count > 0)
                        {
                            Logging.Info("thread {0} starting task, packages to extract={1}", k, packageThreads[k].Count);
                            tasks[k] = Task.Run(() =>
                            {
                                int temp = k;
                                valueLocked = true;
                                ExtractFiles(packageThreads[temp], temp);
                                Prog.CompletedThreads++;
                            });
                            Logging.Debug("thread {0} started, waiting for thread ID value to be locked", k);
                            while (!valueLocked) ;
                            valueLocked = false;
                            Logging.Debug("thread {0} ID value locked, starting next task", k);
                            //also save the task to a list to use for cancel later
                            CreatedChildTasks.Add(tasks[k]);
                        }
                        else
                        {
                            Logging.Info("thread {0} skipped, no packages to extract (number of threads to use > number of packages in group)",k);
                            tasks[k] = null;
                        }
                    }

                    //and log it all
                    Logging.Debug("all threads started on group {0}, master thread now waiting on Task.WaitAll(tasks)", i);
                    Task.WaitAll(tasks.Where(task => task != null).ToArray());
                }

                Logging.Info("Install Group " + i + " finishes now");
            }
            return true;
        }

        private bool RestoreData(List<SelectablePackage> packagesWithData, StringBuilder restoreDataBuilder)
        {
            //progress reporting
            Prog.ChildTotal = packagesWithData.Count;
            Prog.ChildCurrent = Prog.ParrentCurrent = 0;
            Prog.ParrentTotal = 1;
            Prog.Filename = string.Empty;
            Progress.Report(Prog);

            foreach (SelectablePackage package in packagesWithData)
            {
                //actually report
                Prog.ChildCurrent++;
                Progress.Report(Prog);
                Logging.Info(string.Format("Restore data of package {0} starting", package.PackageName));

                //check if the package name folder exists first
                string tempBackupFolder = Path.Combine(Settings.RelhaxTempFolderPath, package.PackageName);
                if(!Directory.Exists(tempBackupFolder))
                {
                    Logging.WriteToLog(string.Format("folder {0} does not exist, skipping", package.PackageName), Logfiles.Application, LogLevel.Error);
                }

                //the list of files that was backed up already exists in a list called Files_saved. use that as the list of files to restore
                foreach (UserFile files in package.UserFiles)
                {
                    foreach(string savedFile in files.Files_saved)
                    {
                        //Files_saved should have the complete path of the destination
                        string fileSourcePath = Path.Combine(Settings.RelhaxTempFolderPath, package.PackageName, Path.GetFileName(savedFile));
                        if (File.Exists(fileSourcePath))
                        {
                            Logging.Info(string.Format("Restoring file {0} of {1}", Path.GetFileName(savedFile), package.PackageName));
                            //make the directory if it does not exist yet
                            if (!Directory.Exists(Path.GetDirectoryName(savedFile)))
                                Directory.CreateDirectory(Path.GetDirectoryName(savedFile));

                            //if it already exists, delete it
                            if (File.Exists(savedFile))
                                File.Delete(savedFile);

                            //then finally move it
                            File.Move(fileSourcePath, savedFile);

                            //and log it
                            restoreDataBuilder.AppendLine(savedFile);
                        }
                        else
                            Logging.Error("file {0} was reported backed up, but does not exist for package {1}", Path.GetFileName(savedFile), package.PackageName);
                    }
                }
            }
            return true;
        }

        private void CreateShortcuts()
        {
            Logging.Info(string.Format("Creating of shortcuts, current install time = {0} msec",
                (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.CreateShortcuts)
            {
                List<Shortcut> shortcuts = MakeShortcutList();
                if (shortcuts.Count > 0)
                {
                    StringBuilder shortcutBuilder = new StringBuilder();
                    shortcutBuilder.AppendLine("/*   Shortcuts   */");
                    CreateShortcutsTask = Task.Factory.StartNew(() =>
                    {
                        ProgShortcuts = CopyProgress(Prog);
                        ProgShortcuts.ParrentTotal = shortcuts.Count;
                        ProgShortcuts.InstallStatus = InstallerExitCodes.ShortcutsError;
                        LockProgress();

                        foreach (Shortcut shortcut in shortcuts)
                        {
                            CancellationToken.ThrowIfCancellationRequested();
                            ProgShortcuts.ParrentCurrent++;
                            LockProgress();

                            if (shortcut.Enabled)
                            {
                                Utils.CreateShortcut(shortcut, shortcutBuilder);
                            }
                        }

                        Logging.Installer(shortcutBuilder.ToString());
                        Logging.Info("Creating of shortcuts complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));

                        ProgShortcuts.TotalCurrent = (int)InstallerExitCodes.ShortcutsError;
                        InstallFinishedArgs.ExitCode = InstallerExitCodes.ShortcutsError;
                        ProgShortcuts.InstallStatus = InstallerExitCodes.ShortcutsError;
                        LockProgress();
                        ProgShortcuts = null;
                    });
                }
                else
                    Logging.Info("...skipped (no shortcut entries parsed)");
            }
            else
                Logging.Info("...skipped (setting is false)");
        }

        private void CreateAtlases()
        {
            Logging.Info(string.Format("Creating of atlases, current install time = {0} msec", (int)InstallStopWatch.Elapsed.TotalMilliseconds));
            List<Atlas> atlases = MakeAtlasList();
            if (atlases.Count > 0)
            {
                //initial progress report
                ProgAtlas = CopyProgress(Prog);
                ProgAtlas.ParrentTotal = atlases.Count;
                //child tasks are based on subtasks in the atlas
                ProgAtlas.ChildTotal = atlases.Count * 10;
                ProgAtlas.ChildCurrent = 0;
                ProgAtlas.InstallStatus = InstallerExitCodes.ContourIconAtlasError;
                LockProgress();

                //load the unmanaged libraries if they are not loaded already
                if (!Utils.FreeImageLibrary.IsLoaded)
                {
                    Logging.Info("freeimage library is not loaded, loading");
                    Utils.FreeImageLibrary.Load();
                    Logging.Info("freeimage library loaded");
                }
                else
                    Logging.Info("freeimage library is loaded");
                if (!Utils.NvTexLibrary.IsLoaded)
                {
                    Logging.Info("nvtt library is not loaded, loading");
                    Utils.NvTexLibrary.Load();
                    Logging.Info("nvtt library loaded");
                }
                else
                    Logging.Info("nvtt library is loaded");

                //start the mod image parsing task
                //get list of all mod texture folders
                List<string> textureFolders = new List<string>();
                foreach(Atlas atlas in atlases)
                {
                    foreach(string path in atlas.ImageFolderList)
                    {
                        if(!textureFolders.Contains(path))
                        {
                            textureFolders.Add(path);
                        }
                    }
                }
                AtlasesCreator.AtlasCreator.ParseModTexturesAsync(textureFolders, CancellationToken);

                //make an array to hold all the atlas tasks
                AtlasTasks = new Task[atlases.Count];
                List<AtlasesCreator.AtlasCreator> atlasCreators = new List<AtlasesCreator.AtlasCreator>();

                for (int i = 0; i < atlases.Count; i++)
                {
                    //spawn atlas threads for each task
                    bool taskValuesLocked = false;
                    AtlasTasks[i] = Task.Run(() =>
                    {
                        //copy the atlas reference into the task scope
                        //the bool creates a spinlock that prevents it from continuing until it's copied the reference
                        Atlas atlasData = atlases[i];
                        taskValuesLocked = true;

                        //create string builder for putting files created to installer log
                        StringBuilder atlasBuilder = new StringBuilder();
                        atlasBuilder.AppendLine("/*   Atlases   */");

                        //replace macros
                        atlasData.Pkg = Utils.MacroReplace(atlasData.Pkg, ReplacementTypes.FilePath);
                        atlasData.AtlasSaveDirectory = Utils.MacroReplace(atlasData.AtlasSaveDirectory, ReplacementTypes.FilePath);
                        for (int j = 0; j < atlasData.ImageFolderList.Count; j++)
                        {
                            CancellationToken.ThrowIfCancellationRequested();
                            atlasData.ImageFolderList[j] = Utils.MacroReplace(atlasData.ImageFolderList[j], ReplacementTypes.FilePath);
                        }

                        CancellationToken.ThrowIfCancellationRequested();
                        LockProgress();

                        //create the atlas
                        AtlasesCreator.AtlasCreator atlasCreator = new AtlasesCreator.AtlasCreator()
                        {
                            Atlas = atlasData,
                            Token = CancellationToken,
                            DebugLockObject = AtlasBuilderLockerObject
                        };
                        atlasCreator.OnAtlasProgres += AtlasCreator_OnAtlasProgres;
                        {
                            lock(AtlasBuilderLockerObject)
                            {
                                atlasCreators.Add(atlasCreator);
                            }
                            AtlasesCreator.FailCode code = atlasCreator.CreateAtlas();
                            if ( code != AtlasesCreator.FailCode.None)
                            {
                                Logging.Exception("Failed to create atlas file {0}: {1}", Path.GetFileName(atlasData.AtlasFile), code.ToString());
                                return;
                            }
                        }

                        lock (Progress)
                        {
                            ProgAtlas.ParrentCurrent++;
                        }
                        LockProgress();

                        //append generated atlas info
                        atlasBuilder.AppendLine(atlasData.MapFile);
                        atlasBuilder.AppendLine(atlasData.AtlasFile);
                        //make sure it's not writing the same time
                        lock (Progress)
                        {
                            Logging.Installer(atlasBuilder.ToString());
                            if (ProgAtlas.ParrentCurrent == ProgAtlas.ParrentTotal)
                            {
                                ProgAtlas.TotalCurrent = (int)InstallerExitCodes.ContourIconAtlasError;
                                InstallFinishedArgs.ExitCode = InstallerExitCodes.ContourIconAtlasError;
                                ProgAtlas.InstallStatus = InstallerExitCodes.ContourIconAtlasError;
                                LockProgress();
                                ProgAtlas = null;
                            }
                        }
                    });
                    while (!taskValuesLocked) ;
                }
                Logging.Info("creating task to wait for all atlas creations before disposal");
                AtlasDisposeTask = Task.Run(() =>
                {
                    Logging.Debug("waiting for all atlas tasks to complete");
                    Task.WaitAll(AtlasTasks);
                    Logging.Debug("all atlas tasks completed, disposal starts");
                    for(int i = 0; i < atlasCreators.Count; i++)
                    {
                        atlasCreators[i].Dispose();
                        atlasCreators[i] = null;
                    }
                    atlasCreators = null;
                    AtlasesCreator.AtlasCreator.DisposeparseModTextures();
                    Logging.Debug("atlas disposal completes");
                });
            }
            else
                Logging.Info("...skipped (no atlas entries parsed)");
        }

        private void AtlasCreator_OnAtlasProgres(object sender, EventArgs e)
        {
            ProgAtlas.ChildCurrent++;
            LockProgress();
        }

        private void InstallFonts()
        {
            Logging.Info(string.Format("Installing of fonts, current install time = {0} msec",
                    (int)InstallStopWatch.Elapsed.TotalMilliseconds));

            //check for any font files to install at all
            string[] fontsToInstall = Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, Settings.FontsToInstallFoldername), SearchOption.TopDirectoryOnly, false, @"*", 50, 3, true);

            //filter out fontReg
            fontsToInstall = fontsToInstall.Where(filename => !filename.Contains(".exe")).ToArray();

            if (fontsToInstall == null || fontsToInstall.Count() == 0)
                Logging.Info("...skipped (no font files to install)");
            else
            {
                CreateFontsTask = Task.Factory.StartNew(() =>
                {
                    Logging.Debug("checking system installed fonts to remove duplicates");

                    string[] fontscurrentlyInstalled = Utils.DirectorySearch(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), SearchOption.TopDirectoryOnly, false, @"*",5,3,false);
                    string[] fontsNamesCurrentlyInstalled = fontscurrentlyInstalled.Select(s => Path.GetFileName(s).ToLower()).ToArray();

                    //remove any fonts whos filename match what is already installed
                    for (int i = 0; i < fontsToInstall.Count(); i++)
                    {
                        string fontToInstallNameLower = Path.GetFileName(fontsToInstall[i]).ToLower();
                        if (fontsNamesCurrentlyInstalled.Contains(fontToInstallNameLower))
                        {
                            //empty the entry
                            fontsToInstall[i] = string.Empty;
                        }
                    }

                    //get the new array of fonts to install that don't already exist
                    string[] realFontsToInstall = fontsToInstall.Where(font => !string.IsNullOrWhiteSpace(font)).ToArray();

                    Logging.Debug("realFontsToInstall count: {0}", realFontsToInstall.Count());

                    if (realFontsToInstall.Count() > 0)
                    {
                        //initial progress
                        ProgFonts = CopyProgress(Prog);
                        ProgFonts.InstallStatus = InstallerExitCodes.FontInstallError;
                        ProgFonts.ParrentTotal = realFontsToInstall.Count();
                        LockProgress();

                        //check if fontReg exists
                        string fontRegPath = Path.Combine(Settings.WoTDirectory, Settings.FontsToInstallFoldername, "FontReg.exe");
                        if (!File.Exists(fontRegPath))
                        {
                            Logging.Error("FontReg was not located in the \"_fonts\" folder!");
                            MessageBox.Show(string.Format("{0}{1}{2}{3}{4}", Translations.GetTranslatedString("fontsPromptError_1"), Environment.NewLine,
                                Settings.WoTDirectory, Environment.NewLine, Translations.GetTranslatedString("fontsPromptError_2")));
                        }

                        CancellationToken.ThrowIfCancellationRequested();
                        Logging.Info("Attempting to install fonts: {0}", string.Join(",", realFontsToInstall));
                        ProcessStartInfo info = new ProcessStartInfo
                        {
                            FileName = fontRegPath,
                            UseShellExecute = true,
                            Verb = "runas", // Provides Run as Administrator
                            Arguments = "/copy",
                            WorkingDirectory = Path.Combine(Settings.WoTDirectory, Settings.FontsToInstallFoldername)
                        };

                        try
                        {
                            Process installFontss = new Process() { StartInfo = info };
                            installFontss.Start();
                            installFontss.WaitForExit();
                            Logging.Info("FontReg.exe ExitCode: " + installFontss.ExitCode);
                            Logging.Info("Installing of fonts complete, took {0} msec", (int)(InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
                        }
                        catch (Exception ex)
                        {
                            Logging.Error("could not start font installer:{0}{1}", Environment.NewLine, ex.ToString());
                            MessageBox.Show(string.Format("{0}{1}{2}{3}{4}", Translations.GetTranslatedString("fontsPromptError_1"), Environment.NewLine,
                                Settings.WoTDirectory, Environment.NewLine, Translations.GetTranslatedString("fontsPromptError_2")));
                        }
                        finally
                        {
                            ProgFonts.TotalCurrent = (int)InstallerExitCodes.FontInstallError;
                            InstallFinishedArgs.ExitCode = InstallerExitCodes.FontInstallError;
                            ProgFonts.InstallStatus = InstallerExitCodes.FontInstallError;
                            LockProgress();
                            ProgFonts = null;
                            CancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                });
            }
        }

        private bool TrimDownloadCache()
        {
            //progress reporting
            Prog.ChildCurrent = Prog.ParrentCurrent = 0;
            Prog.ChildTotal = 1;
            Prog.ParrentTotal = 2;
            Prog.Filename = string.Empty;
            Progress.Report(Prog);

            //get a list of all packages in the database with zip files
            List<DatabasePackage> allFlatList = Utils.GetFlatList(GlobalDependencies, Dependencies, null, ParsedCategoryList).Where(package => !string.IsNullOrWhiteSpace(package.ZipFile)).ToList();

            //convert it to a list of strings
            List<string> zipFilesInDatabase = allFlatList.Select(package => package.ZipFile).ToList();

            //get a list of all files in the download cache folder
            List<string> zipFilesInCache = Utils.DirectorySearch(Settings.RelhaxDownloadsFolderPath, SearchOption.TopDirectoryOnly, false, "*.zip").ToList();
            if(zipFilesInCache == null)
            {
                Logging.Error("failed to get list of zip files in download cache, skipping this step");
                return false;
            }
            Prog.ParrentCurrent = 1;
            Progress.Report(Prog);

            //update the list to only have the filename, not the complete path
            zipFilesInCache = zipFilesInCache.Select(str => Path.GetFileName(str)).ToList();

            //get a list of zip files in the cache that aren't in the database, these are old and can be deleted
            List<string> oldZipFilesNotInDatabase = zipFilesInCache.Except(zipFilesInDatabase).ToList();

            //report progress
            Prog.ChildTotal = oldZipFilesNotInDatabase.Count;

            //if there are any in the above list, it means they are old and can be deleted
            if (oldZipFilesNotInDatabase.Count > 0)
            {
                foreach (string zipfile in oldZipFilesNotInDatabase)
                {
                    Prog.ChildCurrent++;
                    Prog.Filename = zipfile;
                    Progress.Report(Prog);

                    Utils.FileDelete(Path.Combine(Settings.RelhaxDownloadsFolderPath, zipfile));
                }
            }
            return true;
        }

        private bool Cleanup()
        {
            //progress reporting
            Prog.ChildCurrent = Prog.ParrentCurrent = 0;
            Prog.ChildTotal = 1;
            Prog.ParrentTotal = Settings.FoldersToCleanup.Count();
            Prog.Filename = string.Empty;
            Progress.Report(Prog);

            if (AtlasDisposeTask != null)
            {
                Logging.Debug("waiting for atlas disposal task before try to cleanup");
                AtlasDisposeTask.Wait();
                Logging.Debug("atlas disposal complete, continue with cleanup");
            }

            bool success = true;
            foreach (string folder in Settings.FoldersToCleanup)
            {
                Logging.Debug("cleaning folder {0}, if exists", folder);
                Prog.ParrentCurrent++;
                Prog.Filename = folder;
                Progress.Report(Prog);

                string folderPath = Path.Combine(Settings.WoTDirectory, folder);
                bool directoryExists = Directory.Exists(folderPath);
                bool deleteSuccess = false;
                if (directoryExists)
                {
                    deleteSuccess = Utils.DirectoryDelete(folderPath, true);
                    if (!deleteSuccess)
                    {
                        success = false;
                    }
                    Logging.Debug("directoryExists={0}, deleteSuccess={1}", directoryExists.ToString(), deleteSuccess.ToString());
                }
                else
                    Logging.Debug("directoryExists={0}", directoryExists.ToString());
            }
            return success;
        }
        #endregion

        #region Util Methods
        private void ExtractFiles(List<DatabasePackage> packagesToExtract, int threadNum)
        {
            //setup progressing
            Prog.TotalPackagesofAThread[threadNum] = (uint)packagesToExtract.Count();
            Prog.CompletedPackagesOfAThread[threadNum] = 0;

            bool notAllPackagesExtracted = true;
            //setup progress reporting of this thread

            //in case the user selected to "download while installing", there may be cases where
            //some items in this list (earlier, for sake of argument) are not downloaded yet, but others below are.
            //if this is the case, then we need to skip over those items and install others while we wait
            int numExtracted = 0;
            while (notAllPackagesExtracted)
            {
                foreach (DatabasePackage package in packagesToExtract)
                {
                    //check for cancel
                    CancellationToken.ThrowIfCancellationRequested();

                    //check if we are installing while downloading and this package is still downloading
                    if (ModpackSettings.InstallWhileDownloading && package.DownloadFlag)
                    {
                        continue;
                    }
                    //else check if we are installing while downloading and this package's extraction has started
                    else if (ModpackSettings.InstallWhileDownloading && package.ExtractionStarted)
                    {
                        continue;
                    }
                    //else start extraction
                    else
                    {
                        Logging.Info("Thread ID={0}, extraction started of zipfile {1} of packageName {2}", threadNum, package.ZipFile, package.PackageName);

                        //flag that this package's extraction has started
                        package.ExtractionStarted = true;

                        //stop if the zipfile name is blank (no actual zipfile to extract)
                        if (!string.IsNullOrWhiteSpace(package.ZipFile))
                        {
                            StringBuilder zipLogger = new StringBuilder();
                            zipLogger.AppendLine(string.Format("/*   {0}   */", package.ZipFile));
                            Unzip(package, threadNum, zipLogger);
                            Logging.Installer(zipLogger.ToString());
                        }
                        else
                        {
                            Logging.Warning("zipfile for package {0} is blank!", package.PackageName);
                        }

                        Logging.Info("Thread ID={0}, extraction finished of zipfile {1} of packageName {2}", threadNum, package.ZipFile, package.PackageName);

                        //increment counter
                        Logging.Info("Thread ID={0}, extracted {1} of {2}", threadNum, ++numExtracted, packagesToExtract.Count);

                        //update progress of total packages extracted
                        Prog.ParrentCurrent++;

                        //update progress of packages extracted on this thread
                        Prog.CompletedPackagesOfAThread[threadNum]++;

                        //after zip file extraction, process triggers (if enabled)
                        if (!DisableTriggersForInstall)
                        {
                            if (package.Triggers.Count > 0)
                                ProcessTriggers(package.Triggers);
                        }
                    }
                }
                if (numExtracted == packagesToExtract.Count)
                    notAllPackagesExtracted = false;
                else
                    Thread.Sleep(200);
            }
        }

        private void Unzip(DatabasePackage package, int threadNum, StringBuilder zipLogger)
        {
            //for each zip file, put it in a try catch to see if we can catch any issues in case of a one-off IO error
            string zipFilePath = Path.Combine(Settings.RelhaxDownloadsFolderPath, package.ZipFile);
            for(int i = 3; i > 0; i--)//3 strikes and you're out
            {
                try
                {
                    using (RelhaxZipFile zip = new RelhaxZipFile(zipFilePath))
                    {
                        zip.ThreadID = threadNum;
                        //update args and logging here...
                        //first for loop takes care of any path replacing in the zipfile
                        for(int j = 0; j < zip.Entries.Count; j++)
                        {
                            CancellationToken.ThrowIfCancellationRequested();
                            //check for versiondir
                            string zipEntryName = zip[j].FileName;
                            if (zipEntryName.Contains("versiondir"))
                                zipEntryName = zipEntryName.Replace("versiondir", Settings.WoTClientVersion);
                            //check for xvmConfigFolderName
                            if (zipEntryName.Contains("configs/xvm/xvmConfigFolderName"))
                            {
                                //check if the config folder name has been tested for yet
                                //if not, then get it
                                if(string.IsNullOrEmpty(XvmFolderName))
                                {
                                    XvmFolderName = PatchUtils.GetXvmFolderName().Trim();
                                    //also add it to the filepath replace
                                    if (!Utils.FilePathDict.ContainsKey(@"xvmConfigFolderName"))
                                        Utils.FilePathDict.Add(@"xvmConfigFolderName", XvmFolderName);
                                }
                                zipEntryName = zipEntryName.Replace("configs/xvm/xvmConfigFolderName", string.Format("configs/xvm/{0}",XvmFolderName));
                            }
                            if(Regex.IsMatch(zipEntryName, @"_patch.*\.xml"))
                            {
                                //format for new patch names
                                //patchGroup_origName_0 <- can be increased for uniqueness
                                //patchGroup takes care of the global->dep->selectable package order

                                //build the patch name manually
                                StringBuilder sb = new StringBuilder();

                                //first get the "_patch/" prefix (that does not change)
                                sb.Append(zipEntryName.Substring(0,7));

                                //pad and add the patchGroup name
                                sb.Append(package.PatchGroup.ToString("D3"));

                                //name else doesn't need to change, to set the rest of the name and use it
                                sb.Append(zipEntryName.Substring(7));

                                //save the original and new names to the list to apply later
                                //lock on a static
                                lock(duplicatePatchNameObjectLocker)
                                {
                                    //key is sb, sb is new name for disk, is native processing file
                                    //value is original name from zip file
                                    string key = sb.ToString().Substring(7);
                                    string value = zipEntryName.Substring(7);
                                    while (OriginalPatchNames.ContainsKey(key))
                                    {
                                        string orig = key;
                                        //add a underscore before the dot for the file extension
                                        key = string.Join("_.", key.Split('.'));
                                        //also update the stringbuilder
                                        sb = new StringBuilder(string.Format("{0}{1}",sb.ToString().Substring(0,7),key));
                                        Logging.Warning("patchfile \"{0}\" has same name as patch in other zip file! Please fix! Using name \"{1}\" instead", orig, key);
                                    }
                                    OriginalPatchNames.Add(key, value);
                                }
                                //and save it to the string
                                zipEntryName = sb.ToString();
                            }
                            //save zip entry file modifications back to zipfile
                            zip[j].FileName = zipEntryName;
                        }
                        //attach the event handler to report progress
                        zip.ExtractProgress += OnZipfileExtractProgress;
                        //set total number of entries
                        Prog.EntriesTotal = (uint)zip.Entries.Count;
                        Prog.EntriesProcessed = 0;
                        Prog.EntriesProcessedOfAThread[threadNum] = 0;
                        Prog.EntriesTotalOfAThread[threadNum] = (uint)zip.Entries.Count;
                        //second loop extracts each file and checks for extraction macros
                        for (int j = 0; j < zip.Entries.Count; j++)
                        {
                            CancellationToken.ThrowIfCancellationRequested();
                            string zipFilename = zip[j].FileName;
                            //create logging entry
                            string loggingCompletePath = string.Empty;
                            string extractPath = string.Empty;

                            //check each entry if it's going to a custom extraction location by form of macro
                            //default is nothing, goes to root (World_of_Tanks) directory
                            //if macro is found, extract to a different folder
                            //first check for "_AppData" macro in root of zip file
                            //NOTE: "WoTAppData" is not supported
                            //check if the root entry contains it
                            //_AppData = root application data directory
                            if (zipFilename.Length >= "_AppData".Length && zipFilename.Substring(0,"_AppData".Length).Equals("_AppData"))
                            {
                                zipFilename = zipFilename.Replace("_AppData", string.Empty);
                                if(!string.IsNullOrWhiteSpace(zipFilename))
                                {
                                    zip[j].FileName = zipFilename;
                                    zip[j].Extract(Settings.AppDataFolder, ExtractExistingFileAction.OverwriteSilently);
                                    extractPath = Settings.AppDataFolder;
                                }
                            }
                            //_RelhaxRoot = app startup directory
                            else if (zipFilename.Length >= "_RelhaxRoot".Length && zipFilename.Substring(0, "_RelhaxRoot".Length).Equals("_RelhaxRoot"))
                            {
                                zipFilename = zipFilename.Replace("_RelhaxRoot", string.Empty);
                                if (!string.IsNullOrWhiteSpace(zipFilename))
                                {
                                    zip[j].FileName = zipFilename;
                                    zip[j].Extract(Settings.ApplicationStartupPath, ExtractExistingFileAction.OverwriteSilently);
                                    extractPath = Settings.ApplicationStartupPath;
                                }
                            }
                            //default is World_of_Tanks directory
                            else
                            {
                                zip[j].Extract(Settings.WoTDirectory, ExtractExistingFileAction.OverwriteSilently);
                                extractPath = Settings.WoTDirectory;
                            }
                            loggingCompletePath = Path.Combine(extractPath, zipFilename.Replace(@"/", @"\"));
                            zipLogger.AppendLine(package.LogAtInstall ? loggingCompletePath : "#" + loggingCompletePath);
                            Prog.EntriesProcessed++;
                            Prog.EntriesProcessedOfAThread[threadNum]++;
                        }
                    }
                    //set i to 0 so that it breaks out of the loop
                    i = 0;
                }
                catch (Exception e)
                {
                    //if it's a cancel, then stop
                    if (e is OperationCanceledException)
                    {
                        Logging.Debug("cancel detected in extraction thread, aborting");
                        return;
                    }
                    if (i <= 1)
                    {
                        //log as error, 3 tries and all failures
                        Logging.Exception("Failed to extract zipfile {0}, exception message:{1}{2}", package.ZipFile, Environment.NewLine, e.ToString());
                        MessageBox.Show(string.Format("{0}, {1} {2} {3}",
                            Translations.GetTranslatedString("zipReadingErrorMessage1"), package.ZipFile, Translations.GetTranslatedString("zipReadingErrorMessage2"),
                            Translations.GetTranslatedString("zipReadingErrorHeader")));
                        //delete the file (if it exists)
                        if(File.Exists(zipFilePath))
                        {
                            try
                            {
                                File.Delete(zipFilePath);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                Logging.Exception(ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        //log warning, try again
                        Logging.Warning("Exception of type {0} caught, retryNum = {1} (to 1), on thread = {2}, zipfile = {3}", e.GetType().Name, i, threadNum, package.ZipFile);
                    }
                }
            }
        }

        private void OnZipfileExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            //this thing fires for all sorts of events, but we only care about 3. and we want to keep locking to a minimum, as well as progress reporting
            //(no needless locking or progress reporting)
            switch (e.EventType)
            {
                case ZipProgressEventType.Extracting_EntryBytesWritten:
                case ZipProgressEventType.Extracting_BeforeExtractEntry:
                case ZipProgressEventType.Extracting_AfterExtractEntry:
                    lock (Progress)
                    {
                        Prog.BytesProcessed = (int)e.BytesTransferred;
                        Prog.BytesTotal = (int)e.TotalBytesToTransfer;
                        Prog.ChildCurrent = (int)e.BytesTransferred;
                        Prog.ChildTotal = (int)e.TotalBytesToTransfer;
                        Prog.BytesProcessedOfAThread[(uint)(sender as RelhaxZipFile).ThreadID] = e.BytesTransferred;
                        Prog.BytesTotalOfAThread[(uint)(sender as RelhaxZipFile).ThreadID] = e.TotalBytesToTransfer;
                        Prog.EntryFilename = e.CurrentEntry.FileName;
                        Prog.EntryFilenameOfAThread[(uint)(sender as RelhaxZipFile).ThreadID] = e.CurrentEntry.FileName;
                        Prog.Filename = e.ArchiveName;
                        Prog.ThreadID = (uint)(sender as RelhaxZipFile).ThreadID;
                        Progress.Report(Prog);
                    }
                    break;
            }
        }

        private void ProcessTriggers(List<string> packageTriggers)
        {
            if(ModpackSettings.ExportMode)
            {
                Logging.Info("skipped triggers due to export mode");
                return;
            }
            //at least 1 trigger exists
            foreach (string triggerFromPackage in packageTriggers)
            {
                //in theory, each database package trigger is unique in each package AND in installer
                Trigger match = Triggers.Find(search => search.Name.ToLower().Equals(triggerFromPackage.ToLower()));
                if (match == null)
                {
                    Logging.Debug("trigger match is null (no match!) {0}", triggerFromPackage);
                    continue;
                }
                //this could be in multiple threads, so needs to be done in a lock statement (read modify write operation)
                lock (Triggers)
                {
                    match.NumberProcessed++;
                    if (match.NumberProcessed >= match.Total)
                    {
                        string message = string.Format("matched trigger {0} has numberProcessed {1}, total is {2}", match.Name, match.NumberProcessed, match.Total);
                        if (match.NumberProcessed > match.Total)
                            Logging.Error(message);
                        else //it's equal
                            Logging.Debug(message);
                        if (match.Fired)
                        {
                            Logging.Error("trigger {0} has already fired, skipping", match.Name);
                        }
                        else
                        {
                            Logging.Debug("trigger {0} is starting", match.Name);
                            match.Fired = true;
                            //hard-coded list of triggers that can be fired from list at top of class
                            switch (match.Name)
                            {
                                case TriggerContouricons:
                                    CreateAtlases();
                                    break;
                                case TriggerInstallFonts:
                                    InstallFonts();
                                    break;
                                case TriggerCreateShortcuts:
                                    CreateShortcuts();
                                    break;
                                default:
                                    Logging.Error("Invalid trigger name for switch block: {0}", match.Name);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private List<Patch> MakePatchList()
        {
            //get a list of all files in the dedicated patch directory
            //foreach one add it to the patch list
            List<Patch> patches = new List<Patch>();

            //if the patches folder does not exist, then there are no patches to load or run
            if (!Directory.Exists(Path.Combine(Settings.WoTDirectory, Settings.PatchFolderName)))
            {
                Logging.Info("\"{0}\" folder does not exist, skipping", Settings.PatchFolderName);
                Logging.Info("Number of patches: {0}", patches.Count());
                return patches;
            }

            //get the list of all patches in the directory
            string[] patch_files = Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, Settings.PatchFolderName), SearchOption.TopDirectoryOnly, false, @"*.xml", 50, 3, true);
            if (patch_files == null)
                Logging.WriteToLog("Failed to parse patches from patch directory (see above lines for more info", Logfiles.Application, LogLevel.Error);
            else
            {
                Logging.Info("Number of patch files: {0}",patch_files.Count());
                //if there wern't any, don't bother doing anything
                if(patch_files.Count() > 0)
                {
                    string completePath;
                    foreach (string filename in patch_files)
                    {
                        completePath = Path.Combine(Settings.WoTDirectory, Settings.PatchFolderName, filename);
                        //just double check...
                        if(!File.Exists(completePath))
                        {
                            Logging.WriteToLog("patch file does not exist?? " + completePath, Logfiles.Application, LogLevel.Warning);
                            continue;
                        }
                        Utils.ApplyNormalFileProperties(completePath);
                        //ok NOW actually add the file to the patch list
                        XmlUtils.AddPatchesFromFile(patches, completePath, OriginalPatchNames[Path.GetFileName(filename)]);
                    }
                }
            }
            return patches;
        }

        private List<Shortcut> MakeShortcutList()
        {
            List<Shortcut> shortcuts = new List<Shortcut>();

            //if the patches folder does not exist, then there are no patches to load or run
            if (!Directory.Exists(Path.Combine(Settings.WoTDirectory, Settings.ShortcutFolderName)))
            {
                Logging.Info("\"{0}\" folder does not exist, skipping", Settings.ShortcutFolderName);
                Logging.WriteToLog(string.Format("Number of shortcuts: {0}", shortcuts.Count()), Logfiles.Application, LogLevel.Info);
                return shortcuts;
            }

            //get a list of all files in the dedicated shortcuts directory
            //foreach one add it to the patch list
            string[] shortcut_files = Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, Settings.ShortcutFolderName), SearchOption.TopDirectoryOnly, false, @"*.xml", 50, 3, true);
            if (shortcut_files == null)
                Logging.WriteToLog("Failed to parse shortcuts from directory", Logfiles.Application, LogLevel.Error);
            else if (shortcut_files.Count() == 0)
            {
                Logging.Info("No shortcut files to operate on");
            }
            else
            {
                Logging.WriteToLog(string.Format("Number of shortcut xml instruction files: {0}", shortcut_files.Count()), Logfiles.Application, LogLevel.Debug);
                //if there weren't any, don't bother doing anything
                string completePath;
                foreach (string filename in shortcut_files)
                {
                    completePath = Path.Combine(Settings.WoTDirectory, Settings.ShortcutFolderName, filename);
                    //apply "normal" file properties just in case the user's wot install directory is special
                    Utils.ApplyNormalFileProperties(completePath);
                    //ok NOW actually add the file to the patch list
                    Logging.Info("Adding shortcuts from shortcutFile {1}", Logfiles.Application, filename);
                    XmlUtils.AddShortcutsFromFile(shortcuts, filename);
                }
            }
            return shortcuts;
        }

        //XML Unpack
        private List<XmlUnpack> MakeXmlUnpackList()
        {
            List<XmlUnpack> XmlUnpacks = new List<XmlUnpack>();

            //if the patches folder does not exist, then there are no patches to load or run
            if (!Directory.Exists(Path.Combine(Settings.WoTDirectory, Settings.XmlUnpackFolderName)))
            {
                Logging.Info("\"{0}\" folder does not exist, skipping", Settings.XmlUnpackFolderName);
                Logging.WriteToLog(string.Format("Number of XmlUnpack files: {0}", XmlUnpacks.Count()), Logfiles.Application, LogLevel.Info);
                return XmlUnpacks;
            }

            //get a list of all files in the dedicated patch directory
            //foreach one add it to the patch list
            string[] unpack_files = Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, Settings.XmlUnpackFolderName), SearchOption.TopDirectoryOnly, false, @"*.xml", 50, 3, true);
            if (unpack_files == null)
                Logging.WriteToLog("Failed to parse xml unpacks from unpack directory", Logfiles.Application, LogLevel.Error);
            else
            {
                Logging.WriteToLog(string.Format("Number of XmlUnpack files: {0}", unpack_files.Count()), Logfiles.Application, LogLevel.Info);
                //if there wern't any, don't bother doing anything
                if (unpack_files.Count() > 0)
                {
                    string completePath;
                    foreach (string filename in unpack_files)
                    {
                        completePath = Path.Combine(Settings.WoTDirectory, Settings.ShortcutFolderName, filename);

                        //ok NOW actually add the file to the patch list
                        Logging.Info("Adding xml unpack entries from file {1}", Logfiles.Application, filename);
                        XmlUtils.AddXmlUnpackFromFile(XmlUnpacks, filename);
                    }
                }
            }

            //perform macro replacement on all xml unpack entries
            foreach(XmlUnpack xmlUnpack in XmlUnpacks)
            {
                xmlUnpack.DirectoryInArchive = Utils.MacroReplace(xmlUnpack.DirectoryInArchive, ReplacementTypes.FilePath);
                xmlUnpack.FileName = Utils.MacroReplace(xmlUnpack.FileName, ReplacementTypes.FilePath);
                xmlUnpack.ExtractDirectory = Utils.MacroReplace(xmlUnpack.ExtractDirectory, ReplacementTypes.FilePath);
                xmlUnpack.NewFileName = Utils.MacroReplace(xmlUnpack.NewFileName, ReplacementTypes.FilePath);
                xmlUnpack.Pkg = Utils.MacroReplace(xmlUnpack.Pkg, ReplacementTypes.FilePath);
            }

            return XmlUnpacks;
        }

        //Atlas parsing
        private List<Atlas> MakeAtlasList()
        {
            List<Atlas> atlases = new List<Atlas>();

            //if the patches folder does not exist, then there are no patches to load or run
            if (!Directory.Exists(Path.Combine(Settings.WoTDirectory, Settings.AtlasCreationFoldername)))
            {
                Logging.Debug("\"{0}\" folder does not exist, skipping", Settings.AtlasCreationFoldername);
                Logging.WriteToLog(string.Format("Number of atlases: {0}", atlases.Count()), Logfiles.Application, LogLevel.Debug);
                return atlases;
            }

            //get a list of all files in the dedicated patch directory
            //foreach one add it to the patch list
            string[] atlas_files = Utils.DirectorySearch(Path.Combine(Settings.WoTDirectory, Settings.AtlasCreationFoldername), SearchOption.TopDirectoryOnly, false, @"*.xml", 50, 3, true);
            if (atlas_files == null)
                Logging.WriteToLog("Failed to parse atlases from atlas directory", Logfiles.Application, LogLevel.Error);
            else
            {
                Logging.WriteToLog(string.Format("Number of atlas files: {0}", atlas_files.Count()), Logfiles.Application, LogLevel.Debug);
                //if there wern't any, don't bother doing anything
                if (atlas_files.Count() > 0)
                {
                    string completePath;
                    foreach (string filename in atlas_files)
                    {
                        completePath = Path.Combine(Settings.WoTDirectory, Settings.ShortcutFolderName, filename);
                        //apply "normal" file properties just in case the user's wot install directory is special
                        Utils.ApplyNormalFileProperties(completePath);
                        //ok NOW actually add the file to the patch list
                        Logging.Info("Adding atlas entries from file {1}", Logfiles.Application, filename);
                        XmlUtils.AddAtlasFromFile(atlases, filename);
                    }
                }
            }
            return atlases;
        }

        private bool TaskNullOrDone(Task task)
        {
            return task == null || task.IsCompleted;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private bool TaskNullOrDone(Task[] tasks)
#pragma warning restore IDE0051 // Remove unused private members
        {
            foreach (Task t in tasks)
                if (!TaskNullOrDone(t))
                    return false;
            return true;
        }

        private void LockProgress()
        {
            lock(Progress)
            {
                //reports progress based on order of what is done. a progress being done is determined if the installerProgress object is null
                //using if-else gives us order
                if (ProgPatch != null)
                    Progress.Report(ProgPatch);
                else if (ProgShortcuts != null)
                    Progress.Report(ProgShortcuts);
                else if (ProgAtlas != null)
                    Progress.Report(ProgAtlas);
                else
                    Progress.Report(ProgFonts);
            }
        }

        private RelhaxInstallerProgress CopyProgress(RelhaxInstallerProgress progress)
        {
            return new RelhaxInstallerProgress()
            {
                BytesProcessed = progress.BytesProcessed,
                BytesTotal = progress.BytesTotal,
                Filename = string.Copy(progress.Filename),
                EntryFilename = string.Copy(progress.EntryFilename),
                EntriesProcessed = progress.EntriesProcessed,
                EntriesTotal = progress.EntriesTotal,
                ThreadID = progress.ThreadID,
                InstallStatus = progress.InstallStatus,
                UninstallStatus = progress.UninstallStatus,
                ChildCurrent = progress.ChildCurrent,
                ChildTotal = progress.ChildTotal,
                ChildCurrentProgress = string.Copy(progress.ChildCurrentProgress),
                ParrentCurrent = progress.ParrentCurrent,
                ParrentTotal = progress.ParrentTotal,
                ParrentCurrentProgress = string.Copy(progress.ParrentCurrentProgress),
                TotalCurrent = progress.TotalCurrent,
                TotalTotal = progress.TotalTotal,
                TotalCurrentProgress = string.Copy(progress.TotalCurrentProgress),
                ReportMessage = string.Copy(progress.ReportMessage)
            };
        }

        private void CheckForCancel()
        {
            if(CancellationToken.IsCancellationRequested)
            {
                Logging.Info("Cancel detected, waiting all child threads before stopping master thread");
                if(Installing)
                {
                    for (int i = 0; i < CreatedChildTasks.Count; i++)
                    {
                        Task tsk = CreatedChildTasks[i];
                        if (tsk == null)
                        {
                            Logging.Error("task to cancel is null, should not happen!");
                            continue;
                        }
                        while (true)
                        {
                            if (tsk.Status == TaskStatus.Canceled || tsk.Status == TaskStatus.Faulted || tsk.Status == TaskStatus.RanToCompletion)
                                break;
                        }
                        tsk.Dispose();
                    }
                    CreatedChildTasks = null;
                    Logging.Info("all child threads stopped, stopping master");
                }
                else
                {
                    Logging.Debug("engine is in uninstall mode, canceling main task");
                }

                //if canceling, at least making the folders if they don't already exist
                if (!ModpackSettings.ExportMode)
                {
                    Logging.Debug("creating mods and res_mods if they don't already exist, just in case");
                    foreach (string s in new string[] { Path.Combine(Settings.WoTDirectory, "res_mods", Settings.WoTClientVersion),
                    Path.Combine(Settings.WoTDirectory, "mods", Settings.WoTClientVersion) })
                    {
                        if (!Directory.Exists(s))
                            Directory.CreateDirectory(s);
                    }
                }
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose of the Installation engine
        /// </summary>
        /// <param name="disposing">Flag to indicate if the engine should additionally dispose of managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~InstallEngine() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// This code added to correctly implement the disposable pattern.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
