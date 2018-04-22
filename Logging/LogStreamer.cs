using System;
using System.IO;
using System.Text;
using Tofu.Extensions;

namespace Tofu.Logging
{
	#region Enumerations

	// ******************************************************************
	// *																*
	// *					         Enumerations						*
	// *																*
	// ******************************************************************

	/// <summary>
	/// An enumeration that specifies all the supported resource data types
	/// </summary>
	public enum SerializedResourceDataType
    {
        None,
        ByteArray,
        CharArray
    }

    #endregion

    public abstract class LogStreamer : ILogStreamer
    {
        #region Public Contants

        // ******************************************************************
        // *																*
        // *					    Public Constants                        *
        // *																*
        // ******************************************************************

        // Public Constants
        public const string LOGLEVEL_FATAL = "FATAL";
        public const string LOGLEVEL_ERROR = "ERROR";
        public const string LOGLEVEL_WARNING = "WARNG";
        public const string LOGLEVEL_INFO = "INFOR";
        public const string LOGLEVEL_DEBUG = "DEBUG";
        public const string LOGMESSAGETYPE_TEXT = "TEXT";
        public const string LOGMESSAGETYPE_OBJECT = "OBJE";
        public const string LOGMESSAGETYPE_EXCEPTION = "EXCE";
        public const string LOGMESSAGETYPE_BYTES = "BYTE";
        public const string LOGMESSAGETYPE_REPORT = "REPT";
        public const string LOGMESSAGETYPE_PARAMETERS = "PARM";
        public const string LOGMESSAGETYPE_ABSOLUTEVALUE = "ABS";
        public const string LOGMESSAGETYPE_RELATIVEVALUE = "REL";
        public const string SEPARATOR_PARENTHESIS_OPEN = "[";
        public const string SEPARATOR_PARENTHESIS_CLOSE = "]";
        public const string SEPARATOR_CURLYBRACKETS_OPEN = "{";
        public const string SEPARATOR_CURLYBRACKETS_CLOSE = "}";
        public const string SEPARATOR_PARAMETERS = ", ";
        public const string SEPARATOR_TAB = "\t";
        public const string SEPARATOR_HYPHEN = "-";
        public const string SEPARATOR_COLON = ":";
        public const string SEPARATOR_BLANK = " ";
        public const string SEPARATOR_QUOTE = "'";
        public const string SEPARATOR_CARDINAL = "#";
        public const string SEPARATOR_EQUALS = "=";
        public const string NULLSTRING = "<NULL>";
        public const string EMPTYSTRING = "<EMPTY>";
        public const string RESOURCESTRING = "Resource=";

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *					       Public Methods						*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Checks if the specified message contains resource data
        /// </summary>
        /// <param name="message">
        /// A LogMessage reference
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if the specified log message contains resource data that 
        /// also must be serialized; otherwise a bool <i>false</i> will be returned.
        /// </returns>
        public static bool HasResourceData(LogMessage message)
        {
            // Determine if message contains resource data
            return message.LogType == LogMessageType.Bytes || 
				message.LogType == LogMessageType.Exception ||
                message.LogType == LogMessageType.Object ||
                message.LogType == LogMessageType.Report;
		}

