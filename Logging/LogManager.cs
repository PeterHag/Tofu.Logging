using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Tofu.Collections;
using Tofu.Logging.Formatters;
using Tofu.Logging.Streamers;

namespace Tofu.Logging
{
	public class LogManager : ILogManager
	{
		#region Public Constants

		// ******************************************************************
		// *																*
		// *					    Public Constants		                *
		// *																*
		// ******************************************************************

		// Public Constants 
		public const long INVALID_ID = -1;
		public const string CONFIG_EXTENSION = "logconfig";

		// Public constants - Formatter Names
		public const string FORMATTER_OBJECTTOTEXT = "txt";
		public const string FORMATTER_OBJECTTOXML = "xml";
		public const string FORMATTER_OBJECTTOJSON = "json";
		public const string FORMATTER_EXCEPTIONTOTEXT = "txt";

		#endregion

		public const string LOGNAME_LOGMANAGER = "__LogManager";

		#region Protected Member Variables

		// ******************************************************************
		// *																*
		// *					 Protected Member Variables		            *
		// *																*
		// ******************************************************************

		// Protected member variables
		protected long m_sessionId;
		protected TimeSpan m_utcOffset;
		protected SessionModes m_sessionMode;
		protected string m_sessionFolder;
		protected string m_sessionFolderFullName;
		protected string m_sessionResourceFolderFullName;
		protected string m_sessionContext;
		protected ILogConfig m_sessionConfig;
		protected Guid m_sessionGuid;
		protected ILog m_sessionLog;
		protected ILog[] m_logs;
		protected ILogMonitor[] m_monitors;
		protected ILogThread m_thread;
		protected ILogConfig m_config;
		protected string m_context;
		protected object m_syncRoot;
		protected Dictionary<string, ILog> m_dicLogs;

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
		/// <param name="config">
		/// An ILogConfig reference
		/// </param>
		public LogManager(ILogConfig config)
		{
			// Validate input
			if (config == null)
				throw new ArgumentNullException("Invalid Configuration specified; <null> not allowed");

			// Set member variables to default
			m_utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
			m_sessionId = 0;
			m_sessionMode = SessionModes.Stopped;
			m_sessionFolder = string.Empty;
			m_sessionFolderFullName = string.Empty;
			m_sessionContext = string.Empty;
			m_sessionGuid = Guid.Empty;
			m_sessionConfig = null;
			m_sessionLog = null;
			m_dicLogs = new Dictionary<string, ILog>(StringComparer.InvariantCultureIgnoreCase);
			m_logs = new ILog[0];
			m_monitors = new ILogMonitor[0];
			m_thread = new LogThread();
			m_config = config;
			m_syncRoot = new object();
			m_context = CreateDefaultContext();

			// Do we need to auto-start session?
			if (config.AutoStart)
				StartSession();
		}

		#endregion

		#region Public Methods

		// ******************************************************************
		// *																*
		// *					      Public Methods					    *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Creates a new ILogManager instance for the specified type
		/// </summary>
		/// <returns>
		/// A new ILogManager instance
		/// </returns>
		public static ILogManager CreateManager()
		{
			// Delegate call
			return CreateManager(typeof(LogManager), CreateDefaultConfig());
		}

		/// <summary>
		/// Creates a new ILogManager instance
		/// </summary>
		/// <param name="config">
		/// An ILogLogConfig instance
		/// </param>
		/// <returns>
		/// A new ILogManager instance
		/// </returns>
		public static ILogManager CreateManager(ILogConfig config)
		{
			// Delegate call
			return CreateManager(typeof(LogManager), config);
		}

		/// <summary>
		/// Creates a new ILogManager instance for the specified type
		/// </summary>
		/// <param name="logManagerType">
		/// A Type that must implement the ILogManager interface
		/// </param>
		/// <returns>
		/// A new ILogManager instance
		/// </returns>
		public static ILogManager CreateManager(Type logManagerType)
		{
			// Delegate call
			return CreateManager(logManagerType, CreateDefaultConfig());
		}

		/// <summary>
		/// Creates a new ILogManager instance for the specified type
		/// </summary>
		/// <param name="logManagerType">
		/// A Type that must implement the ILogManager interface
		/// </param>
		/// <param name="configFilename">
		/// A string that specifies the filename of the configfile that must be loaded
		/// </param>
		/// <returns>
		/// A new ILogManager instance
		/// </returns>
		public static ILogManager CreateManager(Type logManagerType, string configFilename)
		{
			// Declare variables
			ILogConfig config;
			Exception ex;

			// Try to load config
			if (!TryLoadConfig(configFilename, out config, out ex))
				config = CreateDefaultConfig();

			// Delegate call
			return CreateManager(logManagerType, config);
		}

		/// <summary>
		/// Creates a new ILogManager instance for the specified type
		/// </summary>
		/// <param name="logManagerType">
		/// A Type that must implement the ILogManager interface
		/// </param>
		/// <param name="config">
		/// An ILogConfig instance
		/// </param>
		/// <returns>
		/// A new ILogManager instance
		/// </returns>
		public static ILogManager CreateManager(Type logManagerType, ILogConfig config)
		{
			// Validate type
			ValidateILogManagerType(logManagerType);

			// Create new manager and return reference
			return (ILogManager)Activator.CreateInstance(logManagerType, config);
		}

		/// <summary>
		/// Creates a new ILogManager instance
		/// </summary>
		/// <returns>
		/// A new ILogManager instance
		/// </returns>
		public static T CreateManager<T>() where T : ILogManager
		{
			// Delegate call
			return (T)CreateManager(typeof(T));
		}

