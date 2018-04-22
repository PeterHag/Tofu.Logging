using System;
using System.Collections.Generic;
using Tofu.Extensions;

namespace Tofu.Text
{
	#region Enumerations

	// ******************************************************************
	// *																*
	// *					      Enumerations					        *
	// *																*
	// ******************************************************************

	/// <summary>
	/// An enumeration that specifies all the supported trivial 
	/// Wildcard compare modes
	/// </summary>
	internal enum WildcardCompareModes
    {
        IsAsterixOnly,
        IsEmpty,
        IsPlainString,
        IsComplex
    }

    #endregion

    /// <summary>
    /// A class that converts a wildcard string expression into an wilcard
    /// model that can be re-used. The wildcard string specifies a pattern 
    /// that must searched into a specific searchstring. The wildcard 
    /// expression can include standard dos wildcards such as <i>'*'</i> 
    /// and/or <i>'?'</i>.<br/>
    /// Multiple patterns can be specified by using a semicolon separator
    /// (e.g. "*.exe;*.bat")
    /// </summary>
    public class WildcardExpression
    {
        #region Entry Class

        // ******************************************************************
        // *																*
        // *					    Entry Class					    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// A class that pre-processes a single wildcard string expression
        /// and caches all the relevant variables for performing searchtext
        /// comparisons.
        /// </summary>
        private class Entry
        {
            #region Private Member Variables

            // ******************************************************************
            // *																*
            // *					Private Member Variables					*
            // *																*
            // ******************************************************************

            // Private member variables
            private WildcardCompareModes m_compareMode;
            private List<string> m_wildcardChunks;
            private string m_wildcardString;

            #endregion

            #region Constructors

            // ******************************************************************
            // *																*
            // *					        Constructors					    *
            // *																*
            // ******************************************************************

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="wildcardString">
            /// A string that specifies a pattern to search for. This argument can 
            /// include the standard dos wildcards <i>'*'</i> and/or <i>'?'</i>.
            /// </param>
            public Entry(string wildcardString)
            {
                // Store arguments into members
                m_compareMode = CreateCompareMode(wildcardString);
                m_wildcardString = wildcardString;
                m_wildcardChunks = m_compareMode == WildcardCompareModes.IsComplex ?
                    CreateWildcardChunks(wildcardString) :
                    null;
            }

            #endregion

            #region Private Methods

            // ******************************************************************
            // *																*
            // *					     Private Methods				        *
            // *																*
            // ******************************************************************

            /// <summary>
            /// Compares the current wildcard entry with the specified searchtext 
            /// string
            /// </summary>
            /// <param name="searchString">
            /// A string that will be evaluated against the specified pattern
            /// </param>
            /// <returns>
            /// A bool <i>true</i> if this wildcard entry could be validated 
            /// against the specified search text. Otherwise a bool <i>false</i> 
            /// will be returned.
            /// </returns>
            private bool CompareComplex(string searchString)
            {
                // Apply wildcard chunks to search string
                var chunk = string.Empty;
                var w = false;
                var p = 0;
                var n = 0;
                var stack = new Stack<Tuple<bool, int, int>>();
                for (; n < m_wildcardChunks.Count; n++)
                {
                    // Get local arguments
                    chunk = m_wildcardChunks[n];
                    if (chunk == WildcardExpression.WILDCARD_QUESTION)
                    {
                        p++;
                        w = false;
                    }
                    else if (chunk == WildcardExpression.WILDCARD_ASTERIX)
                    {
                        w = true;
                    }
                    else
                    {
                        if (w)
                        {
                            int o = searchString.IndexOf(chunk, p);
                            if (o >= 0)
                            {
                                p = chunk.Length + o;
                                if (o < searchString.Length - 1)
                                {
                                    stack.Push(new Tuple<bool, int, int>(w, n - 1, o + 1));
                                }
                            }
                            else
                                return false;
                        }
                        else
                        {
                            if (p >= searchString.Length)
                                return false;
                            else if (searchString.Substring(p).StartsWith(chunk))
                                p += chunk.Length;
                            else
                            {
                                if (stack.Count > 0)
                                {
                                    var args = stack.Pop();
                                    w = args.Item1;
                                    n = args.Item2;
                                    p = args.Item3;
                                }
                                else
                                    return false;
                            }
                        }
                    }
                }
                // Check if we parsed entire serach string
                if (p < searchString.Length && !w)
                    return false;

                // Search was successful
                return true;
            }

            #endregion

            #region Public Methods

            // ******************************************************************
            // *																*
            // *					      Public Methods					    *
            // *																*
            // ******************************************************************