        /// <summary>
        /// Serializes the specified logging message into a single line of text
        /// </summary>
        /// <param name="message">
        /// A LogMessage reference
        /// </param>
        /// <param name="resourcePath">
        /// A string that holds the filename (excl. foldername) of the resource file
        /// that is associated with this message
        /// </param>
        /// <returns>
        /// A string that holds the contents of the specified logging message 
        /// concatenated into a single line of text
        /// </returns>
        public static string SerializeMessage(LogMessage message, string resourcePath)
        {
            // Serialize parameters
            var parameters = string.Empty;
            if (message.ParameterNames != null)
                parameters = string.Concat(
                    SEPARATOR_CURLYBRACKETS_OPEN,
                    SerializeMessage(message.ParameterNames, message.ParameterValues),
                    SEPARATOR_CURLYBRACKETS_CLOSE);

            // Convert message into single line of text
            var line = string.Concat(new string[]
            {
                message.DateTime.Year.ToString("0000"),
                SEPARATOR_HYPHEN,
                message.DateTime.Month.ToString("00"),
                SEPARATOR_HYPHEN,
                message.DateTime.Day.ToString("00"),
                SEPARATOR_BLANK,
                message.DateTime.Hour.ToString("00"),
                SEPARATOR_COLON,
                message.DateTime.Minute.ToString("00"),
                SEPARATOR_COLON,
                message.DateTime.Second.ToString("00"),
                SEPARATOR_COLON,
                message.DateTime.Millisecond.ToString("000"),
                SEPARATOR_BLANK,
                SEPARATOR_PARENTHESIS_OPEN,
                SerializeMessage(message.LogLevel),
                SEPARATOR_HYPHEN,
                SerializeMessage(message.LogType),
                SEPARATOR_PARENTHESIS_CLOSE,
                SEPARATOR_TAB,
                SEPARATOR_CARDINAL,
                message.ThreadId.ToString(),
                SEPARATOR_TAB,
                message.Id.ToString(),
                SEPARATOR_TAB,
                SEPARATOR_QUOTE,
                message.Text.CleanUp(),
                SEPARATOR_QUOTE,
                SEPARATOR_TAB,
                parameters,
                SEPARATOR_TAB,
                message.ParentId != LogManager.INVALID_ID ? message.ParentId.ToString() : string.Empty,
                SEPARATOR_TAB,
                message.StartId != LogManager.INVALID_ID ? message.StartId.ToString() : string.Empty,
                SEPARATOR_TAB,
                message.Value != null ? message.Value.ToString() : string.Empty,
                SEPARATOR_TAB,
                message.ValueLowerBound != null ? message.ValueLowerBound.ToString() : string.Empty,
                SEPARATOR_TAB,
                message.ValueUpperBound != null ? message.ValueUpperBound.ToString() : string.Empty
            });

            // Remove trailing whitespace
            line = line.TrimEnd();

            // Do we need to add resource reference
            if (!string.IsNullOrEmpty(resourcePath))
            {
                // Append resource path
                line = string.Concat(
                    line,
                    SEPARATOR_TAB,
                    SEPARATOR_CURLYBRACKETS_OPEN,
                    RESOURCESTRING,
                    resourcePath.CleanUp(),
                    SEPARATOR_CURLYBRACKETS_CLOSE);
            }

            // Finally we add linefeed
            return string.Concat(line, Environment.NewLine);
        }

        /// <summary>
        /// Serializes the specified LogMessageType enum value into a short human readable 
        /// string expression
        /// </summary>
        /// <param name="messageType">
        /// A LogMessageType enum value
        /// </param>
        /// <returns>
        /// A string that holds the specified LogMessageType converted into a short human
        /// readable string expression.
        /// </returns>
        public static string SerializeMessage(LogMessageType messageType)
        {
            switch (messageType)
            {
                case LogMessageType.Text:
                    return LOGMESSAGETYPE_TEXT;
                case LogMessageType.Object:
                    return LOGMESSAGETYPE_OBJECT;
                case LogMessageType.Exception:
                    return LOGMESSAGETYPE_EXCEPTION;
                case LogMessageType.Bytes:
                    return LOGMESSAGETYPE_BYTES;
                case LogMessageType.Report:
                    return LOGMESSAGETYPE_REPORT;
                case LogMessageType.Parameters:
                    return LOGMESSAGETYPE_PARAMETERS;
                case LogMessageType.AbsoluteValue:
                    return LOGMESSAGETYPE_ABSOLUTEVALUE;
                case LogMessageType.RelativeValue:
                    return LOGMESSAGETYPE_RELATIVEVALUE;
                default:
                    throw new ArgumentException(string.Format(
                        "Unsupported LogMessageType specified: '{0}'",
                        messageType));
            }
        }

        /// <summary>
        /// Serializes the specifies parameter names and values arrays into
        /// a single human readable expression
        /// </summary>
        /// <param name="parameterNames">
        /// An array of strings that holds the names of the parameters
        /// </param>
        /// <param name="parameterValues">
        /// An array of objects that holds the value of the parameters
        /// </param>
        /// <returns>
        /// A string that holds the serialized parameter names and values 
        /// serialized into a single human readable expression
        /// </returns>
        public static string SerializeMessage(string[] parameterNames, object[] parameterValues)
        {
            // Declare variables
            int lengthNames = parameterNames != null ? parameterNames.Length : 0;
            int lengthValues = parameterValues != null ? parameterValues.Length : 0;
            int lengthMax = lengthNames > lengthValues ? lengthNames : lengthValues;

            // Check if there is something to iterate (ie avoid creation of stringbuilder)
            if (lengthMax == 0)
                return string.Empty;

            // Compose parameters text by iterating both arrays
            var sbParams = new StringBuilder();
            for (int i = 0; i < lengthMax; i++)
            {
                // Append separator pair
                if (i > 0)
                    sbParams.Append(SEPARATOR_PARAMETERS);

                // Get parameter value object
                object parameterObject = i < lengthValues ? parameterValues[i] : NULLSTRING;

                // Get actual name-value pair from index
                string parameterName = i < lengthNames ? parameterNames[i].CleanUp() : NULLSTRING;
                string parameterValue = parameterObject != null ? parameterObject.ToString().CleanUp() : NULLSTRING;

                // Substitute empty strings
                if (string.IsNullOrWhiteSpace(parameterName))
                    parameterName = EMPTYSTRING;
                if (string.IsNullOrWhiteSpace(parameterValue))
                    parameterValue = EMPTYSTRING;

                // Append key-value pair
                sbParams.Append(parameterName);
                sbParams.Append(SEPARATOR_EQUALS);
                sbParams.Append(parameterValue);
            }

            // Return parameters expression
            return sbParams.ToString();
        }

