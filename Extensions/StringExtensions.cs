using System;
using System.Collections.Generic;
using System.Text;

namespace Tofu.Extensions
{
	public static class StringExtensions
	{
		#region Public Constants

		// ******************************************************************
		// *																*
		// *						Public Constants						*
		// *																*
		// ******************************************************************

		// Public Constants - ParseNameValueExpression default characters
		public const char PARSENAMEVALUEEXPRESSION_SEPARATOR = ';';
		public const char PARSENAMEVALUEEXPRESSION_EQUALS = '=';

		#endregion

		#region Public Methods

		// ******************************************************************
		// *																*
		// *						Public Methods							*
		// *																*
		// ******************************************************************

		/// <summary>
		/// Parses the specified expression into a 2D string array that holds
		/// all the resolved Name-Value pairs from the expression.
		/// </summary>
		/// <remarks>
		/// The algorithm will perform following assumptions:<br/>
		/// 1. All returned values will be trimmed (i.e. no leading and trailing blanks)<br/>
		/// 2. Empty assignments should not be returned<br/>
		/// 3. Consecutive 'Equals' characters will be treated as one<br/>
		/// 4. A value can be regarded as optional<br/>
		/// 5. Default Equals character is '=' and default Separator character is ';'
		/// </remarks>
		/// <example>
		/// E.g. 1<br/>
		/// Input  = "A=1; B = 2; C=3;"<br/>
		/// Output = {{"A", "1"}, {"B", "2"}, {"C", "3"}}<br/>
		/// <br/>
		/// Special cases:<br/>
		/// E.g. 2<br/>
		/// Input  = "A===  4  ; B = = 5;;;=;;; C=6"<br/>
		/// Output = {{"A", "4"}, {"B", "5"}, {"C", "6"}}<br/>
		/// <br/>
		/// E.g. 3<br/>
		/// Input  = "X=10.0; Y=15; NoValue;; Alpha==Something"<br/>
		/// Output = {{"X","10.0"}, {"Y","15"}, {"NoValue",""}, {"Alpha","Something"}}
		/// </example>
		/// <param name="expression">
		/// An expression that contains a set of Name-Value assignments that are
		/// separated by a specific character.<br/>
		/// E.g. A typical expression of this form is a database connectionstring
		/// </param>
		/// <returns>
		/// A two dimensional string that holds all the resolved Name-Value pairs from
		/// specified expression. If an empty expression is specified, or no pairs could be
		/// resolved an empty array will be returned.
		/// </returns>
		public static string[,] ParseNameValueExpression(this string expression)
		{
			// Delegate call
			return ParseNameValueExpression(
				expression,
				PARSENAMEVALUEEXPRESSION_SEPARATOR,
				PARSENAMEVALUEEXPRESSION_EQUALS);
		}

		/// <summary>
		/// Parses the specified expression into a 2D string array that holds
		/// all the resolved Name-Value pairs from the expression.
		/// </summary>
		/// <remarks>
		/// The algorithm will perform following assumptions:<br/>
		/// 1. All returned values will be trimmed (i.e. no leading and trailing blanks)<br/>
		/// 2. Empty assignments will not be returned<br/>
		/// 3. Consecutive 'Equals' characters will be treated as one<br/>
		/// 4. A value can be regarded as optional<br/>
		/// 5. Default Equals character is '=' and default Separator character is ';'
		/// </remarks>
		/// <example>
		/// E.g. 1<br/>
		/// Input  = "A=1; B = 2; C=3;"<br/>
		/// Output = {{"A", "1"}, {"B", "2"}, {"C", "3"}}<br/>
		/// <br/>
		/// Special cases:<br/>
		/// E.g. 2<br/>
		/// Input  = "A===  4  ; B = = 5;;;=;;; C=6"<br/>
		/// Output = {{"A", "4"}, {"B", "5"}, {"C", "6"}}<br/>
		/// <br/>
		/// E.g. 3<br/>
		/// Input  = "X=10.0; Y=15; NoValue;; Alpha==Something"<br/>
		/// Output = {{"X","10.0"}, {"Y","15"}, {"NoValue",""}, {"Alpha","Something"}}
		/// </example>
		/// <param name="expression">
		/// An expression that contains a set of Name-Value assignments that are
		/// separated by a specific character.<br/>
		/// E.g. A typical expression of this form is a database connectionstring
		/// </param>
		/// <param name="separatorChar">
		/// A character that will be used as separator; default value equals ';'
		/// </param>
		/// <param name="equalsChar">
		/// A character that will be used as separator; default value equals '='
		/// </param>
		/// <returns>
		/// A two dimensional string that holds all the resolved Name-Value pairs from
		/// specified expression. If an empty expression is specified, or no pairs could be
		/// resolved an empty array will be returned.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown when the 'Separator' character is same as 'Equals" character
		/// </exception>
		public static string[,] ParseNameValueExpression(
			this string expression,
			char separatorChar,
			char equalsChar)
		{
			// Validate characters
			if (separatorChar == equalsChar)
				throw new ArgumentException("Invalid Separator and/or Equals character; cannot be same");

			// Validate 'null' input
			if (expression == null)
				return new string[0, 0];

			// Step 1: Split into Name-Value expressions according 'separator' character
			var nameValueExpressions = expression.Split(
				new char[] { separatorChar },
				StringSplitOptions.RemoveEmptyEntries);

			// Step 2: Iterate Name-Value expressions and split further into Name-Value pairs
			var nameValueTuples = new List<Tuple<string, string>>();
			foreach (var nameValueExpression in nameValueExpressions)
			{
				// Declare local variables
				string name, value;

				// Find index of first 'equals'
				int idxEquals = nameValueExpression.IndexOf(equalsChar);
				if (idxEquals < 0)
				{
					// No equals found, so assume only name
					name = nameValueExpression.Trim();
					value = string.Empty;
				}
				else
				{
					// Before equal is 'Name'
					name = nameValueExpression.Substring(0, idxEquals).Trim();

					// After equals is 'Value'
					int j = idxEquals + 1;
					while (j < nameValueExpression.Length &&
						(nameValueExpression[j] == ' ' ||
						 nameValueExpression[j] == equalsChar))
					{
						// Consider garbage
						j++;
					}
					value = nameValueExpression.Substring(j).TrimEnd();
				}

				// Do not append if name is empty!
				if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(value))
					continue;

				// Add to output
				nameValueTuples.Add(new Tuple<string, string>(name, value));
			}

