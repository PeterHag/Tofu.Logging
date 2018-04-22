using System;
using Tofu.Collections;

namespace Tofu.Logging
{
	public interface ILogMonitor : IDisposable
    {
        #region Methods

        // ******************************************************************
        // *																*
        // *					          Methods							*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Initializes this monitor instance
        /// </summary>
        /// <param name="logManager">
        /// A ILogManager instance that owns this new monitor instance
        /// </param>
        /// <param name="parameters">
        /// An IParameterDictionary instance that holds user defined parameters in the
        /// configuration file
        /// </param>
        void Init(ILogManager logManager, IParameterDictionary parameters);

        /// <summary>
        /// Called when the associated LogManager starts a new logging session
        /// </summary>
        void SessionStarted();

        /// <summary>
        /// Called when the associated LogManager suspends the current logging session
        /// </summary>
        void SessionSuspended();

        /// <summary>
        /// Called when the associated LogManager resumes the current logging session
        /// </summary>
        void SessionResumed();

        /// <summary>
        /// Called when the associated LogManager stops the current logging session
        /// </summary>
        void SessionStopped();

        #endregion
    }
}