		/// <summary>
		/// Creates a new ILogManager instance
		/// </summary>
		/// <param name="config">
		/// An ILogLogConfig instance
		/// </param>
		/// <returns>
		/// A new ILogManager instance
		/// </returns>
		public static T CreateManager<T>(ILogConfig config) where T : ILogManager
		{
			// Delegate call
			return (T)CreateManager(typeof(T), config);
		}

		/// <summary>
		/// Creates a new ILogManager instance
		/// </summary>
		/// <param name="configFilename">
		/// A string that specifies the filename of the configfile that must be loaded
		/// </param>
		/// <returns>
		/// A new ILogManager instance
		/// </returns>
		public static T CreateManager<T>(string configFilename) where T : ILogManager
		{
			// Delegate call
			return (T)CreateManager(typeof(T), configFilename);
		}

		/// <summary>
		/// Checks if the specified type is a valid implementation of the 
		/// ILogManager interface
		/// </summary>
		/// <param name="logManagerType">
		/// A Type that specifies the class type to validate
		/// </param>
		/// <exception cref="NullReferenceException">
		/// Thrown when a null reference is specified as class type
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when the specified class type does not implement
		/// the <i>ILogManager</i> interface<br/>
		/// <br/>
		/// - Or -<br/>
		/// <br/>
		/// When the specified class type has no constructor that takes
		/// an <i>ILogConfig</i> instance as argument
		/// </exception>
		public static void ValidateILogManagerType(Type logManagerType)
		{
			// Check if new value is valid
			if (logManagerType == null)
				throw new NullReferenceException(
					"Invalid ILogManager type specified; <null> not allowed");

			// Check if ILogManager interface is implemented
			Type t = logManagerType.GetInterface(typeof(ILogManager).Name);
			if (t == null)
				throw new ArgumentException(
					"Invalid type specified; type must implement 'ILogManager' interface");

			// Check if type has constructor that takes ILogConfig argument
			var ctor = logManagerType.GetConstructor(new Type[1] { typeof(LogConfig) });
			if (ctor == null)
				throw new ArgumentException(string.Format(
					"Invalid type specified; type '{0}' has no constructor that takes ILogConfig as argument!",
					logManagerType.Name));
		}

		/// <summary>
		/// Creates a string that specifies the fully qualified config filename for the
		/// current executing application or service
		/// </summary>
		/// <returns>
		/// A string that holds the filename for the config attached to the current 
		/// executing process
		/// </returns>
		public static string CreateDefaultConfigPath()
		{
			return string.Format(
				"{0}.{1}",
				AppDomain.CurrentDomain.BaseDirectory,
				CONFIG_EXTENSION);
		}

		/// <summary>
		/// Creates a new LogConfig instance and apply default initialization
		/// </summary>
		/// <returns>
		/// A new LogConfig instance
		/// </returns>
		public static ILogConfig CreateDefaultConfig()
		{
			// Create a default LogConfig instance
			var config = new LogConfig();
			config.DefaultEntry.AddStreamerType(typeof(LogStreamerFile), "");

			// Create default formatters
			config.FormattersObject.RegisterFormatterType<LogFormatterObjectToString>(FORMATTER_OBJECTTOTEXT, "", true);
			config.FormattersObject.RegisterFormatterType<LogFormatterObjectToXml>(FORMATTER_OBJECTTOXML, "");
			config.FormattersException.RegisterFormatterType<LogFormatterExceptionToText>(FORMATTER_EXCEPTIONTOTEXT, "", true);

			// Return reference to default config
			return config;
		}

		/// <summary>
		/// Creates a context string that indentifies what started the logging sessions
		/// </summary>
		/// <returns>
		/// A string that holds the name and version of the application that
		/// started the log session
		/// </returns>
		public static string CreateDefaultContext()
		{
			// Return name of exe that started process
			var assembly = Assembly.GetEntryAssembly();
			var fileInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			var context = string.Format(
				"Session '{0} v{1}.{2}.{3}.{4}'",
				assembly.GetName().Name,
				fileInfo.ProductMajorPart,
				fileInfo.ProductMinorPart,
				fileInfo.ProductBuildPart,
				fileInfo.ProductPrivatePart);
			return context;
		}

		/// <summary>
		/// Tries to save the contents of the specified config instance to 
		/// the specified path
		/// </summary>
		/// <param name="path">
		/// A string that specifies a fully qualified path including filename 
		/// and extension 
		/// </param>
		/// <param name="config">
		/// An ILogConfig instance whose contents must be saved
		/// </param>
		/// <param name="ex">
		/// An out reference to an Exception in case something went wrong
		/// during the save operation
		/// </param>
		/// <returns>
		/// A bool <i>true</i> if the config file could be save succesfully;
		/// otherwise a bool <i>false</i> will be returned
		/// </returns>
		public static bool TrySaveConfig(string path, ILogConfig config, out Exception ex)
		{
			try
			{
				// Open stream to specified path and delegate call
				SaveConfig(path, config);

				// No exception occurred
				ex = null;
				return true;
			}
			catch (Exception e)
			{
				// Error occured
				ex = e;
				return false;
			}
		}

