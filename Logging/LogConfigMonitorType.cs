using System;
using System.Linq;
using System.Xml.Serialization;
using Tofu.Serialization.Xml;

namespace Tofu.Logging
{
	public class LogConfigMonitorType
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
        public LogConfigMonitorType() : this(null, string.Empty)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="monitorType">
        /// A Type that specify the class type that must implement the ILogMonitor 
        /// interface and has a parameterless constructors
        /// </param>
        /// <param name="parameters">
        /// A string that holds a set of name-value pairs which will be passed to the 
        /// actual LogMonitor instance during its initialization
        /// </param>
        public LogConfigMonitorType(Type monitorType, string parameters)
        {
            // Store arguments
            MonitorType = monitorType;
            Parameters = parameters;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="monitorType">
        /// A LogConfigMonitorType instance whose value members must be copied into
        /// this instance (deep copy)
        /// </param>
        protected LogConfigMonitorType(LogConfigMonitorType monitorType)
        {
            // Deep copy members
            m_type = monitorType.m_type;
            m_params = monitorType.m_params;
        }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *					      Public Methods				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Validates if the specified type implements the <i>ILogMonitor</i>
        /// interface and has a paremeterless constructor
        /// </summary>
        /// <param name="type">
        /// A Type that must be validated
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified type does not implement the <i>ILogMonitor</i>
        /// interface or has no parameterless constructor
        /// </exception>
        public static void ValidateMonitorType(Type type)
        {
            // Check if type implements interface
            Type typeInterface = typeof(ILogMonitor);
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
        /// A new LogConfigMonitorType instance
        /// </returns>
        public virtual LogConfigMonitorType Clone()
        {
            // Create deep copy 
            return new LogConfigMonitorType(this);
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
        /// the ILogMonitor interface and has a parameterless constructors
        /// </summary>
        /// <remarks>
        /// The actual LogMonitor instance will be created when the LogManager 
        /// starts a logging session
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified type does not implement the <i>ILogMonitor</i>
        /// interface or has no parameterless constructor
        /// </exception>
        [XmlElement(Type = typeof(XmlTypeSerializer), ElementName = "type")]
        public virtual Type MonitorType
        {
            get { return m_type; }
            set
            {
                // Check if new value is specified
                if (value == m_type)
                    return;

                // Do not validate null
                if (value != null)
                    ValidateMonitorType(value);

                // Store new type
                m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets a string that holds a set of name-value pairs which will be passed
        /// to the actual LogMoniyot instance during its initialization
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
