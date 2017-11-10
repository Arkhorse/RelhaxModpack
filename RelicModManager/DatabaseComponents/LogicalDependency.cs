﻿using RelhaxModpack.DatabaseComponents;
using System.Collections.Generic;

namespace RelhaxModpack
{
    public class LogicalDependency : IDatabasePackage
    {
        //the zip file of the dependency
        public string ZipFile { get; set; }
        //the Timestamp of last change of zipfile name 
        public long Timestamp { get; set; }
        //the CRC of the dependency
        public string CRC { get; set; }
        //flag to set to disable the dependency from being installed
        public bool Enabled { get; set; }
        //the start address of the zip file location. Enabled us to use sites that
        //generate random filenames for publicly shared files.
        public string StartAddress { get; set; }
        //the end address of the zip file location. enables us to use dropbox (?dl=1)
        public string EndAddress { get; set; }
        //later a unique name of the config entry
        public string PackageName { get; set; }
        //used to determine at install time if the zip file needs to be downloaded
        public bool DownloadFlag { get; set; }
        //acts as a NOT flag
        public bool NegateFlag { get; set; }
        //needed to excatly identify double packageNames and its position
        public int CheckDatabaseListIndex { get; set; }
        public bool ReadyForInstall { get; set; }
        //list of linked mods and configs that use 
        public List<DatabaseLogic> DatabasePackageLogic = new List<DatabaseLogic>();
        public string DevURL { get; set; }
        public string ExtractPath { get; set; }
        public List<Shortcut> Shortcuts = new List<Shortcut>();
        public LogicalDependency() {
            ReadyForInstall = false;
            ExtractPath = "";
        }
        //for the tostring thing
        public override string ToString()
        {
            return NegateFlag? "(Not) " + PackageName : "" + PackageName;
        }
    }
}
