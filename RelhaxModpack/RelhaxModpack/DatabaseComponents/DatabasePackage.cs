﻿using System.Collections.Generic;
using System.Windows.Controls;

namespace RelhaxModpack
{
    /// <summary>
    /// A database component is an abstract class for all Components within the database
    /// </summary>
    public class DatabasePackage
    {

        private static readonly List<string> PackageElementsToXmlParseAttributes = new List<string>()
        {
            nameof(Version),
            nameof(Timestamp),
            nameof(ZipFile),
            nameof(CRC),
            nameof(StartAddress),
            nameof(EndAddress),
            nameof(LogAtInstall),
            nameof(Triggers),
            //nameof(DevURL),
            nameof(InternalNotes)
        };

        private static readonly List<string> PackageElementsToXmlParseNodes = new List<string>()
        {
            nameof(PackageName),
            nameof(_Enabled),
            nameof(InstallGroup),
            nameof(PatchGroup),
        };

        private static Dictionary<string, string> PackageElementsToXmlParseMapper = new Dictionary<string, string>()
        {
            //key, value
            { nameof(_Enabled), nameof(Enabled) }
        };

        /// <summary>
        /// a unique identifier for each component in the database. No two components will have the same PackageName
        /// </summary>
        public string PackageName = "";
        /// <summary>
        /// a method to keep track of the version of the package
        /// </summary>
        public string Version = "";
        /// <summary>
        /// used to determine when the package entry was last modified
        /// </summary>
        public long Timestamp = 0;
        /// <summary>
        /// the zip file to extract (can be "")
        /// </summary>
        public string ZipFile = "";
        protected internal bool _Enabled = false;
        /// <summary>
        /// a toggle to enable and disable the component
        /// </summary>
        public virtual bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }
        /// <summary>
        /// the crc checksum of the zipfile
        /// </summary>
        public string CRC = "";
        /// <summary>
        /// the start address of the url to the zip file
        /// URL format: StartAddress + ZipFile + EndAddress
        /// </summary>
        public string StartAddress = Settings.DefaultStartAddress;
        /// <summary>
        /// the end address of the url to the zip file
        /// URL format: StartAddress + ZipFile + EndAddress
        /// </summary>
        public string EndAddress = Settings.DefaultEndAddress;
        /// <summary>
        /// determine at install time if the package needs to be downloaded
        /// </summary>
        public bool DownloadFlag = false;
        /// <summary>
        /// determine if the mod has been downloaded and is ready for installation
        /// </summary>
        public bool ReadyForInstall = false;
        /// <summary>
        /// determine if the files from the package should be logged for uninstallation
        /// only set this to false if absolutly necessary!
        /// </summary>
        public bool LogAtInstall = true;
        public List<string> Triggers = new List<string>();
        /// <summary>
        /// the URL link of where you can view the webpage of the mod
        /// </summary>
        public  string DevURL = "";
        public int InstallGroup = 0;
        public int PatchGroup = 0;
        public string InternalNotes = "";

        //used for the editor ONLY
        public TreeViewItem EditorTreeViewItem = null;

        public override string ToString()
        {
            return PackageName;
        }
        /// <summary>
        /// Provides (if possible) a complete tree style path in for the cateogry views
        /// </summary>
        public virtual string CompletePath
        { get {  return PackageName; } }

        public static List<string> FieldsToXmlParseAttributes()
        {
            List<string> components = new List<string>(PackageElementsToXmlParseAttributes);
            //https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.values?view=netframework-4.7.2
            foreach(KeyValuePair<string,string> keyValuePair in PackageElementsToXmlParseMapper)
            {
                if (components.Contains(keyValuePair.Key))
                    components[components.IndexOf(keyValuePair.Key)] = keyValuePair.Value;
            }
            return components;
        }

        public static List<string> FieldsToXmlParseNodes()
        {
            List<string> components = new List<string>(PackageElementsToXmlParseNodes);
            foreach (KeyValuePair<string, string> keyValuePair in PackageElementsToXmlParseMapper)
            {
                if (components.Contains(keyValuePair.Key))
                    components[components.IndexOf(keyValuePair.Key)] = keyValuePair.Value;
            }
            return components;
        }
    }
}
