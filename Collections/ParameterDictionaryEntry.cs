namespace Tofu.Collections
{
    public class ParameterDictionaryEntry : IParameterDictionaryEntry
    {
        #region Constructors

        // ******************************************************************
        // *																*
        // *					        Constructors					    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// A string that specifies the name of a parameter
        /// </param>
        /// <param name="value">
        /// A string that specifies the value of a parameter
        /// </param>
        internal ParameterDictionaryEntry(string name, string value)
        {
            // Set members to specified arguments and assume already validated
            Name = name;
            Value = value;
        }

        #endregion

        #region IParameterDictionaryEntry Interface

        // ******************************************************************
        // *																*
        // *		      IParameterDictionaryEntry Interface				*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets a string that holds the name of a parameter
        /// </summary>
        public virtual string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a string that holds the value of a parameter
        /// </summary>
        public virtual string Value
        {
            get;
            private set;
        }

        #endregion
    }
}
