using System;
using System.ComponentModel;

namespace Tofu.Logging
{
    #region Enumerations

    // ******************************************************************
    // *																*
    // *					      Enumerations						    *
    // *																*
    // ******************************************************************

    /// <summary>
    /// An enumeration that specifies all the supported logging levels
    /// that can associated with a specific logging message
    /// </summary>
    public enum LogLevel
    {
        Fatal   = 0x0010,
        Error   = 0x0008,
        Warning = 0x0004,
        Info    = 0x0002,
        Debug   = 0x0001,
    }

    #endregion

    public interface ILog
    {
        #region Methods

        // ******************************************************************
        // *																*
        // *					        Methods						        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Adds the specified text to the log
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value that specifies the severity of the logged message
        /// </param>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
        /// <param name="parentId">
        /// A long that specifies the id of another log message that is considered as parent
        /// for this message. This value will only be used when logging afterward is inspected
        /// in hierarchical mode.<br/>
        /// <b>Note:</b> Specify <i>-1</i> if no parent id is required.
        /// </param>
        /// <param name="startId">
        /// A long that specifies the id of another log message that is considered as starting
        /// point for this message. This value will only be used when logging afterward is 
        /// inspected and a timespan must be computed between the two messages.
        /// <b>Note:</b> Specify <i>-1</i> if no start id is required.
        /// </param>
        /// <returns>
        /// A long that uniquely identifies this message across all loggers within the current 
        /// logging session.
        /// </returns>
        long AddText(
            LogLevel level,
            Func<string> text,
            long parentId = -1,
            long startId = -1);

        /// <summary>
        /// Adds the specified text to the log and the specified object to the resource log
        /// </summary>
        /// <remarks>
        /// <b>Note:</b> You must set argument <i>aFormatAsync</i> to <i>false</i> when logging 
        /// state of UI controls to prevent cross-thread access exceptions!
        /// </remarks>
        /// <param name="level">
        /// A LogLevel enum value that specifies the severity of the logged message
        /// </param>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
        /// <param name="obj">
        /// An Object that must be serialized and appended to the resources log
        /// </param>
        /// <param name="formatterName">
        /// A string that specifies the name of the formatter that must be used to format the object
        /// into a human readable string expression. If this argument equals <i>null</i>, the default
        /// formatter will be used.
        /// </param>
        /// <param name="formatAsync">
        /// A bool that specifies when the formatting opertion must be executed.<br/>
        /// If this argument equals <i>true</i> the formatting will be executed in the logging thread,
        /// in this case application logic is supposed <b>not</b> to change the object after the 
        /// methodcall.<br/>
        /// If this argument equals <i>false</i> the formatting operation will be executed in the calling
        /// thread which might impact application performance.<br/>
        /// <b>Note:</b> You must set this argument to <i>false</i> when logging state of UI controls!
        /// </param>
        /// <param name="parentId">
        /// A long that specifies the id of another log message that is considered as parent
        /// for this message. This value will only be used when logging afterward is inspected
        /// in hierarchical mode.<br/>
        /// <b>Note:</b> Specify <i>-1</i> if no parent id is required.
        /// </param>
        /// <param name="startId">
        /// A long that specifies the id of another log message that is considered as starting
        /// point for this message. This value will only be used when logging afterward is 
        /// inspected and a timespan must be computed between the two messages.
        /// <b>Note:</b> Specify <i>-1</i> if no start id is required.
        /// </param>
        /// <returns>
        /// A long that uniquely identifies this message across all loggers within the current 
        /// logging session.
        /// </returns>
        long AddObject(
            LogLevel level,
            Func<string> text,
            object obj,
            string formatterName = null,
            bool formatAsync = true,
            long parentId = -1,
            long startId = -1);

        /// <summary>
        /// Adds the specified text to the log and the specified exception to the resource log
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value that specifies the severity of the logged message
        /// </param>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
        /// <param name="ex">
        /// An Exception that must be serialized and appended to the resources log
        /// </param>
        /// <param name="formatterName">
        /// A string that specifies the name of the formatter that must be used to format the exception
        /// into a human readable string expression. If this argument equals <i>null</i>, the default
        /// formatter will be used.
        /// </param>
        /// <param name="formatAsync">
        /// A bool that specifies when the formatting opertion must be executed.<br/>
        /// If this argument equals <i>true</i> the formatting will be executed in the logging thread,
        /// in this case application logic is supposed <b>not</b> to change the object after the 
        /// methodcall.<br/>
        /// If this argument equals <i>false</i> the formatting operation will be executed in the calling
        /// thread which might impact application performance.
        /// </param>
        /// <param name="parentId">
        /// A long that specifies the id of another log message that is considered as parent
        /// for this message. This value will only be used when logging afterward is inspected
        /// in hierarchical mode.<br/>
        /// <b>Note:</b> Specify <i>-1</i> if no parent id is required.
        /// </param>
        /// <param name="startId">
        /// A long that specifies the id of another log message that is considered as starting
        /// point for this message. This value will only be used when logging afterward is 
        /// inspected and a timespan must be computed between the two messages.
        /// <b>Note:</b> Specify <i>-1</i> if no start id is required.
        /// </param>
        /// <returns>
        /// A long that uniquely identifies this message across all loggers within the current 
        /// logging session.
        /// </returns>
        long AddException(
            LogLevel level,
            Func<string> text,
            Exception ex,
            string formatterName = null,
            bool formatAsync = true,
            long parentId = -1,
            long startId = -1);

