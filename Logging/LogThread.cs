using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Tofu.Collections;

namespace Tofu.Logging
{
	public class LogThread : ILogThread
    {
        #region Protected Member Variables

        // ******************************************************************
        // *																*
        // *					  Protected Member Variables		        *
        // *																*
        // ******************************************************************

        // Protected Member Variables
        protected Guid m_sessionGuid;
        protected ILogConfig m_sessionConfig;
        protected string m_sessionFolderFullName;
        protected string m_sessionResourceFolderFullName;
        protected Queue<LogMessage> m_messageQueue;
        protected object m_monitorSyncRoot;
        protected Dictionary<string, ILogStreamer[]> m_streamers;
        protected bool m_isRunningFlag;
        protected Thread m_thread;
        protected System.Threading.Timer m_autoFlushTimer;
        protected int m_isWritingLogs;

        #endregion

        #region Constructors

        // ******************************************************************
        // *																*
        // *					      Constructors	                        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
        public LogThread()
        {
            // Store arguments into members
            m_sessionGuid = Guid.Empty;
            m_sessionConfig = null;
            m_sessionFolderFullName = string.Empty;
            m_messageQueue = new Queue<LogMessage>();
            m_monitorSyncRoot = new object();
            m_streamers = new Dictionary<string, ILogStreamer[]>(StringComparer.InvariantCultureIgnoreCase);
            m_isRunningFlag = false;
            m_isWritingLogs = 0;
            m_autoFlushTimer = new System.Threading.Timer(
                new TimerCallback(OnAutoFlushTimer),
                null,
                Timeout.Infinite,
                Timeout.Infinite);
        }

        #endregion

        #region Protected Methods

        // ******************************************************************
        // *																*
        // *					    Protected Methods                       *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Creates and initializes a new set of ILogStreamer instances
        /// </summary>
        /// <param name="message">
        /// A LogMessage instance for which a new set of streamer instances must be created
        /// </param>
        /// <returns>
        /// An array that holds a set of new ILogStreamer instances
        /// </returns>
        protected ILogStreamer[] CreateAndInitializeStreamers(LogMessage message)
        {
            // Resolve configuration entry for this log
            LogConfigDefaultEntry entry = m_sessionConfig.DefaultEntry;
            LogConfigEntry configEntry;
            if (m_sessionConfig.TryGetEntryById(message.ConfigEntryId, out configEntry))
                entry = configEntry;

            // Create streamer instances
            ILogStreamer[] streamers;
            IParameterDictionary[] parameters;
            entry.CreateStreamers(out streamers, out parameters);

            // Count concurrent streamer types
            var dicConcurrentTypesCount = new Dictionary<Type, int>();
            var lstConcurrentTypesIndex = new List<int>();
            for (int i = 0; i < streamers.Length; i++)
            {
                // Did we already encountered this type?
                int count;
                var type = streamers[i].GetType();
                if (dicConcurrentTypesCount.TryGetValue(type, out count))
                {
                    dicConcurrentTypesCount[type] = count + 1;
                    lstConcurrentTypesIndex.Add(count + 1);
                }
                else
                {
                    dicConcurrentTypesCount[type] = 1;
                    lstConcurrentTypesIndex.Add(1);
                }
            }

			// Modify config in case we are working with stream for logmanager
			// This is a special case where we DO NOT want to apply cleanup.
			// We want to ensure that all messages are kept to enable reading!
			var config = m_sessionConfig;
			if( message.LogName == LogManager.LOGNAME_LOGMANAGER )
			{
				// Create copy of config and 
				config = m_sessionConfig.Clone();
				config.CleanupMessages = -1;
			}

            // Initialize streamers
            for (int i = 0; i < streamers.Length; i++)
                streamers[i].Init(new LogStreamerInitArgs(
                    m_sessionGuid,
                    m_sessionFolderFullName,
                    m_sessionResourceFolderFullName,
                    config,
                    parameters[i],
                    message.LogName,
                    dicConcurrentTypesCount[streamers[i].GetType()],
                    lstConcurrentTypesIndex[i]));

            // Return references
            return streamers;
        }

        /// <summary>
        /// Starts the dedicated logging thread
        /// </summary>
        protected virtual void StartThread()
        {
            lock (m_monitorSyncRoot)
            {
                // Start waiting for Monitor pulses until thread is stopped
                while (m_isRunningFlag)
                {
                    Monitor.Wait(m_monitorSyncRoot);
                    PopMessageQueue();
                    Monitor.PulseAll(m_monitorSyncRoot);
                }
            }
        }

