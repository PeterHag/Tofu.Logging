namespace Tofu.Collections
{
    public interface IParameterDictionaryEntry
    {
        #region Properties

        // ******************************************************************
        // *																*
        // *		                    Properties				            *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets a string that holds the name of a parameter
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets a string that holds the value of a parameter
        /// </summary>
        string Value
        {
            get;
        }

        #endregion
    }
}
