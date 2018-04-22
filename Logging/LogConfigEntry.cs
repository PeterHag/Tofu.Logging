using System.Threading;
using System.Xml.Serialization;
using Tofu.Text;

namespace Tofu.Logging
{
	public class LogConfigEntry : LogConfigDefaultEntry
    {
        #region Protected Member Variables

        // ******************************************************************
        // *																*
        // *					 Protected Member Variables				    *
        // *																*
        // ******************************************************************

        // Protected static member variables
        protected static int s_newId;

        // Protected member variables
        protected string m_logNameMask;
        protected WildcardExpression m_wildcardExpression;

        #endregion

        #region Constructors

        // ******************************************************************
        // *																*
        // *					        Constructors				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Static Constructor
        /// </summary>
        static LogConfigEntry()
        {
            // Set static members to default
            s_newId = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LogConfigEntry() : base()
        {
            // Set members to default
            m_id = Interlocked.Increment(ref s_newId);
            m_logNameMask = string.Empty;
            m_wildcardExpression = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entry">
        /// A LogConfigEntry instance whose value members must be copied into
        /// this instance (deep copy)
        /// </param>
        protected LogConfigEntry(LogConfigEntry entry) : base(entry)
        {
            // Deep copy
            m_id = entry.m_id;
            m_logNameMask = entry.m_logNameMask;
            m_wildcardExpression = null;
        }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *					      Public Methods			            *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Checks if the specified log name matches with the mask of this entry instance
        /// </summary>
        /// <param name="logName">
        /// A string that specifies a log name
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if the specified name matches with the mask of this config entry
        /// instance; otherwise bool <i>false</i> will be returned.
        /// </returns>
        public virtual bool IsMatch(string logName)
        {
            // Trivial test
            if (string.IsNullOrEmpty(logName))
                return false;

            // Check if we already cache a WildcardExpression
            if (m_wildcardExpression == null)
                m_wildcardExpression = new WildcardExpression(LogNameMask, false);

            // Check if name matches
            return m_wildcardExpression.Compare(logName);
        }

        /// <summary>
        /// Creates a deep copy of the current instance
        /// </summary>
        /// <returns>
        /// A new LogConfigDefaultEntry instance
        /// </returns>
        public override LogConfigDefaultEntry Clone()
        {
            return new LogConfigEntry(this);
        }

        #endregion

        #region Public Properties

        // ******************************************************************
        // *																*
        // *					     Public Properties				        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Gets or sets a string that specifies the name of a Log.<br/>
        /// This string is allowed to end with an asterix '*' wildcard, the string is 
        /// also allowed to contain multiple entries separated with a semi-colon ';' 
        /// character.
        /// </summary>
        /// <example>
        /// "Tofu.*; Tofu.Controls.*"
        /// </example>
        [XmlAttribute("mask")]
        public virtual string LogNameMask
        {
            get { return m_logNameMask; }
            set
            {
                // Store new name and clear cached wildcardexpression (if any)
                m_logNameMask = value ?? string.Empty;
                m_wildcardExpression = null;
            }
        }

        #endregion
    }
}
