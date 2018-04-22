namespace Tofu.Logging.Monitors
{
    public class LogMonitorMemoryPeakWorkingSet : LogMonitorProcess
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
        public LogMonitorMemoryPeakWorkingSet() : base()
        { }

        #endregion

        #region Protected Methods

        // ******************************************************************
        // *																*
        // *					    Protected Methods				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Resolve name of Process memory property that must be monitored
        /// </summary>
        /// <param name="memoryParameterName">
        /// A string that holds the memory property name of the current process to monitor
        /// </param>
        protected override void OnResolveParameters(out string memoryParameterName)
        {
            // Do not resolve from Parameters but set hard-coded property name
            memoryParameterName = MEMORY_PEAKWORKINGSET;
        }

        #endregion
    }
}
