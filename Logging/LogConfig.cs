using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace Tofu.Logging
{
	public class LogConfig : ILogConfig
    {
        #region Public Constants

        // ******************************************************************
        // *																*
        // *					    Public Constants		                *
        // *																*
        // ******************************************************************

        // Public Constants 
        public const bool DEFAULT_AUTOSTART = false;
        public const int DEFAULT_AUTOFLUSHPERIODICTIME = 1000;
        public const int DEFAULT_AUTOFLUSHSTARTUPTIME = 2000;
        public const int DEFAULT_CLEANUPLOGS = 10;
        public const int DEFAULT_CLEANUPMESSAGES = 20000;
        public const bool DEFAULT_CLEANUPRESOURCES = false;
        public const int DEFAULT_CLEANUPSESSIONS = 10;
        public const string DEFAULT_FOLDER = "";
        public const LogFolderPath DEFAULT_FOLDERPATH = LogFolderPath.ProgramDataFolder;
        public const LogLevelFlags DEFAULT_GLOBALLEVELS = LogLevelFlags.All;
        public const bool DEFAULT_GLOBALTRACE = false;

        #endregion

        #region Protected Member Variables

        // ******************************************************************
        // *																*
        // *					 Protected Member Variables		            *
        // *																*
        // ******************************************************************

        // Protected Member Variables
        protected List<LogConfigEntry> m_listEntries;
        protected List<LogConfigMonitorType> m_listMonitors;
        protected LogConfigFormatterProvider<object> m_formattersObject;
        protected LogConfigFormatterProvider<Exception> m_formattersException;
        protected LogConfigDefaultEntry m_defaultEntry;
        protected string m_folder;
        protected LogFolderPath m_folderPath;
        protected int m_cleanupSessions;
        protected int m_cleanupLogs;
        protected bool m_cleanupResources;
        protected int m_cleanupMessages;
        protected int m_autoFlushStartupTime;
        protected int m_autoFlushPeriodicTime;
        protected LogLevelFlags m_globalLevels;
        protected bool m_globalTrace;
        protected bool m_autoStart;

        #endregion

        #region Constructors

        // ******************************************************************
        // *																*
        // *					        Constructors				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
        public LogConfig()
        {
            // Set members to default
            m_autoStart = DEFAULT_AUTOSTART;
            m_autoFlushPeriodicTime = DEFAULT_AUTOFLUSHPERIODICTIME;
            m_autoFlushStartupTime = DEFAULT_AUTOFLUSHSTARTUPTIME;
            m_cleanupLogs = DEFAULT_CLEANUPLOGS;
            m_cleanupMessages = DEFAULT_CLEANUPMESSAGES;
            m_cleanupResources = DEFAULT_CLEANUPRESOURCES;
            m_cleanupSessions = DEFAULT_CLEANUPSESSIONS;
            m_folder = DEFAULT_FOLDER;
            m_folderPath = DEFAULT_FOLDERPATH;
            m_globalLevels = DEFAULT_GLOBALLEVELS;
            m_globalTrace = DEFAULT_GLOBALTRACE;
            m_listEntries = new List<LogConfigEntry>();
            m_listMonitors = new List<LogConfigMonitorType>();
            m_formattersObject = new LogConfigFormatterProvider<object>();
            m_formattersException = new LogConfigFormatterProvider<Exception>();
            m_defaultEntry = CreateDefaultEntry();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">
        /// A config instance whose value members must be copied into this
        /// instance (deep copy)
        /// </param>
        protected LogConfig(LogConfig config)
        {
            // Create deep copy
            m_autoStart = config.m_autoStart;
            m_autoFlushPeriodicTime = config.m_autoFlushPeriodicTime;
            m_autoFlushStartupTime = config.m_autoFlushStartupTime;
            m_cleanupLogs = config.m_cleanupLogs;
            m_cleanupMessages = config.m_cleanupMessages;
            m_cleanupResources = config.m_cleanupResources;
            m_cleanupSessions = config.m_cleanupSessions;
            m_folder = config.m_folder;
            m_folderPath = config.m_folderPath;
            m_globalLevels = config.m_globalLevels;
            m_globalTrace = config.m_globalTrace;
            m_formattersObject = config.m_formattersObject.Clone();
            m_formattersException = config.m_formattersException.Clone();
            m_defaultEntry = config.m_defaultEntry.Clone();

            // Create deep copy of entries collection
            m_listEntries = new List<LogConfigEntry>();
            foreach (var entry in config.m_listEntries)
                m_listEntries.Add((LogConfigEntry)entry.Clone());

            // Create deep copy of monitors collection
            m_listMonitors = new List<LogConfigMonitorType>();
            foreach (var monitor in config.m_listMonitors)
                m_listMonitors.Add((LogConfigMonitorType)monitor.Clone());
        }

        #endregion

        #region Protected Methods

        // ******************************************************************
        // *																*
        // *					       Protected Methods				    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Creates a new ILogConfigEntry instance that will be used to configure default settings
        /// </summary>
        /// <returns>
        /// A new ILogConfigEntry instance
        /// </returns>
        protected virtual LogConfigDefaultEntry CreateDefaultEntry()
        {
            return new LogConfigDefaultEntry();
        }

        #endregion

        #region ILogConfig Interface

        // ******************************************************************
        // *																*
        // *					   ILogConfig Interface				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets the entry that matches the specified log name. If no specific entry 
        /// could be found in <i>Entries</i> collection that matches the specified
        /// logname, the default entry will be returned
        /// </summary>
        /// <param name="logName">
        /// A string that specifies the name of a log
        /// </param>
        /// <returns>
        /// A reference to an entry from the <i>Entries</i> collection that matches 
        /// the specified name; if no matching entry could be found the default entry
        /// will be returned.
        /// </returns>
        public virtual LogConfigDefaultEntry GetEntry(string logName)
        {
            // By default we use default entry as reference
            var entry = DefaultEntry;

            // Check if we can find a specific entry for the given name
            LogConfigEntry configEntry = null;
            if (TryGetEntryByName(logName, out configEntry))
                entry = configEntry;

            // Return reference of resolved config entry
            return entry;
        }

        /// <summary>
        /// Gets a fully qualified folder path that is representative for the values
        /// defined in the <see cref="Folder"/> and <see cref="FolderPath"/> properties
        /// </summary>
        /// <remarks>
        /// This method does not take into account that the returned folder already might 
        /// exist on the current host system!
        /// </remarks>
        /// <returns>
        /// A string that holds a fully qualified folder path
        /// </returns>
        public virtual string GetFullFolderPath()
        {
            // Declare local variables
            string path;

            // Build path
            switch (FolderPath)
            {
                case LogFolderPath.ApplicationFolder:
                    path = string.Format(
                        "{0}{1}{2}",
                        AppDomain.CurrentDomain.BaseDirectory,
                        Path.DirectorySeparatorChar,
                        "Log");
                    break;
                case LogFolderPath.ProgramDataFolder:
                    path = string.Format(
                        "{0}{1}{2}{1}{3}",
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        Path.DirectorySeparatorChar,
                        Process.GetCurrentProcess().ProcessName,
                        "Log");
                    break;
                case LogFolderPath.UserFolder:
                    path = string.Format(
                        "{0}{1}{2}{1}{3}",
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        Path.DirectorySeparatorChar,
                        Process.GetCurrentProcess().ProcessName,
                        "Log");
                    break;
                case LogFolderPath.Custom:
                default:
                    // Check if folder has value
                    path = Folder;
                    if (string.IsNullOrEmpty(path))
                        path = "Log";
                    if (!Path.IsPathRooted(path))
                        path = string.Format(
                            "{0}{1}{2}",
                            AppDomain.CurrentDomain.BaseDirectory,
                            Path.DirectorySeparatorChar,
                            path);
                    break;
            }

            // Return composed path
            return path;
        }

        /// <summary>
        /// Tries to find a LogConfigEntry whose Id matches the specified Id
        /// </summary>
        /// <param name="configEntryId">
        /// An integer that specifies a conguration entry id
        /// </param>
        /// <param name="entry">
        /// A LogConfigEntry reference to a matching entry if one could be resolved;
        /// otherwise this reference will return <i>null</i>
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if a matching configuration entry could be resolved for
        /// the specified id; otherwise a bool <i>false</i> will be returned.
        /// </returns>
        public virtual bool TryGetEntryById(int configEntryId, out LogConfigEntry entry)
        {
            // Set default return value
            entry = null;

            // Iterate all entries and find one whose Id matches specified name
            foreach (var e in Entries)
            {
                // Does this entry match?
                if (e.Id == configEntryId)
                {
                    // We found a match, now set reference and break loop
                    entry = e;
                    break;
                }
            }

            // Did we find an entry?
            return entry != null;
        }

        /// <summary>
        /// Tries to find a LogConfigEntry whose mask matches the specified log name
        /// </summary>
        /// <param name="logName">
        /// A string that specifies the name of a log
        /// </param>
        /// <param name="entry">
        /// A LogConfigEntry reference to a matching entry if one could be resolved;
        /// otherwise this reference will return <i>null</i>
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if a matching configuration entry could be resolved for
        /// the specified log name; otherwise a bool <i>false</i> will be returned.
        /// </returns>
        public virtual bool TryGetEntryByName(string logName, out LogConfigEntry entry)
        {
            // Set default return value
            entry = null;

            // Iterate all entries and find one whose mask matches specified name
            foreach (var e in Entries)
            {
                // Does this entry match?
                if (e.IsMatch(logName))
                {
                    // We found a match, now set reference and break loop
                    entry = e;
                    break;
                }
            }

            // Did we find an entry?
            return entry != null;
        }

        /// <summary>
        /// Creates a deep copy of the current config instance
        /// </summary>
        /// <returns>
        /// A new ILogConfig instance that is a deep copy
        /// </returns>
        public virtual ILogConfig Clone()
        {
            // Create deep copy
            return new LogConfig(this);
        }

        /// <summary>
        /// Gets or sets a bool that specifies if the LogManager should 
        /// automatically be started 
        /// </summary>
        [XmlElement("autostart")]
        public virtual bool AutoStart
        {
            get { return m_autoStart; }
            set { m_autoStart = value; }
        }

        /// <summary>
        /// Gets or sets an ILogConfigEntry instance that will be used when no specific 
        /// entry could be found that was associated with the an <i>ILog</i>
        /// </summary>
        [XmlElement("defaultentry")]
        public virtual LogConfigDefaultEntry DefaultEntry
        {
            get { return m_defaultEntry; }
            set { m_defaultEntry = value; }
        }

        /// <summary>
        /// Gets or sets an integer that specifies the number of log sessions that should
        /// be preserved before the folder is removed.<br/>
        /// <b>Note:</b> If zero or a negative value is specified, none of the log sessions 
        /// will be removed and used disk space will keep increasing!
        /// </summary>
        [XmlElement("cleanupsessions")]
        public virtual int CleanupSessions
        {
            get { return m_cleanupSessions; }
            set { m_cleanupSessions = value >= 0 ? value : -1; }
        }

        /// <summary>
        /// Gets or sets an integer that specifies the number of log file generations to keep before they
        /// are deleted.<br/>
        /// <b>Note:</b> If zero or a negative value is specified, all generations will be kept which
        /// means that used disk space will keep increasing!
        /// </summary>
        [XmlElement("cleanuplogs")]
        public virtual int CleanupLogs
        {
            get { return m_cleanupLogs; }
            set { m_cleanupLogs = value >= 0 ? value : -1; }
        }

        /// <summary>
        /// Gets or sets a bool that specifies when cleanup is removing log files, the associated resource
        /// files for these logs will not be removed at all.
        /// <b>Note:</b> If true is specified; all generated resource files will be kept which means the
        /// used disk space will keep increasing!
        /// </summary>
        [XmlElement("cleanupresources")]
        public virtual bool CleanupResources
        {
            get { return m_cleanupResources; }
            set { m_cleanupResources = value; }
        }

        /// <summary>
        /// Gets or sets an integer that specifies the maximum number of message that are allowed
        /// to be in a log file. If this amount is exceeded, a new generation of the log file
        /// will be created. If the number of older log files exceeds CleanupKeepFiles value, oldest 
        /// log files will be removed.<br/>
        /// <b>Note:</b> If zero or a negative value is specified no limit will be applied to the
        /// log files, but used disk space will keep increasing!
        /// </summary>
        [XmlElement("cleanupmessages")]
        public virtual int CleanupMessages
        {
            get { return m_cleanupMessages; }
            set { m_cleanupMessages = value >= 0 ? value : -1; }
        }

        /// <summary>
        /// Gets or sets a string that holds the session folder. By default this property
        /// will equal [Application]\[Log]
        /// </summary>
        [XmlElement("folder")]
        public virtual string Folder
        {
            get { return m_folder; }
            set { m_folder = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets a LogFolderPath enum value that specifies the root of the Folder path
        /// </summary>
        [XmlElement("folderpath")]
        public virtual LogFolderPath FolderPath
        {
            get { return m_folderPath; }
            set { m_folderPath = value; }
        }

        /// <summary>
        /// Gets or sets an integer that specifies, in milliseconds, the initial delay of the
        /// auto-flush timer.<br/>
        /// </summary>
        [XmlElement("autoflushstartuptime")]
        public virtual int AutoFlushStartupTime
        {
            get { return m_autoFlushStartupTime; }
            set
            {
                // Validate input
                if (value < 0)
                    throw new ArgumentException("Invalid value; must be zero or positive value");

                // Store new value
                m_autoFlushStartupTime = value;
            }
        }

        /// <summary>
        /// Gets or sets an integer that specifies, in milliseconds, the periodic interval that the
        /// auto-flush timer must apply.<br/>
        /// </summary>
        [XmlElement("autoflushperiodictime")]
        public virtual int AutoFlushPeriodicTime
        {
            get { return m_autoFlushPeriodicTime; }
            set
            {
                // Validate input
                if (value < 0)
                    throw new ArgumentException("Invalid value; must be zero or positive value");

                // Store new value
                m_autoFlushPeriodicTime = value;
            }
        }

        /// <summary>
        /// Gets or sets a bool that specifies if log messages should also
        /// be traced to the debug output console
        /// </summary>
        [XmlElement("globaltrace")]
        public virtual bool GlobalTrace
        {
            get { return m_globalTrace; }
            set { m_globalTrace = value; }
        }

        /// <summary>
        /// Gets or sets a LogLevelFlags enum value that specifies either the
        /// indiviual or combined log levels flags that determine whether 
        /// logging for that particular level will be enabled or disabled
        /// </summary>
        [XmlElement("globallevels")]
        public virtual LogLevelFlags GlobalLevels
        {
            get { return m_globalLevels; }
            set { m_globalLevels = value; }
        }

        /// <summary>
        /// Gets or sets a collection of <i>ILogConfigEntry</i> instances
        /// </summary>
        [XmlElement("entry")]
        public virtual List<LogConfigEntry> Entries
        {
            get { return m_listEntries; }
            set { m_listEntries = value; }
        }

        /// <summary>
        /// Gets or sets a collection of ILogConfigMonitorTypes that holds all the monitors
        /// that must be created when logging session is started
        /// </summary>
        [XmlElement("monitor")]
        public virtual List<LogConfigMonitorType> MonitorTypes
        {
            get { return m_listMonitors; }
            set { m_listMonitors = value; }
        }

        /// <summary>
        /// Gets or sets an LogConfigFormatterProvider class that manages formatters that
        /// can be used when objects are serialized into strings
        /// </summary>
        [XmlElement("formattersobject")]
        public virtual LogConfigFormatterProvider<object> FormattersObject
        {
            get { return m_formattersObject; }
            set { m_formattersObject = value; }
        }

        /// <summary>
        /// Gets or sets an LogConfigFormatterProvider class that manages formatters that
        /// can be used when Exceptions are serialized into strings
        /// </summary>
        [XmlElement("formattersexception")]
        public virtual LogConfigFormatterProvider<Exception> FormattersException
        {
            get { return m_formattersException; }
            set { m_formattersException = value; }
        }

        #endregion
    }
}
