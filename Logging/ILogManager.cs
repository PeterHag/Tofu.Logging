using System;
using System.Drawing;

namespace Tofu.Logging
{
    #region Enumerations

    // ******************************************************************
    // *																*
    // *					      Enumerations						    *
    // *																*
    // ******************************************************************

    /// <summary>
    /// An enumeration that specifies all the supported log levels flags
    /// that individually can be enabled or disabled
    /// </summary>
    [Flags]
    public enum LogLevelFlags
    {
        Global = 0x8000,
        All = Fatal | Error | Warning | Info | Debug,
        Fatal = LogLevel.Fatal,
        Error = LogLevel.Error,
        Warning = LogLevel.Warning,
        Info = LogLevel.Info,
        Debug = LogLevel.Debug,
        None = 0
    }

    /// <summary>
    /// An enumeration that specifies all the supported session modes
    /// </summary>
    public enum SessionModes
    {
        Running,
        Suspended,
        Stopped
    }

    #endregion

    public interface ILogManager
    {
        #region Events

        // ******************************************************************
        // *																*
        // *					          Methods							*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Event that will be raised when session mode has changed
        /// </summary>
        event EventHandler SessionModeChanged;

        /// <summary>
        /// Event that will be raised when either a new log has been created
        /// or when the logs collection has been cleared (eg. session stopped)
        /// </summary>
        event EventHandler LogsChanged;

        #endregion

        #region Methods

        // ******************************************************************
        // *																*
        // *					          Methods							*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Starts a new logging session
        /// </summary>
        /// <returns>
        /// An ILogManager reference
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a session is already running<br/>
        /// <br/>
        /// - Or -<br/>
        /// <br/>
        /// The folder specified in the Configuration does not exist<br/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if an empty folder is specified in the Configuration
        /// </exception>
        ILogManager StartSession();

        /// <summary>
        /// Pauzes the current session by dropping all log messages. This mode should be
        /// tool that allows user to inspect logging on running application
        /// </summary>
        /// <returns>
        /// A bool <i>true</i> if current session could be suspended successfully;
        /// otherwise a bool <i>false</i> will be returned.
        /// </returns>
        bool SuspendSession();

        /// <summary>
        /// Re-starts suspended session
        /// </summary>
        /// <returns>
        /// A bool <i>true</i> if suspended session could be resumed successfully;
        /// otherwise a bool <i>false</i> will be returned.
        /// </returns>
        bool ResumeSession();

        /// <summary>
        /// Stops the current logging session and forces all logs to flush their
        /// pending output
        /// </summary>
        void StopSession();

        /// <summary>
        /// Forces all loggers to flush their pending log messages
        /// </summary>
        void Flush();

        /// <summary>
        /// Checks if the current session is enabled for the specified level
        /// on the specified log
        /// </summary>
        /// <param name="log">
        /// An ILog reference
        /// </param>
        /// <param name="level">
        /// A LogLevel enum value that specifies the level that must 
        /// be checked
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if the current session is enabled for the specified 
        /// session, otherwise a bool <i>false</i> will be returned.
        /// </returns>
        bool IsLevelEnabled(ILog log, LogLevel level);

