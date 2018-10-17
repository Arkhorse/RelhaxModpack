﻿using RelhaxModpack.DatabaseComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace RelhaxModpack
{
    public partial class DatabaseUpdater : RelhaxForum
    {
        private const string databaseUpdateTxt = "databaseUpdate.txt";
        private const string databaseUpdateBackupTxt = "databaseUpdate.txt.bak";
        private const string level1Password = "RelhaxReadOnly2018";
        private const string L2keyFileName = "L2Key.txt";
        private const string L3keyFileName = "L3key.txt";
        private const string L2KeyAddress = "aHR0cDovL3dvdG1vZHMucmVsaGF4bW9kcGFjay5jb20vUmVsaGF4TW9kcGFjay9SZXNvdXJjZXMvZXh0ZXJuYWwvTDJLZXkudHh0";
        private const string L3KeyAddress = "aHR0cDovL3dvdG1vZHMucmVsaGF4bW9kcGFjay5jb20vUmVsaGF4TW9kcGFjay9SZXNvdXJjZXMvZXh0ZXJuYWwvTDNrZXkudHh0";
        private const string ModpackUsername = "modpack@wotmods.relhaxmodpack.com";
        private const string ModpackPassword = "QjFZLi0zaGxsTStY";
        private const string modInfoBackupsFolderLocation = "ftp://wotmods.relhaxmodpack.com/RelhaxModpack/Resources/modInfoBackups/";
        private const string modInfosLocation = "ftp://wotmods.relhaxmodpack.com/RelhaxModpack/Resources/modInfo/";
        private const string ftpModpackRoot = "ftp://wotmods.relhaxmodpack.com/RelhaxModpack/";
        private const string ftpRoot = "ftp://wotmods.relhaxmodpack.com/";
        private const string WotFolderRoot = "ftp://wotmods.relhaxmodpack.com/WoT/";
        private const string supportedClients = "supported_clients.xml";
        private const string managerVersion = "manager_version.xml";
        private enum AuthLevel
        {
            None=0,
            View=1,
            UpdateDatabase=2,
            Admin=3
        }
        private AuthLevel CurrentAuthLevel = AuthLevel.None;
        private WebClient downloader;
        private NetworkCredential credentials;
        private List<Dependency> globalDependencies = new List<Dependency>();
        private List<Dependency> dependencies = new List<Dependency>();
        private List<LogicalDependency> logicalDependencies = new List<LogicalDependency>();
        private List<Category> parsedCatagoryList = new List<Category>();
        private List<DatabasePackage> addedPackages = new List<DatabasePackage>();
        private List<DatabasePackage> updatedPackages = new List<DatabasePackage>();
        private List<DatabasePackage> disabledPackages = new List<DatabasePackage>();
        private List<DatabasePackage> removedPackages = new List<DatabasePackage>();
        private List<DatabasePackage> fileNotFoundPackages = new List<DatabasePackage>();
        private List<DatabasePackage> live_modInfo = new List<DatabasePackage>();
        private List<DatabasePackage> updated_modInfo = new List<DatabasePackage>();
        private StringBuilder globalDepsSB = new StringBuilder();
        private StringBuilder dependenciesSB = new StringBuilder();
        private StringBuilder logicalDependenciesSB = new StringBuilder();
        private StringBuilder packagesSB = new StringBuilder();
        private StringBuilder filesNotFoundSB = new StringBuilder();
        private StringBuilder databaseUpdateText = new StringBuilder();
        private Hashtable HelpfullMessages = new Hashtable();
        //for decoding the password upon application load
        private string ModpackPasswordDecoded = "";
        //the current latest version of the modpack database file. formed in step 3
        private string currentModInfoxml = "";
        //the "version" used for the new database update
        private string parsedDBUpdateVersion = "";
        //the parsed name for the backup file for backing up the old modinfo database file
        private string backupModInfoxmlToServer = "";
        //the modInfoxml downloaded and uesd for before and after comparison
        private string compareModInfoXml = "";

        //from databaseListGenerator
        //create and reset internal variables
        StringBuilder sb = new StringBuilder();
        string packageDisplayName = "";
        string category = "";
        string packageName = "N/A";
        string notApplicatable = "n/a";
        string zipfile = "";
        bool enabled = false;
        bool visible = false;
        string devURL = "";
        string header;


        public DatabaseUpdater()
        {
            InitializeComponent();
        }

        private void OnAuthLevelChange(AuthLevel al)
        {
            CurrentAuthLevel = al;
            //disable everything first, then enable via falling case statements
            foreach (Control c in UpdateDatabaseTab.Controls)
                c.Enabled = false;
            foreach (Control c in UpdateApplicationTab.Controls)
                c.Enabled = false;
            foreach (Control c in CleanOnlineFolders.Controls)
                c.Enabled = false;
            foreach (Control c in CreatePasswordTab.Controls)
                c.Enabled = false;
            if((int)CurrentAuthLevel > 1)
            {
                //ability to update database
                foreach (Control c in UpdateDatabaseTab.Controls)
                    c.Enabled = true;
            }
            if((int)CurrentAuthLevel > 2)
            {
                //admin
                foreach (Control c in UpdateApplicationTab.Controls)
                    c.Enabled = true;
                foreach (Control c in CleanOnlineFolders.Controls)
                    c.Enabled = true;
                foreach (Control c in CreatePasswordTab.Controls)
                    c.Enabled = false;
            }
            AuthStatusLabel.Text = CurrentAuthLevel.ToString();
        }

        private void CRCFileSizeUpdate_Load(object sender, EventArgs e)
        {
            loadDatabaseDialog.InitialDirectory = Application.StartupPath;
            ModpackPasswordDecoded = Utils.Base64Decode(ModpackPassword);
            credentials = new NetworkCredential(ModpackUsername, ModpackPasswordDecoded);
            FillHashTable();
            //DEBUG ONLY
            //OnAuthLevelChange(AuthLevel.Admin);
            AuthStatusLabel.Text = CurrentAuthLevel.ToString();
            //if using a key file then open it and test for l1, then l2, then l3
            if(Program.updateFileKey)
            {
                Logging.Manager("attempting to use key from file " + Program.updateKeyFile);
                if(File.Exists(Program.updateKeyFile))
                {
                    string key = File.ReadAllText(Program.updateKeyFile);

                    //attempt l1 key authorization
                    Logging.Manager("Attempting l1 key authorization");
                    if (key.Equals(level1Password))
                    {
                        Logging.Manager("success");
                        OnAuthLevelChange(AuthLevel.View);
                    }

                    if(CurrentAuthLevel == AuthLevel.None)
                    {
                        Logging.Manager("failed, attempting l2");
                        //attempt l2 key authorization
                        string downloadedKey = "";
                        using (downloader = new WebClient())
                        {
                            downloadedKey = Utils.Base64Decode(downloader.DownloadString(Utils.Base64Decode(L2KeyAddress)));
                        }
                        if (downloadedKey.Equals(key))
                        {
                            Logging.Manager("success");
                            OnAuthLevelChange(AuthLevel.UpdateDatabase);
                        }
                        if(CurrentAuthLevel == AuthLevel.None)
                        {
                            Logging.Manager("failed, attempting l3");
                            //attempt l3 key authorization
                            downloadedKey = "";
                            using (downloader = new WebClient())
                            {
                                downloadedKey = Utils.Base64Decode(downloader.DownloadString(Utils.Base64Decode(L3KeyAddress)));
                            }
                            if (downloadedKey.Equals(key))
                            {
                                Logging.Manager("success");
                                OnAuthLevelChange(AuthLevel.Admin);
                            }
                        }
                    }
                }
                else
                {
                    Logging.Manager("key file does not exist, aborting");
                }
            }
        }

        #region Database Updating
        private void UpdateDatabaseStep1_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update database step 1...");
            if (loadDatabaseDialog.ShowDialog() == DialogResult.Cancel)
                return;
            DatabaseLocationTextBox.Text = loadDatabaseDialog.FileName;
            //for the folder version: //modInfoAlpha.xml/@version
            Settings.TanksVersion = XMLUtils.GetXMLElementAttributeFromFile(DatabaseLocationTextBox.Text, "//modInfoAlpha.xml/@version");
            //for the onlineFolder version: //modInfoAlpha.xml/@onlineFolder
            Settings.TanksOnlineFolderVersion = XMLUtils.GetXMLElementAttributeFromFile(DatabaseLocationTextBox.Text, "//modInfoAlpha.xml/@onlineFolder");
            Text = String.Format("DatabaseUpdateUtility      TanksVersion: {0}    OnlineFolder: {1}", Settings.TanksVersion, Settings.TanksOnlineFolderVersion);
            ReportProgress("Settings.TanksVersion (current game version): " + Settings.TanksVersion);
            ReportProgress("Settings.TanksOnlineFolderVersion (online zip folder): " + Settings.TanksOnlineFolderVersion);
        }

        private void UpdateDatabaseStep2_client_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update database step 2b...");
            // check for database
            if (!File.Exists(DatabaseLocationTextBox.Text))
            {
                ReportProgress("ERROR: managerInfo xml not found! (did you run the previous steps?)");
                return;
            }
            //load current database.xml from online folder
            ReportProgress(string.Format("Loading database.xml from online folder {0}", Settings.TanksOnlineFolderVersion));
            XmlDocument database_xml = new XmlDocument();
            using (WebClient client = new WebClient())
            {
                database_xml.LoadXml(client.DownloadString(string.Format("http://wotmods.relhaxmodpack.com/WoT/{0}/database.xml",Settings.TanksOnlineFolderVersion)));
            }
            //make string list of file names from database.xml
            ReportProgress("Creating string list of names from database.xml");
            List<string> removed_files = new List<string>();
            XmlNode Database_elements = database_xml.LastChild;
            foreach(XmlNode file in Database_elements.ChildNodes)
            {
                removed_files.Add(file.Attributes["name"].Value);
            }
            //load xml string from php script of listing all files in folder
            ReportProgress("Loading list of files in online folder");
            XmlDocument files_in_folder = new XmlDocument();
            using (WebClient client = new WebClient())
            {
                files_in_folder.LoadXml(client.DownloadString(string.Format("http://wotmods.relhaxmodpack.com/scripts/GetZipFiles.php?folder={0}",Settings.TanksOnlineFolderVersion)));
            }
            XmlNode all_files_in_folder = files_in_folder.LastChild;
            List<string> added_files = new List<string>();
            List<string> updated_files = new List<string>();
            List<string> error_files = new List<string>();
            //for each file name:
            int progress = 0;
            int total = all_files_in_folder.ChildNodes.Count;
            StringBuilder summary = new StringBuilder();
            System.Diagnostics.Stopwatch time_for_each_file = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch total_time_php_processing = new System.Diagnostics.Stopwatch();
            total_time_php_processing.Restart();
            foreach (XmlNode file_in_folder in all_files_in_folder)
            {
                ReportProgress(string.Format("Parsing file {0} ({1} of {2})", file_in_folder.Attributes["name"].Value, ++progress, total));
                time_for_each_file.Restart();
                //if exists in string list of database.xml, remove it -> serves as removed files
                if (removed_files.Contains(file_in_folder.Attributes["name"].Value))
                {
                    removed_files.Remove(file_in_folder.Attributes["name"].Value);
                }
                //check if filename is in database.xml, if not, then it's new
                //attribute example: "//root/element/@attribute"
                XmlNode file_in_database_xml = database_xml.SelectSingleNode(string.Format("//database/file[@name = \"{0}\"]", file_in_folder.Attributes["name"].Value));
                if (file_in_database_xml == null)
                {
                    //add
                    //get all file properties
                    XmlDocument file_properties_root = new XmlDocument();
                    using (WebClient client = new WebClient())
                    {
                        string downloaded_xml_string = "";
                        try
                        {
                            downloaded_xml_string = client.DownloadString(string.Format("http://wotmods.relhaxmodpack.com/scripts/GetFileProperties.php?folder={0}&file={1}&getMD5=1", Settings.TanksOnlineFolderVersion, file_in_folder.Attributes["name"].Value));
                            file_properties_root.LoadXml(downloaded_xml_string);
                        }
                        catch (Exception ex)
                        {
                            //https://stackoverflow.com/questions/22094165/timespan-formatting-to-minutes-ans-seconds-without-hours
                            string elapsed_time2 = string.Format(" Took {0}.{1} sec", (int)time_for_each_file.Elapsed.TotalSeconds, time_for_each_file.Elapsed.Milliseconds);
                            ReportProgress("Failed" + elapsed_time2);
                            ReportProgress(ex.ToString());
                            if (!string.IsNullOrWhiteSpace(downloaded_xml_string))
                                ReportProgress(downloaded_xml_string);
                            summary.AppendLine("[ERROR] " + file_in_folder.Attributes["name"].Value);
                            continue;
                        }
                    }
                    XmlNode file_properties = file_properties_root.LastChild.LastChild;
                    //add node to database xml
                    //create element
                    XmlElement file_to_add_to_database_xml = database_xml.CreateElement("file");
                    //create attributes
                    XmlAttribute file_name = database_xml.CreateAttribute("name");
                    file_name.Value = file_properties.Attributes["name"].Value;
                    XmlAttribute file_size = database_xml.CreateAttribute("size");
                    file_size.Value = file_properties.Attributes["size"].Value;
                    XmlAttribute file_md5 = database_xml.CreateAttribute("md5");
                    file_md5.Value = file_properties.Attributes["MD5"].Value;
                    //NEW: add the timestamp to database.xml
                    XmlAttribute file_time = database_xml.CreateAttribute("time");
                    file_time.Value = file_properties.Attributes["time"].Value;
                    //add attributes to new element
                    file_to_add_to_database_xml.Attributes.Append(file_name);
                    file_to_add_to_database_xml.Attributes.Append(file_size);
                    file_to_add_to_database_xml.Attributes.Append(file_md5);
                    file_to_add_to_database_xml.Attributes.Append(file_time);
                    //add element to database xml
                    database_xml.LastChild.AppendChild(file_to_add_to_database_xml);
                    //make UI and log updates
                    summary.AppendLine("[NEW] " + file_in_folder.Attributes["name"].Value);
                    added_files.Add(file_in_folder.Attributes["name"].Value);
                    string elapsed_time = string.Format(" Took {0}.{1} sec", (int)time_for_each_file.Elapsed.TotalSeconds, time_for_each_file.Elapsed.Milliseconds);
                    ReportProgress("Added" + elapsed_time);
                }
                else
                {
                    //update
                    //get file info without md5 check (faster)
                    XmlDocument file_properties_root = new XmlDocument();
                    using (WebClient client = new WebClient())
                    {
                        string downloaded_xml_string = "";
                        try
                        {
                            downloaded_xml_string = client.DownloadString(string.Format("http://wotmods.relhaxmodpack.com/scripts/GetFileProperties.php?folder={0}&file={1}&getMD5=0", Settings.TanksOnlineFolderVersion, file_in_folder.Attributes["name"].Value));
                            file_properties_root.LoadXml(downloaded_xml_string);
                        }
                        catch (Exception ex)
                        {
                            string elapsed_time = string.Format(" Took {0}.{1} sec", (int)time_for_each_file.Elapsed.TotalSeconds, time_for_each_file.Elapsed.Milliseconds);
                            ReportProgress("Failed" + elapsed_time);
                            ReportProgress(ex.ToString());
                            if (!string.IsNullOrWhiteSpace(downloaded_xml_string))
                                ReportProgress(downloaded_xml_string);
                            summary.AppendLine("[ERROR] " + file_in_folder.Attributes["name"].Value);
                            continue;
                        }
                    }
                    XmlNode file_properties = file_properties_root.LastChild.LastChild;
                    //check if time not equal
                    bool force_md5_check = false;//change this when want to check slowly for all MD5 rather than just 
                    //if file time attribute is not there, then add it
                    XmlAttribute file_time_from_database_xml = file_in_database_xml.Attributes["time"];
                    if(file_time_from_database_xml == null)
                    {
                        file_time_from_database_xml = database_xml.CreateAttribute("time");
                        file_time_from_database_xml.Value = file_properties.Attributes["time"].Value;
                        file_in_database_xml.Attributes.Append(file_time_from_database_xml);
                    }
                    //check for morce md5 flag, size change or filetime change
                    XmlAttribute file_size_from_database_xml = file_in_database_xml.Attributes["size"];
                    XmlAttribute file_size_from_properties_xml = file_properties.Attributes["size"];
                    XmlAttribute file_time_from_properties_xml = file_properties.Attributes["time"];
                    if ((force_md5_check) ||
                        (!file_time_from_database_xml.Value.Equals(file_time_from_properties_xml.Value)) ||
                        (!file_size_from_database_xml.Value.Equals(file_size_from_properties_xml.Value)))
                    {
                        //update size and time first
                        file_time_from_database_xml.Value = file_time_from_properties_xml.Value;
                        file_size_from_database_xml.Value = file_size_from_properties_xml.Value;
                        //get the md5 this time
                        file_properties_root = new XmlDocument();
                        using (WebClient client = new WebClient())
                        {
                            string downloaded_xml_string = "";
                            try
                            {
                                downloaded_xml_string = client.DownloadString(string.Format("http://wotmods.relhaxmodpack.com/scripts/GetFileProperties.php?folder={0}&file={1}&getMD5=1", Settings.TanksOnlineFolderVersion, file_in_folder.Attributes["name"].Value));
                                file_properties_root.LoadXml(downloaded_xml_string);
                            }
                            catch (Exception ex)
                            {
                                string elapsed_time = string.Format(" Took {0}.{1} sec", (int)time_for_each_file.Elapsed.TotalSeconds, time_for_each_file.Elapsed.Milliseconds);
                                ReportProgress("Failed" + elapsed_time);
                                ReportProgress(ex.ToString());
                                if (!string.IsNullOrWhiteSpace(downloaded_xml_string))
                                    ReportProgress(downloaded_xml_string);
                                summary.AppendLine("[ERROR] " + file_in_folder.Attributes["name"].Value);
                                continue;
                            }
                        }
                        file_properties = file_properties_root.LastChild.LastChild;
                        //update if MD5 not equal
                        if (!(file_properties.Attributes["MD5"].Value.Equals(file_in_database_xml.Attributes["md5"].Value)))
                        {
                            file_in_database_xml.Attributes["md5"].Value = file_properties.Attributes["MD5"].Value;
                            //add it to list of updated file names
                            updated_files.Add(file_in_folder.Attributes["name"].Value);
                            summary.AppendLine("[UPDATE] " + file_in_folder.Attributes["name"].Value);
                            string elapsed_time = string.Format(" Took {0}.{1} sec", (int)time_for_each_file.Elapsed.TotalSeconds, time_for_each_file.Elapsed.Milliseconds);
                            ReportProgress("Updated" + elapsed_time);
                        }
                        else
                        {
                            string elapsed_time = string.Format(" Took {0}.{1} sec", (int)time_for_each_file.Elapsed.TotalSeconds, time_for_each_file.Elapsed.Milliseconds);
                            ReportProgress("No Change (md5 check)" + elapsed_time);
                        }
                    }
                    else
                    {
                        string elapsed_time = string.Format(" Took {0}.{1} sec", (int)time_for_each_file.Elapsed.TotalSeconds, time_for_each_file.Elapsed.Milliseconds);
                        ReportProgress("No Change (time/size check)" + elapsed_time);
                    }
                }
            }
            string elapsed_time3 = string.Format(" took {0}.{1} sec", (int)total_time_php_processing.Elapsed.TotalSeconds, total_time_php_processing.Elapsed.Milliseconds);
            ReportProgress("Total php file processing" + elapsed_time3);
            foreach(string s in removed_files)
            {
                //delete the entry in the database
                XmlNode file_in_database_xml = database_xml.SelectSingleNode(string.Format("//database/file[@name = \"{0}\"]", s));
                if(file_in_database_xml == null)
                {
                    //error
                }
                else
                {
                    Database_elements.RemoveChild(file_in_database_xml);
                }
                summary.AppendLine("[DELETE] " + s);
            }
            File.WriteAllText("update_files.log", summary.ToString());
            ReportProgress("results saved to update_files.log");
            database_xml.Save("database.xml");
            //upload it back
            using (WebClient client = new WebClient() { Credentials = credentials })
            {
                client.UploadFile(string.Format("ftp://wotmods.relhaxmodpack.com/WoT/{0}/database.xml", Settings.TanksOnlineFolderVersion), "database.xml");
            }
            File.Delete("database.xml");
            ReportProgress("database.xml uploaded to wot folder " + Settings.TanksOnlineFolderVersion);
        }

        private void UpdateDatabaseStep3_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update database step 3...");

            //clear variables for new update attempt
            currentModInfoxml = "";
            backupModInfoxmlToServer = "";
            parsedDBUpdateVersion = "";
            compareModInfoXml = "";

            // check for database
            if (!File.Exists(DatabaseLocationTextBox.Text))
            {
                ReportProgress("ERROR: managerInfo xml not found! (did you run the previous steps?)");
                return;
            }

            // download online database.xml
            ReportProgress("Downloading database.xml (list of crc zip file changes)");
            using (downloader = new WebClient())
            {
                string address = string.Format("http://wotmods.relhaxmodpack.com/WoT/{0}/database.xml", Settings.TanksOnlineFolderVersion);
                string fileName = Path.Combine(Settings.RelhaxTempFolder, Settings.OnlineDatabaseXmlFile);
                downloader.DownloadFile(address, fileName);
            }
            ReportProgress("database.xml downloaded");

            // set this flag, so getMd5Hash and getFileSize should parse downloaded online database.xml
            Program.databaseUpdateOnline = true;

            //prepare output stringBuilders
            ReportProgress("resetting lists in memory");
            filesNotFoundSB.Clear();
            globalDepsSB.Clear();
            dependenciesSB.Clear();
            logicalDependenciesSB.Clear();
            packagesSB.Clear();
            databaseUpdateText.Clear();
            filesNotFoundSB.Append("FILES NOT FOUND:\n");
            globalDepsSB.Append("\nGlobal Dependencies updated:\n");
            dependenciesSB.Append("\nDependencies updated:\n");
            logicalDependenciesSB.Append("\nLogical Dependencies updated:\n");
            packagesSB.Append("\nPackages updated:\n");

            //prepare Lists of database changes
            addedPackages.Clear();
            updatedPackages.Clear();
            disabledPackages.Clear();
            removedPackages.Clear();
            fileNotFoundPackages.Clear();
            live_modInfo.Clear();
            updated_modInfo.Clear();

            //prepare memory database lists
            globalDependencies.Clear();
            parsedCatagoryList.Clear();
            dependencies.Clear();
            logicalDependencies.Clear();

            //download current online modInfo.xml for list comparison
            ReportProgress("loading current modInfo.xml for later comparison");
            //make the current "to be supported" modinfo database file
            currentModInfoxml = "modInfo_" + Settings.TanksVersion + ".xml";
            string lastTanksVersion = "";
            using (downloader = new WebClient() { Credentials = credentials })
            {
                ReportProgress("download supported_clients.xml for latest database names");
                //need to get the latest supported version from supported_clients.xml
                if (File.Exists(supportedClients))
                    File.Delete(supportedClients);
                downloader.DownloadFile(ftpModpackRoot + "Resources/managerInfo/" + supportedClients, supportedClients);
                XmlDocument doc = new XmlDocument();
                doc.Load(supportedClients);
                XmlNodeList supported_clients = doc.SelectNodes("//versions/version");
                //last version is most recent supported
                XmlNode lastSupportred = supported_clients[supported_clients.Count - 1];
                //innerText is tanks version
                lastTanksVersion = lastSupportred.InnerText;
                compareModInfoXml = "modInfo_" + lastTanksVersion + ".xml";
                ReportProgress("compareModInfoXml= " + compareModInfoXml);
                downloader.DownloadFile(modInfosLocation + compareModInfoXml, compareModInfoXml);
                File.Delete(supportedClients);
            }
            //load live database to memory
            XMLUtils.CreateModStructure(compareModInfoXml, globalDependencies, dependencies, logicalDependencies, parsedCatagoryList,true);
            //create first list for comparison
            live_modInfo = CreatePackageList(parsedCatagoryList, globalDependencies, dependencies, logicalDependencies);

            //prepare memory database lists again
            globalDependencies.Clear();
            parsedCatagoryList.Clear();
            dependencies.Clear();
            logicalDependencies.Clear();
            //load actuall new database to memory
            XMLUtils.CreateModStructure(DatabaseLocationTextBox.Text, globalDependencies, dependencies, logicalDependencies, parsedCatagoryList,true);

            //check for duplicates
            int duplicatesCounter = 0;
            if (Utils.Duplicates(parsedCatagoryList) && Utils.DuplicatesPackageName(parsedCatagoryList, ref duplicatesCounter ))
            {
                MessageBox.Show(string.Format("{0} duplicates found !!!",duplicatesCounter));
                Program.databaseUpdateOnline = false;
                return;
            }

            //check if online folder exists first
            //get the list of all the folders based on game version
            ScriptLogOutput.AppendText("Checking if backup version folder exists...");
            string[] folders = FTPListFilesFolders(modInfoBackupsFolderLocation);
            if (!(folders.Contains(lastTanksVersion)))
            {
                ScriptLogOutput.AppendText("Does NOT exist, creating\n");
                //create the folder
                FTPMakeFolder(modInfoBackupsFolderLocation + lastTanksVersion);
            }
            else
            {
                ScriptLogOutput.AppendText("DOES exists\n");
            }

            //check it for if multiple itterations in one day
            //rename it to format to the new date
            //MUST use EST timezone
            ReportProgress("Making new filename with EST timezone, checking if currently on server");
            int itteraton = 1;
            DateTime dt = DateTime.UtcNow;
            TimeZoneInfo EST = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime realToday = TimeZoneInfo.ConvertTimeFromUtc(dt, EST);
            //create the filename for the backup mod info
            string newBackupModInfoFileName = "modInfo_" + lastTanksVersion + "_";
            string dateTimeFormat = string.Format("{0:yyyy-MM-dd}", realToday);
            string realNewBackupModInfoFileName = newBackupModInfoFileName + dateTimeFormat + "_" + itteraton + ".xml";
            bool fileAlreadyExists = true;
            while (fileAlreadyExists)
            {
                ReportProgress(string.Format("Checking if filename '{0}' exists in backup folder...", realNewBackupModInfoFileName));
                string[] modInfoBackups = FTPListFilesFolders(modInfoBackupsFolderLocation + lastTanksVersion + "/");
                if (modInfoBackups.Contains(realNewBackupModInfoFileName))
                {
                    ReportProgress("Exists, trying higher itteration");
                    realNewBackupModInfoFileName = newBackupModInfoFileName + dateTimeFormat + "_" + ++itteraton + ".xml";
                }
                else
                {
                    ReportProgress("Does not exist!");
                    fileAlreadyExists = false;
                    //actually update the global value here
                    backupModInfoxmlToServer = realNewBackupModInfoFileName;
                }
            }
            //create and the parsed database update version. needs to be made seperate from the backup above
            ReportProgress("Creating current database update version string");
            ReportProgress("Checking if folder exists for updated  tanks version");
            string[] newVersionFolders = FTPListFilesFolders(modInfoBackupsFolderLocation);
            if(!newVersionFolders.Contains(Settings.TanksVersion))
            {
                ReportProgress(Settings.TanksVersion + "backup folder does not exist on server, creating");
                FTPMakeFolder(modInfoBackupsFolderLocation + Settings.TanksVersion);
            }
            else
            {
                ReportProgress(Settings.TanksVersion + "backup folder DOES exist");

            }
            //create the string for the datbase update version
            itteraton = 1;
            string newCurrentModInfoFileName = "modInfo_" + Settings.TanksVersion + "_";
            string realNewCurrentModInfoFileName = newCurrentModInfoFileName + dateTimeFormat + "_" + itteraton + ".xml";
            fileAlreadyExists = true;
            while (fileAlreadyExists)
            {
                ReportProgress(string.Format("Checking if filename '{0}' exists in backup folder...", realNewCurrentModInfoFileName));
                string[] modInfoBackups = FTPListFilesFolders(modInfoBackupsFolderLocation + lastTanksVersion + "/");
                if (modInfoBackups.Contains(realNewCurrentModInfoFileName))
                {
                    ReportProgress("Exists, trying higher itteration");
                    realNewCurrentModInfoFileName = newCurrentModInfoFileName + dateTimeFormat + "_" + ++itteraton + ".xml";
                }
                else
                {
                    ReportProgress("Does not exist!");
                    fileAlreadyExists = false;
                    //actually update the global value here
                    parsedDBUpdateVersion = realNewCurrentModInfoFileName;
                }
            }
            parsedDBUpdateVersion = parsedDBUpdateVersion.Substring("modInfo_".Length);
            parsedDBUpdateVersion = parsedDBUpdateVersion.Substring(0, parsedDBUpdateVersion.Length - ".xml".Length);

            //report output
            ReportProgress("Current Global Parsed Strings:");
            ReportProgress("currentModInfoxml = " + currentModInfoxml);
            ReportProgress("backupModInfoxml = " + backupModInfoxmlToServer);
            ReportProgress("parsedDBUpdateVersion = " + parsedDBUpdateVersion);
            ReportProgress("compareModInfoXml = " + compareModInfoXml);

            //update the crc and filesize values
            ReportProgress("Updating database crc and filesize values...");
            string hash;
            //foreach zip file name
            //update the CRC value
            //update the file size
            //can also use this section to specify updated mods
            foreach (Dependency d in globalDependencies)
            {
                if (d.ZipFile.Trim().Equals(""))
                {
                    d.CRC = "";
                }
                else
                {
                    hash = XMLUtils.GetMd5Hash(d.ZipFile);
                    if (!d.CRC.Equals(hash))
                    {
                        d.CRC = hash;
                        if (!hash.Equals("f"))
                        {
                            globalDepsSB.Append(d.ZipFile + "\n");
                            updatedPackages.Add(d);
                        }
                    }
                    if (hash.Equals("f"))
                    {
                        filesNotFoundSB.Append(d.ZipFile + "\n");
                        fileNotFoundPackages.Add(d);
                    }
                }
            }
            foreach (Dependency d in dependencies)
            {
                if (d.ZipFile.Trim().Equals(""))
                {
                    d.CRC = "";
                }
                else
                {
                    hash = XMLUtils.GetMd5Hash(d.ZipFile);
                    if (!d.CRC.Equals(hash))
                    {
                        d.CRC = hash;
                        if (!hash.Equals("f"))
                        {
                            dependenciesSB.Append(d.ZipFile + "\n");
                            updatedPackages.Add(d);
                        }
                    }
                    if (hash.Equals("f"))
                    {
                        filesNotFoundSB.Append(d.ZipFile + "\n");
                        fileNotFoundPackages.Add(d);
                    }
                }
            }
            foreach (LogicalDependency d in logicalDependencies)
            {
                if (d.ZipFile.Trim().Equals(""))
                {
                    d.CRC = "";
                }
                else
                {
                    hash = XMLUtils.GetMd5Hash(d.ZipFile);
                    if (!d.CRC.Equals(hash))
                    {
                        d.CRC = hash;
                        if (!hash.Equals("f"))
                        {
                            logicalDependenciesSB.Append(d.ZipFile + "\n");
                            updatedPackages.Add(d);
                        }
                    }
                    if (hash.Equals("f"))
                    {
                        filesNotFoundSB.Append(d.ZipFile + "\n");
                        fileNotFoundPackages.Add(d);
                    }
                }
            }
            foreach (Category c in parsedCatagoryList)
            {
                foreach (SelectablePackage m in c.Packages)
                {
                    if (m.ZipFile.Trim().Equals(""))
                    {
                        m.CRC = "";
                    }
                    else
                    {
                        m.Size = getFileSize(m.ZipFile);
                        hash = XMLUtils.GetMd5Hash(m.ZipFile);
                        if (!m.CRC.Equals(hash))
                        {
                            m.CRC = hash;
                            if (!hash.Equals("f"))
                            {
                                packagesSB.Append(m.ZipFile + "\n");
                                updatedPackages.Add(m);
                            }
                        }
                        if (hash.Equals("f"))
                        {
                            filesNotFoundSB.Append(m.ZipFile + "\n");
                            fileNotFoundPackages.Add(m);
                        }
                    }
                    if (m.Packages.Count > 0)
                    {
                        processConfigsCRCUpdate(m.Packages);
                    }
                }
            }
            //abort if missing files
            if(fileNotFoundPackages.Count > 0)
            {
                string missingPackagesText = "missingPackages.txt";
                if (File.Exists(missingPackagesText))
                    File.Delete(missingPackagesText);
                StringBuilder missingPackagesSB = new StringBuilder();
                foreach(DatabasePackage sp in fileNotFoundPackages)
                {
                    missingPackagesSB.AppendLine(sp.ZipFile);
                }
                File.WriteAllText(missingPackagesText, missingPackagesSB.ToString());
                Program.databaseUpdateOnline = false;
                ReportProgress("ERROR: " + fileNotFoundPackages.Count + " packages missing files!! (saved to missingPackages.txt)");
                return;
            }

            //create the list for after the update for comparison
            ReportProgress("making comparisons for databaseUpdate.txt");
            updated_modInfo = CreatePackageList(parsedCatagoryList, globalDependencies, dependencies, logicalDependencies);
            //used for disabled, removed, added mods
            PackageComparer pc = new PackageComparer();
            //if in before but not after = removed
            removedPackages = live_modInfo.Except(updated_modInfo, pc).ToList();
            //if not in before but after = added
            addedPackages = updated_modInfo.Except(live_modInfo, pc).ToList();
            //query to find any disabled in before and after
            //list of disabed packages before
            List<DatabasePackage> disabledBefore = live_modInfo.Where(p => !p.Enabled).ToList();
            //list of disabled packages after
            List<DatabasePackage> disabledAfter = updated_modInfo.Where(p => !p.Enabled).ToList();
            //compare except with after.before
            disabledPackages = disabledAfter.Except(disabledBefore, pc).ToList();
            //also need to remove and removed and added and disabled from updated
            updatedPackages = updatedPackages.Except(removedPackages, pc).ToList();
            updatedPackages = updatedPackages.Except(disabledPackages, pc).ToList();
            updatedPackages = updatedPackages.Except(addedPackages, pc).ToList();
            Program.databaseUpdateOnline = false;
            ReportProgress(string.Format("Number of Added packages: {0}\nNumber of Updated packages: {1}\nNumber of Disabled packages: {2}\nNumber of Removed packages: {3}\n",
                addedPackages.Count,updatedPackages.Count,disabledPackages.Count,removedPackages.Count));
            ReportProgress("Done with processing and loading changes, can start next step");
        }

        private void UpdateDatabaseStep4_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update database step 4...");
            //check if previous steps have been run
            // check for database
            if (!File.Exists(DatabaseLocationTextBox.Text))
            {
                ReportProgress("ERROR: managerInfo xml (to update to server) not found! (did you run the previous steps?)");
                return;
            }
            if(string.IsNullOrWhiteSpace(parsedDBUpdateVersion))
            {
                ReportProgress("ERROR: parsedDBUpdateVersion is blank! (did you run the previous steps?)");
                return;
            }
            //at this point compareModInfoXml is the old databsae to backup
            if(!File.Exists(compareModInfoXml))
            {
                ReportProgress("ERROR: currentModInfoxml does not exist! (did you run the previous steps?)");
                return;
            }

            //backup the old live modInfo.xml
            ReportProgress("backing up current live modInfo.xml...");
            using (downloader = new WebClient() { Credentials = credentials })
            {
                File.Move(compareModInfoXml, backupModInfoxmlToServer);
                //upload backup
                downloader.UploadFile(modInfoBackupsFolderLocation + Settings.TanksVersion + "/" + backupModInfoxmlToServer, backupModInfoxmlToServer);
                File.Delete(backupModInfoxmlToServer);
            }

            //save and upload new modInfo file
            ReportProgress("saving and uploading new database");
            XMLUtils.SaveDatabase(DatabaseLocationTextBox.Text, Settings.TanksVersion, Settings.TanksOnlineFolderVersion,
                globalDependencies, dependencies, logicalDependencies, parsedCatagoryList);
            using (downloader = new WebClient() { Credentials = credentials })
            {
                //upload new modInfo
                downloader.UploadFile(modInfosLocation + currentModInfoxml, DatabaseLocationTextBox.Text);
            }

            //process old databaseUpdate.txt versions
            ReportProgress("creating databaseUpdate.txt file");
            if (File.Exists(databaseUpdateBackupTxt))
                File.Delete(databaseUpdateBackupTxt);
            if (File.Exists(databaseUpdateTxt))
            {
                ReportProgress("databaseUpdate.txt exists, saving backup");
                File.Move(databaseUpdateTxt, databaseUpdateBackupTxt);
            }

            //make stringbuilder of databaseUpdate.text
            databaseUpdateText.Clear();
            databaseUpdateText.Append("Database Update!\n\n");
            databaseUpdateText.Append("Updated: " + parsedDBUpdateVersion + "\n\n");
            databaseUpdateText.Append("Added:\n");
            foreach (DatabasePackage dp in addedPackages)
                databaseUpdateText.Append("-" + dp.CompletePath + "\n");
            databaseUpdateText.Append("\nUpdated:\n");
            foreach (DatabasePackage dp in updatedPackages)
                databaseUpdateText.Append("-" + dp.CompletePath + "\n");
            databaseUpdateText.Append("\nDisabled:\n");
            foreach (DatabasePackage dp in disabledPackages)
                databaseUpdateText.Append("-" + dp.CompletePath + "\n");
            databaseUpdateText.Append("\nRemoved:\n");
            foreach (DatabasePackage dp in removedPackages)
                databaseUpdateText.Append("-" + dp.CompletePath + "\n");
            databaseUpdateText.Append("\nNotes:\n-\n\n--------------------------------------------------------------------------------------------------------------------------------------------");

            //save and upload databaseUpdate.txt
            File.WriteAllText(databaseUpdateTxt, databaseUpdateText.ToString());
            ReportProgress("Database.text written and processed");

            //check if supported clients needs to be updated
            using (downloader = new WebClient() { Credentials = credentials })
            {
                //download and check if update needed
                ReportProgress("Checking if supported_clients.xml already has entry for this tanks version " + Settings.TanksVersion);
                if (File.Exists(supportedClients))
                    File.Delete(supportedClients);
                downloader.DownloadFile(ftpModpackRoot + "Resources/managerInfo/" + supportedClients, supportedClients);
                XmlDocument doc = new XmlDocument();
                doc.Load(supportedClients);
                XmlNodeList supported_clients = doc.SelectNodes("//versions/version");
                bool needsUpdate = true;
                foreach (XmlNode supported_client in supported_clients)
                {
                    if (supported_client.InnerText.Equals(Settings.TanksVersion))
                    {
                        needsUpdate = false;
                    }
                }
                if (needsUpdate)
                {
                    ReportProgress("Entry needed, creating and uploading...");
                    XmlNode versionRoot = doc.SelectSingleNode("//versions");
                    XmlElement supported_client = doc.CreateElement("version");
                    supported_client.InnerText = Settings.TanksVersion;
                    supported_client.SetAttribute("folder", Settings.TanksOnlineFolderVersion);
                    versionRoot.AppendChild(supported_client);
                    doc.Save(supportedClients);
                    downloader.UploadFile(ftpModpackRoot + "Resources/managerInfo/" + supportedClients, supportedClients);
                }
                else
                {
                    ReportProgress("Entry already exists");
                }
                File.Delete(supportedClients);
            }

            //update manager_version.xml
            ReportProgress("Updating manager_version.xml");
            using (downloader = new WebClient() { Credentials = credentials })
            {
                downloader.DownloadFile(ftpModpackRoot + "Resources/managerInfo/" + managerVersion, managerVersion);
                XmlDocument doc = new XmlDocument();
                doc.Load(managerVersion);
                XmlNode database_version_text = doc.SelectSingleNode("//version/database");
                database_version_text.InnerText = parsedDBUpdateVersion;
                doc.Save(managerVersion);
                downloader.UploadFile(ftpModpackRoot + "Resources/managerInfo/" + managerVersion, managerVersion);
                File.Delete(managerVersion);
            }
            ReportProgress("Done applying online changes, ready for update scripts");
        }

        private void UpdateDatabaseStep5_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update database step 5...");
            ReportProgress("Uploading databaseUpdate.txt file");
            using (downloader = new WebClient() { Credentials = credentials })
            {
                downloader.UploadFile("ftp://wotmods.relhaxmodpack.com/RelhaxModpack/Resources/managerInfo/" + databaseUpdateTxt, databaseUpdateTxt);
            }
            ReportProgress("Uploaded sucessfully");
        }

        private void UpdateDatabaseStep6_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update database step 6...");
            ReportProgress("Running script CreateModInfo.php...");
            using (WebClient client = new WebClient())
            {
                client.DownloadStringCompleted += Client_DownloadStringCompleted;
                //ScriptLogOutput.Text = client.DownloadString("http://wotmods.relhaxmodpack.com/scripts/CreateModInfo.php").Replace("<br />", "\n");
                client.DownloadStringAsync(new Uri("http://wotmods.relhaxmodpack.com/scripts/CreateModInfo.php"));
            }
        }

        private void UpdateDatabaseStep7_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update database step 7...");
            ReportProgress("Running script CreateManagerInfo.php...");
            using (WebClient client = new WebClient())
            {
                client.DownloadStringCompleted += Client_DownloadStringCompleted;
                //ScriptLogOutput.Text = client.DownloadString("http://wotmods.relhaxmodpack.com/scripts/CreateManagerInfo.php").Replace("<br />", "\n");
                client.DownloadStringAsync(new Uri("http://wotmods.relhaxmodpack.com/scripts/CreateManagerInfo.php"));
            }
        }
        #endregion

        #region Application Updating
        private void UpdateApplicationStep7_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "Running script CreateUpdatePackages.php...";
            using (WebClient client = new WebClient())
            {
                client.DownloadStringCompleted += Client_DownloadStringCompleted;
                //ScriptLogOutput.Text = client.DownloadString("http://wotmods.relhaxmodpack.com/scripts/CreateUpdatePackages.php").Replace("<br />", "\n");
                client.DownloadStringAsync(new Uri("http://wotmods.relhaxmodpack.com/scripts/CreateUpdatePackages.php"));
            }
        }

        private void UpdateApplicationStep8_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "Running script CreateManagerInfo.php...";
            using (WebClient client = new WebClient())
            {
                client.DownloadStringCompleted += Client_DownloadStringCompleted;
                //ScriptLogOutput.Text = client.DownloadString("http://wotmods.relhaxmodpack.com/scripts/CreateManagerInfo.php").Replace("<br />", "\n");
                client.DownloadStringAsync(new Uri("http://wotmods.relhaxmodpack.com/scripts/CreateManagerInfo.php"));
            }
        }
        #endregion

        #region Online Folder Cleaning
        private void CleanFoldersStep1_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "Running script CreateOutDatesFilesList.php...";
            using (WebClient client = new WebClient())
            {
                client.DownloadStringCompleted += Client_DownloadStringCompleted;
                //ScriptLogOutput.Text = client.DownloadString("http://wotmods.relhaxmodpack.com/scripts/CreateOutdatedFileList.php").Replace("<br />", "\n");
                client.DownloadStringAsync(new Uri("http://wotmods.relhaxmodpack.com/scripts/CreateOutdatedFileList.php"));
            }
        }

        private void CleanFoldersStep3_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "";
            if(string.IsNullOrWhiteSpace(CleanFoldersStep2Input.Text))
            {
                ReportProgress("ERROR: text for folder is blank!");
                return;
            }
            ReportProgress(string.Format("Checking if folder {0} exists...", CleanFoldersStep2Input.Text));
            string[] onlineFolders = FTPListFilesFolders("ftp://wotmods.relhaxmodpack.com/WoT/");
            if(!onlineFolders.Contains(CleanFoldersStep2Input.Text))
            {
                ReportProgress(string.Format("ERROR: folder {0} does not exist in WoT folder!", CleanFoldersStep2Input.Text));
                return;
            }
            ReportProgress("Cleaning online folder " + CleanFoldersStep2Input.Text);
            string onlineFolder = CleanFoldersStep2Input.Text;
            string onlineFolderPath = "ftp://wotmods.relhaxmodpack.com/WoT/" + onlineFolder + "/";
            string trashXML = "trash.xml";
            ReportProgress("Downloading trash.xml from " + onlineFolder);
            using (downloader = new WebClient() { Credentials = credentials })
            {
                downloader.DownloadFile(onlineFolderPath + trashXML, trashXML);
            }
            ReportProgress("Parsing " + trashXML);
            XmlDocument doc = new XmlDocument();
            doc.Load(trashXML);
            XmlNodeList trashFiles = doc.SelectNodes("//trash/filename");
            int totalFilesToDelete = trashFiles.Count;
            int filesDeleted = 1;
            foreach(XmlNode file in trashFiles)
            {
                ReportProgress(string.Format("Deleting file {0} of {1}, filename={2}", filesDeleted++, totalFilesToDelete, file.InnerText));
                FTPDeleteFile(onlineFolderPath + file.InnerText);
            }
            ReportProgress("Complete");
            File.Delete(trashXML);
        }
        #endregion

        #region Database Output
        private void DatabaseOutputStep1_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update output step 1...");
            if (loadDatabaseDialog.ShowDialog() == DialogResult.Cancel)
                return;
            DatabaseOutputStep1Location.Text = loadDatabaseDialog.FileName;
            //for the folder version: //modInfoAlpha.xml/@version
            Settings.TanksVersion = XMLUtils.GetXMLElementAttributeFromFile(DatabaseOutputStep1Location.Text, "//modInfoAlpha.xml/@version");
            //for the onlineFolder version: //modInfoAlpha.xml/@onlineFolder
            Settings.TanksOnlineFolderVersion = XMLUtils.GetXMLElementAttributeFromFile(DatabaseOutputStep1Location.Text, "//modInfoAlpha.xml/@onlineFolder");
            Text = String.Format("DatabaseUpdateUtility      TanksVersion: {0}    OnlineFolder: {1}", Settings.TanksVersion, Settings.TanksOnlineFolderVersion);
            ReportProgress("Settings.TanksVersion (current game version): " + Settings.TanksVersion);
            ReportProgress("Settings.TanksOnlineFolderVersion (online zip folder): " + Settings.TanksOnlineFolderVersion);
        }

        private void DatabaseOutputStep2b_Click(object sender, EventArgs e)
        {
            //init
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update database step 3...");

            // check for database
            if (!File.Exists(DatabaseOutputStep1Location.Text))
            {
                ReportProgress("ERROR: managerInfo xml not found! (did you run the previous steps?)");
                return;
            }

            //reset everything
            header = "Category\tMod\tDevURL";
            sb = new StringBuilder();
            packageName = "";
            category = "";
            packageDisplayName = "";
            packageName = "";
            zipfile = "";
            devURL = "";
            enabled = false;
            string saveLocation = Path.Combine(Application.StartupPath, "database_user.csv");

            //save it
            sb.Append(header + "\n");
            foreach (Category cat in parsedCatagoryList)
            {
                category = cat.Name;
                foreach (SelectablePackage m in cat.Packages)
                {
                    //remove the old devURL value if there
                    devURL = "";
                    packageDisplayName = m.Name;
                    devURL = string.IsNullOrWhiteSpace(m.DevURL) ? "" : "=HYPERLINK(\"" + m.DevURL + "\",\"link\")";
                    sb.Append(category + "\t" + packageDisplayName + "\t" + devURL + "\n");
                    if (m.Packages.Count > 0)
                        processConfigsSpreadsheetGenerateUser(m.Packages);
                }
            }
            try
            {
                File.WriteAllText(saveLocation, sb.ToString());
                ReportProgress("Saved in " + saveLocation);
            }
            catch (IOException)
            {
                ReportProgress("Failed to save in " + saveLocation + " (IOException, probably file open in another window)");
            }
        }

        private void DatabaseOutputStep2a_Click(object sender, EventArgs e)
        {
            //init
            ScriptLogOutput.Text = "";
            ReportProgress("Starting Update database step 3...");

            // check for database
            if (!File.Exists(DatabaseOutputStep1Location.Text))
            {
                ReportProgress("ERROR: managerInfo xml not found! (did you run the previous steps?)");
                return;
            }

            //create and reset internal variables
            sb = new StringBuilder();
            packageDisplayName = "";
            category = "";
            packageName = "N/A";
            notApplicatable = "n/a";
            zipfile = "";
            enabled = false;
            visible = false;
            devURL = "";
            string saveLocation = Path.Combine(Application.StartupPath, "database_internal.csv");
            globalDependencies.Clear();
            dependencies.Clear();
            logicalDependencies.Clear();
            parsedCatagoryList.Clear();

            //create database list
            XMLUtils.CreateModStructure(DatabaseOutputStep1Location.Text, globalDependencies, dependencies, logicalDependencies, parsedCatagoryList,true);

            //save it
            header = "PackageName\tCategory\tPackage\tLevel\tZip\tDevURL\tEnabled\tVisible";
            sb.Append(header + "\n");
            //first save globaldependencies
            category = "globalDependencies";
            foreach (Dependency d in globalDependencies)
            {
                packageName = d.PackageName;
                zipfile = string.IsNullOrWhiteSpace(d.ZipFile) ? notApplicatable : d.ZipFile;
                devURL = string.IsNullOrWhiteSpace(d.DevURL) ? "" : "=HYPERLINK(\"" + d.DevURL + "\",\"link\")";
                enabled = d.Enabled;
                sb.Append(packageName + "\t" + category + "\t" + packageDisplayName + "\t" + 0 + "\t" + zipfile + "\t" + devURL + "\t" + enabled + "\n");
            }
            //next save depenedneices
            category = "dependencies";
            foreach (Dependency d in dependencies)
            {
                packageName = d.PackageName;
                zipfile = string.IsNullOrWhiteSpace(d.ZipFile) ? notApplicatable : d.ZipFile;
                devURL = string.IsNullOrWhiteSpace(d.DevURL) ? "" : "=HYPERLINK(\"" + d.DevURL + "\",\"link\")";
                enabled = d.Enabled;
                sb.Append(packageName + "\t" + category + "\t" + packageDisplayName + "\t" + 0 + "\t" + zipfile + "\t" + devURL + "\t" + enabled + "\n");
            }
            //next save logicaldepenedneices
            category = "logicalDependencies";
            foreach (LogicalDependency d in logicalDependencies)
            {
                packageName = d.PackageName;
                zipfile = string.IsNullOrWhiteSpace(d.ZipFile) ? notApplicatable : d.ZipFile;
                devURL = string.IsNullOrWhiteSpace(d.DevURL) ? "" : "=HYPERLINK(\"" + d.DevURL + "\",\"link\")";
                enabled = d.Enabled;
                sb.Append(packageName + "\t" + category + "\t" + packageDisplayName + "\t" + 0 + "\t" + zipfile + "\t" + devURL + "\t" + enabled + "\n");
            }
            foreach (Category cat in parsedCatagoryList)
            {
                category = cat.Name;
                foreach (SelectablePackage m in cat.Packages)
                {
                    packageName = m.PackageName;
                    packageDisplayName = m.Name;
                    zipfile = string.IsNullOrWhiteSpace(m.ZipFile) ? notApplicatable : m.ZipFile;
                    enabled = m.Enabled;
                    visible = m.Visible;
                    devURL = string.IsNullOrWhiteSpace(m.DevURL) ? "" : "=HYPERLINK(\"" + m.DevURL + "\",\"link\")";
                    //header = "Index,Category,Mod,Config,Level,Zip,Enabled";
                    sb.Append(packageName + "\t" + category + "\t" + packageDisplayName + "\t" + m.Level + "\t" + zipfile + "\t" + devURL + "\t" + enabled + "\t" + visible + "\n");
                    if (m.Packages.Count > 0)
                        processConfigsSpreadsheetGenerate(m.Packages);
                }
            }
            try
            {
                File.WriteAllText(saveLocation, sb.ToString());
                ReportProgress("Saved in " + saveLocation);
            }
            catch (IOException)
            {
                ReportProgress("Failed to save in " + saveLocation + " (IOException, probably file open in another window)");
            }
        }
        private void processConfigsSpreadsheetGenerate(List<SelectablePackage> configList)
        {
            foreach (SelectablePackage con in configList)
            {
                packageName = con.PackageName;
                packageDisplayName = con.Name;
                zipfile = string.IsNullOrWhiteSpace(con.ZipFile) ? notApplicatable : con.ZipFile;
                enabled = con.Enabled;
                visible = con.Visible;
                devURL = string.IsNullOrWhiteSpace(con.DevURL) ? "" : "=HYPERLINK(\"" + con.DevURL + "\",\"link\")";
                sb.Append(packageName + "\t" + category + "\t" + packageDisplayName + "\t" + con.Level + "\t" + zipfile + "\t" + devURL + "\t" + enabled + "\t" + visible + "\n");
                if (con.Packages.Count > 0)
                    processConfigsSpreadsheetGenerate(con.Packages);
            }
        }
        private void processConfigsSpreadsheetGenerateUser(List<SelectablePackage> configList)
        {
            foreach (SelectablePackage con in configList)
            {
                //remove the old devURL value if there
                devURL = "";
                packageName = "";
                for (int i = 0; i <= con.Level; i++)
                {
                    packageName = packageName + "--";
                }
                packageName = packageName + con.Name;
                devURL = string.IsNullOrWhiteSpace(con.DevURL) ? "" : "=HYPERLINK(\"" + con.DevURL + "\",\"link\")";
                sb.Append(category + "\t" + packageName + "\t" + devURL + "\n");
                if (con.Packages.Count > 0)
                    processConfigsSpreadsheetGenerateUser(con.Packages);
            }
        }
        #endregion

        #region FTP methods
        private void FTPMakeFolder(string addressWithDirectory)
        {
            WebRequest folderRequest = WebRequest.Create(addressWithDirectory);
            folderRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            folderRequest.Credentials = credentials;
            using (FtpWebResponse response = (FtpWebResponse)folderRequest.GetResponse())
            { }
        }

        private string[] FTPListFilesFolders(string address)
        {
            WebRequest folderRequest = WebRequest.Create(address);
            folderRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            folderRequest.Credentials = credentials;
            using (FtpWebResponse response = (FtpWebResponse)folderRequest.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string temp = reader.ReadToEnd();
                return temp.Split(new[] { "\r\n" }, StringSplitOptions.None);
            }
        }

        private void FTPDeleteFile(string address)
        {
            WebRequest folderRequest = WebRequest.Create(address);
            folderRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            folderRequest.Credentials = credentials;
            using (FtpWebResponse response = (FtpWebResponse)folderRequest.GetResponse())
            { }
        }
        #endregion

        #region Authorization
        private void RequestL1AuthButton_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "Verifying...";
            if (L1AuthPasswordAttempt.Text.Equals(level1Password))
            {
                MessageBox.Show("Success!");
                OnAuthLevelChange(AuthLevel.View);
            }
            else
            {
                MessageBox.Show("Denied!");
            }
            ScriptLogOutput.Text = "";
        }

        private void RequestL2AuthButton_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "Verifying...";
            string downloadedKey = "";
            using (downloader = new WebClient())
            {
                downloadedKey = Utils.Base64Decode(downloader.DownloadString(Utils.Base64Decode(L2KeyAddress)));
            }
            if(downloadedKey.Equals(L2PasswordAttempt.Text))
            {
                MessageBox.Show("Success!");
                OnAuthLevelChange(AuthLevel.UpdateDatabase);
            }
            else
            {
                MessageBox.Show("Denied!");
            }
            ScriptLogOutput.Text = "";
        }

        private void RequestL3AuthButton_Click(object sender, EventArgs e)
        {
            ScriptLogOutput.Text = "Verifying...";
            string downloadedKey = "";
            using (downloader = new WebClient())
            {
                downloadedKey = Utils.Base64Decode(downloader.DownloadString(Utils.Base64Decode(L3KeyAddress)));
            }
            if (downloadedKey.Equals(L3PasswordAttempt.Text))
            {
                MessageBox.Show("Success!");
                OnAuthLevelChange(AuthLevel.Admin);
            }
            else
            {
                MessageBox.Show("Denied!");
            }
            ScriptLogOutput.Text = "";
        }

        private void GenerateL2Password_Click(object sender, EventArgs e)
        {
            if (File.Exists(L2keyFileName))
                File.Delete(L2keyFileName);
            File.WriteAllText(L2keyFileName, Utils.Base64Encode(L2TextPassword.Text));
            L2GenerateOutput.Text = "Password key file saved to" + L2keyFileName;
        }

        private void GenerateL3Password_Click(object sender, EventArgs e)
        {
            if (File.Exists(L3keyFileName))
                File.Delete(L3keyFileName);
            File.WriteAllText(L3keyFileName, Utils.Base64Encode(L3TextPassword.Text));
            L3GenerateOutput.Text = "Password key file saved to " + L3keyFileName;
        }

        private void ReadbackL2Password_Click(object sender, EventArgs e)
        {
            if(!File.Exists(L2keyFileName))
            {
                L2GenerateOutput.Text = L2keyFileName + " does not exist!";
            }
            else
            {
                L2GenerateOutput.Text = "L2 Key read as " + Utils.Base64Decode(File.ReadAllText(L2keyFileName));
            }
        }

        private void ReadbackL3Password_Click(object sender, EventArgs e)
        {
            if (!File.Exists(L3keyFileName))
            {
                L3GenerateOutput.Text = L3keyFileName + " does not exist!";
            }
            else
            {
                L3GenerateOutput.Text = "L3 Key read as " + Utils.Base64Decode(File.ReadAllText(L3keyFileName));
            }
        }
        #endregion

        #region Boring methods
        //creates a single list of all packages, a de-leveled snapshot of all packages in the database
        private List<DatabasePackage> CreatePackageList(List<Category> parsedCategoryList, List<Dependency> globalDependencies,
            List<Dependency> dependencies, List<LogicalDependency> logicalDependencies)
        {
            List<DatabasePackage> ListToReturn = new List<DatabasePackage>();
            //making a new list means no refrences, they are now new instances
            List<Category> newParsedCategoryList = new List<Category>(parsedCatagoryList);
            ListToReturn.AddRange(new List<DatabasePackage>(globalDependencies));
            ListToReturn.AddRange(new List<DatabasePackage>(dependencies));
            ListToReturn.AddRange(new List<DatabasePackage>(logicalDependencies));
            foreach (Category c in newParsedCategoryList)
            {
                foreach (SelectablePackage package in c.Packages)
                {
                    ListToReturn.Add(package);
                    if (package.Packages.Count > 0)
                    {
                        CreatePackageList(ListToReturn, package.Packages);
                    }
                }
            }
            return ListToReturn;
        }

        private void CreatePackageList(List<DatabasePackage> ListToReturn, List<SelectablePackage> packages)
        {
            foreach (SelectablePackage package in packages)
            {
                ListToReturn.Add(package);
                if (package.Packages.Count > 0)
                {
                    CreatePackageList(ListToReturn, package.Packages);
                }
            }
        }

        private void ReportProgress(string message)
        {
            //reports to the log file and the console otuptu
            Logging.Manager(message);
            ScriptLogOutput.AppendText(message + "\n");
            ScriptLogOutput.SelectionStart = ScriptLogOutput.Text.Length;
            ScriptLogOutput.ScrollToCaret();
            Application.DoEvents();
        }

        private void DatabaseUpdateTabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (CurrentAuthLevel == AuthLevel.None)
                DatabaseUpdateTabControl.SelectedTab = AuthStatus;
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ScriptLogOutput.Text = e.Result.Replace("<br />", "\n");
        }

        private void processConfigsCRCUpdate_old(List<SelectablePackage> cfgList)
        {
            foreach (SelectablePackage cat in cfgList)
            {
                int cindex = this.getZipIndex(cat.ZipFile);
                if (cindex != -1)
                {
                    cat.Size = this.getFileSize(addZipsDialog.FileNames[cindex]);
                    if (cat.CRC == null || cat.CRC.Equals("") || cat.CRC.Equals("f"))
                    {
                        cat.CRC = Utils.CreateMD5Hash(addZipsDialog.FileNames[cindex]);

                        packagesSB.Append(cat.ZipFile + "\n");
                    }
                }
                if (cat.Packages.Count > 0)
                {
                    this.processConfigsCRCUpdate_old(cat.Packages);
                }
            }
        }

        private int getZipIndex(string zipFile)
        {
            for (int i = 0; i < addZipsDialog.FileNames.Count(); i++)
            {
                string fileName = Path.GetFileName(addZipsDialog.FileNames[i]);
                if (fileName.Equals(zipFile))
                    return i;
            }
            return -1;
        }

        private void CRCFileSizeUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logging.Manager("|------------------------------------------------------------------------------------------------|");
        }

        private void processConfigsCRCUpdate(List<SelectablePackage> cfgList)
        {
            string hash;
            foreach (SelectablePackage cat in cfgList)
            {
                if (cat.ZipFile.Trim().Equals(""))
                {
                    cat.CRC = "";
                }
                else
                {
                    hash = XMLUtils.GetMd5Hash(cat.ZipFile);
                    cat.Size = getFileSize(cat.ZipFile);
                    if (cat.Size != 0)
                    {
                        if (!cat.CRC.Equals(hash))
                        {
                            cat.CRC = hash;
                            if (!hash.Equals("f"))
                            {
                                packagesSB.Append(cat.ZipFile + "\n");
                                updatedPackages.Add(cat);
                            }
                        }
                    }
                    else
                    {
                        cat.CRC = "f";
                    }
                    if (hash.Equals("f") | cat.CRC.Equals("f"))
                    {
                        filesNotFoundSB.Append(cat.ZipFile + "\n");
                        fileNotFoundPackages.Add(cat);
                    }

                }
                if (cat.Packages.Count > 0)
                {
                    processConfigsCRCUpdate(cat.Packages);
                }
            }
        }

        private Int64 getFileSize(string file)
        {
            Int64 fileSizeBytes = 0;
            if (Program.databaseUpdateOnline)
            {
                try
                {
                    XDocument doc = XDocument.Load(Settings.OnlineDatabaseXmlFile);
                    try
                    {
                        XElement element = doc.Descendants("file")
                           .Where(arg => arg.Attribute("name").Value == file)
                           .Single();
                        Int64.TryParse(element.Attribute("size").Value, out fileSizeBytes);
                    }
                    catch (InvalidOperationException)
                    {
                        // catch the Exception if no entry is found
                    }
                }
                catch (Exception ex)
                {
                    Utils.ExceptionLog("getFileSize", "read from onlineDatabaseXml: " + file, ex);
                }
            }
            else
            {
                try
                {
                    FileInfo fi = new FileInfo(file);
                    fileSizeBytes = fi.Length;
                }
                catch (Exception ex)
                {
                    Utils.ExceptionLog("getFileSize", "FileInfo from local file: " + file, ex);
                }
            }
            try
            {
                return fileSizeBytes;
            }
            catch (Exception ex)
            {
                Utils.ExceptionLog("getFileSize", "building format", ex);
            }
            return 0;
        }

        private void FillHashTable()
        {
            string serverInfo = "creating the manageInfo.dat file, containing the files:\n" +
            "manager_version.xml\n" +
            "supported_clients.xml\n" +
            "databaseUpdate.txt\n" +
            "releaseNotes.txt\n" +
            "releaseNotes_beta.txt\n" +
            "default_checked.xml";
            string modInfo = "creating the modInfo.dat file at every online version folder  of WoT,\n added the onlineFolder name to the root element,\n " +
                "added the \"selections\" (developerSelections) names, creation date and\n filenames to the modInfo.xml, adding all parsed develeoperSelection-Config\n " +
                "files to the modInfo.dat archive";
            HelpfullMessages.Add(UpdateDatabaseStep6.Name, modInfo);
            ButtonInfo.SetToolTip(UpdateDatabaseStep6, modInfo);
            HelpfullMessages.Add(UpdateDatabaseStep7.Name, serverInfo);
            ButtonInfo.SetToolTip(UpdateDatabaseStep7, serverInfo);
        }
        private void EUGERFormusLinksBUtton_Click(object sender, EventArgs e)
        {
            Utils.CallBrowser("http://forum.worldoftanks.eu/index.php?/topic/624499-");
        }

        private void EUENGFormsLinkButton_Click(object sender, EventArgs e)
        {
            Utils.CallBrowser("http://forum.worldoftanks.eu/index.php?/topic/623269-");
        }

        private void NAForumsLinkButton_Click(object sender, EventArgs e)
        {
            Utils.CallBrowser("http://forum.worldoftanks.com/index.php?/topic/535868-");
        }
        #endregion

        private void CreateMD5HashButton_Click(object sender, EventArgs e)
        {
            ReportProgress("Starting hashing");
            if(zipsToHash.ShowDialog() != DialogResult.OK)
            {
                ReportProgress("Hashing Aborted");
            }
            foreach(string s in zipsToHash.FileNames)
            {
                ReportProgress(string.Format("hash of {0}:", Path.GetFileName(s)));
                ReportProgress(Utils.CreateMD5Hash(s));

            }
            ReportProgress("Done");
        }
    }
}
