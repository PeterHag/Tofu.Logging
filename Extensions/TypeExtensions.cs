using System;
using System.Text;

namespace Tofu.Extensions
{
    public static class TypeExtensions
    {
        #region Public Methods

        // ******************************************************************
        // *																*
        // *						Public Methods							*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Creates a descriptive name for the specified Type
        /// </summary>
        /// <remarks>
        /// This method will also include the <i>Generics</i> information 
        /// into the returned name.
        /// </remarks>
        /// <param name="type">
        /// A Type reference
        /// </param>
        /// <param name="preferGenericTypeDefinition">
        /// A bool that specifies if either the <i>Generic</i> argument
        /// definition names should be used in the composed name (i.e. 
        /// <i>true</i>); if this argument equals <i>false</i> the
        /// <i>Generic</i> argument Types will be included instead.<br/>
        /// E.g. 'MyGenericMember&lt;T&gt;' or 'MyGenericMember&lt;String&gt;'
        /// </param>
        /// <returns>
        /// A string that holds a descriptive name for the specified Type
        /// </returns>
        public static string CreateName(
            this Type type,
            bool preferGenericTypeDefinition)
        {
            // Delegate call
            return CreateName(type, false, preferGenericTypeDefinition);
        }

        /// <summary>
        /// Creates a descriptive name for the specified Type
        /// </summary>
        /// <remarks>
        /// This method will also include the <i>Generics</i> information 
        /// into the returned name.
        /// </remarks>
        /// <param name="type">
        /// A Type reference
        /// </param>
        /// <param name="preferFullName">
        /// A bool that specifies if the FullName of the Type should be prefered
        /// (i.e. when available) instead of using Name
        /// </param>
        /// <param name="preferGenericTypeDefinition">
        /// A bool that specifies if either the <i>Generic</i> argument
        /// definition names should be used in the composed name (i.e. 
        /// <i>true</i>); if this argument equals <i>false</i> the
        /// <i>Generic</i> argument Types will be included instead.<br/>
        /// E.g. 'MyGenericMember&lt;T&gt;' or 'MyGenericMember&lt;String&gt;'
        /// </param>
        /// <returns>
        /// A string that holds a descriptive name for the specified Type
        /// </returns>
        public static string CreateName(
            this Type type,
            bool preferFullName,
            bool preferGenericTypeDefinition)
        {
            // Check input
            if (type == null)
                throw new ArgumentNullException(
                    "Invalid Type specified; <null> not allowed");

            // Create generic Type representation or keep declaring types
            // (e.g. "MyGeneric<T>" or "MyGeneric<String>")
            if (type.IsGenericType && preferGenericTypeDefinition)
            {
                Type typeGeneric = type.GetGenericTypeDefinition();
                if (typeGeneric != null)
                    type = typeGeneric;
            }

            // Create default name
            string name = preferFullName && type.FullName != null ?
                type.FullName :
                type.Name;

            // Convert generic?
            if (type.IsGenericType && preferGenericTypeDefinition)
            {
                // Declare local variables
                StringBuilder sbArgs = new StringBuilder();
                Type[] types = type.GetGenericArguments();

                // Iterate generic types and append
                sbArgs.Append('<');
                foreach (Type typeGeneric in types)
                {
                    // Append separator
                    if (sbArgs.Length > 1)
                        sbArgs.Append(',');

                    // Append type name
                    sbArgs.Append(CreateName(
                        typeGeneric,
                        preferGenericTypeDefinition));
                }
                sbArgs.Append('>');

                // Substitute text
                name = name.Replace("`" + types.Length, sbArgs.ToString());
            }

            // Return composed name
            return name;
        }

        #endregion
    }
}
