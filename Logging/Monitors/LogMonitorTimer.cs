using System;
using System.Timers;
using Tofu.Collections;

namespace Tofu.Logging.Monitors
{
	public abstract class LogMonitorTimer : LogMonitor
    {
        #region Public Constants

        // ******************************************************************
        // *																*
        // *					    Public Constants				        *
        // *																*
        // ******************************************************************

        // Public Constants - Supported parameter names
        public const string PARAMETER_INTERVAL = "interval";
        public const string PARAMETER_ENABLED = "enabled";
        public const string PARAMETER_LEVEL = "level";

        // Public Constants - Default values
        public const double DEFAULT_INTERVAL = 1000;
        public const bool DEFAULT_ENABLED = true;
        public const LogLevel DEFAULT_LEVEL = LogLevel.Debug;

        #endregion

        #region Private Member Variables

        // ******************************************************************
        // *																*
        // *					 Private Member Variables				    *
        // *																*
        // ******************************************************************

        // Private member variables
        private Timer m_timer;

        #endregion

        #region Private Methods

        // ******************************************************************
        // *																*
        // *					      Private Methods				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Called each time interval of timer has expired
        /// </summary>
        /// <param name="sender">
        /// An object reference to the invoker of the event
        /// </param>
        /// <param name="e">
        /// An ElapsedEventArgs instance that holds additional information that
        /// is associated with the event
        /// </param>
        private void OnTimerElapsedInternal(object sender, ElapsedEventArgs e)
        {
            try
            {
                // Delegate to abstract method
                OnTimerElapsed();
            }
            catch (Exception ex)
            {
                // Log exception
                Log.AddException(Level, () => ex.Message, ex);

                // Kill timer because something went wrong
                Enabled = false;
                m_timer.Stop();
            }
        }

        #endregion

        #region Protected Methods

        // ******************************************************************
        // *																*
        // *					    Protected Methods				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Called each time interval of timer has expired
        /// </summary>
        protected abstract void OnTimerElapsed();

        /// <summary>
        /// Called when a name for the associated log file must be created
        /// </summary>
        /// <returns>
        /// A string that holds the name for the associated log file
        /// </returns>
        protected abstract string OnCreateLogName();

        #endregion

        #region Public Properties

        // ******************************************************************
        // *																*
        // *			            Public Properties		                *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets an ILog reference to the log that is associated with this monitor
        /// </summary>
        public ILog Log
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a bool that indicates if the timer is enabled or not
        /// </summary>
        public bool Enabled
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a double that specifies the interval of the timer
        /// </summary>
        public double Interval
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a LogLevel enum value that specifies the logging level of monitor logs
        /// </summary>
        public LogLevel Level
        {
            get;
            private set;
        }

        #endregion

        #region ILogMonitor Interface

        // ******************************************************************
        // *																*
        // *			           ILogMonitor Interface		            *
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
        public override void Init(
            ILogManager logManager,
            IParameterDictionary parameters)
        {
            // Call base implementation first
            base.Init(logManager, parameters);

            // Set members to default
            Enabled = Parameters.GetBool(PARAMETER_ENABLED, DEFAULT_ENABLED);
            Interval = Parameters.GetDouble(PARAMETER_INTERVAL, DEFAULT_INTERVAL);
            Level = Parameters.GetEnum<LogLevel>(PARAMETER_LEVEL, DEFAULT_LEVEL);

            // Create new timer
            m_timer = new System.Timers.Timer();
            m_timer.Elapsed += OnTimerElapsedInternal;
            m_timer.AutoReset = true;
            m_timer.Interval = Interval;
        }

        /// <summary>
        /// Called when the associated LogManager starts a new logging session
        /// </summary>
        public override void SessionStarted()
        {
            // Is timer enabled
            if (Enabled)
            {
                // Check if we have a log file?
                if (Log == null)
                    Log = LogManager.GetLog(OnCreateLogName());

                // Start timer
                m_timer.Start();
            }
        }

        /// <summary>
        /// Called when the associated LogManager suspends the current logging session
        /// </summary>
        public override void SessionSuspended()
        {
            // Pause timer
            if (Enabled)
                m_timer.Stop();
        }

        /// <summary>
        /// Called when the associated LogManager resumes the current logging session
        /// </summary>
        public override void SessionResumed()
        {
            // Resume timer
            if (Enabled)
                m_timer.Start();
        }

        /// <summary>
        /// Called when the associated LogManager stops the current logging session
        /// </summary>
        public override void SessionStopped()
        {
            // Stop timer
            if (Enabled)
                m_timer.Stop();
        }

        #endregion

        #region IDisposable Interface

        // ******************************************************************
        // *																*
        // *			           IDisposable Interface		            *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Called when this instance is going to be GC'ed and must release 
        /// used resources
        /// </summary>
        public override void Dispose()
        {
            // Stop and dispose timer
            if (Enabled)
                m_timer.Stop();
            m_timer.Dispose();
        }

        #endregion
    }
}
