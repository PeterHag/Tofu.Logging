using System;

namespace Tofu.Logging
{
    public interface ILog4N : ILog
    {
        #region Methods

        // ******************************************************************
        // *																*
        // *					          Methods					        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Adds the specified fatal text to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
        void Fatal(object text);

        /// <summary>
        /// Adds the specified fatal text to the log
        /// </summary>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
		void Fatal(Func<string> text);

        /// <summary>
        /// Adds the specified fatal exception to the log
        /// </summary>
        /// <param name="ex">
        /// An Exception that must be logged
        /// </param>
        void Fatal(Exception ex);

        /// <summary>
        /// Adds the specified fatal text and exception to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
        /// <param name="ex">
        /// An Exception that must be logged
        /// </param>
        void Fatal(object text, Exception ex);

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
		void FatalFormat(string format, params object[] args);

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
        void FatalFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// Adds the specified error text to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
        void Error(object text);

        /// <summary>
        /// Adds the specified error text to the log
        /// </summary>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
		void Error(Func<string> text);

        /// <summary>
        /// Adds the specified error exception to the log
        /// </summary>
        /// <param name="ex">
        /// An Exception that must be logged
        /// </param>
		void Error(Exception ex);

        /// <summary>
        /// Adds the specified error text and exception to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
        /// <param name="ex">
        /// An Exception that must be logged
        /// </param>
		void Error(object text, Exception ex);

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
        void ErrorFormat(string format, params object[] args);

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
        void ErrorFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// Adds the specified warning text to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
        void Warn(object text);

        /// <summary>
        /// Adds the specified warning text to the log
        /// </summary>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
		void Warn(Func<string> text);

        /// <summary>
        /// Adds the specified warning text and exception to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
        /// <param name="ex">
        /// An Exception that must be logged
        /// </param>
		void Warn(object text, Exception ex);

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
		void WarnFormat(string format, params object[] args);

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
		void WarnFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// Adds the specified information text to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
        void Info(object text);

        /// <summary>
        /// Adds the specified information text to the log
        /// </summary>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
        void Info(Func<string> text);

        /// <summary>
        /// Adds the specified information text and exception to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
        /// <param name="ex">
        /// An Exception that must be logged
        /// </param>
		void Info(object text, Exception ex);

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
		void InfoFormat(string format, params object[] args);

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
		void InfoFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// Adds the specified debug text to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
		void Debug(object text);

        /// <summary>
        /// Adds the specified debug text to the log
        /// </summary>
        /// <param name="text">
        /// A delegate that provides the text that must be logged. This delegate will only
        /// be executed when logging for this level has been enabled.
        /// </param>
		void Debug(Func<string> text);

        /// <summary>
        /// Adds the specified debug text and exception to the log
        /// </summary>
        /// <param name="text">
        /// An object that specifies the text that must be logged
        /// </param>
        /// <param name="ex">
        /// An Exception that must be logged
        /// </param>
		void Debug(object text, Exception ex);

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
		void DebugFormat(string format, params object[] args);

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
		void DebugFormat(IFormatProvider provider, string format, params object[] args);

        #endregion

        #region Properties

        // ******************************************************************
        // *																*
        // *					        Properties					        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets a bool that specifies if the <i>Fatal</i> loglevel is enabled
        /// </summary>
        bool IsFatalEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a bool that specifies if the <i>Error</i> loglevel is enabled
        /// </summary>
        bool IsErrorEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a bool that specifies if the <i>Warning</i> loglevel is enabled
        /// </summary>
        bool IsWarningEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a bool that specifies if the <i>Info</i> loglevel is enabled
        /// </summary>
        bool IsInfoEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a bool that specifies if the <i>Debug</i> loglevel is enabled
        /// </summary>
        bool IsDebugEnabled
        {
            get;
        }

        #endregion
    }
}