		/// <summary>
		/// Tries to save the contents of the specified config instance to 
		/// the specified stream
		/// </summary>
		/// <param name="stream">
		/// A Stream to which the contents of the specified config will be saved
		/// </param>
		/// <param name="config">
		/// An ILogConfig instance whose contents must be saved
		/// </param>
		/// <param name="ex">
		/// An out reference to an Exception in case something went wrong during 
		/// the save operation
		/// </param>
		/// <returns>
		/// A bool <i>true</i> if the config file could be save succesfully;
		/// otherwise a bool <i>false</i> will be returned
		/// </returns>
		public static bool TrySaveConfig(Stream stream, ILogConfig config, out Exception ex)
		{
			try
			{
				// Use XmlSerializer to write to stream
				SaveConfig(stream, config);

				// No exception occurred
				ex = null;
				return true;
			}
			catch (Exception e)
			{
				// Error occured
				ex = e;
				return false;
			}
		}

		/// <summary>
		/// Tries to load the contents of the specified file into a new
		/// config instance
		/// </summary>
		/// <param name="path">
		/// A string that specifies a fully qualified path including filename 
		/// and extension 
		/// </param>
		/// <param name="config">
		/// An out reference to a new ILogConfig instance that will hold the 
		/// loaded contents
		/// </param>
		/// <param name="ex">
		/// An out reference to an Exception in case something went wrong
		/// during the load operation
		/// </param>
		/// <returns>
		/// A bool <i>true</i> if the config file could be loaded succesfully;
		/// otherwise a bool <i>false</i> will be returned
		/// </returns>
		public static bool TryLoadConfig(string path, out ILogConfig config, out Exception ex)
		{
			try
			{
				// Check if file exists
				if (!File.Exists(path))
				{
					ex = null;
					config = null;
					return false;
				}

				// Load config
				config = LoadConfig(path);

				// No exception occured
				ex = null;
				return true;
			}
			catch (Exception e)
			{
				// Error occured
				ex = e;
				config = null;
				return false;
			}
		}

		/// <summary>
		/// Tries to load an ILogConfig from the specified stream
		/// </summary>
		/// <param name="bytes">
		/// An array of bytes that represent a MemoryStream from which an ILogConfig 
		/// instance must be loaded
		/// </param>
		/// <param name="config">
		/// An out reference to a new ILogConfig instance that will hold the 
		/// loaded contents
		/// </param>
		/// <param name="ex">
		/// An out reference to an Exception in case something went wrong
		/// during the load operation
		/// </param>
		/// <returns>
		/// A bool <i>true</i> if the config file could be loaded succesfully;
		/// otherwise a bool <i>false</i> will be returned
		/// </returns>
		public static bool TryLoadConfig(byte[] bytes, out ILogConfig config, out Exception ex)
		{
			try
			{
				// Load config
				config = LoadConfig(bytes);

				// No exception occured
				ex = null;
				return true;
			}
			catch (Exception e)
			{
				// Error occured
				ex = e;
				config = null;
				return false;
			}
		}

		/// <summary>
		/// Tries to load an ILogConfig from the specified stream
		/// </summary>
		/// <param name="stream">
		/// A Stream from which an ILogConfig instance must be loaded
		/// </param>
		/// <param name="config">
		/// An out reference to a new ILogConfig instance that will hold the 
		/// loaded contents
		/// </param>
		/// <param name="ex">
		/// An out reference to an Exception in case something went wrong
		/// during the load operation
		/// </param>
		/// <returns>
		/// A bool <i>true</i> if the config file could be loaded succesfully;
		/// otherwise a bool <i>false</i> will be returned
		/// </returns>
		public static bool TryLoadConfig(Stream stream, out ILogConfig config, out Exception ex)
		{
			try
			{
				// Load config
				config = LoadConfig(stream);

				// No exception occured
				ex = null;
				return true;
			}
			catch (Exception e)
			{
				// Exception occured
				ex = e;
				config = null;
				return false;
			}
		}

		/// <summary>
		/// Loads the contents of the specified file into a new config instance
		/// </summary>
		/// <param name="path">
		/// A string that specifies a fully qualified path including filename 
		/// and extension 
		/// </param>
		/// <returns>
		/// An ILogConfig reference to a new ILogConfig instance that will hold the 
		/// loaded contents
		/// </returns>
		public static ILogConfig LoadConfig(string path)
		{
			// Delegate to stream loader
			using (var rdr = new StreamReader(path))
				return LoadConfig(rdr.BaseStream);
		}

		/// <summary>
		/// Loads the contents of the specified file into a new config instance
		/// </summary>
		/// <param name="bytes">
		/// An array of bytes that represent a MemoryStream from which an ILogConfig 
		/// instance must be loaded
		/// </param>
		/// <returns>
		/// An ILogConfig reference to a new ILogConfig instance that will hold the 
		/// loaded contents
		/// </returns>
		public static ILogConfig LoadConfig(byte[] bytes)
		{
			// Delegate to stream loader
			using (var stream = new MemoryStream(bytes))
				return LoadConfig(stream);
		}

		/// <summary>
		/// Loads the contents of the specified file into a new config instance
		/// </summary>
		/// <param name="stream">
		/// A Stream from which an ILogConfig instance must be loaded
		/// </param>
		/// <returns>
		/// An ILogConfig reference to a new ILogConfig instance that will hold the 
		/// loaded contents
		/// </returns>
		public static ILogConfig LoadConfig(Stream stream)
		{
			// Deserialize config from stream
			var xmlSerializer = new XmlSerializer(typeof(LogConfig));
			return (ILogConfig)xmlSerializer.Deserialize(stream);
		}

		/// <summary>
		/// Saves the contents of the specified config instance to the specified path
		/// </summary>
		/// <param name="path">
		/// A string that specifies a fully qualified path including filename 
		/// and extension 
		/// </param>
		/// <param name="config">
		/// An ILogConfig instance whose contents must be saved
		/// </param>
		public static void SaveConfig(string path, ILogConfig config)
		{
			using (var wrt = new StreamWriter(path))
				SaveConfig(wrt.BaseStream, config);
		}

