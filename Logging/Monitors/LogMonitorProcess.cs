using System;
using System.Diagnostics;
using Tofu.Collections;

namespace Tofu.Logging.Monitors
{
	public class LogMonitorProcess : LogMonitorTimer
    {
        #region Public Constants

        // ******************************************************************
        // *																*
        // *					    Public Constants			            *
        // *																*
        // ******************************************************************

        // Public Constants - Size 
        public const string PARAMETER_MEMORY = "memory";

        // Public Constants - Supported values
        public const string MEMORY_NONPAGEDSYSTEMMEMORYSIZE = "NonpagedSystemMemorySize64";
        public const string MEMORY_PAGEDMEMORYSIZE = "PagedMemorySize64";
        public const string MEMORY_PAGEDSYSTEMMEMORYSIZE = "PagedSystemMemorySize64";
        public const string MEMORY_PEAKPAGEDMEMORYSIZE = "PeakPagedMemorySize64";
        public const string MEMORY_PEAKVIRTUALMEMORYSIZE = "PeakVirtualMemorySize64";
        public const string MEMORY_PEAKWORKINGSET = "PeakWorkingSet64";
        public const string MEMORY_PRIVATEMEMORYSIZE = "PrivateMemorySize64";
        public const string MEMORY_VIRTUALMEMORYSIZE = "VirtualMemorySize64";
        public const string MEMORY_WORKINGSET = "WorkingSet64";

        #endregion

        #region Protected Member Variables

        // ******************************************************************
        // *																*
        // *					 Protected Member Variables				    *
        // *																*
        // ******************************************************************

        // Protected member variables
        protected Process m_process;
        protected string m_memory;

        #endregion

        #region Constructors

        // ******************************************************************
        // *																*
        // *					        Constructors				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
        public LogMonitorProcess() : base()
        { }

        #endregion

        #region Protected Methods

        // ******************************************************************
        // *																*
        // *					    Protected Methods				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Tries to get the actual memory size in MB for the specified memory parameter
        /// </summary>
        /// <param name="memoryParameterName">
        /// A string that specifies the process property name of the memory parameter to get
        /// </param>
        /// <param name="process">
        /// A Process from which to get the memory size
        /// </param>
        /// <param name="memorySize">
        /// A long that will receive the actual memory size for the specified type of memory in MB
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if the actual memory size could be resolved; otherwise a bool
        /// <i>false</i> will be returned
        /// </returns>
        protected virtual bool TryGetMemorySizeInMB(
            string memoryParameterName,
            Process process,
            out long memorySize)
        {
            // Declare variables
            var comp = StringComparison.InvariantCultureIgnoreCase;

            // Refresh counters
            process.Refresh();

            // Determine which value to get
            if (string.Equals(memoryParameterName, MEMORY_NONPAGEDSYSTEMMEMORYSIZE, comp))
                memorySize = process.NonpagedSystemMemorySize64 / 1024L / 1024L;
            else if (string.Equals(memoryParameterName, MEMORY_PAGEDMEMORYSIZE, comp))
                memorySize = process.PagedMemorySize64 / 1024L / 1024L;
            else if (string.Equals(memoryParameterName, MEMORY_PAGEDSYSTEMMEMORYSIZE, comp))
                memorySize = process.PagedSystemMemorySize64 / 1024L / 1024L;
            else if (string.Equals(memoryParameterName, MEMORY_PEAKPAGEDMEMORYSIZE, comp))
                memorySize = process.PeakPagedMemorySize64 / 1024L / 1024L;
            else if (string.Equals(memoryParameterName, MEMORY_PEAKVIRTUALMEMORYSIZE, comp))
                memorySize = process.PeakVirtualMemorySize64 / 1024L / 1024L;
            else if (string.Equals(memoryParameterName, MEMORY_PEAKWORKINGSET, comp))
                memorySize = process.PeakWorkingSet64 / 1024L / 1024L;
            else if (string.Equals(memoryParameterName, MEMORY_PRIVATEMEMORYSIZE, comp))
                memorySize = process.PrivateMemorySize64 / 1024L / 1024L;
            else if (string.Equals(memoryParameterName, MEMORY_VIRTUALMEMORYSIZE, comp))
                memorySize = process.VirtualMemorySize64 / 1024L / 1024L;
            else if (string.Equals(memoryParameterName, MEMORY_WORKINGSET, comp))
                memorySize = process.WorkingSet64 / 1024L / 1024L;
            else
                memorySize = -1;

            // Return success
            return memorySize >= 0;
        }

        /// <summary>
        /// Called each time interval of timer has expired
        /// </summary>
        protected override void OnTimerElapsed()
        {
            // Log memory size
            long memorySize;
            if (TryGetMemorySizeInMB(m_memory, m_process, out memorySize))
                Log.AddAbsoluteValue(
                    Level,
                    () => string.Concat(m_memory, " (MB)"),
                    memorySize,
                    0,
                    2D * 1024D * 1024D); // Set hard-coded limit to 2GB
        }

        /// <summary>
        /// Called when a name for the associated log file must be created
        /// </summary>
        /// <returns>
        /// A string that holds the name for the associated log file
        /// </returns>
        protected override string OnCreateLogName()
        {
            // Create log name
            return string.Format("LogMonitorProcess.{0}", m_memory);
        }

        /// <summary>
        /// Resolve name of Process memory property that must be monitored
        /// </summary>
        /// <param name="memoryParameterName">
        /// A string that holds the memory property name of the current process to monitor
        /// </param>
        protected virtual void OnResolveParameters(out string memoryParameterName)
        {
            memoryParameterName = Parameters.GetString(PARAMETER_MEMORY, MEMORY_WORKINGSET);
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
        public override void Init(ILogManager logManager, IParameterDictionary parameters)
        {
            // Call base implementation
            base.Init(logManager, parameters);

            // Set members to default
            m_process = Process.GetCurrentProcess();

            // Resolve user definable parameters
            OnResolveParameters(out m_memory);

            // Check if a valid memory parameter could be resolved
            long memSize;
            if (!TryGetMemorySizeInMB(m_memory, m_process, out memSize))
                throw new ArgumentException(string.Format(
                    "Invalid 'memory' value; parameter '{0}' is not supported",
                    m_memory));
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
            // Call base implementation
            base.Dispose();

            // Dispose members
            m_process.Dispose();
        }

        #endregion
    }
}
