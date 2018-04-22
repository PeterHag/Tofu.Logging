using Tofu.Collections;

namespace Tofu.Logging
{
    public interface ILogFormatter<T>
    {
        #region Methods

        // ******************************************************************
        // *																*
        // *					         Methods					        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Initializes the formatter
        /// </summary>
        /// <param name="parameters">
        /// A IParameterDictionary instance that holds custom parameters that are
        /// defined in the configuration
        /// </param>
        void Init(IParameterDictionary parameters);

        /// <summary>
        /// Formats the specified instance
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value
        /// </param>
        /// <param name="obj">
        /// A reference to the instance that must be formatted
        /// </param>
        /// <returns>
        /// A LogFormatterResult that holds the formatted result
        /// </returns>
        LogFormatterResult Format(LogLevel level, T obj);

        #endregion
    }
}
