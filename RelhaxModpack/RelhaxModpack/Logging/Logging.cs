﻿using System.IO;
using System.Windows;

namespace RelhaxModpack
{
    /// <summary>
    /// The different log files currently used in the modpack
    /// </summary>
    public enum Logfiles
    {
        /// <summary>
        /// The default modpack logfile
        /// </summary>
        Application,
        /// <summary>
        /// logfile for when installing mods
        /// </summary>
        Installer,
        /// <summary>
        /// logfile for when uninstalling mods
        /// </summary>
        Uninstaller
    }
    /// <summary>
    /// The level of severity of the log message
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug message
        /// </summary>
        Debug,
        /// <summary>
        /// Informational message
        /// </summary>
        Info,
        /// <summary>
        /// A problem, but can be worked around
        /// </summary>
        Warning,
        /// <summary>
        /// Something is wrong, something may not work
        /// </summary>
        Error,
        /// <summary>
        /// Something is wrong, something will not work
        /// </summary>
        Exception,
        /// <summary>
        /// The application is closing now
        /// </summary>
        ApplicationHalt
    }
    /// <summary>
    /// A static constant refrence to common logging variables and common log refrences
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// The filename of the application log file
        /// </summary>
        public const string ApplicationLogFilename = "Relhax.log";
        /// <summary>
        /// The filename of the old application log file
        /// </summary>
        public const string OldApplicationLogFilename = "RelHaxLog.txt";
        /// <summary>
        /// The name of the install log file
        /// </summary>
        public const string InstallLogFilename = "TODO";
        /// <summary>
        /// the name of the uninstall log file
        /// </summary>
        public const string UninstallLogFilename = "TODO";
        private const string ApplicationLogfileTimestamp = "yyyy-MM-dd HH:mm:ss.fff";
        /// <summary>
        /// The header and end that shows the start and stop of the application log file
        /// </summary>
        public const string ApplicationlogStartStop = "|------------------------------------------------------------------------------------------------|";
        /// <summary>
        /// Provides a constant refrence to the log file
        /// </summary>
        public static Logfile ApplicationLogfile;
        /// <summary>
        /// Provides a refrence to an instance of an install log file
        /// </summary>
        public static Logfile InstallLogfile;
        /// <summary>
        /// Provides a refrence to an instalce of an uninstall log file
        /// </summary>
        public static Logfile UninstallLogfile;
        private static bool FailedToWriteToLogWindowShown = false;
        /// <summary>
        /// Initialize the logging subsystem for the appilcation
        /// </summary>
        /// <returns>True if sucessfull initialization, false otherwise</returns>
        public static bool InitApplicationLogging()
        {
            if (ApplicationLogfile != null)
                throw new BadMemeException("only do this once jackass");
            string oldLogFilePath = Path.Combine(Settings.ApplicationStartupPath, OldApplicationLogFilename);
            string newLogFilePath = Path.Combine(Settings.ApplicationStartupPath, ApplicationLogFilename);
            //if the old log exists and the new one does not, move the logging to the new one
            try
            {
                if (File.Exists(oldLogFilePath) && !File.Exists(newLogFilePath))
                    File.Move(oldLogFilePath, newLogFilePath);
                Settings.FirstLoadToV2 = true;
            }
            catch
            {
                MessageBox.Show("Failed to move logfile");
                return false;
            }
            ApplicationLogfile = new Logfile(newLogFilePath, ApplicationLogfileTimestamp);
            if(!ApplicationLogfile.Init())
            {
                MessageBox.Show("Failed to initialize logfile, check file permissions");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Dispose of the application logging subsystem
        /// </summary>
        public static void DisposeApplicationLogging()
        {
            ApplicationLogfile.Dispose();
            ApplicationLogfile = null;
        }
        /// <summary>
        /// Writes a message to a logfile instance, if it exists
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="logfiles">The logfile to write to</param>
        /// <param name="logLevel">The level of severity of the message</param>
        public static void WriteToLog(string message, Logfiles logfiles = Logfiles.Application,LogLevel logLevel = LogLevel.Info)
        {
            Logfile fileToWriteTo = null;
            switch(logfiles)
            {
                case Logfiles.Application:
                    fileToWriteTo = ApplicationLogfile;
                    break;
                case Logfiles.Installer:
                    fileToWriteTo = InstallLogfile;
                    break;
                case Logfiles.Uninstaller:
                    fileToWriteTo = UninstallLogfile;
                    break;
            }
            //check if logfile is null
            if (fileToWriteTo == null)
            {
                //check if it's the application logfile
                if(fileToWriteTo == ApplicationLogfile)
                {
                    if(!FailedToWriteToLogWindowShown)
                    {
                        MessageBox.Show("Failed to write to application log: instance is null!");
                        FailedToWriteToLogWindowShown = true;
                    }
                }
                else
                {
                    WriteToLog(string.Format("Tried to write to null log instance: {0}", logfiles.ToString()), Logfiles.Application, LogLevel.Error);
                }
                return;
            }
            fileToWriteTo.Write(message, logLevel);
        }

        public static void WriteToLog(string messageFormat, Logfiles logfile, LogLevel level, params object[] args)
        {
            WriteToLog(string.Format(messageFormat, args),logfile,level);
        }

        public static void Debug(string message)
        {
            WriteToLog(message, Logfiles.Application, LogLevel.Debug);
        }

        public static void Debug(string message, string sendingClass)
        {
            WriteToLog(string.Format("[{0}]: {1}", sendingClass, message), Logfiles.Application, LogLevel.Debug);
        }

        public static void Debug(string message, params object[] args)
        {
            WriteToLog(message, Logfiles.Application, LogLevel.Debug, args);
        }

        public static void Debug(string message, string sendingClass, params object[] args)
        {
            WriteToLog(string.Format("[{0}]: {1}", sendingClass, message), Logfiles.Application, LogLevel.Debug, args);
        }

        public static void Info(string message, Logfiles logfile)
        {
            switch(logfile)
            {
                case Logfiles.Application:
                    WriteToLog(message, logfile, LogLevel.Info);
                    return;
                case Logfiles.Installer:

                    return;
                case Logfiles.Uninstaller:

                    return;
            }
        }

        public static void Info(string message, Logfiles logfile, params object[] args)
        {
            switch (logfile)
            {
                case Logfiles.Application:
                    WriteToLog(message, logfile, LogLevel.Info, args);
                    return;
                case Logfiles.Installer:

                    return;
                case Logfiles.Uninstaller:

                    return;
            }
        }

        public static void Warning(string message)
        {
            WriteToLog(message, Logfiles.Application, LogLevel.Warning);
        }

        public static void Warning(string message, params object[] args)
        {
            WriteToLog(message, Logfiles.Application, LogLevel.Warning, args);
        }

        public static void Error(string message)
        {
            WriteToLog(message, Logfiles.Application, LogLevel.Error);
        }

        public static void Error(string message, string sendingClass)
        {
            WriteToLog(string.Format("[{0}]: {1}",sendingClass,message), Logfiles.Application, LogLevel.Error);
        }

        public static void Error(string message, params object[] args)
        {
            WriteToLog(message, Logfiles.Application, LogLevel.Error, args);
        }

        public static void Error(string message, string sendingClass, params object[] args)
        {
            WriteToLog(string.Format("[{0}]: {1}",sendingClass,message), Logfiles.Application, LogLevel.Error,args);
        }

        public static void Exception(string message)
        {
            WriteToLog(message, Logfiles.Application, LogLevel.Error);
        }

        public static void Exception(string message, params object[] args)
        {
            WriteToLog(message, Logfiles.Application, LogLevel.Exception, args);
        }
    }
}