        /// <summary>
        /// Serializes the specified LogLevel enum value into a short human readable 
        /// string expression
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value
        /// </param>
        /// <returns>
        /// A string that holds the specified LogLevel converted into a short human
        /// readable string expression.
        /// </returns>
        public static string SerializeMessage(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    return LOGLEVEL_FATAL;
                case LogLevel.Error:
                    return LOGLEVEL_ERROR;
                case LogLevel.Warning:
                    return LOGLEVEL_WARNING;
                case LogLevel.Info:
                    return LOGLEVEL_INFO;
                case LogLevel.Debug:
                    return LOGLEVEL_DEBUG;
                default:
                    throw new ArgumentException(string.Format(
                        "Unsupported LogLevel specified: '{0}'",
                        level));
            }
        }

        /// <summary>
        /// Serializes the resource object from the specified message into either
        /// a byte array or a char array
        /// </summary>
        /// <param name="config">
        /// An ILogConfig reference
        /// </param>
        /// <param name="message">
        /// A LogMessage that contains the resource that must be serialized
        /// </param>
        /// <param name="resourceData">
        /// An object out parameter that will receive the serialized resource data. Typically
        /// this is either a Char array or Byte array depending on the prefered file storage.
        /// </param>
        /// <param name="resourceFileExtension">
        /// A string that will receive a proposed extensionname for the serialized resource data
        /// </param>
        /// <returns>
        /// A SerializedResourceDataType enum value that specifies the format of the serialized
        /// resource data.
        /// </returns>
        public static SerializedResourceDataType SerializeResource(
            ILogConfig config,
            LogMessage message,
            out object resourceData,
            out string resourceFileExtension)
        {
            // Determine type of resource
            switch (message.LogType)
            {
				case LogMessageType.Bytes:
					resourceData = message.Bytes;
					resourceFileExtension = message.BytesExtension.CleanUpFileName().ToLower();
					return SerializedResourceDataType.ByteArray;

				case LogMessageType.Exception:
                    var formattedException = message.FormattedText;
                    if (message.FormattedText == null)
                        formattedException = config.FormattersException.Format(
                            message.LogLevel,
                            message.Exception,
                            message.FormatterName);
					resourceData = formattedException.Text.ToCharArray();
					resourceFileExtension = formattedException.Extension.CleanUpFileName().ToLower();
                    return SerializedResourceDataType.CharArray;

                case LogMessageType.Object:
                    var formattedObject = message.FormattedText;
                    if (message.FormattedText == null)
                        formattedObject = config.FormattersObject.Format(
                            message.LogLevel,
                            message.Object,
                            message.FormatterName);
					resourceData = formattedObject.Text.ToCharArray();
					resourceFileExtension = formattedObject.Extension.CleanUpFileName().ToLower();
                    return SerializedResourceDataType.CharArray;

                case LogMessageType.Report:
					resourceData = message.Report.ToCharArray();
					resourceFileExtension = message.ReportExtension.CleanUpFileName().ToLower();
                    return SerializedResourceDataType.CharArray;

                default:
                    resourceData = null;
                    resourceFileExtension = null;
                    return SerializedResourceDataType.None;
            }
        }

        #endregion

        #region ILogStreamer Interface

        // ******************************************************************
        // *																*
        // *					   ILogStreamer Interface		            *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Initializes the streamer
        /// </summary>
        /// <param name="args">
        /// A LogStreamerInitArgs instance that holds all the required information to 
        /// initialize the streamer instance.
        /// </param>
        public abstract void Init(LogStreamerInitArgs args);

        /// <summary>
        /// Closes the streamer 
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Flushes the buffer of the streamer
        /// </summary>
        public abstract void Flush();

        /// <summary>
        /// Writes the specified log message to the streamer
        /// </summary>
        /// <param name="message">
        /// A LogMessage that must be writted to the stream
        /// </param>
        public abstract void Write(LogMessage message);

        #endregion
    }
}