        /// <summary>
        /// Adds the specified text to the log and the specified image to the resource log
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value that specifies the severity of the logged message
        /// </param>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
        /// <param name="bytes">
        /// A delegate that provides an array of bytes (eg. from image) that must be logged. 
		/// This delegate will only be executed when logging for this level has been enabled.
        /// </param>
        /// <param name="bytesExtension">
        /// A delegate that provides an extension that identifies the file type of the bytes array. 
        /// </param>
        /// <param name="parentId">
        /// A long that specifies the id of another log message that is considered as parent
        /// for this message. This value will only be used when logging afterward is inspected
        /// in hierarchical mode.<br/>
        /// <b>Note:</b> Specify <i>-1</i> if no parent id is required.
        /// </param>
        /// <param name="startId">
        /// A long that specifies the id of another log message that is considered as starting
        /// point for this message. This value will only be used when logging afterward is 
        /// inspected and a timespan must be computed between the two messages.
        /// <b>Note:</b> Specify <i>-1</i> if no start id is required.
        /// </param>
        /// <returns>
        /// A long that uniquely identifies this message across all loggers within the current 
        /// logging session.
        /// </returns>
        long AddBytes(
            LogLevel level,
            Func<string> text,
            Func<byte[]> bytes,
            Func<string> bytesExtension,
            long parentId = -1,
            long startId = -1);

		/// <summary>
		/// Adds the specified text to the log and the specified report to the resource log
		/// </summary>
		/// <param name="level">
		/// A LogLevel enum value that specifies the severity of the logged message
		/// </param>
		/// <param name="text">
		/// A delegate that provides the text that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		/// <param name="report">
		/// A delegate that provides a report text that must be serialized and appended to the 
		/// resources log. This delegate will only be executed when logging for this level has
		/// been enabled.
		/// </param>
		/// <param name="reportExtension">
		/// A delegate that provides an extension that identifies the file type of the report.
		/// </param>
		/// <param name="parentId">
		/// A long that specifies the id of another log message that is considered as parent
		/// for this message. This value will only be used when logging afterward is inspected
		/// in hierarchical mode.<br/>
		/// <b>Note:</b> Specify <i>-1</i> if no parent id is required.
		/// </param>
		/// <param name="startId">
		/// A long that specifies the id of another log message that is considered as starting
		/// point for this message. This value will only be used when logging afterward is 
		/// inspected and a timespan must be computed between the two messages.
		/// <b>Note:</b> Specify <i>-1</i> if no start id is required.
		/// </param>
		/// <returns>
		/// A long that uniquely identifies this message across all loggers within the current 
		/// logging session.
		/// </returns>
		long AddReport(
            LogLevel level,
            Func<string> text,
            Func<string> report,
            Func<string> reportExtension,
            long parentId = -1,
            long startId = -1);

        /// <summary>
        /// Adds the specified text to the log and the specified absolute value to the log
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value that specifies the severity of the logged message
        /// </param>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
        /// <param name="value">
        /// A double that specifies an absolute value 
        /// </param>
        /// <param name="valueLowerBound">
        /// A nullable double that can specify the lower boundary of the absolute value
        /// </param>
        /// <param name="valueUpperBound">
        /// A nullable double that can specify the upper boundary of the absolute value
        /// </param>
        /// <param name="parentId">
        /// A long that specifies the id of another log message that is considered as parent
        /// for this message. This value will only be used when logging afterward is inspected
        /// in hierarchical mode.<br/>
        /// <b>Note:</b> Specify <i>-1</i> if no parent id is required.
        /// </param>
        /// <param name="startId">
        /// A long that specifies the id of another log message that is considered as starting
        /// point for this message. This value will only be used when logging afterward is 
        /// inspected and a timespan must be computed between the two messages.
        /// <b>Note:</b> Specify <i>-1</i> if no start id is required.
        /// </param>
        /// <returns>
        /// A long that uniquely identifies this message across all loggers within the current 
        /// logging session.
        /// </returns>
        long AddAbsoluteValue(
            LogLevel level,
            Func<string> text,
            double value,
            double? valueLowerBound,
            double? valueUpperBound,
            long parentId = -1,
            long startId = -1);

