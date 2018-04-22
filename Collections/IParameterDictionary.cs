using System;

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
    public interface IParameterDictionary
    {
        #region Methods

        // ******************************************************************
        // *																*
        // *		                     Methods				            *
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
        void Add(string name, string value);

        /// <summary>
        /// Updates either an existing name-value pair or adds a new entry to the dictionary
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of the pair
        /// </param>
        /// <param name="value">
        /// A string that specifies the value of the pair
        /// </param>
        void Update(string name, string value);

        /// <summary>
        /// Clears the entire contents of the dictionary
        /// </summary>
        void Clear();

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
        bool ContainsName(string name);

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
        /// <exception cref="ArgumentException">
        /// Thrown if an invalid name is specified
        /// </exception>
        bool Remove(string name);

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
        void Reset(string expression);

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
        string GetString(string name, string defaultValue);

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
        bool GetBool(string name, bool defaultValue);

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
        int GetInt(string name, int defaultValue);

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
        long GetLong(string name, long defaultValue);

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
        float GetFloat(string name, float defaultValue);

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
        double GetDouble(string name, double defaultValue);

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
        DateTime GetDateTime(string name, DateTime defaultValue);

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
        Tenum GetEnum<Tenum>(string name, Tenum defaultValue) where Tenum : struct;

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
        bool TryGet(string name, out string value);

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
        bool TryGetString(string name, string defaultValue, out string value);

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
        bool TryGetBool(string name, bool defaultValue, out bool value);

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
        bool TryGetInt(string name, int defaultValue, out int value);

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
        bool TryGetLong(string name, long defaultValue, out long value);

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
        bool TryGetFloat(string name, float defaultValue, out float value);

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
        bool TryGetDouble(string name, double defaultValue, out double value);

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
        bool TryGetDateTime(string name, DateTime defaultValue, out DateTime value);

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
        bool TryGetEnum<Tenum>(string name, Tenum defaultValue, out Tenum value) where Tenum : struct;

        #endregion

        #region Properties

        // ******************************************************************
        // *																*
        // *		                     Properties			                *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets an integer that holds the current number of entries in the dictionary
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Gets an array of IParameterDictionariEntry instances that holds the current
        /// contents of the dictionary
        /// </summary>
        IParameterDictionaryEntry[] Entries
        {
            get;
        }

        /// <summary>
        /// Gets or sets a string that holds the contents of the dictionary formatted
        /// into a human readable expression
        /// </summary>
        string Expression
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an object reference that must be used to synchronize access to the dictionary
        /// </summary>
        object SyncRoot
        {
            get;
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
        string this[string name]
        {
            get;
            set;
        }

        #endregion
    }
}
