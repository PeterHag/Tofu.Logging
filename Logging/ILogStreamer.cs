namespace Tofu.Logging
{
    public interface ILogStreamer
    {
        #region Methods

        // ******************************************************************
        // *																*
        // *					          Methods							*
        // *																*
        // ******************************************************************

        /// <summary>
        /// Initializes the streamer
        /// </summary>
        /// <param name="args">
        /// A LogStreamerInitArgs instance that holds all the required information to 
        /// initialize the streamer instance.
        /// </param>
        void Init(LogStreamerInitArgs args);

        /// <summary>
        /// Writes the specified log message to the streamer
        /// </summary>
        /// <param name="message">
        /// A LogMessage that must be writted to the stream
        /// </param>
        void Write(LogMessage message);
  
		/// <summary>
        /// Flushes the buffer of the streamer
        /// </summary>
        void Flush();

        /// <summary>
        /// Closes the streamer 
        /// </summary>
        void Close();

        #endregion
    }
}
