﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using RelhaxModpack.UIComponents;
using System.IO;

namespace RelhaxModpack.InstallerComponents
{
    #region Event stuff
    public enum InstallerExitCodes
    {
        Success,
        DownloadModsError,
        BackupModsError,
        BackupDataError,
        ClearCacheError,
        ClearLogsError,
        CleanModsError,
        ExtractionError,
        RestoreUserdataError,
        PatchError,
        ShortcustError,
        ContourIconAtlasError,
        CleanupError,
        UnknownError
    }
    public class RelhaxInstallFinishedEventArgs : EventArgs
    {
        public InstallerExitCodes ExitCodes;
        public string ErrorMessage;
    }
    public delegate void InstallFinishedDelegate(object sender, RelhaxInstallFinishedEventArgs e);
    public delegate void InstallProgressDelegate(object sender, RelhaxProgress e);
    #endregion

    public class InstallEngine : IDisposable
    {
        #region Instance Variables
        public List<DatabasePackage>[] OrderedPackagesToInstall;
        public List<SelectablePackage> FlatListSelectablePackages;
        public event InstallFinishedDelegate OnInstallFinish;
        public event InstallProgressDelegate OnInstallProgress;
        public RelhaxInstallFinishedEventArgs InstallFinishedEventArgs = new RelhaxInstallFinishedEventArgs();
        public Stopwatch InstallStopWatch = new Stopwatch();
        private TimeSpan OldTime;
        private RelhaxProgress Progress = new RelhaxProgress();
        #endregion

        #region More boring stuff
        public InstallEngine()
        {
            //init the install engine
        }
        private void ReportProgress()
        {
            if(OnInstallProgress != null)
            {
                OnInstallProgress(this, Progress);
            }
        }
        private void ReportFinish()
        {
            if(OnInstallFinish != null)
            {
                OnInstallFinish(this, InstallFinishedEventArgs);
            }
        }
        #endregion

