using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Tofu.Extensions;

namespace Tofu.Collections
{
	/// <summary>
	/// A string dictionary with support for name-value expressions such as
	/// can be found in connection strings.<br/>
	/// Main features are:
	/// <ul>
	/// <li>Name value considered case insensitive</li>
	/// <li>Both Name and Value entries whitespace trimmed</li>
	/// <li>Threadsafe access</li>
	/// <li>Xml serializable</li>
	/// <li>Support for Name only entries (i.e. no value defined)</li>
	/// </ul>
	/// </summary>
	/// <example>
	/// E.g. Expression: "Datasource=C:\SomePath; MaxClients=3; NoValueEntry"
	/// </example>
	public class ParameterDictionary : IParameterDictionary
    {
        #region Private Member Variables

        // ******************************************************************
        // *																*
        // *				     Private Member Variables				    *
        // *																*
        // ******************************************************************

        // Private member variables
        private IDictionary<string, string> m_dictionary;
        private object m_syncRoot;
        private IParameterDictionaryEntry[] m_entries;
        private string m_expression;

        #endregion

        #region Constructors

        // ******************************************************************
        // *																*
        // *					    Public Constructors					    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
        public ParameterDictionary() : this(string.Empty)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="expression">
        /// A string that specifies an initial expression with which the collection must be filled   
        /// </param>
        public ParameterDictionary(string expression)
        {
            // Set members to default
            m_dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            m_syncRoot = new object();

            // Reset contents
            if (string.IsNullOrEmpty(expression))
            {
                // Set to default
                m_expression = string.Empty;
                m_entries = new IParameterDictionaryEntry[0];
            }
            else
            {
                // Set to clear
                m_expression = null;
                m_entries = null;

                // Reset
                Reset(expression);
            }
        }

        #endregion

        #region Protected Methods

        // ******************************************************************
        // *																*
        // *					    Protected Methods					    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Removes white space from the specified name
        /// </summary>
        /// <param name="name">
        /// A string that specifies a name
        /// </param>
        /// <returns>
        /// A string that holds the trimmed name
        /// </returns>
        protected virtual string TrimName(string name)
        {
            // Cleanup name by removing whitespace
            return name != null ? name.Trim() : string.Empty;
        }

        /// <summary>
        /// Add or updates the specified name-value pair and invalidates caches
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the pair
        /// </param>
        /// <param name="value">
        /// A string that specifies the value of the pair
        /// </param>
        /// <param name="allowUpdate">
        /// A bool that specifies if either an <i>Add</i> or <i>Update</i> action should
        /// be performed. In case of an add an additional check will be performed to see
        /// if the specified name not already exists in the dictionary
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified name already exists in the dictionary
        /// </exception>
        protected virtual void InternalAddOrUpdate(
            string name,
            string value,
            bool allowUpdate)
        {
            // Trim name
            var trimmedName = TrimName(name);

            lock (SyncRoot)
            {
                // Validate if name already exists
                if (!allowUpdate && m_dictionary.ContainsKey(trimmedName))
                    throw new ArgumentException(string.Format(
                        "Invalid Name specified; '{0}' already exists in dictionary",
                        name));

                // Update dictionary
                m_dictionary[trimmedName] = value != null ? value.Trim() : string.Empty;

                // Clear cache
                m_entries = null;
                m_expression = null;
            }
        }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *					      Public Methods					    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Converts the contents of this dictionary into a human readable expression
        /// </summary>
        /// <returns>
        /// A string that holds the contents of this dictionary formatted into a human
        /// readable expression
        /// </returns>
        public override string ToString()
        {
            return Expression;
        }

        #endregion

        #region IParameterDictionary Interface

        // ******************************************************************
        // *																*
        // *		          IParameterDictionary Interface                *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Adds a new name-value pair to the dictionary
        /// </summary>
        /// <remarks>
        /// This method will throw an <see cref="ArgumentException"/> if the dictionary already
        /// contains the specified name. Use the <seealso cref="Update"/> method to add or update
        /// an existing pair if the name already occurs in the dictionary.
        /// </remarks>
        /// <param name="name">
        /// A string that specifies the name of the pair
        /// </param>
        /// <param name="value">
        /// A string that specifies the value of the pair
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified name already exists in the dictionary
        /// </exception>
        public virtual void Add(string name, string value)
        {
            InternalAddOrUpdate(name, value, false);
        }

        /// <summary>
        /// Updates either an existing name-value pair or adds a new entry to the dictionary
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the pair
        /// </param>
        /// <param name="value">
        /// A string that specifies the value of the pair
        /// </param>
        public virtual void Update(string name, string value)
        {
            InternalAddOrUpdate(name, value, true);
        }

