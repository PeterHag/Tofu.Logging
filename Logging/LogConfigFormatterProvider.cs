using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Tofu.Logging
{
    public class LogConfigFormatterProvider<T>
    {
        #region Protected Member Variables

        // ******************************************************************
        // *																*
        // *					 Protected Member Variables				    *
        // *																*
        // ******************************************************************

        // Protected member variables
        protected object m_syncRoot;
        protected List<LogConfigFormatterType<T>> m_formatterTypes;
        protected LogConfigFormatterType<T> m_defaultFormatterType;

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
        public LogConfigFormatterProvider()
        {
            // Set members to default
            m_syncRoot = new object();
            m_defaultFormatterType = null;
            m_formatterTypes = new List<LogConfigFormatterType<T>>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider">
        /// A LogConfigFormatterProvider instance whose value members must be copied into
        /// this instance (deep copy)
        /// </param>
        protected LogConfigFormatterProvider(LogConfigFormatterProvider<T> provider)
        {
            // Copy members 
            m_syncRoot = new object();
            m_defaultFormatterType = provider.m_defaultFormatterType.Clone();
            m_formatterTypes = new List<LogConfigFormatterType<T>>();

            // Clone formatters
            for (int i = 0; i < provider.FormatterTypes.Count; i++)
                m_formatterTypes.Add(provider.FormatterTypes[i].Clone());
        }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *			               Public Methods		                *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Formats the specified instance with the formatter that matches the specified name
        /// </summary>
        /// <param name="level">
        /// A LogLevel enum value
        /// </param>
        /// <param name="obj">
        /// A reference to the instance that must be formatted
        /// </param>
        /// <param name="formatterName">
        /// A string that specifies which formatter to use. If this argument equals <i>null</i>
        /// the default formatter will be used
        /// </param>
        /// <returns>
        /// A LogFormatterResult that holds the formatted result
        /// </returns>
        public virtual LogFormatterResult Format(
            LogLevel level,
            T obj,
            string formatterName = null)
        {
            lock (m_syncRoot)
            {
                try
                {
                    // Determine if we need to use default formatter or not
                    if (formatterName == null)
                    {
                        // Default formatting
                        return DefaultFormatter.GetFormatter().Format(level, obj);
                    }
                    else
                    {
                        // Specific formatter
                        LogConfigFormatterType<T> formatterType;
                        if (TryGetFormatterType(formatterName, out formatterType))
                            return formatterType.GetFormatter().Format(level, obj);
                    }

                    // If we reach this line, we could not resolve a formatter
                    return new LogFormatterResult(new InvalidOperationException(string.Format(
                        "Log formatting failed; unable to format instance '{0}' because formatter '{1}' could not be found",
                        obj,
                        formatterName)));
                }
                catch (Exception ex)
                {
                    // Store exception into result
                    return new LogFormatterResult(new InvalidOperationException(
                        "Log formatting failed; an exception occured during formatting", ex));
                }
            }
        }

        /// <summary>
        /// Either adds a new or update an existing type in the <i>FormatterTypes</i> collection
        /// </summary>
        /// <param name="formatterName">
        /// A string that specifies the name that must be associated with this new formatter
        /// </param>
        /// <param name="parameters">
        /// A string that holds a name-value pairs expression which will be passed 
        /// to the actual LogFormatter instance during its initialization
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified type does not implement the <i>ILogFormatter</i>
        /// interface or has no parameterless constructor
        /// </exception>
        public virtual void RegisterFormatterType<Tformatter>(
            string formatterName,
            string parameters)
            where Tformatter : ILogFormatter<T>
        {
            // Delegate call
            RegisterFormatterType<Tformatter>(formatterName, parameters, false);
        }

        /// <summary>
        /// Either adds a new or update an existing type in the <i>FormatterTypes</i> collection
        /// </summary>
        /// <param name="formatterName">
        /// A string that specifies the name that must be associated with this new formatter
        /// </param>
        /// <param name="parameters">
        /// A string that holds a name-value pairs expression which will be passed 
        /// to the actual LogFormatter instance during its initialization
        /// </param>
        /// <param name="isDefaultFormatter">
        /// A bool that specifies if the registered formatter should also be set a default formatter
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified type does not implement the <i>ILogFormatter</i>
        /// interface or has no parameterless constructor
        /// </exception>
        public virtual void RegisterFormatterType<Tformatter>(
            string formatterName,
            string parameters,
            bool isDefaultFormatter)
            where Tformatter : ILogFormatter<T>
        {
            lock (FormatterTypes)
            {
                // Check if we need to update exiting or add new entry
                LogConfigFormatterType<T> formatterType;
                if (TryGetFormatterType(formatterName, out formatterType))
                {
                    // Update existing
                    formatterType.Name = formatterName;
                    formatterType.Parameters = parameters;
                    formatterType.FormatterType = typeof(Tformatter);
                }
                else
                {
                    // Add new formatter
                    FormatterTypes.Add(new LogConfigFormatterType<T>(
                        formatterName,
                        typeof(Tformatter),
                        parameters));
                }

                // Do we set formatter as new default formatter?
                if (isDefaultFormatter)
                {
                    // Create also new default formatter
                    DefaultFormatter = new LogConfigFormatterType<T>(
                        formatterName,
                        typeof(Tformatter),
                        parameters);
                }
                else
                {
                    // Check if default formatter also matches
                    if (DefaultFormatter != null &&
                        StringComparer.InvariantCultureIgnoreCase.Equals(formatterName, DefaultFormatter.Name))
                    {
                        // Update default formatter
                        DefaultFormatter.Name = formatterName;
                        DefaultFormatter.Parameters = parameters;
                        DefaultFormatter.FormatterType = typeof(Tformatter);
                    }
                }
            }
        }

        /// <summary>
        /// Tries to get a LogConfigFormatterType that is associated with the specified name
        /// </summary>
        /// <param name="formatterName">
        /// A string that specifies the name of the formatter that must be resolved
        /// </param>
        /// <param name="formatter">
        /// A LogConfigFormatterType out variable the will either return a reference to teh resolved
        /// formatter; other this argument will return a <i>null</i> reference
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if a LogConfigFormatterType could be found for the specified name;
        /// otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGetFormatterType(
            string formatterName,
            out LogConfigFormatterType<T> formatter)
        {
            // Declare variables
            var name = formatterName ?? string.Empty;

            lock (m_syncRoot)
            {
                // Look for specific formatter in list
                for (int i = 0; i < FormatterTypes.Count; i++)
                {
                    // Is this a match?
                    if (StringComparer.InvariantCultureIgnoreCase.Equals(name, FormatterTypes[i].Name))
                    {
                        // We found a match, now set result
                        formatter = FormatterTypes[i];
                        return true;
                    }
                }

                // Did we find a match, if not the check if default formatter
                // Note: normally default formatter should also be in formatters list!
                if (DefaultFormatter != null &&
                    StringComparer.InvariantCultureIgnoreCase.Equals(name, DefaultFormatter.Name))
                {
                    // Name equals default formatter
                    formatter = DefaultFormatter;
                    return true;
                }

                // We did not find a formatter for specified name
                formatter = null;
                return false;
            }
        }

        /// <summary>
        /// Creates a deep copy of the current config instance
        /// </summary>
        /// <returns>
        /// A new ILogFormatterProvider instance that is a deep copy
        /// </returns>
        public virtual LogConfigFormatterProvider<T> Clone()
        {
            return new LogConfigFormatterProvider<T>(this);
        }

        #endregion

        #region Public Properties

        // ******************************************************************
        // *																*
        // *			             Public Properties		                *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets or sets a collection of LogConfigFormatterType that holds all the formatters available
        /// for this class type T
        /// </summary>
        [XmlElement("formatter")]
        public virtual List<LogConfigFormatterType<T>> FormatterTypes
        {
            get { return m_formatterTypes; }
            set { m_formatterTypes = value; }
        }

        /// <summary>
        /// Gets or sets a LogConfigFormatterType instance that will be used a default formatter
        /// </summary>
        [XmlElement("defaultformatter")]
        public virtual LogConfigFormatterType<T> DefaultFormatter
        {
            get { return m_defaultFormatterType; }
            set { m_defaultFormatterType = value; }
        }

        #endregion
    }
}