        public async void RunInstallationAsync()
        {
            if(OrderedPackagesToInstall == null || OrderedPackagesToInstall.Count() == 0)
                throw new BadMemeException("WOW you really suck at programming");
            if (OnInstallFinish == null || OnInstallProgress == null)
                throw new BadMemeException("HOW DAFAQ DID YOU FAQ THIS UP???");
            Logging.WriteToLog("Installation starts now from RunInstallation() in Install Engine");
            //do more stuff here im sure like init log files
            List<SelectablePackage> selectedPackages = FlatListSelectablePackages.Where(package => package.Checked).ToList();
            List<SelectablePackage> packagesWithData = selectedPackages.Where(package => package.UserFiles.Count > 0).ToList();
            //and reset the stopwatch
            InstallStopWatch.Reset();

            //step 1 on install: backup user mods
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog("Backup of mods, current install time = 0 msec");
            if (ModpackSettings.BackupModFolder)
            {
                if (!await BackupModsAsync())
                {
                    ReportProgress();
                    ReportFinish();
                    return;
                }
                Logging.WriteToLog(string.Format("Backup of mods complete, took {0}msec",
                    InstallStopWatch.Elapsed.TotalMilliseconds - OldTime.TotalMilliseconds));
            }
            else
                Logging.WriteToLog("...skipped");

            //step 2: backup data
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog(string.Format("Backup of userdata, current install time = {0} msec",
                InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.SaveUserData)
            {
                
            }
            else
                Logging.WriteToLog("...skipped");

            //step 3: clear cache
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog(string.Format("Cleaning of cache folders, current install time = {0} msec",
                InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.ClearCache)
            {

            }
            else
                Logging.WriteToLog("...skipped");

            //step 4: clear logs
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog(string.Format("Cleaning of logs, current install time = {0} msec",
                InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.DeleteLogs)
            {

            }
            else
                Logging.WriteToLog("...skipped");

            //step 5: clean mods folders
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog(string.Format("Cleaning of mods foldres, current install time = {0} msec",
                InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.CleanInstallation)
            {

            }
            else
                Logging.WriteToLog("...skipped");

            //step 6: extract mods
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog(string.Format("Exctacting mods, current install time = {0} msec",
                InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.CleanInstallation)
            {

            }
            else
                Logging.WriteToLog("...skipped?");

            //step 7: restore data
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog(string.Format("Restore of userdata, current install time = {0} msec",
                InstallStopWatch.Elapsed.TotalMilliseconds));
            //if statement TODO
            if (ModpackSettings.SaveUserData)
            {

            }
            else
                Logging.WriteToLog("...skipped");

            //step 8: patch files (async option)
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog(string.Format("Cleaning of mods foldres, current install time = {0} msec",
                InstallStopWatch.Elapsed.TotalMilliseconds));
            //if statement TODO
            if (ModpackSettings.CleanInstallation)
            {

            }
            else
                Logging.WriteToLog("...skipped");

            //step 9: create create shortcuts (async option)
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog(string.Format("Creating of shortcuts, current install time = {0} msec",
                InstallStopWatch.Elapsed.TotalMilliseconds));
            if (ModpackSettings.CreateShortcuts)
            {

            }
            else
                Logging.WriteToLog("...skipped");

            //step 10: create atlasas (async option)
            OldTime = InstallStopWatch.Elapsed;
            Progress.TotalCurrent++;
            InstallFinishedEventArgs.ExitCodes++;
            Logging.WriteToLog(string.Format("Creating of atlases, current install time = {0} msec",
                InstallStopWatch.Elapsed.TotalMilliseconds));
            //if statement TODO
            if (ModpackSettings.CleanInstallation)
            {

            }
            else
                Logging.WriteToLog("...skipped");

            //barrier goes here to make sure cleanup is the last thing to do

            //step 9: cleanup (whatever that implies lol)

            //and i guess we're done
        }

        private async Task<bool> BackupModsAsync()
        {
            //check first if the directory exists first
            if (!Directory.Exists(Settings.RelhaxModBackupFolder))
                Directory.CreateDirectory(Settings.RelhaxModBackupFolder);
            //create the directory for this version to backup to
            string folderDateName = string.Format("{0:yyyy-MM-dd-HH-mm-ss}", DateTime.Now);
            string fullFolderPath = Path.Combine(Settings.RelhaxModBackupFolder, folderDateName);
            if (Directory.Exists(fullFolderPath))
                throw new BadMemeException("how does this already exist, like seriously");
            Directory.CreateDirectory(fullFolderPath);
            //make zip files maybe? TODO
            if (false)
            { 
                using (Ionic.Zip.ZipFile backupZip = new Ionic.Zip.ZipFile(fullFolderPath + ".zip"))
                {
                    backupZip.AddDirectory(Path.Combine(Settings.WoTDirectory, "mods"));
                    backupZip.AddDirectory(Path.Combine(Settings.WoTDirectory, "res_mods"));
                    backupZip.Save();
                }
            }
            else
            {
                //copy the files over
                //progress TODO
                Utils.DirectoryCopy(Path.Combine(Settings.WoTDirectory, "mods"), Path.Combine(fullFolderPath, "mods"),true);
                Utils.DirectoryCopy(Path.Combine(Settings.WoTDirectory, "res_mods"), Path.Combine(fullFolderPath, "res_mods"), true);
            }
            return true;
        }

        private async Task<bool> BackupDataAsync(List<SelectablePackage> packagesWithData)
        {
            foreach(SelectablePackage package in packagesWithData)
            {
                Logging.WriteToLog(string.Format("Backup data of package {0} starting", package.PackageName));
                foreach(UserFiles files in package.UserFiles)
                {
                    //use the search parameter to get the actual files to move
                    //remove the mistake i made over a year ago of the double slashes
                    string searchPattern = files.Pattern.Replace(@"\\", @"\");
                    //legacy compatibility: eithor the item will be with a macro start or not.
                    //they need to be treated differently
                    string root_directory = "";
                    string actual_search = "";
                    if (searchPattern[0].Equals('{'))
                    {
                        //it has macros, and the first one is the base path. remove and macro it
                        //root_directory = macro using split of }
                    }
                    else
                    {
                        //it's an old style (or at least can assume that it's assuming root WoT direcotry
                        root_directory = Settings.WoTDirectory;
                        //actual_search = Utils.MacroParse();

                    }
                    throw new BadMemeException("TODO");
                    files.Files_saved = Directory.GetFiles(root_directory, actual_search, SearchOption.AllDirectories).ToList();
                    //make root directory to store the temp files
                    if(files.Files_saved.Count > 0)
                    {
                        if (!Directory.Exists(Path.Combine(Settings.RelhaxTempFolder, package.PackageName)))
                            Directory.CreateDirectory(Path.Combine(Settings.RelhaxTempFolder, package.PackageName));
                        foreach(string s in files.Files_saved)
                        {
                            File.Move(s, Path.Combine(Path.Combine(Settings.RelhaxTempFolder, package.PackageName, Path.GetFileName(s))));
                        }
                    }
                }
            }
            return true;
        }

        private async Task<bool> ClearCacheAsync()
        {
            //make sure that the app data folder exists
            //if it does not, then it does not need to run this
            if(!Directory.Exists(Settings.AppDataFolder))
            {
                Logging.WriteToLog("Appdata folder does not exist, creating");
                Directory.CreateDirectory(Settings.AppDataFolder);
                return true;
            }
            Logging.WriteToLog("Appdata folder exists, backing up user settings and clearing cache");
            //make the temp folder if it does not already exist
            string AppPathTempFolder = Path.Combine(Settings.RelhaxTempFolder, "AppDataBackup");
            if (Directory.Exists(AppPathTempFolder))
                Directory.Delete(AppPathTempFolder, true);
            Directory.CreateDirectory(AppPathTempFolder);
            string[] fileFolderNames = { "preferences.xml", "preferences_ct.xml", "modsettings.dat", "xvm", "pmod" };
            //check if the directories are files or folders
            //if files they can move directly
            //if folders they have to be re-created on the destination and files moved manually
            Logging.WriteToLog("Starting clearing cache step 1 of 3: backing up old files", Logfiles.Application, LogLevel.Debug);
            foreach(string file in fileFolderNames)
            {
                Logging.WriteToLog("Processing cache file/folder to move: " + file, Logfiles.Application, LogLevel.Debug);
                if(File.Exists(Path.Combine(Settings.AppDataFolder, file)))
                {
                    File.Move(Path.Combine(Settings.AppDataFolder, file), Path.Combine(AppPathTempFolder, file));
                }
                else if (Directory.Exists(Path.Combine(Settings.AppDataFolder, file)))
                {
                    //Utils.DirectoryMove(Path.Combine(Settings.AppDataFolder, file,) Path.Combine(AppPathTempFolder, file), true, null);
                }
                else
                {
                    Logging.WriteToLog("File or folder does not exist in step clearCache: " + file,Logfiles.Application,LogLevel.Debug);
                }
            }

            //now delete the temp folder
            Logging.WriteToLog("Starting clearing cache step 2 of 3: actually clearing cache", Logfiles.Application, LogLevel.Debug);
            Directory.Delete(Settings.AppDataFolder, true);

            //then put the above files back
            Logging.WriteToLog("Starting clearing cache step 3 of 3: restoring old files", Logfiles.Application, LogLevel.Debug);
            Directory.CreateDirectory(Settings.AppDataFolder);
            foreach (string file in fileFolderNames)
            {
                Logging.WriteToLog("Processing cache file/folder to move: " + file, Logfiles.Application, LogLevel.Debug);
                if (File.Exists(Path.Combine(AppPathTempFolder, file)))
                {
                    File.Move(Path.Combine(AppPathTempFolder, file), Path.Combine(Settings.AppDataFolder, file));
                }
                else if (Directory.Exists(Path.Combine(AppPathTempFolder, file)))
                {
                    //Utils.DirectoryMove(Path.Combine(AppPathTempFolder, file), Path.Combine(Settings.AppDataFolder, file), true, null);
                    Utils.DirectoryMove(Path.Combine(AppPathTempFolder, file), Path.Combine(Settings.AppDataFolder, file), true);
                }
                else
                {
                    Logging.WriteToLog("File or folder does not exist in step clearCache: " + file, Logfiles.Application, LogLevel.Debug);
                }
            }
            return true;
        }

        private async Task<bool> ClearLogsAsync()
        {
            string[] logsToDelete = new string[]
            {
                Path.Combine(Settings.WoTDirectory, "python.log"),
                Path.Combine(Settings.WoTDirectory, "xvm.log"),
                Path.Combine(Settings.WoTDirectory, "pmod.log"),
                Path.Combine(Settings.WoTDirectory, "WoTLauncher.log"),
                Path.Combine(Settings.WoTDirectory, "cef.log")
            };
            foreach(string s in logsToDelete)
            {
                Logging.WriteToLog("Processing log file (if exists) " + s, Logfiles.Application, LogLevel.Debug);
                if (File.Exists(s))
                    File.Delete(s);
            }
            return true;
        }

        private async Task<bool> ClearModsFoldersAsync()
        {

            return true;
        }

        private async Task<bool> ExtractFilesAsync()
        {
            //this is the only really new one. for each install group, spawn a bunch of threads to start the instal process
            //get the number of threads we will use for each of the install stepps
            int numThreads = ModpackSettings.MulticoreExtraction ? Settings.NumLogicalProcesors : 1;
            Logging.WriteToLog(string.Format("Number of threads to use for install is {0}, (MulticoreExtraction={1}, LogicalProcesosrs={2}",
                numThreads, ModpackSettings.MulticoreExtraction, Settings.NumLogicalProcesors));
            for (int i = 0; i < OrderedPackagesToInstall.Count(); i++)
            {
                Logging.WriteToLog("Install Group " + i + " starts now");
                //get the list of packages to install
                List<DatabasePackage> packages = new List<DatabasePackage>(OrderedPackagesToInstall[i]);
                //this list represents all te packages in this group that can all be installed at the same time
                //i.e. there are NO conflicting zip file paths in ALL of the files, in other works all the files are mutually exclusive

                //first sort the packages by the size parameter
                //https://stackoverflow.com/questions/3309188/how-to-sort-a-listt-by-a-property-in-the-object
                //TODO: the size of all objects MUST BE KNOWN??
                //List<DatabasePackage> packagesSortedBySize = packages.OrderByDescending(pack => pack.)
                //for not just go with the packages as they are, they should alrady be in alphabetical order

                //make a list of packages again, but size is based on number of logical processors and/or multicore install mdos
                //if a user has 8 cores, then make a lists of packages to install
                List<DatabasePackage>[] packageThreads = new List<DatabasePackage>[numThreads];
                //assign each package one at a time into a package thread
                for(int j = 0; j < packages.Count; j++)
                {
                    int threadSelector = j % numThreads;
                    packageThreads[threadSelector].Add(packages[j]);
                    Logging.WriteToLog(string.Format("j index = {0} package {1} has been assigned to packageThread {2}", j, packages[j].PackageName,
                        threadSelector), Logfiles.Application, LogLevel.Debug);
                }
                //now the fun starts. these all can run at once
                //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming
                Task[] tasks = new Task[numThreads];
                if (tasks.Count() != packageThreads.Count())
                    throw new BadMemeException("ohhhhhhhhh, NOW you f*cked UP!");
                for(int k = 0; k < tasks.Count(); k++)
                {
                    Logging.WriteToLog(string.Format("thread {0} starting task, packages to extract={1}", k, packageThreads[k].Count));
                    //tasks[k] = ExtractFilesAsync(packageThreads[k], k);
                    //use the task factory to create tasks(threads) for each logical cores
                    tasks[k] = Task.Factory.StartNew(() => ExtractFiles(packageThreads[k], k));
                }
                Logging.WriteToLog(string.Format("all threads started on group {0}, master thread now waiting on Task.WaitAll(tasks)",i),
                    Logfiles.Application,LogLevel.Debug);
                Task.WaitAll(tasks);
                Logging.WriteToLog("Install Group " + i + " finishes now");
            }
            return true;
        }

        private void ExtractFiles(List<DatabasePackage> packagesToExtract, int threadNum)
        {
            bool notAllPackagesExtracted = true;
            //in case the user selected to "download and install at the same time", there may be cases where
            //some items in this list (earlier, for sake of areugment) are not downloaded yet, but others below are.
            //if this is the case, then we need to skip over those items for now while they download in the background
            while(notAllPackagesExtracted)
            {
                int numExtracted = 0;
                foreach (DatabasePackage package in packagesToExtract)
                {
                    if(ModpackSettings.DownloadInstantExtraction && package.DownloadFlag)
                    {
                        continue;
                    }
                    else
                    {
                        Logging.WriteToLog("Thread ID=" + threadNum + ", starting extraction of " + package.ZipFile);
                        numExtracted++;
                        Unzip(package);
                    }
                }
                if (numExtracted == packagesToExtract.Count)
                    notAllPackagesExtracted = false;
                else
                    System.Threading.Thread.Sleep(200);
            }
            
        }

        private void Unzip(DatabasePackage package)
        {
            //do any zip file processing, then extract
            if (string.IsNullOrWhiteSpace(package.ZipFile))
                throw new BadMemeException("REEEEEEEE");

        }

        private async Task<bool> RestoreDataAsync(List<SelectablePackage> packagesWithData)
        {
            foreach (SelectablePackage package in packagesWithData)
            {
                Logging.WriteToLog(string.Format("Restore data of package {0} starting", package.PackageName));
                //check if the package name exists first
                string tempBackupFolder = Path.Combine(Settings.RelhaxTempFolder, package.PackageName);
                if(!Directory.Exists(tempBackupFolder))
                {
                    Logging.WriteToLog(string.Format("folder {0} does not exist, skipping", package.PackageName), Logfiles.Application, LogLevel.Debug);
                }
                foreach (UserFiles files in package.UserFiles)
                {
                    foreach(string savedFile in files.Files_saved)
                    {
                        string filePath = Path.Combine(Settings.RelhaxTempFolder, package.PackageName, Path.GetFileName(savedFile));
                        if(File.Exists(filePath))
                        {
                            Logging.WriteToLog(string.Format("Restoring file {0} of {1}", Path.GetFileName(savedFile), package.PackageName));
                            if (!Directory.Exists(Path.GetDirectoryName(savedFile)))
                                Directory.CreateDirectory(Path.GetDirectoryName(savedFile));
                            if (File.Exists(savedFile))
                                File.Delete(savedFile);
                            File.Move(filePath, savedFile);
                        }
                    }
                }
            }
            return true;
        }

        private async Task<bool> PatchFilesAsync()
        {

            return true;
        }

        private async Task<bool> CreateShortcutsAsync()
        {

            return true;
        }

        private async Task<bool> CreateAtlasesAsync()
        {

            return true;
        }

        private bool Cleanup()
        {

            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

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
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
