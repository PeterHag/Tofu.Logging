using System;
using System.Linq;
using System.Xml.Serialization;
using Tofu.Serialization.Xml;

namespace Tofu.Logging
{
	public class LogConfigStreamerType
    {
        #region Protected Member Variables

        // ******************************************************************
        // *																*
        // *					 Protected Member Variables				    *
        // *																*
        // ******************************************************************

        // Protected member variables
        protected Type m_type;
        protected string m_params;

        #endregion

        #region Constructors

        // ******************************************************************
        // *																*
        // *			              Constructors				            *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
        public LogConfigStreamerType() : this(null, string.Empty)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="streamerType">
        /// A Type that specify the class type that must implement the ILogStreamer 
        /// interface and has a parameterless constructors
        /// </param>
        /// <param name="parameters">
        /// A string that holds a set of name-value pairs which will be passed to the 
        /// actual LogStreamer instance during its initialization
        /// </param>
        public LogConfigStreamerType(Type streamerType, string parameters)
        {
            // Store arguments
            StreamerType = streamerType;
            Parameters = parameters;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="streamerType">
        /// A LogConfigStreamerType instance whose value members must be copied into
        /// this instance (deep copy)
        /// </param>
        protected LogConfigStreamerType(LogConfigStreamerType streamerType)
        {
            // Deep copy members
            m_type = streamerType.m_type;
            m_params = streamerType.m_params;
        }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *					      Public Methods				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Validates if the specified type implements the <i>ILogStreamer</i>
        /// interface and has a paremeterless constructor
        /// </summary>
        /// <param name="type">
        /// A Type that must be validated
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified type does not implement the <i>ILogStreamer</i>
        /// interface or has no parameterless constructor
        /// </exception>
        public static void ValidateStreamerType(Type type)
        {
            // Check if type implements interface
            Type typeInterface = typeof(ILogStreamer);
            bool isImplementation = type.IsInterface ?
                type == typeInterface ||
                type.IsSubclassOf(typeInterface) :
                type.GetInterfaces().Any(typeInterface.Equals);
            if (!isImplementation)
                throw new InvalidOperationException(string.Format(
                    "Invalid type specified; type '{0}' does not implement interface '{1}'",
                    type.Name,
                    typeInterface.Name));

            // Check if type has parameterless constructor
            var constructInfo = type.GetConstructor(new Type[0]);
            if (constructInfo == null)
                throw new InvalidOperationException(string.Format(
                    "Invalid type specified; type '{0}' has no parameterless constructor!",
                    type.Name));
        }

        /// <summary>
        /// Creates a deep copy of the current instance
        /// </summary>
        /// <returns>
        /// A new LogConfigStreamerType instance
        /// </returns>
        public virtual LogConfigStreamerType Clone()
        {
            // Create deep copy 
            return new LogConfigStreamerType(this);
        }

        #endregion

        #region Public Properties

        // ******************************************************************
        // *																*
        // *			              Public Properties				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Get or sets a Type that specify the class type that must implement
        /// the ILogStreamer interface and has a parameterless constructors
        /// </summary>
        /// <remarks>
        /// The actual LogStreamer instance will be created together when the
        /// Log instance is created that manages the streamer
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified type does not implement the <i>ILogStreamer</i>
        /// interface or has no parameterless constructor
        /// </exception>
        [XmlElement(Type = typeof(XmlTypeSerializer), ElementName = "type")]
        public virtual Type StreamerType
        {
            get { return m_type; }
            set
            {
                // Check if new value is specified
                if (value == m_type)
                    return;

                // Do not validate null
                if (value != null)
                    ValidateStreamerType(value);

                // Store new type
                m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets a string that holds a set of name-value pairs which will be passed
        /// to the actual LogStreamer instance during its initialization
        /// </summary>
        /// <example>
        /// Eg. Parameters = "DataSource=C:\Temp\MyDB.db2; NumerOfClients=2; NoResetFlag"
        /// </example>
        /// <see cref="Tofu.Collections.IParameterDictionary.Expression"/>
        [XmlElement("parameters")]
        public virtual string Parameters
        {
            get { return m_params; }
            set { m_params = value ?? string.Empty; }
        }

        #endregion
    }
}
