﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using TeximpNet.Unmanaged;
using Ionic.Zip;
using RelhaxModpack.AtlasesCreator;
using TeximpNet;

namespace RelhaxModpack
{
    /// <summary>
    /// A wrapper class around the TexImpNet FreeImage library class
    /// </summary>
    /// <remarks>The class handles: 32 and 64 bit library loading determination, Extraction, and Loading into memory</remarks>
    public class RelhaxFreeImageLibrary : IRelhaxUnmanagedLibrary
    {
        private FreeImageLibrary library = FreeImageLibrary.Instance;

        /// <summary>
        /// Gets the name of the embedded zip file containing the dll, 32 or 64 bit version
        /// </summary>
        public string EmbeddedFilename
        {
            get { return UnmanagedLibrary.Is64Bit ? "FreeImage64.zip" : "FreeImage32.zip"; }
        }

        /// <summary>
        /// Gets the name of the dll file inside the embedded zip file, 32 or 64bit version
        /// </summary>
        public string ExtractedFilename
        {
            get { return UnmanagedLibrary.Is64Bit ? "FreeImage64.dll" : "FreeImage32.dll"; }
        }

        /// <summary>
        /// Gets the absolute path to the dll file
        /// </summary>
        public string Filepath
        {
            get
            { return Path.Combine(Settings.RelhaxLibrariesFolderPath, ExtractedFilename); }
        }

        /// <summary>
        /// Determines if the file is extracted to the Filepath property location
        /// </summary>
        public bool IsExtracted
        {
            get
            { return File.Exists(Filepath); }
        }

        /// <summary>
        /// Determines if the library is loaded into memory
        /// </summary>
        public bool IsLoaded
        {
            get
            { return library.IsLibraryLoaded; }
        }

        /// <summary>
        /// Attempts to load the library using the Filepath property
        /// </summary>
        /// <returns>True if the library load was successful</returns>
        public bool Load()
        {
            if (!IsExtracted)
                Extract();
            try
            {
                return library.LoadLibrary(Filepath);
            }
            catch (TeximpException ex)
            {
                Logging.Exception("failed to load native library");
                Logging.Exception(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Attempts to unload the library
        /// </summary>
        /// <returns>True if the library was unloaded, false otherwise</returns>
        public bool Unload()
        {
            if (!IsLoaded)
                return true;
            else
            {
                return library.FreeLibrary();
            }
        }

        /// <summary>
        /// Extracts the embedded compressed library to the location in the Filepath property
        /// </summary>
        public void Extract()
        {
            if(IsExtracted)
            {
                Logging.Warning("Unmanaged library {0} is already extracted", EmbeddedFilename);
                return;
            }
            //https://stackoverflow.com/questions/38381684/reading-zip-file-from-byte-array-using-ionic-zip
            string resourceName = Utils.GetAssemblyName(EmbeddedFilename);
            Logging.Info("Extracting unmanaged teximpnet library: {0}", EmbeddedFilename);
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (ZipFile zout = ZipFile.Read(stream))
            {
                zout.ExtractAll(Settings.RelhaxLibrariesFolderPath);
            }
        }
    }
}
