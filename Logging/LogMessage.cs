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
	/// An enumeration that specifies all the supported log message types
	/// </summary>
	public enum LogMessageType
	{
		Text,
		Object,
		Exception,
		Bytes,
		Report,
		Parameters,
		AbsoluteValue,
		RelativeValue
	}

	#endregion

	public class LogMessage
	{
		#region Constructors

		// ******************************************************************
		// *																*
		// *					        Constructors						*
		// *																*
		// ******************************************************************

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">
		/// A long that specifies the unique log id
		/// </param>
		/// <param name="parentId">
		/// A long that specifies the id of the parent log message
		/// </param>
		/// <param name="startId">
		/// A long that specifies the id of the start log message
		/// </param>
		/// <param name="threadId">
		/// An int that specifies the id of the calling thread
		/// </param>
		/// <param name="dateTime">
		/// A DateTime that specifies the moment the message was created
		/// </param>
		/// <param name="outputAsDebugString">
		/// A bool that specifies if message should also be send to debug output
		/// </param>
		/// <param name="level">
		/// A LogLevel enum value that specifies the level of the message
		/// </param>
		/// <param name="logName">
		/// A string that specifies the name of the logger
		/// </param>
		/// <param name="configEntryId">
		/// An integer that holds the Id of the configuration entry whose
		/// settings were used to initialize this log instance.<br/>
		/// <b>Note:</b> If no specific entry was used but default entry was
		/// used, this argument will equal <i>LogConfigEntry.INVALID_ID</i>
		/// constant.
		/// </param>
		/// <param name="messageType">
		/// A LogMessageType enum value that specifies the type of the message
		/// </param>
		/// <param name="text">
		/// A delegate that provides the text that must be logged
		/// </param>
		/// <param name="report">
		/// A delegate that provides the report that must be logged
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
		/// <param name="formatterName">
		/// A string that specifies the name of a formatter to use in case async formatting 
		/// must be executed in the logging worker thread
		/// </param>
		/// <param name="formattedText">
		/// A LogFormatterResult that holds the formatted text in case sync formatting is required
		/// (i.e. format is executed in calling thread). If async formatting is required this
		/// argument will equal <i>null</i>
		/// </param>
		public LogMessage(
			long id,
			long parentId,
			long startId,
			int threadId,
			DateTime dateTime,
			bool outputAsDebugString,
			string logName,
			int configEntryId,
			LogLevel level,
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
			string formatterName,
			LogFormatterResult formattedText)
		{
			// Store arguments into members
			Id = id;
			ParentId = parentId;
			StartId = startId;
			ThreadId = threadId;
			DateTime = dateTime;
			OutputAsDebugString = outputAsDebugString;
			LogName = logName;
			ConfigEntryId = configEntryId;
			LogLevel = level;
			LogType = messageType;
			Text = text != null ? text() : string.Empty;
			Report = report != null ? report() : string.Empty;
			ReportExtension = reportExtension != null ? reportExtension() : string.Empty;
			Bytes = bytes != null ? bytes() : null;
			BytesExtension = bytesExtension != null ? bytesExtension() : string.Empty;
			Value = value;
			ValueLowerBound = valueLowerBound;
			ValueUpperBound = valueUpperBound;
			Exception = ex;
			ParameterNames = parameterNames != null ? parameterNames() : null;
			ParameterValues = parameterValues != null ? parameterValues() : null;
			Object = obj;
			FormatterName = formatterName;
			FormattedText = formattedText;
		}

		#endregion

		#region Public Properties

		// ******************************************************************
		// *																*
		// *					      Public Properties    				    *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Gets a long that specifies the unique log id
		/// </summary>
		public virtual long Id
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a long that specifies the id of the parent log message
		/// </summary>
		public virtual long ParentId
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a long that specifies the id of the start log message
		/// </summary>
		public virtual long StartId
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets an int that specifies the id of the calling thread
		/// </summary>
		public virtual int ThreadId
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a DateTime that specifies the moment the message was created
		/// </summary>
		public virtual DateTime DateTime
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a bool that specifies if message should also be send to debug output
		/// </summary>
		public virtual bool OutputAsDebugString
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a LogLevel enum value that specifies the level of the message
		/// </summary>
		public virtual LogLevel LogLevel
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a string that specifies the name of the logger
		/// </summary>
		public virtual string LogName
		{
			get;
			private set;
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
		/// Gets a LogMessageType enum value that specifies the type of the message
		/// </summary>
		public virtual LogMessageType LogType
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a string that the actual message to log
		/// </summary>
		public virtual string Text
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a string that holds report message that must be logged
		/// </summary>
		public virtual string Report
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a string that identifies the file type of the report.
		/// </summary>
		public virtual string ReportExtension
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets an array of bytes that must be logged
		/// </summary>
		public virtual byte[] Bytes
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a string that indentifies the file type of the Bytes array
		/// </summary>
		public virtual string BytesExtension
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a double? that specifies a value that must be logged
		/// </summary>
		public virtual double? Value
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a double? that specifies the lower boundary of the logged value
		/// </summary>
		public virtual double? ValueLowerBound
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a double? that specifies the upper boundary of the logged value
		/// </summary>
		public virtual double? ValueUpperBound
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets an Exception instance that must be logged
		/// </summary>
		public virtual Exception Exception
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets and array of strings that specifies the names of the parameters that must
		/// be logged. The contents of this array must be in sync with the contents 
		/// of the parameter values array.
		/// </summary>
		public virtual string[] ParameterNames
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets an array objects that specifies the values of the parameters that must 
		/// be logged. The contents of this array must be in sync with the contents 
		/// of the parameter names array.
		/// </summary>
		public virtual object[] ParameterValues
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets an object instance  that must be logged
		/// </summary>
		public virtual object Object
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a string that specifies the name of a formatter to use in case async formatting 
		/// must be executed in the logging worker thread
		/// </summary>
		public virtual string FormatterName
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a LogFormatterResult that holds the formatted text in case sync formatting is required
		/// (i.e. format is executed in calling thread). If async formatting is required this
		/// argument will equal <i>null</i>
		/// </summary>
		public virtual LogFormatterResult FormattedText
		{
			get;
			private set;
		}

		#endregion
	}
}