            /// <summary>
            /// Compares the current wildcard entry with the specified searchtext 
            /// string
            /// </summary>
            /// <param name="searchString">
            /// A string that will be evaluated against the specified pattern
            /// </param>
            /// <returns>
            /// A bool <i>true</i> if this wildcard entry could be validated 
            /// against the specified search text. Otherwise a bool <i>false</i> 
            /// will be returned.
            /// </returns>
            public bool Compare(string searchString)
            {
                // Declare variables
                bool ret;

                // Compare according compare mode
                switch (m_compareMode)
                {
                    case WildcardCompareModes.IsAsterixOnly:
                        // Wildcard equals '*' => always a match!
                        ret = true;
                        break;
                    case WildcardCompareModes.IsEmpty:
                        // Wildcard equals '' => never a match!
                        ret = false;
                        break;
                    case WildcardCompareModes.IsPlainString:
                        // Wildcard does not contain '*' or '?' => simple string compare!
                        ret = searchString == m_wildcardString;
                        break;
                    case WildcardCompareModes.IsComplex:
                    default:
                        // Wildcard is complex expression => use wildcard chunks!
                        ret = CompareComplex(searchString);
                        break;
                }

                // Return comparison result
                return ret;
            }

            #endregion
        }

        #endregion

        #region Public Constants

        // ******************************************************************
        // *																*
        // *					    Public Constants			            *
        // *																*
        // ******************************************************************

        // Public Constants - Wildcard expression characters
        public const string WILDCARD_ASTERIX = "*";  // Must be 1 char!
        public const string WILDCARD_QUESTION = "?";  // Must be 1 char!
        public const char WILDCARD_SEPARATOR = ';';

        #endregion

        #region Private Member Variables

        // ******************************************************************
        // *																*
        // *					Private Member Variables			        *
        // *																*
        // ******************************************************************

        // Private member variables
        private string m_wildcardOrig;
        private string m_wildcardClean;
        private bool m_caseSensitive;
        private WildcardCompareModes m_compareMode;
        private Entry[] m_entries;

        #endregion

        #region Constructors

        // ******************************************************************
        // *																*
        // *					      Constructors					        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
		/// <param name="wildcardString">
		/// A string that specifies a pattern to search for. This argument can 
		/// include the standard dos wildcards <i>'*'</i> and/or <i>'?'</i>.
        /// Multiple patterns can be specified by using a semicolon separator
        /// (e.g. "*.exe;*.bat")
		/// </param>
		/// <param name="caseSensitive">
		/// A bool that indicates if the test should be case sensitive or not
		/// </param>
        public WildcardExpression(
            string wildcardString,
            bool caseSensitive)
        {
            // Evaluate wildcard expression
            m_compareMode = CreateCompareMode(wildcardString);
            m_wildcardOrig = wildcardString;
            m_wildcardClean = wildcardString.CleanUp();
            m_caseSensitive = caseSensitive;

            // Check casing
            if (!m_caseSensitive)
                m_wildcardClean = m_wildcardClean.ToLower();

            // Check if we have a non-trivial expression
            if (m_compareMode == WildcardCompareModes.IsPlainString ||
                m_compareMode == WildcardCompareModes.IsComplex)
            {
                // Entries are required to evaluate this expression
                string[] expressions = Split(m_wildcardClean);

                // Create an entry for each expression
                if (expressions.Length != 0)
                {
                    // Create entries
                    m_entries = new Entry[expressions.Length];
                    for (int i = 0; i < expressions.Length; i++)
                        m_entries[i] = new Entry(expressions[i]);
                }
                else
                {
                    // Dummy expression
                    m_entries = null;
                    m_compareMode = WildcardCompareModes.IsEmpty;
                }
            }
            else
            {
                // Entries are not required
                m_entries = null;
            }
        }

        #endregion

        #region Private Methods

        // ******************************************************************
        // *																*
        // *					      Private Methods					    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Creates a new CompareModes enum value for the specified wildcard
        /// string expression
        /// </summary>
        /// <param name="wildcardString">
        /// A string that specifies a wildcard string expression
        /// </param>
        /// <returns>
        /// A CompareModes enum value that specifies with compare mode
        /// should be executed for the specified wildcard string.
        /// </returns>
        private static WildcardCompareModes CreateCompareMode(string wildcardString)
        {
            // Evaluate Wildcard string
            if (wildcardString == WILDCARD_ASTERIX)
                return WildcardCompareModes.IsAsterixOnly;
            else if (string.IsNullOrEmpty(wildcardString))
                return WildcardCompareModes.IsEmpty;
            else if (wildcardString.IndexOfAny(
                new char[] { WILDCARD_ASTERIX[0], WILDCARD_QUESTION[0] }) < 0)
                return WildcardCompareModes.IsPlainString;
            else
                return WildcardCompareModes.IsComplex;
        }

