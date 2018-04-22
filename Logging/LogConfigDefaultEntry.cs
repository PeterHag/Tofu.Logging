using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Tofu.Collections;

namespace Tofu.Logging
{
	#region Enumerations

	// ******************************************************************
	// *																*
	// *					      Enumerations						    *
	// *																*
	// ******************************************************************

	/// <summary>
	/// An enumeration that specifies all the supported local modes for 
	/// tracing debug strings
	/// </summary>
	public enum LocalTraceMode
    {
        Global,
        Off,
        On
    }

    #endregion

    public class LogConfigDefaultEntry
    {
        #region Constants

        // ******************************************************************
        // *																*
        // *					          Constants				            *
        // *																*
        // ******************************************************************

        // Public Constants
        public const int INVALID_ID = -1;

        #endregion

        #region Protected Member Variables

        // ******************************************************************
        // *																*
        // *					 Protected Member Variables				    *
        // *																*
        // ******************************************************************

        // Protected member variables
        protected List<LogConfigStreamerType> m_listTypes;
        protected LogLevelFlags m_localEnabledLevels;
        protected LocalTraceMode m_localTrace;
        protected int m_id;

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
        public LogConfigDefaultEntry()
        {
            // Set members to default
            m_id = INVALID_ID;
            m_localEnabledLevels = LogLevelFlags.Global;
            m_localTrace = LocalTraceMode.Global;
            m_listTypes = new List<LogConfigStreamerType>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="defaultEntry">
        /// A LogConfigDefaultEntry instance whose value members must be copied into
        /// this instance (deep copy)
        /// </param>
        protected LogConfigDefaultEntry(LogConfigDefaultEntry defaultEntry)
        {
            // Deep copy members
            m_id = defaultEntry.m_id;
            m_localEnabledLevels = defaultEntry.m_localEnabledLevels;
            m_localTrace = defaultEntry.m_localTrace;

            // Create deep copy of streamers collection
            m_listTypes = new List<LogConfigStreamerType>();
            foreach (var streamerType in defaultEntry.m_listTypes)
                m_listTypes.Add(streamerType.Clone());
        }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *				          Public Methods				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Adds a new type to the <i>StreamerTypes</i> collection
        /// </summary>
        /// <param name="streamerType">
        /// A Type that specify the class type that must implement the ILogStreamer 
        /// interface and has a parameterless constructors
        /// </param>
        /// <param name="parameters">
        /// A string that holds a name-value pairs expression which will be passed 
        /// to the actual LogStreamer instance during its initialization
        /// </param>
        /// <returns>
        /// A reference to the LogConfigStreamerType instance that was added to
        /// the <i>StreamerTypes</i> collection
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified type does not implement the <i>ILogStreamer</i>
        /// interface or has no parameterless constructor
        /// </exception>
        public virtual LogConfigStreamerType AddStreamerType(
            Type streamerType,
            string parameters)
        {
            // Declare variables
            var type = new LogConfigStreamerType(
                streamerType,
                parameters);

            // Add new streamer type to collection and return reference
            StreamerTypes.Add(type);
            return type;
        }

        /// <summary>
        /// Creates a new set of streamers instances for all the types that are currently
        /// present in the <i>StreamerTypes</i> collection.<br/>
        /// <b>Note:</b> The method WILL NOT initialize this new streamer instances!
        /// </summary>
        /// <param name="streamers">
        /// An array of new ILogStreamer instances
        /// </param>
        /// <param name="parameters">
        /// An array of new IParameterDictionary instances
        /// </param>
        public virtual void CreateStreamers(
            out ILogStreamer[] streamers,
            out IParameterDictionary[] parameters)
        {
            // Create all attached streamers
            var listStreamers = new List<ILogStreamer>();
            var listParameters = new List<IParameterDictionary>();
            for (int i = 0; i < StreamerTypes.Count; i++)
            {
                // Create new instances and append to collections
                listStreamers.Add((ILogStreamer)Activator.CreateInstance(StreamerTypes[i].StreamerType));
                listParameters.Add(new ParameterDictionary(StreamerTypes[i].Parameters));
            }

            // Set output arguments
            streamers = listStreamers.ToArray();
            parameters = listParameters.ToArray();
        }

        /// <summary>
        /// Creates a deep copy of the current instance
        /// </summary>
        /// <returns>
        /// A new LogConfigDefaultEntry instance
        /// </returns>
        public virtual LogConfigDefaultEntry Clone()
        {
            return new LogConfigDefaultEntry(this);
        }

        #endregion

        #region Public Properties

        // ******************************************************************
        // *																*
        // *				          Public Properties				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets an integer that holds the unique Id of this configuration entry
        /// </summary>
        [XmlIgnore]
        public virtual int Id
        {
            get { return m_id; }
        }

        /// <summary>
        /// Gets or sets a collection of ILogConfigStreamerTypes that holds all the streamers
        /// that must be associated with this log
        /// </summary>
        [XmlElement("streamer")]
        public virtual List<LogConfigStreamerType> StreamerTypes
        {
            get { return m_listTypes; }
            set { m_listTypes = value; }
        }

        /// <summary>
        /// Gets or sets a bool that specified the default state of <i>LocalLevels</i> flag
        /// on the actual Log instance when its created
        /// </summary>
        [XmlElement("locallevels")]
        public virtual LogLevelFlags LocalLevels
        {
            get { return m_localEnabledLevels; }
            set { m_localEnabledLevels = value; }
        }

        /// <summary>
        /// Gets or sets a LocalTraceMode enum value that specifies if this log should
        /// deviate from global mode for tracing debug strings
        /// </summary>
        [XmlElement("localtrace")]
        public virtual LocalTraceMode LocalTrace
        {
            get { return m_localTrace; }
            set { m_localTrace = value; }
        }

        #endregion
    }
}
