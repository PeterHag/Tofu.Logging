using Tofu.Collections;

namespace Tofu.Logging
{
	public abstract class LogMonitor : ILogMonitor
	{
		#region Constructors

		// ******************************************************************
		// *																*
		// *			              Constructors				            *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Constructor
		/// </summary>
		public LogMonitor()
		{ }

		#endregion

		#region Public Properties

		// ******************************************************************
		// *																*
		// *			             Public Properties			            *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Gets an ILogManager reference to the manager that was passed during
		/// initialization
		/// monitor instance
		/// </summary>
		public virtual ILogManager LogManager
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets an IParameterDictionary instance that holds the parameters 
		/// that were passed during initialization
		/// </summary>
		public virtual IParameterDictionary Parameters
		{
			get;
			protected set;
		}

		#endregion

		#region ILogMonitor Interface

		// ******************************************************************
		// *																*
		// *			           ILogMonitor Interface		            *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Initializes this monitor instance
		/// </summary>
		/// <param name="logManager">
		/// A ILogManager instance that owns this new monitor instance
		/// </param>
		/// <param name="parameters">
		/// An IParameterDictionary instance that holds user defined parameters in the
		/// configuration file
		/// </param>
		public virtual void Init(ILogManager logManager, IParameterDictionary parameters)
		{
			// Store arguments into properties
			LogManager = logManager;
			Parameters = parameters;
		}

		/// <summary>
		/// Called when the associated LogManager starts a new logging session
		/// </summary>
		public abstract void SessionStarted();

		/// <summary>
		/// Called when the associated LogManager suspends the current logging session
		/// </summary>
		public abstract void SessionSuspended();

		/// <summary>
		/// Called when the associated LogManager resumes the current logging session
		/// </summary>
		public abstract void SessionResumed();

		/// <summary>
		/// Called when the associated LogManager stops the current logging session
		/// </summary>
		public abstract void SessionStopped();

		#endregion

		#region IDisposable Interface

		// ******************************************************************
		// *																*
		// *			           IDisposable Interface		            *
		// *																*
		// ******************************************************************

		/// <summary>
		/// Called when this instance is going to be GC'ed and must release 
		/// used resources
		/// </summary>
		public virtual void Dispose()
		{ }

		#endregion
	}
}
