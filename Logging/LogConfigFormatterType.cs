using System;
using System.Linq;
using System.Xml.Serialization;
using Tofu.Collections;
using Tofu.Serialization.Xml;

namespace Tofu.Logging
{
	public class LogConfigFormatterType<T>
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
        protected string m_name;
        protected ILogFormatter<T> m_formatter;

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
        public LogConfigFormatterType() : this(string.Empty, null, string.Empty)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="formatterName">
        /// A string that specifies the name that will be associated with this formatter
        /// </param>
        /// <param name="formatterType">
        /// A Type that specify the class type that must implement the ILogStreamer 
        /// interface and has a parameterless constructors
        /// </param>
        /// <param name="parameters">
        /// A string that holds a set of name-value pairs which will be passed to the 
        /// actual LogFormatter instance during its initialization
        /// </param>
        public LogConfigFormatterType(string formatterName, Type formatterType, string parameters)
        {
            // Store arguments
            Name = formatterName;
            FormatterType = formatterType;
            Parameters = parameters;

            // Clear formatter
            m_formatter = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="formatterType">
        /// A LogConfigFormatterType instance whose value members must be copied into
        /// this instance (deep copy)
        /// </param>
        protected LogConfigFormatterType(LogConfigFormatterType<T> formatterType)
        {
            // Copy members
            m_type = formatterType.m_type;
            m_name = formatterType.m_name;
            m_params = formatterType.m_params;
            m_formatter = null;
        }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *					      Public Methods				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Validates if the specified type implements the <i>ILogFormatter</i>
        /// interface and has a paremeterless constructor
        /// </summary>
        /// <param name="type">
        /// A Type that must be validated
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified type does not implement the <i>ILogFormatter</i>
        /// interface or has no parameterless constructor
        /// </exception>
        public static void ValidateFormatterType(Type type)
        {
            // Check if type implements interface
            Type typeInterface = typeof(ILogFormatter<T>);
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
        /// Gets a reference to an ILogFormatter instance 
        /// </summary>
        /// <returns>
        /// An ILogFormatter instance reference
        /// </returns>
        public virtual ILogFormatter<T> GetFormatter()
        {
            // Check if we already have created a formatter
            if (m_formatter == null)
            {
                // Create and initialize new formatter
                m_formatter = (ILogFormatter<T>)Activator.CreateInstance(FormatterType);
                m_formatter.Init(new ParameterDictionary(Parameters));
            }

            // Return formatter reference
            return m_formatter;
        }

        /// <summary>
        /// Creates a deep copy of the current instance
        /// </summary>
        /// <returns>
        /// A new LogConfigStreamerType instance
        /// </returns>
        public virtual LogConfigFormatterType<T> Clone()
        {
            // Create deep copy 
            return new LogConfigFormatterType<T>(this);
        }

        #endregion

        #region Public Properties

        // ******************************************************************
        // *																*
        // *					    Public Properties				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets or sets a string that specifies the name that will be associated 
        /// with this formatter
        /// </summary>
        [XmlAttribute("name")]
        public virtual string Name
        {
            get { return m_name; }
            set { m_name = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets a string that holds a set of name-value pairs which will be passed
        /// to the actual LogFormatter instance during its initialization
        /// </summary>
        /// <example>
        /// Eg. Parameters = "Mask=00.0"
        /// </example>
        /// <see cref="Tofu.Collections.IParameterDictionary.Expression"/>
        [XmlElement("parameters")]
        public virtual string Parameters
        {
            get { return m_params; }
            set { m_params = value ?? string.Empty; }
        }

        /// <summary>
        /// Get or sets a Type that specify the class type that must implement
        /// the ILogFormatter interface and has a parameterless constructors
        /// </summary>
        /// <remarks>
        /// The actual LogFormatter instance will be created on the first call to
        /// the <see cref="GetFormatter"/> method
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified type does not implement the <i>ILogFormatter</i>
        /// interface or has no parameterless constructor
        /// </exception>
        [XmlElement(Type = typeof(XmlTypeSerializer), ElementName = "type")]
        public virtual Type FormatterType
        {
            get { return m_type; }
            set
            {
                // Check if new value is specified
                if (value == m_type)
                    return;

                // Do not validate null
                if (value != null)
                    ValidateFormatterType(value);

                // Store new type and clear old formatter
                m_type = value;
                m_formatter = null;
            }
        }

        #endregion
    }
}