        /// <summary>
        /// Adds the specified text to the log and the specified relative value to the log
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value that specifies the severity of the logged message
        /// </param>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
        /// <param name="value">
        /// A double that specifies a relative value (i.e. nominal between 0 and 1).
        /// </param>
        /// <param name="parentId">
        /// A long that specifies the id of another log message that is considered as parent
        /// for this message. This value will only be used when logging afterward is inspected
        /// in hierarchical mode.<br/>
        /// <b>Note:</b> Specify <i>-1</i> if no parent id is required.
        /// </param>
        /// <param name="startId">
        /// A long that specifies the id of another log message that is considered as starting
        /// point for this message. This value will only be used when logging afterward is 
        /// inspected and a timespan must be computed between the two messages.
        /// <b>Note:</b> Specify <i>-1</i> if no start id is required.
        /// </param>
        /// <returns>
        /// A long that uniquely identifies this message across all loggers within the current 
        /// logging session.
        /// </returns>
        long AddRelativeValue(
            LogLevel level,
            Func<string> text,
            double value,
            long parentId = -1,
            long startId = -1);

        /// <summary>
        /// Adds the specified text together with parameter name-value pairs to the log
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value that specifies the severity of the logged message
        /// </param>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
        /// <param name="parameterNames">
        /// A delegate that provides an array of strings that specifies the names of the
        /// parameters that must be logged. The contents of this array must be in sync
        /// with the contents of the parameter values array. This delegate will only be 
        /// executed when logging for for this level has been enabled
        /// </param>
        /// <param name="parameterValues">
        /// A delegate that provides an array of objectshat specifies the values of the
        /// parameters that must be logged. The contents of this array must be in sync
        /// with the contents of the parameter names array. This delegate will only be 
        /// executed when logging for for this level has been enabled
        /// </param>
        /// <param name="parentId">
        /// A long that specifies the id of another log message that is considered as parent
        /// for this message. This value will only be used when logging afterward is inspected
        /// in hierarchical mode.<br/>
        /// <b>Note:</b> Specify <i>-1</i> if no parent id is required.
        /// </param>
        /// <param name="startId">
        /// A long that specifies the id of another log message that is considered as starting
        /// point for this message. This value will only be used when logging afterward is 
        /// inspected and a timespan must be computed between the two messages.
        /// <b>Note:</b> Specify <i>-1</i> if no start id is required.
        /// </param>
        /// <returns>
        /// A long that uniquely identifies this message across all loggers within the current 
        /// logging session.
        /// </returns>
        long AddParams(
            LogLevel level,
            Func<string> text,
            Func<string[]> parameterNames,
            Func<object[]> parameterValues,
            long parentId = 1,
            long startId = -1);

        /// <summary>
        /// Updates the read-only <see cref="ConfigEntryId"/> property value. This method
        /// will be called when the associated <see cref="LogManager"/> is restarted.
        /// </summary>
        /// <param name="configEntryId">
        /// An int that holds a new entry id value
        /// </param>
        void SetConfigEntryId(int configEntryId);

        #endregion

        #region Properties

        // ******************************************************************
        // *																*
        // *					        Properties					        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets an ILogManager reference to logging manager that is associated
        /// with this logger
        /// </summary>
        ILogManager LogManager
        {
            get;
        }

        /// <summary>
        /// Gets an integer that holds the Id of the configuration entry whose
        /// settings were used to initialize this log instance.<br/>
        /// <b>Note:</b> If no specific entry was used but default entry was
        /// used, this property value will equal <i>LogConfigEntry.INVALID_ID</i>
        /// constant 
        /// </summary>
        int ConfigEntryId
        {
            get;
        }

        /// <summary>
        /// Gets a string that holds the name that is associated with this logger
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets a LogLevelFlags enum value that specifies the local enabled flags
        /// </summary>
        [DefaultValue(LogLevelFlags.Global)]
        LogLevelFlags LocalLevels
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a LocalTraceMode enum value that specifies if the message
        /// should be traced to debug console
        /// </summary>
        [DefaultValue(LocalTraceMode.Global)]
        LocalTraceMode LocalTrace
        {
            get;
            set;
        }

        #endregion
    }
}