        /// <summary>
        /// Clears the entire contents of the dictionary
        /// </summary>
        public virtual void Clear()
        {
            lock (SyncRoot)
            {
                // Clear dictionary
                m_dictionary.Clear();
                m_entries = new IParameterDictionaryEntry[0];
                m_expression = string.Empty;
            }
        }

        /// <summary>
        /// Checks if the dictionary contains the specified name
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name to check
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if the dictionary contains an entry for the specified name;
        /// otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool ContainsName(string name)
        {
            lock (SyncRoot)
            {
                // Check if dictionary contains specifies name
                return m_dictionary.ContainsKey(TrimName(name));
            }
        }

        /// <summary>
        /// Removes the pair that is associated with the specified name from the dictionary
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the pair to remove
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if the specified pair was removed succesfully; otherwise
        /// a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool Remove(string name)
        {
            lock (SyncRoot)
            {
                // Try to remove from collection
                var success = m_dictionary.Remove(TrimName(name));

                // Do we need to update cache?
                if (success)
                {
                    // Invalidate caches
                    m_entries = null;
                    m_expression = null;
                }

                // Return flag
                return success;
            }
        }

        /// <summary>
        /// Resets the contents of the dictionary by replacing its current contents with the
        /// contents of the specified expression
        /// </summary>
        /// <param name="expression">
        /// A string that specifies the new contents of the dictionary
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified expression contains duplicate entries
        /// </exception>
        public virtual void Reset(string expression)
        {
            // Trivial check to see if reset can me dropped
            if (m_expression == expression)
                return;

            // Declare variables
            var pairs = expression.ParseNameValueExpression();
            var dic = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            // Copy into temporary dictionary, if a duplicate entry exists in
            // new expression an exception will be raised here but content of
            // ParameterDictionary will remain unchanged!
            int length = pairs.GetLength(0);
            for (int i = 0; i < length; i++)
                dic.Add(pairs[i, 0], pairs[i, 1]);

            lock (SyncRoot)
            {
                // Update contents of dictionary
                m_dictionary.Clear();
                foreach (var pair in dic)
                    m_dictionary.Add(pair.Key, pair.Value);

                // Clear caches
                m_entries = null;
                m_expression = null;
            }
        }


        /// <summary>
        /// Attempts to get the value that is associated with the specified name
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="value">
        /// An out string that will receive the associated value in case the dictionary
        /// contains an entry for the specified name
        /// </param>
        /// <returns>
        /// A bool <i>true</i> is a value could be resolved in the dictionary for the 
        /// specified name; otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGet(string name, out string value)
        {
            lock (SyncRoot)
            {
                // Try to get associated value
                return m_dictionary.TryGetValue(TrimName(name), out value);
            }
        }

        /// <summary>
        /// Attempts to get the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A string that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name
        /// </param>
        /// <param name="value">
        /// An out string that will receive the associated value in case the dictionary
        /// contains an entry for the specified name; otherwise the specified default value
        /// will be returned
        /// </param>
        /// <returns>
        /// A bool <i>true</i> is a value could be resolved in the dictionary for the 
        /// specified name; otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGetString(string name, string defaultValue, out string value)
        {
            // Try to get string value
            if (!TryGet(name, out value))
            {
                // Return default value 
                value = defaultValue;
                return false;
            }

            // If we reach this line, value could be resolved
            return true;
        }

