using Tofu.Collections;

namespace Tofu.Logging
{
    public abstract class LogFormatter<T> : ILogFormatter<T>
    {
        #region Constructors

        // ******************************************************************
        // *																*
        // *			              Constructors				            *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
        public LogFormatter()
        { }

        #endregion

        #region Public Properties

        // ******************************************************************
        // *																*
        // *			             Public Properties			            *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets an IParameterDictionary instance that holds the parameters that were 
        /// passed during initialization
        /// </summary>
        public virtual IParameterDictionary Parameters
        {
            get;
            private set;
        }

        #endregion

        #region ILogFormatter Interface

        // ******************************************************************
        // *																*
        // *					  ILogFormatter Interface				    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Initializes the formatter
        /// </summary>
        /// <param name="parameters">
        /// A IParameterDictionary instance that holds custom parameters that are
        /// defined in the configuration
        /// </param>
        public virtual void Init(IParameterDictionary parameters)
        {
            // Store parameters argument
            Parameters = parameters;
        }

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
        public abstract LogFormatterResult Format(LogLevel level, T obj);

        #endregion
    }
}
