﻿using System;
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
    public partial class CRCFileSizeUpdate : Form
    {
        private WebClient downloader;
        private List<Dependency> globalDependencies;
        private List<Dependency> dependencies;
        private List<LogicalDependnecy> logicalDependencies;
        private List<Category> parsedCatagoryList;
        StringBuilder globalDepsSB = new StringBuilder();
        StringBuilder dependenciesSB = new StringBuilder();
        StringBuilder logicalDependenciesSB = new StringBuilder();
        StringBuilder modsSB = new StringBuilder();
        StringBuilder configsSB = new StringBuilder();
        StringBuilder filesNotFoundSB = new StringBuilder();

        public CRCFileSizeUpdate()
        {
            InitializeComponent();
        }

        private void loadDatabaseButton_Click(object sender, EventArgs e)
        {
            if (loadDatabaseDialog.ShowDialog() == DialogResult.Cancel)
                return;
            databaseLocationTextBox.Text = loadDatabaseDialog.FileName;
        }

        private void updateDatabaseOnline_Click(object sender, EventArgs e)
        {
            // check for database
            if (databaseLocationTextBox.Text.Equals("-none-"))
                return;
            // read gameVersion of the selected local modInfo.xml to get the right online database.xml
            string gameVersion = Utils.readVersionFromModInfo(databaseLocationTextBox.Text);
            Utils.appendToLog("working with game version: " + gameVersion);
            // download online database.xml
            try
            {
                using (downloader = new WebClient())
                {
                    string address = "http://wotmods.relhaxmodpack.com/WoT/" + gameVersion + "/database.xml";
                    string fileName = Path.Combine(Application.StartupPath, "RelHaxTemp", MainWindow.onlineDatabaseXmlFile);
                    downloader.DownloadFile(address, fileName);
                }
            }
            catch (Exception ex)
            {
                Utils.exceptionLog("loadZipFilesButton_Click", "http://wotmods.relhaxmodpack.com/WoT/" + gameVersion + "/database.xml", ex);
                MessageBox.Show("FAILED to download online file database");
                Application.Exit();
            }
            // set this flag, so getMd5Hash and getFileSize should parse downloaded online database.xml
            Program.databaseUpdateOnline = true;
            globalDepsSB.Clear();
            dependenciesSB.Clear();
            logicalDependenciesSB.Clear();
            modsSB.Clear();
            configsSB.Clear();
            //load database
            globalDependencies = new List<Dependency>();
            parsedCatagoryList = new List<Category>();
            dependencies = new List<Dependency>();
            logicalDependencies = new List<LogicalDependnecy>();
            Utils.createModStructure(databaseLocationTextBox.Text, globalDependencies, dependencies, logicalDependencies, parsedCatagoryList);
            //check for duplicates
            int duplicatesCounter = 0;
            if (Utils.duplicates(parsedCatagoryList) && Utils.duplicatesPackageName(parsedCatagoryList, ref duplicatesCounter ))
            {
                MessageBox.Show(string.Format("{0} duplicates found !!!",duplicatesCounter));
                Program.databaseUpdateOnline = false;
                return;
            }
            updatingLabel.Text = "Updating database...";
            Application.DoEvents();
            filesNotFoundSB.Append("FILES NOT FOUND:\n");
            globalDepsSB.Append("\nGlobal Dependencies updated:\n");
            dependenciesSB.Append("\nDependencies updated:\n");
            logicalDependenciesSB.Append("\nLogical Dependencies updated:\n");
            modsSB.Append("\nMods updated:\n");
            configsSB.Append("\nConfigs updated:\n");
            string hash;
            //foreach zip file name
            foreach (Dependency d in globalDependencies)
            {
                hash = Utils.getMd5Hash(d.dependencyZipFile);
                if (!d.dependencyZipCRC.Equals(hash))
                {
                    d.dependencyZipCRC = hash;
                    if (hash.Equals("f"))
                    {
                        filesNotFoundSB.Append(d.dependencyZipFile + "\n");
                    }
                    else
                    {
                        globalDepsSB.Append(d.dependencyZipFile + "\n");
                    };
                }
            }
            foreach (Dependency d in dependencies)
            {
                hash = Utils.getMd5Hash(d.dependencyZipFile);
                if (!d.dependencyZipCRC.Equals(hash))
                {
                    d.dependencyZipCRC = hash;
                    if (hash.Equals("f"))
                    {
                        filesNotFoundSB.Append(d.dependencyZipFile + "\n");
                    }
                    else
                    {
                        dependenciesSB.Append(d.dependencyZipFile + "\n");
                    }
                }
            }
            foreach (LogicalDependnecy d in logicalDependencies)
            {
                hash = Utils.getMd5Hash(d.dependencyZipFile);
                if (!d.dependencyZipCRC.Equals(hash))
                {
                    d.dependencyZipCRC = hash;
                    if (hash.Equals("f"))
                    {
                        filesNotFoundSB.Append(d.dependencyZipFile + "\n");
                    }
                    else
                    {
                        logicalDependenciesSB.Append(d.dependencyZipFile + "\n");
                    }
                }
            }
            foreach (Category c in parsedCatagoryList)
            {
                foreach (Mod m in c.mods)
                {
                    if (!m.zipFile.Equals(""))
                    {
                        m.size = this.getFileSize(m.zipFile);
                        hash = Utils.getMd5Hash(m.zipFile);
                        if (!m.crc.Equals(hash))
                        {
                            m.crc = hash;
                            if (hash.Equals("f"))
                            {
                                filesNotFoundSB.Append(m.zipFile + "\n");
                            }
                            else
                            {
                                modsSB.Append(m.zipFile + "\n");
                            }
                        }
                    }
                    if (m.configs.Count > 0)
                    {
                        this.processConfigsCRCUpdate(m.configs);
                    }
                }
            }
            //update the crc value
            //update the file size
            //save config file
            // string newModInfo = databaseLocationTextBox.Text;
            this.saveDatabase(databaseLocationTextBox.Text, gameVersion);
            MessageBox.Show(filesNotFoundSB.ToString() + globalDepsSB.ToString() + dependenciesSB.ToString() + logicalDependenciesSB.ToString() + modsSB.ToString() + configsSB.ToString());
            updatingLabel.Text = "Idle";
            Program.databaseUpdateOnline = false;
        }

        private void processConfigsCRCUpdate(List<Config> cfgList)
        {
            string hash;
            foreach (Config cat in cfgList)
            {
                if (!cat.zipFile.Equals(""))
                {
                    cat.size = this.getFileSize(cat.zipFile);
                    if (cat.size != 0)
                    {
                        hash = Utils.getMd5Hash(cat.zipFile);
                        if (!cat.crc.Equals(hash))
                        {
                            cat.crc = hash;
                            if (hash.Equals("f"))
                            {
                                filesNotFoundSB.Append(cat.zipFile + "\n");
                            }
                            else
                            {
                                configsSB.Append(cat.zipFile + "\n");
                            }
                        }
                    }
                    else
                    {
                        cat.crc = "";
                    }
                }
                if (cat.configs.Count > 0)
                {
                    this.processConfigsCRCUpdate(cat.configs);
                }
            }
        }
        
        private float getFileSize(string file)
        {
            Int64 fileSizeBytes = 0;
            if (Program.databaseUpdateOnline)
            {
                try
                {
                    XDocument doc = XDocument.Load(MainWindow.onlineDatabaseXmlFile);
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
                    Utils.exceptionLog("getMd5Hash", "read from databaseupdate = " + file, ex);
                }
            }
            else
            {
                FileInfo fi = new FileInfo(file);
                fileSizeBytes = fi.Length;
            }
            try
            {
                float fileSizeKBytes = fileSizeBytes / 1024;
                float fileSizeMBytes = fileSizeKBytes / 1024;
                fileSizeMBytes = (float)Math.Round(fileSizeMBytes, 1);
                if (fileSizeMBytes == 0.0)
                    fileSizeMBytes = 0.1f;
                return fileSizeMBytes;
            }
            catch (Exception ex)
            {
                Utils.exceptionLog("getFileSize", "building format", ex);
            }
            return 0;
        }

        private void CRCFileSizeUpdate_Load(object sender, EventArgs e)
        {
            //font scaling
            this.AutoScaleMode = Settings.appScalingMode;
            this.Font = Settings.appFont;
            if (Settings.appScalingMode == System.Windows.Forms.AutoScaleMode.Dpi)
            {
                this.Scale(new System.Drawing.SizeF(Settings.scaleSize, Settings.scaleSize));
            }
            loadDatabaseDialog.InitialDirectory = Application.StartupPath;
        }

        //saves the mod database
        private void saveDatabase(string saveLocation, string gameVersion)
        {
            XmlDocument doc = new XmlDocument();
            //database root modInfo.xml
            XmlElement root = doc.CreateElement("modInfoAlpha.xml");
            root.SetAttribute("version", gameVersion);
            doc.AppendChild(root);
            //global dependencies
            XmlElement globalDependenciesXml = doc.CreateElement("globaldependencies");
            foreach (Dependency d in globalDependencies)
            {
                //declare dependency root
                XmlElement globalDependencyRoot = doc.CreateElement("globaldependency");
                //make dependency
                XmlElement globalDepZipFile = doc.CreateElement("dependencyZipFile");
                globalDepZipFile.InnerText = d.dependencyZipFile;
                globalDependencyRoot.AppendChild(globalDepZipFile);
                XmlElement globalDepStartAddress = doc.CreateElement("startAddress");
                globalDepStartAddress.InnerText = d.startAddress;
                globalDependencyRoot.AppendChild(globalDepStartAddress);
                XmlElement globalDepEndAddress = doc.CreateElement("endAddress");
                globalDepEndAddress.InnerText = d.endAddress;
                globalDependencyRoot.AppendChild(globalDepEndAddress);
                XmlElement globalDepCRC = doc.CreateElement("dependencyZipCRC");
                globalDepCRC.InnerText = d.dependencyZipCRC;
                globalDependencyRoot.AppendChild(globalDepCRC);
                XmlElement globalDepEnabled = doc.CreateElement("dependencyenabled");
                globalDepEnabled.InnerText = "" + d.enabled;
                globalDependencyRoot.AppendChild(globalDepEnabled);
                XmlElement globalDepPackageName = doc.CreateElement("packageName");
                globalDepPackageName.InnerText = d.packageName;
                globalDependencyRoot.AppendChild(globalDepPackageName);
                //attach dependency root
                globalDependenciesXml.AppendChild(globalDependencyRoot);
            }
            root.AppendChild(globalDependenciesXml);
            //dependencies
            XmlElement DependenciesXml = doc.CreateElement("dependencies");
            foreach (Dependency d in dependencies)
            {
                //declare dependency root
                XmlElement dependencyRoot = doc.CreateElement("dependency");
                //make dependency
                XmlElement depZipFile = doc.CreateElement("dependencyZipFile");
                depZipFile.InnerText = d.dependencyZipFile;
                dependencyRoot.AppendChild(depZipFile);
                XmlElement depStartAddress = doc.CreateElement("startAddress");
                depStartAddress.InnerText = d.startAddress;
                dependencyRoot.AppendChild(depStartAddress);
                XmlElement depEndAddress = doc.CreateElement("endAddress");
                depEndAddress.InnerText = d.endAddress;
                dependencyRoot.AppendChild(depEndAddress);
                XmlElement depCRC = doc.CreateElement("dependencyZipCRC");
                depCRC.InnerText = d.dependencyZipCRC;
                dependencyRoot.AppendChild(depCRC);
                XmlElement depEnabled = doc.CreateElement("dependencyenabled");
                depEnabled.InnerText = "" + d.enabled;
                dependencyRoot.AppendChild(depEnabled);
                XmlElement depPackageName = doc.CreateElement("packageName");
                depPackageName.InnerText = d.packageName;
                dependencyRoot.AppendChild(depPackageName);
                //attach dependency root
                DependenciesXml.AppendChild(dependencyRoot);
            }
            root.AppendChild(DependenciesXml);
            //dependencies
            XmlElement logicalDependenciesXml = doc.CreateElement("logicalDependencies");
            foreach (LogicalDependnecy d in logicalDependencies)
            {
                //TODO
            }
            root.AppendChild(logicalDependenciesXml);
            //catagories
            XmlElement catagoriesHolder = doc.CreateElement("catagories");
            foreach (Category c in parsedCatagoryList)
            {
                //catagory root
                XmlElement catagoryRoot = doc.CreateElement("catagory");
                //make catagory
                XmlElement catagoryName = doc.CreateElement("name");
                catagoryName.InnerText = c.name;
                catagoryRoot.AppendChild(catagoryName);
                XmlElement catagorySelectionType = doc.CreateElement("selectionType");
                catagorySelectionType.InnerText = c.selectionType;
                catagoryRoot.AppendChild(catagorySelectionType);
                //dependencies for catagory
                XmlElement catagoryDependencies = doc.CreateElement("dependencies");
                foreach (Dependency d in c.dependencies)
                {
                    //declare dependency root
                    XmlElement DependencyRoot = doc.CreateElement("dependency");
                    XmlElement DepPackageName = doc.CreateElement("packageName");
                    DepPackageName.InnerText = d.packageName;
                    DependencyRoot.AppendChild(DepPackageName);
                    //attach dependency root
                    catagoryDependencies.AppendChild(DependencyRoot);
                }
                catagoryRoot.AppendChild(catagoryDependencies);
                //mods for catagory
                XmlElement modsHolder = doc.CreateElement("mods");
                foreach (Mod m in c.mods)
                {
                    //add it to the list
                    XmlElement modRoot = doc.CreateElement("mod");
                    XmlElement modName = doc.CreateElement("name");
                    modName.InnerText = m.name;
                    modRoot.AppendChild(modName);
                    XmlElement modVersion = doc.CreateElement("version");
                    modVersion.InnerText = m.version;
                    modRoot.AppendChild(modVersion);
                    XmlElement modZipFile = doc.CreateElement("zipFile");
                    modZipFile.InnerText = m.zipFile;
                    modRoot.AppendChild(modZipFile);
                    XmlElement modStartAddress = doc.CreateElement("startAddress");
                    modStartAddress.InnerText = m.startAddress;
                    modRoot.AppendChild(modStartAddress);
                    XmlElement modEndAddress = doc.CreateElement("endAddress");
                    modEndAddress.InnerText = m.endAddress;
                    modRoot.AppendChild(modEndAddress);
                    XmlElement modZipCRC = doc.CreateElement("crc");
                    modZipCRC.InnerText = m.crc;
                    modRoot.AppendChild(modZipCRC);
                    XmlElement modEnabled = doc.CreateElement("enabled");
                    modEnabled.InnerText = "" + m.enabled;
                    modRoot.AppendChild(modEnabled);
                    XmlElement modVisible = doc.CreateElement("visible");
                    modVisible.InnerText = "" + m.visible;
                    modRoot.AppendChild(modVisible);
                    XmlElement modPackageName = doc.CreateElement("packageName");
                    modPackageName.InnerText = m.packageName;
                    modRoot.AppendChild(modPackageName);
                    XmlElement modZipSize = doc.CreateElement("size");
                    modZipSize.InnerText = "" + m.size;
                    modRoot.AppendChild(modZipSize);
                    XmlElement modUpdateComment = doc.CreateElement("updateComment");
                    modUpdateComment.InnerText = m.updateComment;
                    modRoot.AppendChild(modUpdateComment);
                    XmlElement modDescription = doc.CreateElement("description");
                    modDescription.InnerText = m.description;
                    modRoot.AppendChild(modDescription);
                    XmlElement modDevURL = doc.CreateElement("devURL");
                    modDevURL.InnerText = m.devURL;
                    modRoot.AppendChild(modDevURL);
                    //datas for the mods
                    XmlElement modDatas = doc.CreateElement("userDatas");
                    foreach (string s in m.userFiles)
                    {
                        XmlElement userData = doc.CreateElement("userData");
                        userData.InnerText = s;
                        modDatas.AppendChild(userData);
                    }
                    modRoot.AppendChild(modDatas);
                    //pictures for the mods
                    XmlElement modPictures = doc.CreateElement("pictures");
                    foreach (Media p in m.pictureList)
                    {
                        XmlElement pictureRoot = doc.CreateElement("picture");
                        XmlElement pictureType = doc.CreateElement("type");
                        XmlElement pictureURL = doc.CreateElement("URL");
                        pictureURL.InnerText = p.URL;
                        pictureType.InnerText = "" + (int)p.mediaType;
                        pictureRoot.AppendChild(pictureType);
                        pictureRoot.AppendChild(pictureURL);
                        modPictures.AppendChild(pictureRoot);
                    }
                    modRoot.AppendChild(modPictures);
                    //configs for the mods
                    XmlElement configsHolder = doc.CreateElement("configs");
                    //if statement here
                    if (m.configs.Count > 0)
                        saveDatabaseConfigLevel(doc, configsHolder, m.configs);
                    modRoot.AppendChild(configsHolder);
                    XmlElement modDependencies = doc.CreateElement("dependencies");
                    foreach (Dependency d in m.dependencies)
                    {
                        //declare dependency root
                        XmlElement DependencyRoot = doc.CreateElement("dependency");
                        //make dependency
                        XmlElement DepPackageName = doc.CreateElement("packageName");
                        DepPackageName.InnerText = d.packageName;
                        DependencyRoot.AppendChild(DepPackageName);
                        //attach dependency root
                        modDependencies.AppendChild(DependencyRoot);
                    }
                    modRoot.AppendChild(modDependencies);
                    modsHolder.AppendChild(modRoot);
                }
                catagoryRoot.AppendChild(modsHolder);
                //append catagory
                catagoriesHolder.AppendChild(catagoryRoot);
            }
            root.AppendChild(catagoriesHolder);
            doc.Save(saveLocation);
        }
        private void saveDatabaseConfigLevel(XmlDocument doc, XmlElement configsHolder, List<Config> configsList)
        {
            foreach (Config cc in configsList)
            {
                //add the config to the list
                XmlElement configRoot = doc.CreateElement("config");
                configsHolder.AppendChild(configRoot);
                XmlElement configName = doc.CreateElement("name");
                configName.InnerText = cc.name;
                configRoot.AppendChild(configName);
                XmlElement configVersion = doc.CreateElement("version");
                configVersion.InnerText = cc.version;
                configRoot.AppendChild(configVersion);
                XmlElement configZipFile = doc.CreateElement("zipFile");
                configZipFile.InnerText = cc.zipFile;
                configRoot.AppendChild(configZipFile);
                XmlElement configStartAddress = doc.CreateElement("startAddress");
                configStartAddress.InnerText = cc.startAddress;
                configRoot.AppendChild(configStartAddress);
                XmlElement configEndAddress = doc.CreateElement("endAddress");
                configEndAddress.InnerText = cc.endAddress;
                configRoot.AppendChild(configEndAddress);
                XmlElement configZipCRC = doc.CreateElement("crc");
                configZipCRC.InnerText = cc.crc;
                configRoot.AppendChild(configZipCRC);
                XmlElement configEnabled = doc.CreateElement("enabled");
                configEnabled.InnerText = "" + cc.enabled;
                configRoot.AppendChild(configEnabled);
                XmlElement configVisible = doc.CreateElement("visible");
                configVisible.InnerText = "" + cc.visible;
                configRoot.AppendChild(configVisible);
                XmlElement configPackageName = doc.CreateElement("packageName");
                configPackageName.InnerText = cc.packageName;
                configRoot.AppendChild(configPackageName);
                XmlElement configSize = doc.CreateElement("size");
                configSize.InnerText = "" + cc.size;
                configRoot.AppendChild(configSize);
                XmlElement configComment = doc.CreateElement("updateComment");
                configComment.InnerText = cc.updateComment;
                configRoot.AppendChild(configComment);
                XmlElement configDescription = doc.CreateElement("description");
                configDescription.InnerText = cc.description;
                configRoot.AppendChild(configDescription);
                XmlElement configDevURL = doc.CreateElement("devURL");
                configDevURL.InnerText = cc.devURL;
                configRoot.AppendChild(configDevURL);
                XmlElement configType = doc.CreateElement("type");
                configType.InnerText = cc.type;
                configRoot.AppendChild(configType);
                //datas for the mods
                XmlElement configDatas = doc.CreateElement("userDatas");
                foreach (string s in cc.userFiles)
                {
                    XmlElement userData = doc.CreateElement("userData");
                    userData.InnerText = s;
                    configDatas.AppendChild(userData);
                }
                configRoot.AppendChild(configDatas);
                //pictures for the configs
                XmlElement configPictures = doc.CreateElement("pictures");
                foreach (Media p in cc.pictureList)
                {
                    XmlElement pictureRoot = doc.CreateElement("picture");
                    XmlElement pictureType = doc.CreateElement("type");
                    XmlElement pictureURL = doc.CreateElement("URL");
                    pictureURL.InnerText = p.URL;
                    pictureType.InnerText = "" + (int)p.mediaType;
                    pictureRoot.AppendChild(pictureType);
                    pictureRoot.AppendChild(pictureURL);
                    configPictures.AppendChild(pictureRoot);
                }
                configRoot.AppendChild(configPictures);
                //configs for the configs (meta)
                XmlElement configsHolderSub = doc.CreateElement("configs");
                //if statement here
                if (cc.configs.Count > 0)
                    saveDatabaseConfigLevel(doc, configsHolderSub, cc.configs);
                configRoot.AppendChild(configsHolderSub);
                //dependencies for the configs
                XmlElement catDependencies = doc.CreateElement("dependencies");
                foreach (Dependency d in cc.dependencies)
                {
                    //declare dependency root
                    XmlElement DependencyRoot = doc.CreateElement("dependency");
                    //make dependency
                    XmlElement DepPackageName = doc.CreateElement("packageName");
                    DepPackageName.InnerText = d.packageName;
                    DependencyRoot.AppendChild(DepPackageName);
                    //attach dependency root
                    catDependencies.AppendChild(DependencyRoot);
                }
                configRoot.AppendChild(catDependencies);
                configsHolder.AppendChild(configRoot);
            }
        }

        private void CRCFileSizeUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utils.appendToLog("|------------------------------------------------------------------------------------------------|");
        }

        private void RunOnlineScriptButton_Click(object sender, EventArgs e)
        {
            OnlineScriptOutput.Text = "Running online script...";
            Application.DoEvents();
            using (WebClient client = new WebClient())
            {
                OnlineScriptOutput.Text = client.DownloadString("http://wotmods.relhaxmodpack.com/scripts/CreateDatabase.php").Replace("<br />", "\n");
            }
            Application.DoEvents();
        }

        private void updateDatabaseOffline_Click(object sender, EventArgs e)
        {
            //check for database
            if (databaseLocationTextBox.Text.Equals("-none-"))
                return;
            //show file dialog
            if (addZipsDialog.ShowDialog() == DialogResult.Cancel)
                return;
            globalDepsSB.Clear();
            dependenciesSB.Clear();
            modsSB.Clear();
            configsSB.Clear();
            //load database
            globalDependencies = new List<Dependency>();
            parsedCatagoryList = new List<Category>();
            dependencies = new List<Dependency>();
            logicalDependencies = new List<LogicalDependnecy>();
            string gameVersion = Utils.readVersionFromModInfo(databaseLocationTextBox.Text);
            Utils.createModStructure(databaseLocationTextBox.Text, globalDependencies, dependencies, logicalDependencies, parsedCatagoryList);
            int duplicatesCounter = 0;
            //check for duplicates
            if (Utils.duplicates(parsedCatagoryList) && Utils.duplicatesPackageName(parsedCatagoryList, ref duplicatesCounter))
            {
                MessageBox.Show(string.Format("{0} duplicates found !!!", duplicatesCounter));
                return;
            }
            updatingLabel.Text = "Updating database...";
            Application.DoEvents();
            globalDepsSB.Append("Global Dependencies updated:\n");
            dependenciesSB.Append("Dependencies updated:\n");
            modsSB.Append("Mods updated:\n");
            configsSB.Append("Configs updated:\n");
            //foreach zip file name
            foreach (Dependency d in globalDependencies)
            {
                int index = this.getZipIndex(d.dependencyZipFile);
                if (index == -1)
                {
                    continue;
                }
                if (d.dependencyZipCRC == null || d.dependencyZipCRC.Equals("") || d.dependencyZipCRC.Equals("f"))
                {
                    d.dependencyZipCRC = Utils.createMd5Hash(addZipsDialog.FileNames[index]);
                    globalDepsSB.Append(d.dependencyZipFile + "\n");
                }
            }
            foreach (Dependency d in dependencies)
            {
                int index = this.getZipIndex(d.dependencyZipFile);
                if (index == -1)
                {
                    continue;
                }
                if (d.dependencyZipCRC == null || d.dependencyZipCRC.Equals("") || d.dependencyZipCRC.Equals("f"))
                {
                    d.dependencyZipCRC = Utils.createMd5Hash(addZipsDialog.FileNames[index]);
                    dependenciesSB.Append(d.dependencyZipFile + "\n");
                }
            }
            foreach (Category c in parsedCatagoryList)
            {
                foreach (Mod m in c.mods)
                {
                    int index = this.getZipIndex(m.zipFile);
                    if (index != -1)
                    {
                        m.size = this.getFileSize(addZipsDialog.FileNames[index]);
                        if (m.crc == null || m.crc.Equals("") || m.crc.Equals("f"))
                        {
                            m.crc = Utils.createMd5Hash(addZipsDialog.FileNames[index]);

                            modsSB.Append(m.zipFile + "\n");
                        }
                    }
                    if (m.configs.Count > 0)
                    {
                        this.processConfigsCRCUpdate_old(m.configs);
                    }
                }
            }
            //update the crc value
            //update the file size
            //save config file
            string newModInfo = databaseLocationTextBox.Text;
            this.saveDatabase(databaseLocationTextBox.Text, gameVersion);
            MessageBox.Show(globalDepsSB.ToString() + dependenciesSB.ToString() + modsSB.ToString() + configsSB.ToString());
            updatingLabel.Text = "Idle";
        }

        private void processConfigsCRCUpdate_old(List<Config> cfgList)
        {
            foreach (Config cat in cfgList)
            {
                int cindex = this.getZipIndex(cat.zipFile);
                if (cindex != -1)
                {
                    cat.size = this.getFileSize(addZipsDialog.FileNames[cindex]);
                    if (cat.crc == null || cat.crc.Equals("") || cat.crc.Equals("f"))
                    {
                        cat.crc = Utils.createMd5Hash(addZipsDialog.FileNames[cindex]);

                        configsSB.Append(cat.zipFile + "\n");
                    }
                }
                if (cat.configs.Count > 0)
                {
                    this.processConfigsCRCUpdate_old(cat.configs);
                }
            }
        }

        private float getFileSize_old(string file)
        {
            FileInfo fi = new FileInfo(file);
            float fileSizeBytes = fi.Length;
            float fileSizeKBytes = fileSizeBytes / 1024;
            float fileSizeMBytes = fileSizeKBytes / 1024;
            fileSizeMBytes = (float)Math.Round(fileSizeMBytes, 1);
            if (fileSizeMBytes == 0.0)
                fileSizeMBytes = 0.1f;
            return fileSizeMBytes;
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
    }
}