        /// <summary>
        /// Attempts to get the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A bool that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name
        /// </param>
        /// <param name="value">
        /// An out bool that will receive the associated value in case the dictionary
        /// contains an entry for the specified name; otherwise the specified default value
        /// will be returned
        /// </param>
        /// <returns>
        /// A bool <i>true</i> is a value could be resolved in the dictionary for the 
        /// specified name; otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGetBool(string name, bool defaultValue, out bool value)
        {
            // Try to get string value and do conversion
            string val;
            if (TryGet(name, out val) && bool.TryParse(val, out value))
                return true;

            // Conversion failed so use specified default value
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Attempts to get the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// An integer that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name
        /// </param>
        /// <param name="value">
        /// An out integer that will receive the associated value in case the dictionary
        /// contains an entry for the specified name; otherwise the specified default value
        /// will be returned
        /// </param>
        /// <returns>
        /// A bool <i>true</i> is a value could be resolved in the dictionary for the 
        /// specified name; otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGetInt(string name, int defaultValue, out int value)
        {
            // Try to get string value and do conversion
            string val;
            if (TryGet(name, out val) && int.TryParse(val, out value))
                return true;

            // Conversion failed so use specified default value
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Attempts to get the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A long that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name
        /// </param>
        /// <param name="value">
        /// An out long that will receive the associated value in case the dictionary
        /// contains an entry for the specified name; otherwise the specified default value
        /// will be returned
        /// </param>
        /// <returns>
        /// A bool <i>true</i> is a value could be resolved in the dictionary for the 
        /// specified name; otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGetLong(string name, long defaultValue, out long value)
        {
            // Try to get string value and do conversion
            string val;
            if (TryGet(name, out val) && long.TryParse(val, out value))
                return true;

            // Conversion failed so use specified default value
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Attempts to get the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A float that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name
        /// </param>
        /// <param name="value">
        /// An out float that will receive the associated value in case the dictionary
        /// contains an entry for the specified name; otherwise the specified default value
        /// will be returned
        /// </param>
        /// <returns>
        /// A bool <i>true</i> is a value could be resolved in the dictionary for the 
        /// specified name; otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGetFloat(string name, float defaultValue, out float value)
        {
            // Try to get string value and do conversion
            string val;
            if (TryGet(name, out val) && float.TryParse(val, out value))
                return true;

            // Conversion failed so use specified default value
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Attempts to get the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A double that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name
        /// </param>
        /// <param name="value">
        /// An out double that will receive the associated value in case the dictionary
        /// contains an entry for the specified name; otherwise the specified default value
        /// will be returned
        /// </param>
        /// <returns>
        /// A bool <i>true</i> is a value could be resolved in the dictionary for the 
        /// specified name; otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGetDouble(string name, double defaultValue, out double value)
        {
            // Try to get string value and do conversion
            string val;
            if (TryGet(name, out val) && double.TryParse(val, out value))
                return true;

            // Conversion failed so use specified default value
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Attempts to get the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A DateTime that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name
        /// </param>
        /// <param name="value">
        /// An out DateTime that will receive the associated value in case the dictionary
        /// contains an entry for the specified name; otherwise the specified default value
        /// will be returned
        /// </param>
        /// <returns>
        /// A bool <i>true</i> is a value could be resolved in the dictionary for the 
        /// specified name; otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGetDateTime(string name, DateTime defaultValue, out DateTime value)
        {
            // Try to get string value and do conversion
            string val;
            if (TryGet(name, out val) && DateTime.TryParse(val, out value))
                return true;

            // Conversion failed so use specified default value
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Attempts to get the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <typeparam name="Tenum">
        /// A type that specifies the class type of the enumeration
        /// </typeparam>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A Tenum value that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name
        /// </param>
        /// <param name="value">
        /// An out Tenum that will receive the associated value in case the dictionary
        /// contains an entry for the specified name; otherwise the specified default value
        /// will be returned
        /// </param>
        /// <returns>
        /// A bool <i>true</i> is a value could be resolved in the dictionary for the 
        /// specified name; otherwise a bool <i>false</i> will be returned
        /// </returns>
        public virtual bool TryGetEnum<Tenum>(
            string name,
            Tenum defaultValue,
            out Tenum value)
            where Tenum : struct
        {
            // Try to get string value 
            string val;
            if (TryGet(name, out val) && Enum.TryParse<Tenum>(val, true, out value))
                return true;

            // Conversion failed so use specified default value
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Gets the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A string that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name
        /// </param>
        /// <returns>
        /// A string that holds either the resolved value or the specified default value
        /// </returns>
        public virtual string GetString(string name, string defaultValue)
        {
            // Declare variables
            string value;

            // Return either resolved value or specified default value
            return TryGetString(name, defaultValue, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A bool that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name, or if value could not be converted
        /// </param>
        /// <returns>
        /// A bool that holds either the resolved value or the specified default value
        /// </returns>
        public virtual bool GetBool(string name, bool defaultValue)
        {
            // Declare variables
            bool value;

            // Return either resolved value or specified default value
            return TryGetBool(name, defaultValue, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// An integer that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name, or if value could not be converted
        /// </param>
        /// <returns>
        /// An integer that holds either the resolved value or the specified default value
        /// </returns>
        public virtual int GetInt(string name, int defaultValue)
        {
            // Declare variables
            int value;

            // Return either resolved value or specified default value
            return TryGetInt(name, defaultValue, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A long that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name, or if value could not be converted
        /// </param>
        /// <returns>
        /// A long that holds either the resolved value or the specified default value
        /// </returns>
        public virtual long GetLong(string name, long defaultValue)
        {
            // Declare variables
            long value;

            // Return either resolved value or specified default value
            return TryGetLong(name, defaultValue, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A float that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name, or if value could not be converted
        /// </param>
        /// <returns>
        /// A float that holds either the resolved value or the specified default value
        /// </returns>
        public virtual float GetFloat(string name, float defaultValue)
        {
            // Declare variables
            float value;

            // Return either resolved value or specified default value
            return TryGetFloat(name, defaultValue, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A double that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name, or if value could not be converted
        /// </param>
        /// <returns>
        /// A double that holds either the resolved value or the specified default value
        /// </returns>
        public virtual double GetDouble(string name, double defaultValue)
        {
            // Declare variables
            double value;

            // Return either resolved value or specified default value
            return TryGetDouble(name, defaultValue, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A DateTime that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name, or if value could not be converted
        /// </param>
        /// <returns>
        /// A DateTime that holds either the resolved value or the specified default value
        /// </returns>
        public virtual DateTime GetDateTime(string name, DateTime defaultValue)
        {
            // Declare variables
            DateTime value;

            // Return either resolved value or specified default value
            return TryGetDateTime(name, defaultValue, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value that is associated with the specified name; if the dictionary
        /// does not contain an entry for the name, the specified default value will be returned
        /// </summary>
        /// <typeparam name="Tenum">
        /// A type that specifies the class type of the enumeration
        /// </typeparam>
        /// <param name="name">
        /// A string that specifies the name of the value to get
        /// </param>
        /// <param name="defaultValue">
        /// A Tenum value that specifies the default value that will be returned in case the dictionary
        /// does not contain an entry for the specified name, or if value could not be converted
        /// </param>
        /// <returns>
        /// A Tenum value that holds either the resolved value or the specified default value
        /// </returns>
        public virtual Tenum GetEnum<Tenum>(string name, Tenum defaultValue) where Tenum : struct
        {
            // Declare variables
            Tenum value;

            // Return either resolved value or specified default value
            return TryGetEnum<Tenum>(name, defaultValue, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets an integer that holds the current number of entries in the dictionary
        /// </summary>
        [XmlIgnore]
        public virtual int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return m_dictionary.Count;
                }
            }
        }

        /// <summary>
        /// Gets an array of IParameterDictionariEntry instances that holds the current
        /// contents of the dictionary
        /// </summary>
        [XmlIgnore]
        public virtual IParameterDictionaryEntry[] Entries
        {
            get
            {
                // Check if we already cache
                if (m_entries != null)
                    return m_entries;

                // Build new set
                lock (SyncRoot)
                {
                    if (m_entries == null)
                    {
                        // Create temporary list and iterate dictionary
                        var list = new List<IParameterDictionaryEntry>();
                        foreach (var pair in m_dictionary)
                            list.Add(new ParameterDictionaryEntry(pair.Key, pair.Value));

                        // Convert list into array
                        m_entries = list.ToArray();
                    }

                    // Return cache set
                    return m_entries;
                }
            }
        }

        /// <summary>
        /// Gets or sets a string that holds the contents of the dictionary formatted
        /// into a human readable expression
        /// </summary>
        [XmlAttribute("expression")]
        public virtual string Expression
        {
            get
            {
                // Check if we already cache expression
                if (m_expression != null)
                    return m_expression;

                // Build new expression
                lock (SyncRoot)
                {
                    if (m_expression == null)
                    {
                        var sbExp = new StringBuilder();
                        foreach (var pair in m_dictionary)
                        {
                            // Append separator?
                            if (sbExp.Length > 0)
                            {
                                sbExp.Append(StringExtensions.PARSENAMEVALUEEXPRESSION_SEPARATOR);
                                sbExp.Append(' ');
                            }

                            // Append Name-Value pair
                            sbExp.Append(pair.Key);
                            if (!string.IsNullOrEmpty(pair.Value))
                            {
                                sbExp.Append(StringExtensions.PARSENAMEVALUEEXPRESSION_EQUALS);
                                sbExp.Append(pair.Value);
                            }
                        }

                        // Create expression
                        m_expression = sbExp.ToString();
                    }

                    // Return new expression
                    return m_expression;
                }
            }
            set
            {
                // Reset contents
                Reset(value);
            }
        }

        /// <summary>
        /// Gets an object reference that must be used to synchronize access to the dictionary
        /// </summary>
        [XmlIgnore]
        public virtual object SyncRoot
        {
            get { return m_syncRoot; }
        }

        /// <summary>
        /// Gets or sets a value for the specified name. If the specified name already exists
        /// the value will be overwritten; otherwise the value will be added as new entry.
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name
        /// </param>
        /// <returns>
        /// A string that holds the associated value
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when an unknown name is specified
        /// </exception>
        [XmlIgnore]
        public virtual string this[string name]
        {
            get
            {
                lock (SyncRoot)
                {
                    // Try to get associated value
                    return m_dictionary[TrimName(name)];
                }
            }
            set
            {
                InternalAddOrUpdate(name, value, true);
            }
        }

        #endregion
    }
}
