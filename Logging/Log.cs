using System;
using System.ComponentModel;

namespace Tofu.Logging
{
	public class Log : ILog, ILog4N
	{
		#region Constructors

		// ******************************************************************
		// *																*
		// *					        Constructors				        *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">
		/// A string that specifies the name that will be associated 
		/// with this logger
		/// </param>
		/// <param name="logManager">
		/// An ILogManager refence
		/// </param>
		/// <param name="configEntryId">
		/// An integer that holds the Id of the configuration entry whose
		/// settings were used to initialize this log instance.<br/>
		/// <b>Note:</b> If no specific entry was used but default entry was
		/// used, this argument will equal <i>LogConfigEntry.INVALID_ID</i>
		/// constant.
		/// </param>
		internal Log(
			string name,
			ILogManager logManager,
			int configEntryId)
		{
			// Set members to default
			this.Name = name;
			this.LogManager = logManager;
			this.ConfigEntryId = configEntryId;
			this.LocalLevels = LogLevelFlags.Global;
			this.LocalTrace = LocalTraceMode.Global;
		}

		#endregion

		#region ILog Interface

		// ******************************************************************
		// *																*
		// *					        ILog Interface					    *
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
		public virtual long AddText(
			LogLevel level,
			Func<string> text,
			long parentId = Tofu.Logging.LogManager.INVALID_ID,
			long startId = Tofu.Logging.LogManager.INVALID_ID)
		{
			// Delegate LogManager
			return LogManager.SessionMode != SessionModes.Running ?
				Tofu.Logging.LogManager.INVALID_ID :
				LogManager.AddMessage(
					this,
					level,
					parentId,
					startId,
					LogMessageType.Text,
					text,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified text to the log and the specified object to the resource log.
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
		/// thread which might impact application performance.
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
		public virtual long AddObject(
		   LogLevel level,
		   Func<string> text,
		   object obj,
		   string formatterName = null,
		   bool formatAsync = true,
		   long parentId = -1,
		   long startId = -1)
		{
			// Delegate to protected overridable method
			return LogManager.SessionMode != SessionModes.Running ?
				Tofu.Logging.LogManager.INVALID_ID :
				LogManager.AddMessage(
					this,
					level,
					parentId,
					startId,
					LogMessageType.Object,
					text,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					obj,
					formatAsync,
					formatterName);
		}

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
		/// A string that specifies the name of the formatter that must be used format the exception
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
		public virtual long AddException(
			LogLevel level,
			Func<string> text,
			Exception ex,
			string formatterName = null,
			bool formatAsync = true,
			long parentId = Tofu.Logging.LogManager.INVALID_ID,
			long startId = Tofu.Logging.LogManager.INVALID_ID)
		{
			// Delegate to protected overridable method
			return LogManager.SessionMode != SessionModes.Running ?
				Tofu.Logging.LogManager.INVALID_ID :
				LogManager.AddMessage(
					this,
					level,
					parentId,
					startId,
					LogMessageType.Exception,
					text,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					ex,
					null,
					null,
					null,
					formatAsync,
					formatterName);
		}

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
		public virtual long AddBytes(
			LogLevel level,
			Func<string> text,
			Func<byte[]> bytes,
			Func<string> bytesExtension,
			long parentId = Tofu.Logging.LogManager.INVALID_ID,
			long startId = Tofu.Logging.LogManager.INVALID_ID)
		{
			// Delegate to protected overridable method
			return LogManager.SessionMode != SessionModes.Running ?
				Tofu.Logging.LogManager.INVALID_ID :
				LogManager.AddMessage(
					this,
					level,
					parentId,
					startId,
					LogMessageType.Bytes,
					text,
					null,
					null,
					bytes,
					bytesExtension,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

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
		public virtual long AddReport(
			LogLevel level,
			Func<string> text,
			Func<string> report,
			Func<string> reportExtension,
			long parentId = Tofu.Logging.LogManager.INVALID_ID,
			long startId = Tofu.Logging.LogManager.INVALID_ID)
		{
			// Delegate to protected overridable method
			return LogManager.SessionMode != SessionModes.Running ?
				Tofu.Logging.LogManager.INVALID_ID :
				LogManager.AddMessage(
					this,
					level,
					parentId,
					startId,
					LogMessageType.Report,
					text,
					report,
					reportExtension,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

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
		public virtual long AddAbsoluteValue(
			LogLevel level,
			Func<string> text,
			double value,
			double? valueLowerBound,
			double? valueUpperBound,
			long parentId = Tofu.Logging.LogManager.INVALID_ID,
			long startId = Tofu.Logging.LogManager.INVALID_ID)
		{
			// Delegate to protected overridable method
			return LogManager.SessionMode != SessionModes.Running ?
				Tofu.Logging.LogManager.INVALID_ID :
				LogManager.AddMessage(
					this,
					level,
					parentId,
					startId,
					LogMessageType.AbsoluteValue,
					text,
					null,
					null,
					null,
					null,
					value,
					valueLowerBound,
					valueUpperBound,
					null,
					null,
					null,
					null,
					false,
					null);
		}

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
		public virtual long AddRelativeValue(
			LogLevel level,
			Func<string> text,
			double value,
			long parentId = Tofu.Logging.LogManager.INVALID_ID,
			long startId = Tofu.Logging.LogManager.INVALID_ID)
		{
			// Delegate to protected overridable method
			return LogManager.SessionMode != SessionModes.Running ?
				Tofu.Logging.LogManager.INVALID_ID :
				LogManager.AddMessage(
					this,
					level,
					parentId,
					startId,
					LogMessageType.RelativeValue,
					text,
					null,
					null,
					null,
					null,
					value,
					0D,
					1D,
					null,
					null,
					null,
					null,
					false,
					null);
		}

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
		public virtual long AddParams(
			LogLevel level,
			Func<string> text,
			Func<string[]> parameterNames,
			Func<object[]> parameterValues,
			long parentId = Tofu.Logging.LogManager.INVALID_ID,
			long startId = Tofu.Logging.LogManager.INVALID_ID)
		{
			// Delegate to protected overridable method
			return LogManager.SessionMode != SessionModes.Running ?
				Tofu.Logging.LogManager.INVALID_ID :
				LogManager.AddMessage(
					this,
					level,
					parentId,
					startId,
					LogMessageType.Parameters,
					text,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					parameterNames,
					parameterValues,
					null,
					false,
					null);
		}

		/// <summary>
		/// Updates the read-only <see cref="ConfigEntryId"/> property value. This method
		/// will be called when the associated <see cref="LogManager"/> is restarted.
		/// </summary>
		/// <param name="configEntryId">
		/// An int that holds a new entry id value
		/// </param>
		public virtual void SetConfigEntryId(int configEntryId)
		{
			// Set new value
			ConfigEntryId = configEntryId;
		}


		/// <summary>
		/// Gets an ILogManager reference to logging manager that is associated
		/// with this logger
		/// </summary>
		public virtual ILogManager LogManager
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets an integer that holds the Id of the configuration entry whose
		/// settings were used to initialize this log instance.<br/>
		/// <b>Note:</b> If no specific entry was used but default entry was
		/// used, this property value will equal <i>LogConfigEntry.INVALID_ID</i>
		/// constant 
		/// </summary>
		public virtual int ConfigEntryId
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a string that holds the name that is associated with this logger
		/// </summary>
		public virtual string Name
		{
			get;
			protected set;
		}


		/// <summary>
		/// Gets or sets a LogLevelFlags enum value that specifies the local enabled flags
		/// </summary>
		[DefaultValue(LogLevelFlags.Global)]
		public virtual LogLevelFlags LocalLevels
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a LocalTraceMode enum value that specifies if the message
		/// should be traced to debug console
		/// </summary>
		[DefaultValue(LocalTraceMode.Global)]
		public virtual LocalTraceMode LocalTrace
		{
			get;
			set;
		}

		#endregion

		#region ILog4N Interface

		// ******************************************************************
		// *																*
		// *					      ILog4N Interface			            *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Adds the specified fatal text to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		void ILog4N.Fatal(object text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Fatal,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified fatal text to the log
		/// </summary>
		/// <param name="text">
		/// A delegate that provides the text that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		void ILog4N.Fatal(Func<string> text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Fatal,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					text,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified fatal exception to the log
		/// </summary>
		/// <param name="ex">
		/// An Exception that must be logged
		/// </param>
		void ILog4N.Fatal(Exception ex)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Fatal,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Exception,
					() => ex.Message,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					ex,
					null,
					null,
					null,
					true,
					null);
		}

		/// <summary>
		/// Adds the specified fatal text and exception to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		/// <param name="ex">
		/// An Exception that must be logged
		/// </param>
		void ILog4N.Fatal(object text, Exception ex)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Fatal,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Exception,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					ex,
					null,
					null,
					null,
					true,
					null);
		}

		/// <summary>
		/// Adds a formatted fatal text to the log
		/// </summary>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.FatalFormat(string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Fatal,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds a formatted fatal text to the log
		/// </summary>
		/// <param name="provider">
		/// An IFormateProvider instance that will be used to format the items when they
		/// are inserted into the specified format
		/// </param>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Fatal,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(provider, format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified error text to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		void ILog4N.Error(object text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Error,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified error text to the log
		/// </summary>
		/// <param name="text">
		/// A delegate that provides the text that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		void ILog4N.Error(Func<string> text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Error,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					text,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified error exception to the log
		/// </summary>
		/// <param name="ex">
		/// An Exception that must be logged
		/// </param>
		void ILog4N.Error(Exception ex)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Error,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Exception,
					() => ex.Message,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					ex,
					null,
					null,
					null,
					true,
					null);
		}