		/// <summary>
		/// Creates a new LogMessage and pushes it towards the async logging thread
		/// </summary>
		/// <param name="log">
		/// An ILog reference to the logger that is pushing the message
		/// </param>
		/// <param name="level">
		/// A LogLevel enum value that specifies the severity of the logged message
		/// </param>
		/// <param name="parentId">
		/// A long that specifies the id of the parent log message
		/// </param>
		/// <param name="startId">
		/// A long that specifies the id of the start log message
		/// </param>
		/// <param name="messageType">
		/// A LogMessageType enum value that specifies the type of the message
		/// </param>
		/// <param name="text">
		/// A delegate that provides the text that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		/// <param name="report">
		/// A delegate that provides the report that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		/// <param name="reportExtension">
		/// A delegate that provides an extension that identifies the file type of the report.
		/// </param>
		/// <param name="bytes">
		/// A delegate that provides an array of bytes (eg. from image) that must be logged. 
		/// This delegate will only be executed when logging for this level has been enabled.
		/// </param>
		/// <param name="bytesExtension">
		/// A delegate that provides an extension that identifies the file type of the bytes array. 
		/// </param>
		/// <param name="value">
		/// A double? that specifies a value that must be logged
		/// </param>
		/// <param name="valueLowerBound">
		/// A double? that specifies the lower boundary of the logged value
		/// </param>
		/// <param name="valueUpperBound">
		/// A double? that specifies the upper boundary of the logged value
		/// </param>
		/// <param name="ex">
		/// An Exception instance that must be logged
		/// </param>
		/// <param name="parameterNames">
		/// A delegate that provides an array of strings that specifies the names of the
		/// parameters that must be logged. The contents of this array must be in sync
		/// with the contents of the parameter values array. This delegate will only be 
		/// executed when logging for for this level has been enabled
		/// </param>
		/// <param name="parameterValues">
		/// A delegate that provides an array of objects that specifies the values of the
		/// parameters that must be logged. The contents of this array must be in sync
		/// with the contents of the parameter names array. This delegate will only be 
		/// executed when logging for for this level has been enabled
		/// </param>
		/// <param name="obj">
		/// An object instance  that must be logged
		/// </param>
		/// <param name="formatAsync">
		/// A bool <i>true</i> in case the logging worker thread should do formatting. If the
		/// calling thread already performs formatting, this argument will equal <i>false</i>
		/// and the formatted text will be provided in the <i>aFormattedText</i> argument
		/// </param>
		/// <param name="formatterName">
		/// A string that specifies the name of a formatter to use in case async formatting 
		/// must be executed in the logging worker thread
		/// </param>
		/// <returns>
		/// A long that uniquely identifies this message across all loggers within the current 
		/// logging session.
		/// </returns>
		long AddMessage(
            ILog log,
            LogLevel level,
            long parentId,
            long startId,
            LogMessageType messageType,
            Func<string> text,
            Func<string> report,
            Func<string> reportExtension,
            Func<byte[]> bytes,
            Func<string> bytesExtension,
            double? value,
            double? valueLowerBound,
            double? valueUpperBound,
            Exception ex,
            Func<string[]> parameterNames,
            Func<object[]> parameterValues,
            object obj,
            bool formatAsync,
            string formatterName);

		/// <summary>
		/// Gets the ILog that is associated with the specified name. If no log
		/// yet exists then the manager will create a new instance
		/// </summary>
		/// <param name="logName">
		/// A string that specifies the name of a log.<br/>
		/// Please note name must be treated as case-insensitive!
		/// </param>
		/// <returns>
		/// An ILog to either an already existing or a newly created one.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if an invalid logname is specified
		/// </exception>
		ILog GetLog(string logName);

        /// <summary>
        /// Gets the ILog that is associated with the fully qualified name of the
        /// specified type. If no log yet exists then the manager will create 
        /// a new instance
        /// </summary>
        /// <param name="logType">
        /// A Type whose full name will be used to either lookup an exisiting log or,
        /// in case log does not exist, create a new ILog instance.
        /// </param>
        /// <returns>
        /// An ILog to either an already existing or a newly created one.
        /// </returns>
        ILog GetLog(Type logType);

        #endregion 

        #region Properties

        // ******************************************************************
        // *																*
        // *					          Properties						*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets a SessionModes enum value that indicates current state of session
        /// </summary>
        SessionModes SessionMode
        {
            get;
        }

        /// <summary>
        /// Gets a Guid that uniquely identifies the current session.
        /// </summary>
        Guid SessionGuid
        {
            get;
        }

        /// <summary>
        /// Gets a string that specifies the context of the current session.<br/>
        /// Typically the context represents the name of the executable.
        /// </summary>
        string SessionContext
        {
            get;
        }

        /// <summary>
        /// Gets a string that holds the current session folder 
        /// </summary>
        string SessionFolder
        {
            get;
        }

        /// <summary>
        /// Gets a string that holds the current session folder (i.e. location where
        /// current log(s) will be persisted) including process context name
        /// </summary>
        string SessionFolderFullName
        {
            get;
        }

        /// <summary>
        /// Gets an array that holds all the monitor instances that are bound
        /// to the current logging session
        /// </summary>
        ILogMonitor[] SessionMonitors
        {
            get;
        }

        /// <summary>
        /// Gets or sets an ILogConfig reference
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a null reference is specified
        /// </exception>
        ILogConfig Config
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a string that specifies a subfolder within the main application 
        /// logging folder where all the logs per session will be stored
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when an empty or blank string is specified<br/>
        /// <br/>
        /// - Or -<br/>
        /// <br/>
        /// Thrown when the specified value contains an illegal file name character
        /// such as\ / : * ? " &lt; &gt; |
        /// </exception>
        string Context
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an array reference to all the currently created ILog instances
        /// </summary>
        ILog[] Logs
        {
            get;
        }

        /// <summary>
        /// Gets an object reference that can be used to get synchronized access to the manager
        /// </summary>
        object SyncRoot
        {
            get;
        }

        #endregion
    }
}