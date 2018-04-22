using System;
using System.IO;

namespace Tofu.Logging.Streamers
{
	public class LogStreamerFile : LogStreamer
    {
        #region Protected Member Variables

        // ******************************************************************
        // *																*
        // *					 Protected Member Variables		            *
        // *																*
        // ******************************************************************

        // Protected Member Variables
        protected int m_logCount;
        protected int m_messageCount;
        protected LogStreamerInitArgs m_initArgs;
        protected StreamWriter m_writer;

        #endregion

        #region Constructors

        // ******************************************************************
        // *																*
        // *					        Constructors		                *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Constructor
        /// </summary>
        public LogStreamerFile()
        {
            // Set members to default
            m_logCount = 0;
            m_messageCount = 0;
            m_initArgs = null;
            m_writer = null;
        }

        #endregion

        #region Protected Methods

        // ******************************************************************
        // *																*
        // *					     Protected Methods						*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Deletes the specified file or files (if wildcards are used)
        /// </summary>
        /// <param name="path">
        /// A string that either specifies a single file or a set of files by using wildcards
        /// </param>
        /// <returns>
        /// A bool <i>true</i> if either the single file or all files with specified wildcards
        /// could be deleted; otherwise a bool <i>false</i> will be returned.
        /// </returns>
        protected virtual bool TryDeleteFile(string path)
        {
            // Only attempt to delete is non-empty string is specified
            if (string.IsNullOrEmpty(path))
                return false;

            // Check if we need to perform wildcard delete or single delete
            if (path.IndexOfAny(new char[] { '*', '?' }) >= 0)
            {
                // Wildcard delete

                // Try to determine directory (if any)
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    // A directory was specified, but does it exist?
                    if (!Directory.Exists(directory))
                    {
                        // Unknown directory
                        return false;
                    }
                }

                // Try to determine filename
                string file = Path.GetFileName(path);
                if (string.IsNullOrEmpty(path))
                {
                    // Empty filename
                    return false;
                }

                try
                {
                    // Assume a wildcard delete must be executed
                    bool success = true;

                    // Try to delete and return success
                    string[] files = Directory.GetFiles(directory, file);
                    foreach (string wildcardFile in files)
                        try
                        {
                            // Try to delete this file
                            File.Delete(wildcardFile);
                        }
                        catch (Exception)
                        {
                            // Delete failed, but allow loop to continue
                            success = false;
                        }

                    // Return 
                    return success;
                }
                catch (Exception) { }
            }
            else
            {
                // Single file delete
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                        return true;
                    }
                    catch (Exception) { }
                }
            }

            // If we reach this line, delete failed
            return false;
        }

        /// <summary>
        /// Creates a fully qualified path for the log with specified count
        /// </summary>
        /// <param name="includeFolderFullName">
        /// A bool that specifies if the full foldername should be included into the 
        /// resource path.
        /// </param>
        /// <param name="logCount">
        /// An integer that specifies the sequence number of the log file
        /// </param>
        /// <returns>
        /// A string that holds the fully qualified path for the log
        /// </returns>
        protected virtual string CreateLogPath(
            bool includeFolderFullName,
            int logCount)
        {
            // Determine text to insert for streamer type
            string streamerType = m_initArgs.ConcurrentStreamersSameTypeCount > 1 ?
                string.Concat(GetType().Name, m_initArgs.ConcurrentStreamersSameTypeIndex + 1) :
                string.Empty;

            // Masks depends on fact if we have concurrent streamers for same type
            return string.Format(
                includeFolderFullName ? "{0}{1}{2}_{3}_{4}.log" : "{2}_{3}_{4}",
                m_initArgs.SessionFolderFullName,
                Path.DirectorySeparatorChar,
                m_initArgs.LogName,
                streamerType,
                logCount);
        }

        /// <summary>
        /// Creates a fully qualified path for the specified resource arguments
        /// </summary>
        /// <param name="includeFolderFullName">
        /// A bool that specifies if the full foldername should be included into the 
        /// resource path.
        /// </param>
        /// <param name="message">
        /// A LogMessage that is associated with the specified resource.<br/>
        /// <b>Note:</b> If this argument equals <i>null</i>, asterix characters will
        /// be used inserted instead into the reosurce path.
        /// </param>
        /// <param name="logCount">
        /// An integer that specifies the sequence number of the log file
        /// </param>
        /// <param name="resourceExtension">
        /// A string that specifies the filename extension
        /// </param>
        /// <returns>
        /// A string that holds the fully qualified path for the specified resource arguments
        /// </returns>
        protected virtual string CreateResourcePath(
            bool includeFolderFullName,
            LogMessage message,
            int logCount,
            string resourceExtension)
        {
            // Create full path
            return string.Format(
                includeFolderFullName ? "{0}{1}{2}_{3}_{4}.{5}" : "{2}_{3}_{4}.{5}",
                m_initArgs.SessionResourceFolderFullName,
                Path.DirectorySeparatorChar,
                message != null ? message.Id.ToString() : "*",
                CreateLogPath(false, logCount),
                message != null ? message.LogType.ToString() : "*",
                message != null ? resourceExtension : "*");
        }

        /// <summary>
        /// Serializes the specified logging message into a single line of text
        /// </summary>
        /// <param name="message">
        /// A LogMessage reference
        /// </param>
        /// <param name="resourcePath">
        /// A string that holds the filename (excl. foldername) of the resource file
        /// that is associated with this message
        /// </param>
        /// <returns>
        /// A string that holds the contents of the specified logging message 
        /// concatenated into a single line of text
        /// </returns>
        protected virtual string InternalSerializeMessage(
            LogMessage message,
            string resourcePath)
        {
            // Delegate to static method by default
            // This virtual method is provided for inheritors
            return SerializeMessage(message, resourcePath);
        }

        /// <summary>
        /// Serializes the resource object from the specified message into either
        /// a byte array or a char array
        /// </summary>
        /// <param name="message">
        /// A LogMessage that contains the resource that must be serialized
        /// </param>
        /// <param name="resourceData">
        /// An object out parameter that will receive the serialized resource data. Typically
        /// this is either a Char array or Byte array depending on the prefered file storage.
        /// </param>
        /// <param name="resourceFileExtension">
        /// A string that will receive a proposed extensionname for the serialized resource data
        /// </param>
        /// <returns>
        /// A SerializedResourceDataType enum value that specifies the format of the serialized
        /// resource data.
        /// </returns>
        protected virtual SerializedResourceDataType InternalSerializeResource(
            LogMessage message,
            out object resourceData,
            out string resourceFileExtension)
        {
            // Delegate to static method by default
            // This virtual method is provided for inheritors
            return SerializeResource(
                m_initArgs.SessionConfig,
                message,
                out resourceData,
                out resourceFileExtension);
        }

        #endregion

        #region ILogStreamer Interface

        // ******************************************************************
        // *																*
        // *					   ILogStreamer Interface		            *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Initializes the streamer
        /// </summary>
        /// <param name="args">
        /// A LogStreamerInitArgs instance that holds all the required information to 
        /// initialize the streamer instance.
        /// </param>
        public override void Init(LogStreamerInitArgs args)
        {
            // Store arguments into members and reset counters
            m_initArgs = args;
            m_logCount = 0;
            m_messageCount = 0;

            // Reset writer (if any)
            Close();
        }

        /// <summary>
        /// Closes the streamer 
        /// </summary>
        public override void Close()
        {
            // Close current stream
            if (m_writer != null)
                lock (m_writer)
                {
                    m_writer.Close();
                    m_writer.Dispose();
                    m_writer = null;
                }
        }

        /// <summary>
        /// Flushes the buffer of the streamer
        /// </summary>
        public override void Flush()
        {
            if (m_writer != null)
                lock (m_writer)
                    m_writer.Flush();
        }

        /// <summary>
        /// Writes the specified log message to the streamer
        /// </summary>
        /// <param name="message">
        /// A LogMessage that must be writted to the stream
        /// </param>
        public override void Write(LogMessage message)
        {
            // Declare variables
            string resourceExtension;
            string resourceFile;
            string resourcePath;
            object resourceData;
            SerializedResourceDataType resourceType;

            // Serialize resource data (if any)
            if (HasResourceData(message))
            {
                // We have a resource
                resourceType = InternalSerializeResource(
                    message,
                    out resourceData,
                    out resourceExtension);
                resourceFile = CreateResourcePath(
                    false,
                    message,
                    m_logCount,
                    resourceExtension);
                resourcePath = CreateResourcePath(
                    true,
                    message,
                    m_logCount,
                    resourceExtension);
            }
            else
            {
                // We do not have a resource
                resourceType = SerializedResourceDataType.None;
                resourceData = null;
                resourceExtension = null;
                resourceFile = null;
                resourcePath = null;
            }

            // Serialize log message 
            string messageText = InternalSerializeMessage(message, resourceFile);

            // Write log message
            if (m_writer == null)
                m_writer = new StreamWriter(CreateLogPath(true, m_logCount));
            lock (m_writer)
                m_writer.Write(messageText);

            // Write resource data
            if (resourceType != SerializedResourceDataType.None)
            {
                // Determine how to write resource (either text or binary)
                switch (resourceType)
                {
                    case SerializedResourceDataType.CharArray:
                        using (var streamWrt = new StreamWriter(resourcePath, false))
                        {
                            streamWrt.Write((char[])resourceData);
                            streamWrt.Close();
                        }
                        break;
                    case SerializedResourceDataType.ByteArray:
                        using (var streamFile = new FileStream(resourcePath, FileMode.Create))
                        using (var streamMem = new MemoryStream((byte[])resourceData))
                            streamMem.WriteTo(streamFile);
                        break;
                }
            }

            // Check if we need to do cleanup
            if (m_initArgs.SessionConfig.CleanupMessages > 0)
            {
                // Increase current message counter and check for overflow
                m_messageCount++;
                if (m_messageCount >= m_initArgs.SessionConfig.CleanupMessages)
                {
                    // Close current log file (rely on next 'Write()' to create new instance)
                    Close();

                    // Check if we need to kill oldest log
                    if (m_initArgs.SessionConfig.CleanupLogs > 0)
                    {
                        // Do we have more logs than allowed?
                        if (m_logCount >= m_initArgs.SessionConfig.CleanupLogs)
                        {
                            // Kill oldest log
                            int logCount = m_logCount - m_initArgs.SessionConfig.CleanupLogs;
                            TryDeleteFile(CreateLogPath(true, logCount));

                            // Kill associated resources (if allowed)
                            if (!m_initArgs.SessionConfig.CleanupResources)
                                TryDeleteFile(CreateResourcePath(true, null, logCount, ""));
                        }
                    }

                    // Reset message counter
                    m_logCount++;
                    m_messageCount = 0;
                }
            }
        }

        #endregion
    }
}