		/// <summary>
		/// Saves the contents of the specified config instance to the specified stream
		/// </summary>
		/// <param name="stream">
		/// A Stream to which the contents of the specified config will be saved
		/// </param>
		/// <param name="config">
		/// An ILogConfig instance whose contents must be saved
		/// </param>
		public static void SaveConfig(Stream stream, ILogConfig config)
		{
			// Use XmlSerializer to write to stream
			var xmlSerializer = new XmlSerializer(typeof(LogConfig));
			xmlSerializer.Serialize(
				stream,
				config,
				new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));
		}

		#endregion

		/// <summary>
		/// Creates a new unregistered ILog instance
		/// </summary>
		/// <param name="logName">
		/// A string that specifies the name that must be associated with the log
		/// </param>
		/// <param name="config">
		/// An ILogConfig instance from which the configuration must be used
		/// </param>
		/// <returns>
		/// A new unregistered ILog instance
		/// </returns>
		protected virtual ILog CreateLog(string logName, ILogConfig config)
		{
			// Determine which config entry to use for specified name
			var entry = config.GetEntry(logName);

			// Create new log and apply local entry settings
			return new Log(logName, this, entry.Id)
			{
				LocalLevels = entry.LocalLevels,
				LocalTrace = entry.LocalTrace
			};
		}

		#region Protected Methods

		// ******************************************************************
		// *																*
		// *					     Protected Methods					    *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Creates an new unique log id
		/// </summary>
		/// <remarks>
		/// Starting a new sessing will reset the id generator
		/// </remarks>
		/// <param name="name">
		/// A string that specifies the name of the calling log
		/// </param>
		/// <returns>
		/// A long that holds a new unique id
		/// </returns>
		protected virtual long CreateSessionLogId(string name)
		{
			lock (SyncRoot)
			{
				// Increment id generator
				return m_sessionId++;
			}
		}

		/// <summary>
		/// Tries to get a full list of all context related child folders in the
		/// specified main session folder
		/// </summary>
		/// <param name="sessionFolder">
		/// A string that specifies the main session folder (i.e. logging folder)<br/>
		/// <b>Note:</b> The returned folders will be ordered descending on creation date
		/// (i.e. oldest folders will be stored last in list)
		/// </param>
		/// <param name="context">
		/// A string that specifies the session context
		/// </param>
		/// <param name="sessionFolders">
		/// A list that will receive all context related child folders that were
		/// found in the specified session folder
		/// </param>
		/// <returns>
		/// A bool <i>true</i> if resolving of folders was succesfull; otherwise
		/// a bool <i>false</i> will be returned.
		/// </returns>
		protected virtual bool TryGetFolders(string sessionFolder, string context, out List<DirectoryInfo> sessionFolders)
		{
			// Set default return value
			sessionFolders = new List<DirectoryInfo>();

			try
			{
				// Build list with all session directories in logging folder
				foreach (string folder in Directory.EnumerateDirectories(
					sessionFolder,
					string.Format("*{0}*", context)))
				{
					// Get info of this directory and add to list
					sessionFolders.Add(new DirectoryInfo(folder));
				}

				// Order by creation (i.e. recent folder first, oldest folder last)
				if (sessionFolders.Count > 0)
					sessionFolders = sessionFolders.OrderByDescending(
						d => d.CreationTime).ToList();

				// If we reach this line, resolve was success
				return true;
			}
			catch (Exception)
			{
				// We do not propagate exception upward because:
				// - Session folder might empty
				// - Child folder might be in lock due to debug session
				// - Assume that exception while cleaning an old folder is not critical
				return false;
			}
		}

		/// <summary>
		/// Tries to find previous session folders for the specified context and
		/// then performs a cleanup according configuration so that only recent
		/// ones will remain.
		/// </summary>
		/// <param name="cleanupKeepSessions">
		/// An integer that specifies how many most recent previous sessions should 
		/// be kept.
		/// </param>
		/// <param name="folder">
		/// A string that specifies the folder in which to look for previous sessions
		/// </param>
		/// <param name="context">
		/// A string that specifies the context for the current application
		/// </param>
		/// <param name="sessionFolders">
		/// A collection that receives all remaining session folders.<br/>
		/// <b>Note:</b> If according configuration no session folders are to
		/// be cleaned, this method will return <i>null</i>!
		/// </param>
		/// <returns>
		/// A bool <i>true</i> if all previous session folders could succesfully
		/// be deleted; otherwise a bool <i>false</i> will be returned.
		/// </returns>
		protected virtual bool TryDeletePreviousSessions(
			int cleanupKeepSessions,
			string folder,
			string context,
			out List<DirectoryInfo> sessionFolders)
		{
			// Declare variables
			bool success = true;

			// Set default return value
			sessionFolders = null;

			// Do we need to perform cleanup of session folders?
			if (cleanupKeepSessions > 0)
			{
				// Try to get list with all child folders
				if (TryGetFolders(folder, context, out sessionFolders) &&
					sessionFolders.Count >= cleanupKeepSessions)
				{
					// Delete older directories
					while (sessionFolders.Count >= cleanupKeepSessions)
					{
						try
						{
							// Resolve name of oldest directory (i.e. last entry in list)
							string directoryDelete = sessionFolders[sessionFolders.Count - 1].FullName;
							sessionFolders.RemoveAt(sessionFolders.Count - 1);

							// Try to delete this folder
							Directory.Delete(directoryDelete, true);
						}
						catch
						{
							// At least one session could not be removed
							success = false;
						}
					}
				}
			}

			// Return session folders
			return success;
		}