			// Convert list into 2 dimensional array
			var ret = new string[nameValueTuples.Count, 2];
			for (int i = 0; i < nameValueTuples.Count; i++)
			{
				// Copy tuple into array
				ret[i, 0] = nameValueTuples[i].Item1;
				ret[i, 1] = nameValueTuples[i].Item2;
			}

			// Return array
			return ret;
		}

		/// <summary>
		/// Cleans the specified text by removing all the leading and trailing whitespace 
		/// and removing escape characters (i.e. '\n', '\r' and '\t' characters)
		/// </summary>
		/// <param name="text">
		/// A string that must be cleaned
		/// </param>
		/// <returns>
		/// A string that holds the cleaned text
		/// </returns>
		public static string CleanUp(this string text)
		{
			// Declare variables
			string txt = text;

			// Trivial check for null value
			if (txt == null)
			{
				// Convert null expression
				txt = string.Empty;
			}
			else if (txt != string.Empty)
			{
				// Remove escape characters
				txt = txt.Replace("\n", null);
				txt = txt.Replace("\r", null);
				txt = txt.Replace("\t", null);

				// Remove whitespace 
				txt = txt.Trim();
			}

			// Return formatted string
			return txt;
		}

		/// <summary>
		/// Cleans the specified text by removing all the leading and trailing whitespace 
		/// and removing escape characters (i.e. '\n', '\r' and '\t' characters) and all
		/// invalid filename characters.
		/// </summary>
		/// <param name="text">
		/// A string that must be cleaned
		/// </param>
		/// <returns>
		/// A string that holds the cleaned text
		/// </returns>
		public static string CleanUpFileName(this string text)
		{
			// Check input
			if (string.IsNullOrWhiteSpace(text))
				return string.Empty;

			// Declare variables
			var sbText = new StringBuilder();

			// Iterate input text
			for (int c = 0; c < text.Length; c++)
			{
				// Get character at index location
				var chr = text[c];

				// Skip line or tab characters
				if (chr == '\n' || chr == '\r' || chr == '\t')
					continue;

				// Skip invalid Filename characters
				if (chr == '\\' || chr == '/' || chr == '*' || chr == '?' || chr == '|' ||
					chr == '<' || chr == '>' || chr == ':' || chr == '"')
					continue;

				// Allow remaining character
				sbText.Append(chr);
			}

			// Remove leading/trailing whitespace (if any)
			return sbText.ToString().Trim();
		}

		/// <summary>
		/// Compacts the specified string by truncating at first '\n' or '\r' character.
		/// The method also replaces all '\t' characters with a single space and replaces
		/// large whitespace sequences with a single blank.
		/// </summary>
		/// <param name="text">
		/// A string that must be compacted
		/// </param>
		/// <returns>
		/// A string that holds a reduced version of the specified text
		/// </returns>
		public static string Compact(this string text)
		{
			return Compact(text, true);
		}

		/// <summary>
		/// Compacts the specified string by truncating at first '\n' or '\r' character.
		/// The method also replaces all '\t' characters with a single space and replaces
		/// large whitespace sequences with a single blank.
		/// </summary>
		/// <param name="text">
		/// A string that must be compacted
		/// </param>
		/// <param name="addEllipsesWhenTrimmed">
		/// A bool that specifies if three dots must be appended at the end in case the 
		/// string was truncated by either a '\n' or a '\r' character
		/// </param>
		/// <returns>
		/// A string that holds a reduced version of the specified text
		/// </returns>
		public static string Compact(this string text, bool addEllipsesWhenTrimmed)
		{
			// Declare variables
			var isTrimmed = false;
			var txt = text ?? string.Empty;

			// Clip text on linefieeds
			int idx = txt.IndexOf('\n');
			if (idx >= 0)
			{
				txt = txt.Substring(0, idx);
				isTrimmed = true;
			}
			idx = txt.IndexOf('\r');
			if (idx >= 0)
			{
				txt = txt.Substring(0, idx);
				isTrimmed = true;
			}

			// Append ellipses in case text was clipped
			if (isTrimmed && addEllipsesWhenTrimmed)
				txt = txt + " ...";

			// Replace tabs
			txt = txt.Replace('\t', ' ');

			// Remove large whitespaces
			while (txt.IndexOf("  ") >= 0)
				txt = txt.Replace("  ", " ");

			// Cleaned text
			return txt;
		}

		#endregion
	}
}
