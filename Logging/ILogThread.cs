using System;

namespace Tofu.Logging
{
    public interface ILogThread
    {
        #region Methods

        // ******************************************************************
        // *																*
        // *					          Methods                           *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Starts the logger thread
        /// </summary>
        /// <param name="sessionGuid">
        /// A Guid that uniquely indentifies the current logging session
        /// </param>
        /// <param name="sessionFolderFullName">
        /// A string that holds the current session folder (i.e. location where
        /// current log(s) will be persisted) including process context name
        /// </param>
        /// <param name="sessionResourceFolderFullName">
        /// A string that specifies the current session resource folder (i.e. location
        /// inside session folder where resource files will be persisted)
        /// </param>
        /// <param name="config">
        /// An ILogConfig reference.<br/>
        /// Note: this instance wil be cloned to ensure that configuration changes after 
        /// thread has been started will not affect current execution.
        /// </param>
        void Start(
            Guid sessionGuid,
            string sessionFolderFullName,
            string sessionResourceFolderFullName,
            ILogConfig config);

        /// <summary>
        /// Pushes the specified log message onto the queue
        /// </summary>
        /// <param name="message">
        /// A LogMessage that must be pushed onto the queue
        /// </param>
        void PushMessage(LogMessage message);

        /// <summary>
        /// Flushes all pending messages by pulsing the <i>Monitor</i> that controls
        /// the logging thread
        /// </summary>
        void Flush();

        /// <summary>
        /// Stops the logger thread
        /// </summary>
        void Stop();

        #endregion
    }
}
