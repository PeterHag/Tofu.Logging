using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Tofu.Logging.Formatters
{
	public class LogFormatterObjectToXml : LogFormatterObject
	{
		#region Protected Member Variables

		// ******************************************************************
		// *																*
		// *					 Protected Member Variables				    *
		// *																*
		// ******************************************************************

		// Protected member variables
		protected Dictionary<Type, XmlSerializer> m_serializers;

		#endregion

		#region Constructors

		// ******************************************************************
		// *																*
		// *					      Constructors				            *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Constructor
		/// </summary>
		public LogFormatterObjectToXml()
		{ }

		#endregion

		#region Public Methods

		// ******************************************************************
		// *																*
		// *					      Public Methods				        *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Initializes the formatter
		/// </summary>
		/// <param name="parameters">
		/// A IParameterDictionary instance that holds custom parameters that are
		/// defined in the configuration
		/// </param>
		public override void Init(Collections.IParameterDictionary parameters)
		{
			// Call base implementation first
			base.Init(parameters);

			// Create storage for serializers per type
			m_serializers = new Dictionary<Type, XmlSerializer>();
		}

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
			// Obtain object type
			var type = obj != null ? obj.GetType() : typeof(object);

			// Check if we already cache a serializer for this type
			XmlSerializer xmlSer;
			if (!m_serializers.TryGetValue(type, out xmlSer))
			{
				// Create a new serializer for this type
				xmlSer = new XmlSerializer(type);
				m_serializers.Add(type, xmlSer);
			}

			// Serialize object to xml string
			using (var strMem = new MemoryStream())
			{
				xmlSer.Serialize(strMem, obj);
				using (var rdr = new StreamReader(strMem))
				{
					strMem.Seek(0, SeekOrigin.Begin);
					return new LogFormatterResult(
						rdr.ReadToEnd(),
						Parameters.GetString("extension", "xml"));
				}
			}
		}

		#endregion
	}
}
