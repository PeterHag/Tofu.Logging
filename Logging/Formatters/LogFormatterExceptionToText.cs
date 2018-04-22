using System;
using Tofu.Extensions;

namespace Tofu.Logging.Formatters
{
	public class LogFormatterExceptionToText : LogFormatter<Exception>
    {
        #region Constructors

        // ******************************************************************
        // *																*
        // *					      Constructors				            *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
        public LogFormatterExceptionToText()
        { }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *					      Public Methods				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Formats the specified instance
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value
        /// </param>
        /// <param name="ex">
        /// A reference to the exception instance that must be formatted
        /// </param>
        /// <returns>
        /// A LogFormatterResult that holds the formatted result
        /// </returns>
        public override LogFormatterResult Format(LogLevel level, Exception ex)
        {
			// Create plain text description
			return new LogFormatterResult(
				ex.CreateDescription(), 
				Parameters.GetString("extension", "txt"));
        }

        #endregion
    }
}
