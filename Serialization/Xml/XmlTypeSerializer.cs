using System;
using System.Xml.Serialization;

namespace Tofu.Serialization.Xml
{
    /// <summary>
    /// A class that can be used to serialize Type values with the System.Xml.Serialization.XmlSerializer
    /// class. This class uses a shorter notation than <seealso cref="XmlTypeSerializer"/> by dropping the
    /// versionnumber and the public key
    /// </summary>
    public class XmlTypeSerializer
    {
        #region Protected Member Variables

        // ******************************************************************
        // *																*
        // *				     Protected Member Variables				    *
        // *																*
        // ******************************************************************

        // Protected Member Variables
        protected string m_typeName;

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
        public XmlTypeSerializer() : this(string.Empty)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeName">
        /// An string that holds the fully qualified name of the Type
        /// </param>
        public XmlTypeSerializer(string typeName)
        {
            // Store argument into member
            m_typeName = typeName ?? string.Empty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">
        /// A Type value
        /// </param>
        public XmlTypeSerializer(Type type)
        {
            // Store argument into member
            if (type != null)
            {
                string asmName = type.Assembly.ManifestModule.Name;
                int idx = asmName.IndexOf(".dll", StringComparison.InvariantCultureIgnoreCase);
                if (idx >= 0)
                    asmName = asmName.Substring(0, idx);
                m_typeName = string.Format("{0}, {1}", type.FullName, asmName);
            }
            else
                m_typeName = string.Empty;
        }

        #endregion

        #region Public Properties

        // ******************************************************************
        // *																*
        // *						Public Properties						*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets or sets a string that holds the fullname of the type
        /// </summary>
        [XmlAttribute("name")]
        public string TypeName
        {
            get { return m_typeName; }
            set { m_typeName = value; }
        }

        #endregion

        #region Cast Operators

        // ******************************************************************
        // *																*
        // *						  Cast Operators						*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Implicit cast operator that converts the specified Type into an 
        /// XmlTypeSerialize object
        /// </summary>
        /// <param name="type">
        /// A Type that must be converted into an XmlTypeSerializer 
        /// </param>
        /// <returns>
        /// An XmlTypeSerializer instance that wraps the specified
        /// Type 
        /// </returns>
        public static implicit operator XmlTypeSerializer(Type type)
        {
            return new XmlTypeSerializer(type);
        }

        /// <summary>
        /// Implicit cast operator that converts the specified XmlTypeSerializer 
        /// into a Type value
        /// </summary>
        /// <param name="serializer">
        /// An XmlTypeSerializer that must be converted into a Type value
        /// </param>
        /// <returns>
        /// A Type value that was wrapped by the specified XmlTypeSerializer instance
        /// </returns>
        public static implicit operator Type(XmlTypeSerializer serializer)
        {
            // Defensive programming
            if (serializer == null || string.IsNullOrEmpty(serializer.TypeName))
                return null;

            // Try to get actual type from string 
            var type = Type.GetType(serializer.TypeName);
            if (type == null)
                throw new InvalidCastException(string.Format(
                    "Cannot deserialize Type from string '{0}'",
                    serializer.TypeName));

            // Return resolved type
            return type;
        }

        #endregion
    }
}