        /// <summary>
        /// Pops all pending messages from message queue and sends them
        /// to their respective stream writer
        /// </summary>
        protected virtual void PopMessageQueue()
        {
            // Signal logs are being written
            Interlocked.Increment(ref m_isWritingLogs);

            // Declare variables
            LogMessage[] logMessages;
            var usedStreamers = new Dictionary<string, ILogStreamer[]>();

            // Get all pending messages
            lock (m_messageQueue)
            {
                logMessages = m_messageQueue.ToArray();
                m_messageQueue.Clear();
            }

            // Do we need to trace messages?
            var traceMsg = m_sessionConfig.GlobalTrace ?
                new StringBuilder() :
                null;

            // Process messages by writing to respective stream
            foreach (var message in logMessages)
            {
                // Create try get a set of streamers associated with this log
                ILogStreamer[] streamers;
                if (!m_streamers.TryGetValue(message.LogName, out streamers))
                {
                    streamers = CreateAndInitializeStreamers(message);
                    m_streamers.Add(message.LogName, streamers);
                }

                // Iterate all streamers for this log and write message
                foreach (var streamer in streamers)
                    streamer.Write(message);

                // Append used set of streamers to set of 'to be flushed' streamers
                usedStreamers[message.LogName] = streamers;

                // Now check if we must also send to debug output (plain vanilla format)
                if (traceMsg != null && message.OutputAsDebugString)
                    traceMsg.Append(LogStreamer.SerializeMessage(message, string.Empty));
            }

            // Flush trace (if any)
            if (traceMsg != null)
                Trace.Write(traceMsg.ToString());

            // Flush all used writers
            foreach (var streamers in usedStreamers.Values)
                foreach (var streamer in streamers)
                    streamer.Flush();

            // Indicate logs have been written
            Interlocked.Decrement(ref m_isWritingLogs);
        }

        /// <summary>
        /// Called when the auto-flush timer raised an expired event
        /// </summary>
        /// <param name="state">
        /// An object reference to a state object
        /// </param>
        protected virtual void OnAutoFlushTimer(object state)
        {
            // Only auto-flush if no writing is being executed
            if (m_isWritingLogs == 0)
                Flush();
        }

        #endregion

        #region ILogThread Interface

        // ******************************************************************
        // *																*
        // *					  ILogThread Interface                      *
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
        public virtual void Start(
            Guid sessionGuid,
            string sessionFolderFullName,
            string sessionResourceFolderFullName,
            ILogConfig config)
        {
            // Check if not already running
            if (m_isRunningFlag)
                throw new InvalidOperationException("Cannot start a running LogThread");

            // Turn running flag on
            m_isRunningFlag = true;
            m_sessionGuid = sessionGuid;
            m_sessionConfig = config.Clone();
            m_sessionFolderFullName = sessionFolderFullName;
            m_sessionResourceFolderFullName = sessionResourceFolderFullName;

            // Ensure message queue is empty
            m_messageQueue.Clear();

            // Ensure cache of streamers is also empty
            m_streamers.Clear();

            // Create a new logging thread and start it
            m_thread = new Thread(new ThreadStart(StartThread)) { Name = string.Format("Log Thread - {0}", sessionGuid) };
            m_thread.Start();

            // Start auto-flush timer?
            if (m_sessionConfig.AutoFlushStartupTime > 0 &&
                m_sessionConfig.AutoFlushPeriodicTime > 0)
            {
                m_autoFlushTimer.Change(
                    m_sessionConfig.AutoFlushStartupTime,
                    m_sessionConfig.AutoFlushPeriodicTime);
            }
        }

        /// <summary>
        /// Pushes the specified log message onto the queue
        /// </summary>
        /// <param name="message">
        /// A LogMessage that must be pushed onto the queue
        /// </param>
        public virtual void PushMessage(LogMessage message)
        {
            // Push message onto queue
            lock (m_messageQueue)
            {
                // Push entry onto queue and return reference
                m_messageQueue.Enqueue(message);
            }
        }

        /// <summary>
        /// Flushes all pending messages by pulsing the <i>Monitor</i> that controls
        /// the logging thread
        /// </summary>
        public virtual void Flush()
        {
            lock (m_messageQueue)
            {
                // Check if there are logs that need to be dumped
                if (m_messageQueue.Count == 0)
                    return;
            }

            lock (m_monitorSyncRoot)
            {
                // Signal we need to pop messages
                Monitor.PulseAll(m_monitorSyncRoot);
            }
        }

        /// <summary>
        /// Stops the logger thread
        /// </summary>
        public virtual void Stop()
        {
            // Check if we are not already stopped
            if (!m_isRunningFlag)
                throw new InvalidOperationException("Cannot stop an already stopped LogThread");

            // Now stop and flush all pending messages
            lock (m_monitorSyncRoot)
            {
                // Check if we are running
                if (m_isRunningFlag)
                {
                    // Stop auto-flush timer
                    m_autoFlushTimer.Change(Timeout.Infinite, Timeout.Infinite);

                    // Signal we need to flush all pending messages
                    Monitor.PulseAll(m_monitorSyncRoot);

                    // Clear running flag
                    m_isRunningFlag = false;

                    // Signal again a pulse to stop the worker thread
                    Monitor.PulseAll(m_monitorSyncRoot);

                    // Wait until worker is finished
                    Monitor.Wait(m_monitorSyncRoot);

                    // Finally ensure we close all streamers
                    foreach (var streamersPerLog in m_streamers.Values)
                        foreach (var streamer in streamersPerLog)
                            streamer.Close();

                    // Thread will exit but make sure we clear all references so it can be disposed
                    m_thread = null;
                }
            }
        }

        #endregion
    }
}
