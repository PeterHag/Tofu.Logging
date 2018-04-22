using System;
using System.Text;
using Tofu.Extensions;

namespace Tofu.Logging.Formatters
{
	public class LogFormatterObjectToString : LogFormatterObject
	{
		#region Constructors

		// ******************************************************************
		// *																*
		// *					      Constructors				            *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Constructor
		/// </summary>
		public LogFormatterObjectToString()
		{ }

		#endregion

		#region Public Methods

		// ******************************************************************
		// *																*
		// *					      Public Methods				        *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Formats the specified instance
		/// </summary>
		/// <param name="level">
		/// A LogLevel enum value
		/// </param>
		/// <param name="obj">
		/// A reference to the instance that must be formatted
		/// </param>
		/// <returns>
		/// A LogFormatterResult that holds the formatted result
		/// </returns>
		public override LogFormatterResult Format(LogLevel level, object obj)
		{
			// Convert object into metadata tree
			var metaData = LogFormatterObject.DecomposeObject(obj);

			// Recurse down metadata tree and format plain text output
			var sbText = new StringBuilder();
			FormatMetaData(metaData, sbText, "", "    ");

			// Store result
			return new LogFormatterResult(
				sbText.ToString(),
				Parameters.GetString("extension", "txt"));
		}

		#endregion

		#region Private Methods

		// ******************************************************************
		// *																*
		// *					      Private Methods				        *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Formats the specified metadata tree structure into a plain human readable expression
		/// </summary>
		/// <param name="metaData">
		/// An ObjectMetaData object reference
		/// </param>
		/// <param name="output">
		/// A StringBuilder to which the plain text must be formatted
		/// </param>
		/// <param name="linePrefix">
		/// A string that specifies the initial line indentation
		/// </param>
		/// <param name="tab">
		/// A string that specifies size of the indentation increment
		/// </param>
		private static void FormatMetaData(
			ObjectMetaData metaData,
			StringBuilder output,
			string linePrefix,
			string tab)
		{
			// Always start new line with necessary indentation
			output.Append(linePrefix);

			// Determine type of metadata
			switch (metaData.Type)
			{
				case ObjectMetaDataType.RootType:
					output.Append("Object=");
					break;
				case ObjectMetaDataType.PropertyType:
					output.Append(string.Concat(metaData.Property.Name, "="));
					break;
				case ObjectMetaDataType.FieldType:
					output.Append(string.Concat(metaData.Field.Name, "="));
					break;
				case ObjectMetaDataType.EnumerableType:
					output.Append(string.Concat("[", metaData.EnumerableIndex, "]="));
					break;
				case ObjectMetaDataType.DuplicateType:
					output.Append(string.Concat("Duplicate entry (see above) ..." + Environment.NewLine));
					return;
			}

			// Create value text
			string val;
			if (metaData.ValueException != null)
			{
				// Create value exception message
				val = string.Concat(
					metaData.ValueException.GetType().CreateName(true),
					" (",
					metaData.ValueException.Message,
					")");
			}
			else if (metaData.Value != null)
			{
				// Create value message prefixed with value type
				val = string.Concat("(", metaData.Value.GetType().CreateName(true));
				if (!(metaData.Value is ValueType) && !(metaData.Value is string))
					val = string.Concat(val, " #", metaData.Value.GetHashCode());
				val = string.Concat(val, ")", metaData.Value.ToString());
			}
			else
			{
				// Create value 'null' message
				val = "(null)";
			}

			// Append value and linefeed
			output.Append(string.Concat(val.Compact(), Environment.NewLine));

			// Recurse down children (if any)
			if (metaData.ChildEntries.Count > 0)
			{
				output.Append(string.Concat(
					linePrefix,
					metaData.ValueIsEnumerable ? "(" : "{",
					Environment.NewLine));
				foreach (var childEntry in metaData.ChildEntries)
					FormatMetaData(childEntry, output, linePrefix + tab, tab);
				output.Append(string.Concat(
					linePrefix,
					metaData.ValueIsEnumerable ? ")" : "}",
					Environment.NewLine));
			}
		}

		#endregion
	}
}