		/// <summary>
		/// Tries the find the highest index value of previous session folders
		/// </summary>
		/// <param name="sessionFolders">
		/// A collection that holds the previous session folders that could be
		/// associated with this context
		/// </param>
		/// <returns>
		/// A integer that holds the highest index value from the set of previous
		/// session folders. If no index value could be resolved, value <i>-1</i>
		/// will be returned.
		/// </returns>
		protected virtual int IndexOfPreviousSession(List<DirectoryInfo> sessionFolders)
		{
			// Declare variables
			int index = -1;

			// Check if we can resolve most recent folder
			if (sessionFolders != null && sessionFolders.Count > 0)
			{
				// Get name with 'highest' index
				var name = sessionFolders.OrderByDescending(d => d.Name).ToList()[0].Name;

				// Try to find an index value (if any)
				int i = name.LastIndexOf("(");
				if (i >= 0)
				{
					int j = name.IndexOf(")", i + 1);
					if (i >= 0 && j >= 0)
					{
						// Try to parse index from substring
						int parseIndex;
						if (int.TryParse(name.Substring(i + 1, j - i - 1), out parseIndex))
						{
							// Assume next available index for new session is parsed value + 1
							index = parseIndex + 1;
						}
					}
				}
			}

			// Return index value
			return index;
		}

		/// <summary>
		/// Starts a new logging session by creating a new output folder in the specified path
		/// </summary>
		/// <param name="folder">
		/// A string that specifies the output folder where the log files must be stored
		/// </param>
		/// <param name="sessionGuid">
		/// A Guid that uniquely identifies this logging session
		/// </param>
		protected virtual void InternalStartSession(string folder, Guid sessionGuid)
		{
			// Declare variables
			string sessionContext = Context;

			// Try to delete older session folders first
			List<DirectoryInfo> sessionFolders;
			TryDeletePreviousSessions(
				Config.CleanupSessions,
				folder,
				sessionContext,
				out sessionFolders);

			// Try get highest session value
			var highestIndex = IndexOfPreviousSession(sessionFolders);

			// Create new empty folder basename for session
			int count = highestIndex >= 0 ? highestIndex : 2;
			string folderFullNameBase = string.Format(
				"{0}{1}{2}",
				folder,
				Path.DirectorySeparatorChar,
				sessionContext);
			string folderFullName = folderFullNameBase;
			while (highestIndex >= 0 || Directory.Exists(folderFullName))
			{
				// Append counter to folder name and try again
				highestIndex = -1;
				folderFullName = string.Format(
					"{0} ({1})",
					folderFullNameBase,
					count++);
			}
			// Create session folder
			if (!Directory.Exists(folderFullName))
				Directory.CreateDirectory(folderFullName);

			// Ensure resource folder is also either created or cleaned
			string resourceFolderFullName = string.Format(
				"{0}{1}{2}",
				folderFullName,
				Path.DirectorySeparatorChar,
				"Resources");
			if (!Directory.Exists(resourceFolderFullName))
				Directory.CreateDirectory(resourceFolderFullName);

			// Update members to reflect new logging session
			m_logs = new ILog[0];
			m_sessionId = 0;
			m_sessionContext = sessionContext;
			m_sessionFolder = folder;
			m_sessionFolderFullName = folderFullName;
			m_sessionResourceFolderFullName = resourceFolderFullName;
			m_sessionMode = SessionModes.Running;
			m_sessionGuid = sessionGuid;
			m_sessionConfig = Config.Clone();
			m_sessionLog = CreateLog(LOGNAME_LOGMANAGER, m_sessionConfig);

			// Update config entry id's
			// We assume that calling code probably will keep ILog references in members.
			// Therefore we need to update mapping of these logs to specific streamers in config.
			foreach (var pair in m_dicLogs)
				pair.Value.SetConfigEntryId(Config.GetEntry(pair.Key).Id);

			// Create new monitors
			var listMonitors = new List<ILogMonitor>();
			foreach (var monitorType in Config.MonitorTypes)
			{
				// Create and initialize new monitor instance
				var monitor = (ILogMonitor)Activator.CreateInstance(monitorType.MonitorType);
				monitor.Init(this, new ParameterDictionary(monitorType.Parameters));

				// Add new monitor instance to list
				listMonitors.Add(monitor);
			}

			// Dispose old monitors and store new monitors
			foreach (var monitor in m_monitors)
				monitor.Dispose();
			m_monitors = listMonitors.ToArray();

			// Start logger thread
			m_thread.Start(sessionGuid, folderFullName, resourceFolderFullName, Config);

			// Log event
			m_sessionLog.AddParams(
				LogLevel.Debug, 
				() => "StartSession",
				() => new string[] { "SessionGuid" },
				() => new string[] { m_sessionGuid.ToString() });
			m_sessionLog.AddObject(
				LogLevel.Debug, 
				() => "SessionConfig", 
				m_sessionConfig, 
				FORMATTER_OBJECTTOXML);

			// Raise event that mode has been changed
			OnLogsChanged();
			OnSessionModeChanged();

			// Start all monitors
			foreach (var monitor in m_monitors)
				monitor.SessionStarted();
		}

		/// <summary>
		/// Suspends the current session
		/// </summary>
		protected virtual void InternalSuspendSession()
		{
			// Log message
			m_sessionLog.AddText(LogLevel.Debug, () => "SuspendSession");

			// Change session state
			m_sessionMode = SessionModes.Suspended;

			// Suspend all monitors
			foreach (var monitor in SessionMonitors)
				monitor.SessionSuspended();

			// Raise event that mode has been changed
			OnSessionModeChanged();
		}