        /// <summary>
        /// Splits the specified text with the semicolon chacater and
        /// removes the empty and double entries.
        /// </summary>
        /// <param name="text">
        /// A string that must be splitted
        /// </param>
        /// <returns>
        /// An array of stings that holds the splitted values
        /// </returns>
        private static string[] Split(string text)
        {
            // Check trivial input
            if (string.IsNullOrEmpty(text))
                return new string[0];

            // Check if argument contains separator
            if (text.IndexOf(WILDCARD_SEPARATOR) < 0)
                return new string[1] { text.Trim() };

            // Declare variables
            var list = new List<string>();
            var texts = text.Split(WILDCARD_SEPARATOR);

            // Check for empty entries, doubles and remove whitespace
            for (int i = 0; i < texts.Length; i++)
            {
                // Remove whitespace
                texts[i] = texts[i].Trim();

                // Check text entry is not empty and not already in list
                if (texts[i] != string.Empty && !list.Contains(texts[i]))
                {
                    // Append new entry to list
                    list.Add(texts[i]);
                }
            }

            // Return string array
            return list.ToArray();
        }

        /// <summary>
        /// Creates a new List instance with wildcard chunks. This List
        /// will be used during complex compares.
        /// </summary>
        /// <param name="wildcardString">
        /// A string that specifies a wildcard string expression
        /// </param>
        /// <returns>
        /// A new List instance that will be filled with wildcard chunks.
        /// </returns>
        private static List<string> CreateWildcardChunks(string wildcardString)
        {
            // Declare variables
            List<string> wildChunks = new List<string>();

            // Loop through wildcardstring and create chunks
            int j = 0;
            char last = char.MinValue;
            for (int i = 0; i < wildcardString.Length; i++)
            {
                if (wildcardString[i] == WildcardExpression.WILDCARD_ASTERIX[0] ||
                    wildcardString[i] == WildcardExpression.WILDCARD_QUESTION[0])
                {
                    if (j < i)
                    {
                        wildChunks.Add(wildcardString.Substring(j, i - j));
                    }
                    if (last != WildcardExpression.WILDCARD_ASTERIX[0] ||
                        wildcardString[i] != WildcardExpression.WILDCARD_ASTERIX[0])
                    {
                        wildChunks.Add(wildcardString[i].ToString());
                    }
                    last = wildcardString[i];
                    j = i + 1;
                }
            }
            if (j < wildcardString.Length)
                wildChunks.Add(wildcardString.Substring(j, wildcardString.Length - j));

            // Return resolved chunks
            return wildChunks;
        }

        /// <summary>
        /// Check if the specified wildcard constructor arguments match 
        /// with the current of arguments
        /// </summary>
        /// <param name="wildcardString">
        /// A string that specifies a wildcard expression
        /// </param>
        /// <param name="caseSensitive">
        /// A bool that specifies a test should be case-sensitive or not
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if the specified arguments match with the
        /// current arguments of this expression
        /// </returns>
        private bool IsSameExpression(string wildcardString, bool caseSensitive)
        {
            // Check if arguments match
            return caseSensitive == m_caseSensitive && wildcardString == m_wildcardOrig;
        }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *					      Public Methods					    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Compares the current wildcard expression with the specified 
        /// searchtext string
        /// </summary>
        /// <param name="searchString">
        /// A string that will be evaluated against the specified pattern
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if this wildcard expression could be 
        /// validated against the specified search text. Otherwise a bool 
        /// <i>false</i> will be returned.
        /// </returns>
        public bool Compare(string searchString)
        {
            // Declare variables
            bool ret = false;

            // Compare according compare mode
            switch (m_compareMode)
            {
                case WildcardCompareModes.IsAsterixOnly:
                    // Wildcard equals '*' => always a match!
                    ret = true;
                    break;
                case WildcardCompareModes.IsEmpty:
                    // Wildcard equals '' => never a match!
                    ret = false;
                    break;
                default:
                    // Clean input and check casing
                    searchString = searchString.CleanUp();
                    if (!m_caseSensitive)
                        searchString = searchString.ToLower();

                    // Iterate chunks
                    foreach (Entry entry in m_entries)
                    {
                        // Perform comparison
                        ret |= entry.Compare(searchString);

                        // Break on first positive answer?
                        if (ret)
                            break;
                    }
                    break;
            }

            // Return comparison result
            return ret;
        }

        /// <summary>
        /// Converts the contents of this instance into a human readable
        /// expression
        /// </summary>
        /// <returns>
        /// A string that holds the contents of this instance formatted
        /// into a human readable expression.
        /// </returns>
        public override string ToString()
        {
            return m_wildcardOrig;
        }

        #endregion

        #region Public Properties

        // ******************************************************************
        // *																*
        // *					   Public Properties						*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets a string that holds the expression
        /// </summary>
        public string Expression
        {
            get { return m_wildcardOrig; }
        }

        /// <summary>
        /// Gets a bool that indicates of the expression is case sensitive
        /// </summary>
        public bool IsCaseSensitive
        {
            get { return m_caseSensitive; }
        }

        #endregion
    }
}
