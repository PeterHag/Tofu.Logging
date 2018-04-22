using System;
using Tofu.Extensions;

namespace Tofu.Logging
{
	public class LogFormatterResult
	{
		#region Protected Member Variables

		// ******************************************************************
		// *																*
		// *					 Protected Member Variables		            *
		// *																*
		// ******************************************************************

		// Protected member variables
		protected string m_text;
		protected string m_extension;
		protected Exception m_exception;

		#endregion

		#region Constructors

		// ******************************************************************
		// *																*
		// *					        Constructors				        *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">
		/// A string that holds the actual formatted text in plain txt format
		/// </param>
		public LogFormatterResult(string text) : this(text, "txt")
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">
		/// A string that holds the actual formatted text
		/// </param>
		/// <param name="extension">
		/// A string that specifies the file type of the text
		/// </param>
		public LogFormatterResult(string text, string extension)
		{
			// Store arguments into members
			m_text = text ?? string.Empty;
			m_extension = extension ?? string.Empty;
			m_exception = null;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ex">
		/// An Exception that occured during formatting
		/// </param>
		public LogFormatterResult(Exception ex)
		{
			// Store arguments into members
			m_text = ex.CreateDescription();
			m_extension = "txt";
			m_exception = ex;
		}

		#endregion

		#region Public Properties

		// ******************************************************************
		// *																*
		// *					      Public Properties			            *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Gets a string that holds the actual formatted text
		/// </summary>
		public virtual string Text
		{
			get { return m_text; }
		}

		/// <summary>
		/// Gets a string that specifies the file type of the text
		/// </summary>
		public virtual string Extension
		{
			get { return m_extension; }
		}

		/// <summary>
		/// Gets a reference to the exception that occured during formatting
		/// </summary>
		public virtual Exception Exception
		{
			get { return m_exception; }
		}

		#endregion
	}
}