		/// <summary>
		/// Resumeshe current session
		/// </summary>
		protected virtual void InternalResumeSession()
		{
			// Change session state
			m_sessionMode = SessionModes.Running;

			// Resume all monitors
			foreach (var monitor in SessionMonitors)
				monitor.SessionResumed();

			// Log message
			m_sessionLog.AddText(LogLevel.Debug, () => "ResumeSession");

			// Raise event that mode has been changed
			OnSessionModeChanged();
		}

		/// <summary>
		/// Stops the current logging session and forces all logs to flush their
		/// pending output
		/// </summary>
		protected virtual void InternalStopSession()
		{
			// Log message
			m_sessionLog.AddText(LogLevel.Debug, () => "StopSession");

			// Change session state
			m_sessionMode = SessionModes.Stopped;

			// Stop all monitors
			foreach (var monitor in SessionMonitors)
				monitor.SessionStopped();

			// Stop logging thread and flush all messages
			m_thread.Stop();

			// Raise event that mode has been changed
			OnSessionModeChanged();
		}

		/// <summary>
		/// Raises a <see cref="SessionModeChanged"/> event
		/// </summary>
		protected virtual void OnSessionModeChanged()
		{
			// Check if there are subscribers
			if (SessionModeChanged != null)
				SessionModeChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises a <see cref="LogsChanged"/> event
		/// </summary>
		protected virtual void OnLogsChanged()
		{
			// Check if there are subscribers
			if (LogsChanged != null)
				LogsChanged(this, EventArgs.Empty);
		}

		#endregion

		#region ILogManager Interface

		// ******************************************************************
		// *																*
		// *					    ILogManager Interface					*
		// *																*
		// ******************************************************************

		/// <summary>
		/// Event that will be raised when session mode has changed
		/// </summary>
		public virtual event EventHandler SessionModeChanged;

		/// <summary>
		/// Event that will be raised when either a new log has been created
		/// or when the logs collection has been cleared (eg. session stopped)
		/// </summary>
		public virtual event EventHandler LogsChanged;

		/// <summary>
		/// Starts a new logging session
		/// </summary>
		/// <returns>
		/// An ILogManager reference
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if a session is already running<br/>
		/// <br/>
		/// - Or -<br/>
		/// <br/>
		/// The folder specified in the Configuration does not exist<br/>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if an empty folder is specified in the Configuration
		/// </exception>
		public virtual ILogManager StartSession()
		{
			// Declar variables
			string folder = Config.GetFullFolderPath();

			// Check if a valid folder name is specified
			if (string.IsNullOrWhiteSpace(folder))
				throw new ArgumentException("Empty folder name is not allowed");

			// Check if we are not already running
			if (SessionMode != SessionModes.Stopped)
				throw new InvalidOperationException("Previous session not stopped");

			// Check if specified folder exists
			if (!Directory.Exists(folder))
			{
				try
				{
					// Try to create folder
					Directory.CreateDirectory(folder);
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException(string.Format(
						"Folder '{0}' could not be created",
						folder),
						ex);
				}
			}

			lock (SyncRoot)
			{
				// Start new logging session
				InternalStartSession(folder, Guid.NewGuid());
				return this;
			}
		}

		/// <summary>
		/// Pauzes the current session by dropping all log messages. This mode should be
		/// tool that allows user to inspect logging on running application
		/// </summary>
		/// <returns>
		/// A bool <i>true</i> if current session could be suspended successfully;
		/// otherwise a bool <i>false</i> will be returned.
		/// </returns>
		public virtual bool SuspendSession()
		{
			// Declare variable
			bool success = false;

			// Check if we are in running mode
			if (SessionMode != SessionModes.Suspended)
			{
				lock (SyncRoot)
				{
					if (SessionMode == SessionModes.Running)
					{
						InternalSuspendSession();
						success = true;
					}
				}
			}

			// Return flag if session was suspended
			return success;
		}

		/// <summary>
		/// Re-starts suspended session
		/// </summary>
		/// <returns>
		/// A bool <i>true</i> if suspended session could be resumed successfully;
		/// otherwise a bool <i>false</i> will be returned.
		/// </returns>
		public virtual bool ResumeSession()
		{
			// Declare variable
			bool success = false;

			// Check if we are in suspended mode
			if (SessionMode == SessionModes.Suspended)
			{
				lock (SyncRoot)
				{
					if (SessionMode == SessionModes.Suspended)
					{
						InternalResumeSession();
						success = true;
					}
				}
			}

			// Return flag if session was resumed
			return success;
		}

		/// <summary>
		/// Stops the current logging session and forces all logs to flush their
		/// pending output
		/// </summary>
		public virtual void StopSession()
		{
			// Check if we are not already stopped
			if (SessionMode != SessionModes.Stopped)
			{
				lock (SyncRoot)
				{
					InternalStopSession();
				}
			}
		}

		/// <summary>
		/// Forces all loggers to flush their pending log messages
		/// </summary>
		public virtual void Flush()
		{
			// Check if we are running
			if (SessionMode == SessionModes.Running)
			{
				// Flush logging thread
				m_thread.Flush();
			}
		}

		/// <summary>
		/// Checks if the current session is enabled for the specified level
		/// on the specified log
		/// </summary>
		/// <param name="log">
		/// An ILog reference
		/// </param>
		/// <param name="level">
		/// A LogLevel enum value that specifies the level that must 
		/// be checked
		/// </param>
		/// <returns>
		/// A bool <i>true</i> if the current session is enabled for the specified 
		/// session, otherwise a bool <i>false</i> will be returned.
		/// </returns>
		public virtual bool IsLevelEnabled(ILog log, LogLevel level)
		{
			// Get level of specified log
			int levels = (int)log.LocalLevels;

			// Do we need to super-impose global flags from manager?
			if ((log.LocalLevels & LogLevelFlags.Global) != 0)
				levels |= (int)(m_sessionConfig != null ?
					m_sessionConfig.GlobalLevels :
					Config.GlobalLevels);

			// Check if required level flag is present
			return ((int)level & levels) == 0;
		}

		/// <summary>
		/// Creates a new LogMessage and pushes it towards the async logging thread
		/// </summary>
		/// <param name="log">
		/// An ILog reference to the logger that is pushing the message
		/// </param>
		/// <param name="level">
		/// A LogLevel enum value that specifies the severity of the logged message
		/// </param>
		/// <param name="parentId">
		/// A long that specifies the id of the parent log message
		/// </param>
		/// <param name="startId">
		/// A long that specifies the id of the start log message
		/// </param>
		/// <param name="messageType">
		/// A LogMessageType enum value that specifies the type of the message
		/// </param>
		/// <param name="text">
		/// A delegate that provides the text that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		/// <param name="report">
		/// A delegate that provides the report that must be logged. This delegate will only
		/// be executed when logging for this level has been enabled.
		/// </param>
		/// <param name="reportExtension">
		/// A delegate that provides an extension that identifies the file type of the report.
		/// </param>
		/// <param name="bytes">
		/// A delegate that provides an array of bytes (eg. from image) that must be logged. 
		/// This delegate will only be executed when logging for this level has been enabled.
		/// </param>
		/// <param name="bytesExtension">
		/// A delegate that provides an extension that identifies the file type of the bytes array. 
		/// </param>
		/// <param name="value">
		/// A double? that specifies a value that must be logged
		/// </param>
		/// <param name="valueLowerBound">
		/// A double? that specifies the lower boundary of the logged value
		/// </param>
		/// <param name="valueUpperBound">
		/// A double? that specifies the upper boundary of the logged value
		/// </param>
		/// <param name="ex">
		/// An Exception instance that must be logged
		/// </param>
		/// <param name="parameterNames">
		/// A delegate that provides an array of strings that specifies the names of the
		/// parameters that must be logged. The contents of this array must be in sync
		/// with the contents of the parameter values array. This delegate will only be 
		/// executed when logging for for this level has been enabled
		/// </param>
		/// <param name="parameterValues">
		/// A delegate that provides an array of objects that specifies the values of the
		/// parameters that must be logged. The contents of this array must be in sync
		/// with the contents of the parameter names array. This delegate will only be 
		/// executed when logging for for this level has been enabled
		/// </param>
		/// <returns>
		/// A long that uniquely identifies this message across all loggers within the current 
		/// logging session.
		/// </returns>
		/// <param name="obj">
		/// An object instance  that must be logged
		/// </param>
		/// <param name="formatAsync">
		/// A bool <i>true</i> in case the logging worker thread should do formatting. If the
		/// calling thread already performs formatting, this argument will equal <i>false</i>
		/// and the formatted text will be provided in the <i>aText</i> argument
		/// </param>
		/// <param name="formatterName">
		/// A string that specifies the name of a formatter to use in case async formatting 
		/// must be executed in the logging worker thread
		/// </param>
		public virtual long AddMessage(
			ILog log,
			LogLevel level,
			long parentId,
			long startId,
			LogMessageType messageType,
			Func<string> text,
			Func<string> report,
			Func<string> reportExtension,
			Func<byte[]> bytes,
			Func<string> bytesExtension,
			double? value,
			double? valueLowerBound,
			double? valueUpperBound,
			Exception ex,
			Func<string[]> parameterNames,
			Func<object[]> parameterValues,
			object obj,
			bool formatAsync,
			string formatterName)
		{
			// Check if logging is enabled
			if (SessionMode != SessionModes.Running)
				return INVALID_ID;

			// Check if message adheres to required level
			int levels = (int)log.LocalLevels;
			if ((log.LocalLevels & LogLevelFlags.Global) != 0)
				levels |= (int)m_sessionConfig.GlobalLevels;
			if (((int)level & levels) == 0)
				return INVALID_ID;

			// Do we need to do some object or exception formatting?
			LogFormatterResult formattedText = null;
			if (messageType == LogMessageType.Object && formatAsync == false)
			{
				// Format object
				formattedText = m_sessionConfig.FormattersObject.Format(level, obj, formatterName);
			}
			else if (messageType == LogMessageType.Exception && formatAsync == false)
			{
				// Format exception
				formattedText = m_sessionConfig.FormattersObject.Format(level, ex, formatterName);
			}

			// If we reach this line, we need to create a new logmessage
			var logMsg = new LogMessage(
				CreateSessionLogId(log.Name),
				parentId,
				startId,
				Thread.CurrentThread.ManagedThreadId,
				DateTime.SpecifyKind(
					DateTime.UtcNow + m_utcOffset,
					DateTimeKind.Local),
				log.LocalTrace != LocalTraceMode.Global ?
					log.LocalTrace == LocalTraceMode.On :
					m_sessionConfig.GlobalTrace,
				log.Name,
				log.ConfigEntryId,
				level,
				messageType,
				text,
				report,
				reportExtension,
				bytes,
				bytesExtension,
				value,
				valueLowerBound,
				valueUpperBound,
				ex,
				parameterNames,
				parameterValues,
				obj,
				formatterName,
				formattedText);

			// Push message onto queue and return Id of create message
			m_thread.PushMessage(logMsg);
			return logMsg.Id;
		}

		/// <summary>
		/// Gets the ILog that is associated with the specified name. If no log
		/// yet exists then the manager will create a new instance
		/// </summary>
		/// <param name="logName">
		/// A string that specifies the name of a log.<br/>
		/// Please note name must be treated as case-insensitive!
		/// </param>
		/// <returns>
		/// An ILog to either an already existing or a newly created one.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if an invalid logname is specified
		/// </exception>
		public virtual ILog GetLog(string logName)
		{
			// Check if an empty name is specified
			if (string.IsNullOrEmpty(logName))
				throw new ArgumentException("Invalid log name specified; name cannot be empty");

			// Check if name contains invalid filename characters
			if (logName.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }) >= 0)
				throw new ArgumentException("Invalid log name specified; name cannot contain characters \\ / : * ? \" < > |");