		/// <summary>
		/// Adds the specified error text and exception to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		/// <param name="ex">
		/// An Exception that must be logged
		/// </param>
		void ILog4N.Error(object text, Exception ex)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Error,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Exception,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					ex,
					null,
					null,
					null,
					true,
					null);
		}

		/// <summary>
		/// Adds a formatted error text to the log
		/// </summary>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.ErrorFormat(string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Error,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds a formatted error text to the log
		/// </summary>
		/// <param name="provider">
		/// An IFormateProvider instance that will be used to format the items when they
		/// are inserted into the specified format
		/// </param>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Error,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(provider, format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified warning text to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		void ILog4N.Warn(object text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Warning,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified warning text to the log
		/// </summary>
		/// <param name="text">
		/// A delegate that provides the text that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		void ILog4N.Warn(Func<string> text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Warning,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					text,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified warning text and exception to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		/// <param name="ex">
		/// An Exception that must be logged
		/// </param>
		void ILog4N.Warn(object text, Exception ex)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Warning,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Exception,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					ex,
					null,
					null,
					null,
					true,
					null);
		}

		/// <summary>
		/// Adds a formatted warning text to the log
		/// </summary>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.WarnFormat(string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Warning,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds a formatted warning text to the log
		/// </summary>
		/// <param name="provider">
		/// An IFormateProvider instance that will be used to format the items when they
		/// are inserted into the specified format
		/// </param>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Warning,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(provider, format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified information text to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		void ILog4N.Info(object text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Info,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified information text to the log
		/// </summary>
		/// <param name="text">
		/// A delegate that provides the text that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		void ILog4N.Info(Func<string> text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Info,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					text,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified information text and exception to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		/// <param name="ex">
		/// An Exception that must be logged
		/// </param>
		void ILog4N.Info(object text, Exception ex)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Info,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Exception,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					ex,
					null,
					null,
					null,
					true,
					null);
		}

		/// <summary>
		/// Adds a formatted information text to the log
		/// </summary>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.InfoFormat(string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Info,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds a formatted information text to the log
		/// </summary>
		/// <param name="provider">
		/// An IFormateProvider instance that will be used to format the items when they
		/// are inserted into the specified format
		/// </param>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Info,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(provider, format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified debug text to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		void ILog4N.Debug(object text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Debug,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified debug text to the log
		/// </summary>
		/// <param name="text">
		/// A delegate that provides the text that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		void ILog4N.Debug(Func<string> text)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Debug,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					text,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds the specified debug text and exception to the log
		/// </summary>
		/// <param name="text">
		/// An object that specifies the text that must be logged
		/// </param>
		/// <param name="ex">
		/// An Exception that must be logged
		/// </param>
		void ILog4N.Debug(object text, Exception ex)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Debug,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Exception,
					() => text != null ? text.ToString() : Tofu.Logging.LogStreamer.NULLSTRING,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					ex,
					null,
					null,
					null,
					true,
					null);
		}

		/// <summary>
		/// Adds a formatted debug text to the log
		/// </summary>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.DebugFormat(string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Debug,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Adds a formatted debug text to the log
		/// </summary>
		/// <param name="provider">
		/// An IFormateProvider instance that will be used to format the items when they
		/// are inserted into the specified format
		/// </param>
		/// <param name="format">
		/// A string that specifies the formatting mask that contains items that must be 
		/// replaced with the specified arguments
		/// </param>
		/// <param name="args">
		/// An array of objects that specifies the actual items that must be inserted
		/// into the specified format
		/// </param>
		void ILog4N.DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (LogManager.SessionMode == SessionModes.Running)
				LogManager.AddMessage(
					this,
					LogLevel.Debug,
					Tofu.Logging.LogManager.INVALID_ID,
					Tofu.Logging.LogManager.INVALID_ID,
					LogMessageType.Text,
					() => string.Format(provider, format, args),
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					false,
					null);
		}

		/// <summary>
		/// Gets a bool that specifies if the <i>Fatal</i> loglevel is enabled
		/// </summary>
		bool ILog4N.IsFatalEnabled
		{
			get { return LogManager.IsLevelEnabled(this, LogLevel.Fatal); }
		}

		/// <summary>
		/// Gets a bool that specifies if the <i>Error</i> loglevel is enabled
		/// </summary>
		bool ILog4N.IsErrorEnabled
		{
			get { return LogManager.IsLevelEnabled(this, LogLevel.Error); }
		}

		/// <summary>
		/// Gets a bool that specifies if the <i>Warning</i> loglevel is enabled
		/// </summary>
		bool ILog4N.IsWarningEnabled
		{
			get { return LogManager.IsLevelEnabled(this, LogLevel.Warning); }
		}

		/// <summary>
		/// Gets a bool that specifies if the <i>Info</i> loglevel is enabled
		/// </summary>
		bool ILog4N.IsInfoEnabled
		{
			get { return LogManager.IsLevelEnabled(this, LogLevel.Info); }
		}

		/// <summary>
		/// Gets a bool that specifies if the <i>Debug</i> loglevel is enabled
		/// </summary>
		bool ILog4N.IsDebugEnabled
		{
			get { return LogManager.IsLevelEnabled(this, LogLevel.Debug); }
		}

		#endregion
	}
}
