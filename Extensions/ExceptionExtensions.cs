using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Tofu.Extensions
{
	public static class ExceptionExtensions
    {
        #region Public Methods

        // ******************************************************************
        // *																*
        // *						Public Methods							*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Creates a detailed text describing the specified exception
        /// </summary>
        /// <remarks>
        /// The detailed text will include stacktrace information and
        /// a listing of the currently loaded assemblies
        /// </remarks>
        /// <param name="ex">
        /// The exception for which the detailed text must be created
        /// </param>
        /// <returns>
        /// A string that contains a detailed description for the 
        /// specified exception
        /// </returns>
        public static string CreateDescription(this Exception ex)
        {
            // Delegate call
            return CreateDescription(ex, true, true);
        }

        /// <summary>
        /// Creates a detailed text describing the specified exception
        /// </summary>
        /// <param name="ex">
        /// The exception for which the detailed text must be created
        /// </param>
        /// <param name="includeStacktraceInfo">
        /// Indicates if stacktrace information must be included
        /// </param>
        /// <param name="includeAssemblyInfo">
        /// Indicates if loaded assembly information must be included
        /// </param>
        /// <returns>
        /// A string that contains a detailed description for the 
        /// specified exception
        /// </returns>
        public static string CreateDescription(
            this Exception ex,
            bool includeStacktraceInfo,
            bool includeAssemblyInfo)
        {
            // Declare variables
            int count = 1;
            StringBuilder sbMsg = new StringBuilder();

            // Create text
            while (ex != null)
            {
                // Header
                sbMsg.Append("Information on exception #");
                sbMsg.Append(count.ToString());
                sbMsg.Append(":");
                sbMsg.Append(Environment.NewLine);
                sbMsg.Append("----------------------------------------------");
                sbMsg.Append(Environment.NewLine);

                // Type of exception
                sbMsg.Append("Exception type: ");
                sbMsg.Append(ex.GetType().FullName);
                sbMsg.Append(Environment.NewLine);
                sbMsg.Append(Environment.NewLine);

                // Message of exception
                sbMsg.Append("Message: ");
                sbMsg.Append(ex.Message.CleanUp());
                sbMsg.Append(Environment.NewLine);

                // Targetsite of exception
                sbMsg.Append("TargetSite: ");
                if (ex.TargetSite != null)
                    sbMsg.Append(ex.TargetSite.ToString().CleanUp());
                else
                    sbMsg.Append("<null>");
                sbMsg.Append(Environment.NewLine);

                // Helplink
                sbMsg.Append("HelpLink: ");
                if (ex.HelpLink != null)
                    sbMsg.Append(ex.HelpLink.ToString().CleanUp());
                else
                    sbMsg.Append("<null>");
                sbMsg.Append(Environment.NewLine);

                // Source
                sbMsg.Append("Source: ");
                try
                {
                    if (ex.Source != null)
                        sbMsg.Append(ex.Source.CleanUp());
                    else
                        sbMsg.Append("<null>");
                }
                catch (Exception e)
                {
                    sbMsg.Append(string.Format(
                        "<error: an exception of type: '{0}' occured>",
                        e.GetType().FullName));
                }
                sbMsg.Append(Environment.NewLine);

                // Stacktrace
                if (includeStacktraceInfo)
                {
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append("STACKTRACE INFORMATION:");
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append(ex.StackTrace.CleanUp());
                    sbMsg.Append(Environment.NewLine);
                }

                // Data
                sbMsg.Append(Environment.NewLine);
                sbMsg.Append("EXCEPTION DATA:");
                sbMsg.Append(Environment.NewLine);
                foreach (DictionaryEntry entry in ex.Data)
                {
                    sbMsg.AppendFormat(
                        "({0} - {1})",
                        entry.Key,
                        entry.Value ?? "<null>");
                    sbMsg.Append(Environment.NewLine);
                }
                sbMsg.Append(Environment.NewLine);

                // Recurse down and increment counter
                ex = ex.InnerException;
                count++;
            }

            // Add loaded assemblies in application domain
            if (includeAssemblyInfo)
            {
                // Reset counter
                count = 1;

                // Loop through assemblies in current domain
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    // Write header
                    sbMsg.Append("Assembly in AppDomain #");
                    sbMsg.Append(count.ToString());
                    sbMsg.Append(":");
                    sbMsg.Append(Environment.NewLine);
                    sbMsg.Append("--------------------------------------------");
                    sbMsg.Append(Environment.NewLine);

                    // Get assembly info
                    AssemblyName asmName = asm.GetName();

                    // Write assembly info
                    sbMsg.Append("FullName: ");
                    sbMsg.Append(asmName.FullName.CleanUp());
                    sbMsg.Append(Environment.NewLine);

                    // Try to get location
                    sbMsg.Append("Location: ");
                    string location = string.Empty;
                    try
                    {
                        // Get location from assembly
                        location = asm.Location.CleanUp();
                    }
                    catch (Exception e)
                    {
                        // Build location exception text
                        location = string.Format(
                            "{0} ({1})",
                            e.GetType().Name,
                            e.Message);
                    }
                    sbMsg.Append(location);
                    sbMsg.Append(Environment.NewLine);

                    // Inlude empty line
                    sbMsg.Append(Environment.NewLine);

                    // Increment counter
                    count++;
                }
            }

            // Return compose text
            return sbMsg.ToString();
        }

        #endregion
    }
}