			// Check if name equals reserver logmanager name
			if (logName == LOGNAME_LOGMANAGER)
				throw new ArgumentException(string.Format(
					"Invalid log name specified; '{0}' is reserved for internal use",
					logName));

			// Try to get log
			lock (SyncRoot)
			{
				// Declare variables
				ILog log = null;

				// Is requested log already created?
				if (!m_dicLogs.TryGetValue(logName, out log))
				{
					// Create a new log instance
					log = CreateLog(logName, m_sessionConfig ?? Config);

					// Log Message
					m_sessionLog.AddParams(
						LogLevel.Debug,
						() => "GetLog",
						() => new string[] { "logName" },
						() => new string[] { logName });

					// Register new entry in dictionary and clear cache
					m_dicLogs.Add(logName, log);
					m_logs = null;
				}

				// Raise event
				OnLogsChanged();

				// Return log reference
				return log;
			}
		}

		/// <summary>
		/// Gets the ILog that is associated with the fully qualified name of the
		/// specified type. If no log yet exists then the manager will create 
		/// a new instance
		/// </summary>
		/// <param name="logType">
		/// A Type whose full name will be used to either lookup an exisiting log or,
		/// in case log does not exist, create a new ILog instance.
		/// </param>
		/// <returns>
		/// An ILog to either an already existing or a newly created one.
		/// </returns>
		public virtual ILog GetLog(Type logType)
		{
			// Delegate call
			return GetLog(logType.FullName);
		}

		/// <summary>
		/// Gets a SessionModes enum value that indicates current state of session
		/// </summary>
		public virtual SessionModes SessionMode
		{
			get { return m_sessionMode; }
		}

		/// <summary>
		/// Gets a Guid that uniquely identifies the current session.
		/// </summary>
		public virtual Guid SessionGuid
		{
			get { return m_sessionGuid; }
		}

		/// <summary>
		/// Gets a string that specifies the context of the current session.<br/>
		/// Typically the context represents the name of the executable.
		/// </summary>
		public virtual string SessionContext
		{
			get { return m_sessionContext; }
		}

		/// <summary>
		/// Gets a string that holds the current session folder
		/// </summary>
		public virtual string SessionFolder
		{
			get { return m_sessionFolder; }
		}

		/// <summary>
		/// Gets a string that holds the current session folder (i.e. location where
		/// current log(s) will be persisted) including process context name
		/// </summary>
		public virtual string SessionFolderFullName
		{
			get { return m_sessionFolderFullName; }
		}

		/// <summary>
		/// Gets an array that holds all the monitor instances that are bound
		/// to the current logging session
		/// </summary>
		public virtual ILogMonitor[] SessionMonitors
		{
			get { return m_monitors; }
		}

		/// <summary>
		/// Gets an ILogConfig reference
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Thrown when a null reference is specified
		/// </exception>
		public virtual ILogConfig Config
		{
			get { return m_config; }
			set
			{
				// Check input
				if (value == null)
					throw new ArgumentNullException("Invalid Config specified; <null> not allowed");

				// Store config reference
				m_config = value;
			}
		}

		/// <summary>
		/// Gets or sets a string that specifies a subfolder within the main application 
		/// logging folder where all the logs per session will be stored
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Thrown when an empty or blank string is specified<br/>
		/// <br/>
		/// - Or -<br/>
		/// <br/>
		/// Thrown when the specified value contains an illegal file name character
		/// such as\ / : * ? " &lt; &gt; |
		/// </exception>
		public virtual string Context
		{
			get { return m_context; }
			set
			{
				// Check input
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException("Context cannot be empty");
				if (value.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '\"', '<', '>', '|' }) >= 0)
					throw new ArgumentException(string.Format(
						@"Context '{0}' cannot contain illegal file name characters such as: " +
						@"\ / : * ? "" < > |"));

				// Store new value
				m_context = value;
			}
		}

		/// <summary>
		/// Gets an array reference to all the currently created ILog instances
		/// </summary>
		public virtual ILog[] Logs
		{
			get
			{
				// Check if we already cache
				if (m_logs != null)
					return m_logs;

				lock (SyncRoot)
				{
					// Is cache already created by other thread?
					if (m_logs == null)
					{
						// Create new cache
						m_logs = new ILog[m_dicLogs.Count];
						m_dicLogs.Values.CopyTo(m_logs, 0);
					}
					return m_logs;
				}
			}
		}

		/// <summary>
		/// Gets an object reference that can be used to get synchronized access to the manager
		/// </summary>
		public virtual object SyncRoot
		{
			get { return m_syncRoot; }
		}

		#endregion
	}
}
