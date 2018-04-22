using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Tofu.Logging
{
	#region Enumerations

	// ******************************************************************
	// *																*
	// *					      Enumerations					        *
	// *																*
	// ******************************************************************

	/// <summary>
	/// An enumeration that specifies all the supported folder paths
	/// </summary>
	public enum LogFolderPath
    {
        [Description("Maps to 'Environment.SpecialFolder.CommonApplicationData'")]
        ProgramDataFolder,

        [Description("Maps to 'Environment.SpecialFolder.LocalApplicationData'")]
        UserFolder,

        [Description("Maps to 'Application.StartupPath'")]
        ApplicationFolder,

        [Description("Maps to a custom user defined path. If path not rooted, then application folder will be used as offset")]
        Custom
    }

    #endregion

    public interface ILogConfig
    {
        #region Methods

        // ******************************************************************
        // *																*
        // *					         Methods					        *
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
        LogConfigDefaultEntry GetEntry(string logName);

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
        string GetFullFolderPath();

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
        bool TryGetEntryById(int configEntryId, out LogConfigEntry entry);

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
        bool TryGetEntryByName(string logName, out LogConfigEntry entry);

        /// <summary>
        /// Creates a deep copy of the current config instance
        /// </summary>
        /// <returns>
        /// A new ILogConfig instance that is a deep copy
        /// </returns>
        ILogConfig Clone();

        #endregion

        #region Properties

        // ******************************************************************
        // *																*
        // *					        Properties						    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets or sets a bool that specifies if the LogManager should 
        /// automatically be started 
        /// </summary>
        bool AutoStart
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an ILogConfigEntry instance that will be used when no specific 
        /// entry could be found that was associated with the an <i>ILog</i>
        /// </summary>
        LogConfigDefaultEntry DefaultEntry
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a string that holds the session folder. By default this property
        /// will equal [Application]\[Log]
        /// </summary>
        string Folder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a LogFolderPath enum value that specifies the root path
        /// where the log files should be stored. By default this property will equal
        /// [ProgramDataFolder]
        /// </summary>
        LogFolderPath FolderPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an LogConfigFormatterProvider class that manages formatters that
        /// can be used when objects are serialized into strings
        /// </summary>
        LogConfigFormatterProvider<object> FormattersObject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an LogConfigFormatterProvider class that manages formatters that
        /// can be used when Exceptions are serialized into strings
        /// </summary>
        LogConfigFormatterProvider<Exception> FormattersException
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an integer that specifies, in milliseconds, the initial delay of the
        /// auto-flush timer.<br/>
        /// <b>Note:</b> If this value equals <i>0</i> the auto-flush timer will be disabled.
        /// </summary>
        [DefaultValue(2000)]
        int AutoFlushStartupTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an integer that specifies, in milliseconds, the periodic interval that the
        /// auto-flush timer must apply.<br/>
        /// <b>Note:</b> If this value equals <i>0</i> the auto-flush timer will be disabled.
        /// </summary>
        [DefaultValue(1000)]
        int AutoFlushPeriodicTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a bool that specifies if log messages should also
        /// be traced to the debug output console
        /// </summary>
        [DefaultValue(false)]
        bool GlobalTrace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a LogLevelFlags enum value that specifies either the
        /// indiviual or combined log levels flags that determine whether 
        /// logging for that particular level will be enabled or disabled
        /// </summary>
        [DefaultValue(LogLevelFlags.All)]
        LogLevelFlags GlobalLevels
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an integer that specifies the number of log sessions that should
        /// be preserved before the folder is removed.<br/>
        /// <b>Note:</b> If zero or a negative value is specified none of the log sessions 
        /// will be removed, but used disk space will keep increasing!
        /// </summary>
        [DefaultValue(5)]
        int CleanupSessions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an integer that specifies the number of log file generations to keep before they
        /// are deleted.<br/>
        /// <b>Note:</b> If zero or a negative value is specified, all generations will be kept which
        /// means that used disk space will keep increasing!
        /// </summary>
        [DefaultValue(5)]
        int CleanupLogs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a bool that specifies when cleanup is removing log files, the associated resource
        /// files for these logs will not be removed at all.
        /// <b>Note:</b> If true is specified; all generated resource files will be kept which means the
        /// used disk space will keep increasing!
        /// </summary>
        [DefaultValue(false)]
        bool CleanupResources
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an integer that specifies the maximum number of message that are allowed
        /// to be in a log file. If this amount is exceeded, a new generation of the log file
        /// will be created. If the number of older log files exceeds CleanupKeepFiles value, oldest 
        /// log files will be removed.<br/>
        /// <b>Note:</b> If zero or a negative value is specified no limit will be applied to the
        /// log files, but used disk space will keep increasing!
        /// </summary>
        [DefaultValue(25000)]
        int CleanupMessages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a collection of <i>ILogConfigEntry</i> instances
        /// </summary>
        [XmlElement("entry")]
        List<LogConfigEntry> Entries
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a collection of ILogConfigMonitorTypes that holds all the monitors
        /// that must be created when logging session is started
        /// </summary>
        [XmlElement("monitor")]
        List<LogConfigMonitorType> MonitorTypes
        {
            get;
            set;
        }

        #endregion
    }
}
