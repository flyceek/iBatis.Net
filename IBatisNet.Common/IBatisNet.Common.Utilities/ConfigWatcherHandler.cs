using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using IBatisNet.Common.Logging;

namespace IBatisNet.Common.Utilities
{
	/// <summary>
	/// Class used to watch config files.
	/// </summary>
	/// <remarks>
	/// Uses the <see cref="T:System.IO.FileSystemWatcher" /> to monitor
	/// changes to a specified file. Because multiple change notifications
	/// may be raised when the file is modified, a timer is used to
	/// compress the notifications into a single event. The timer
	/// waits for the specified time before delivering
	/// the event notification. If any further <see cref="T:System.IO.FileSystemWatcher" />
	/// change notifications arrive while the timer is waiting it
	/// is reset and waits again for the specified time to
	/// elapse.
	/// </remarks>
	public sealed class ConfigWatcherHandler
	{
		/// <summary>
		/// The default amount of time to wait after receiving notification
		/// before reloading the config file.
		/// </summary>
		private const int TIMEOUT_MILLISECONDS = 500;

		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// The timer used to compress the notification events.
		/// </summary>
		private Timer _timer = null;

		/// <summary>
		/// A list of configuration files to watch.
		/// </summary>
		private static ArrayList _filesToWatch = new ArrayList();

		/// <summary>
		/// The list of FileSystemWatcher.
		/// </summary>
		private static ArrayList _filesWatcher = new ArrayList();

		/// <summary>
		///             -
		/// </summary>
		/// <param name="state">
		/// Represent the call context of the SqlMap or DaoManager ConfigureAndWatch method call.
		/// </param>
		/// <param name="onWhatchedFileChange"></param>
		public ConfigWatcherHandler(TimerCallback onWhatchedFileChange, StateConfig state)
		{
			for (int i = 0; i < _filesToWatch.Count; i++)
			{
				FileInfo configFile = (FileInfo)_filesToWatch[i];
				AttachWatcher(configFile);
				_timer = new Timer(onWhatchedFileChange, state, -1, -1);
			}
		}

		private void AttachWatcher(FileInfo configFile)
		{
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
			fileSystemWatcher.Path = configFile.DirectoryName;
			fileSystemWatcher.Filter = configFile.Name;
			fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
			fileSystemWatcher.Changed += ConfigWatcherHandler_OnChanged;
			fileSystemWatcher.Created += ConfigWatcherHandler_OnChanged;
			fileSystemWatcher.Deleted += ConfigWatcherHandler_OnChanged;
			fileSystemWatcher.Renamed += ConfigWatcherHandler_OnRenamed;
			fileSystemWatcher.EnableRaisingEvents = true;
			_filesWatcher.Add(fileSystemWatcher);
		}

		/// <summary>
		/// Add a file to be monitored.
		/// </summary>
		/// <param name="configFile"></param>
		public static void AddFileToWatch(FileInfo configFile)
		{
			if (_logger.IsDebugEnabled)
			{
				_logger.Debug("Adding file [" + Path.GetFileName(configFile.FullName) + "] to list of watched files.");
			}
			_filesToWatch.Add(configFile);
		}

		/// <summary>
		/// Reset the list of files being monitored.
		/// </summary>
		public static void ClearFilesMonitored()
		{
			_filesToWatch.Clear();
			for (int i = 0; i < _filesWatcher.Count; i++)
			{
				FileSystemWatcher fileSystemWatcher = (FileSystemWatcher)_filesWatcher[i];
				fileSystemWatcher.EnableRaisingEvents = false;
				fileSystemWatcher.Dispose();
			}
		}

		/// <summary>
		/// Event handler used by <see cref="T:IBatisNet.Common.Utilities.ConfigWatcherHandler" />.
		/// </summary>
		/// <param name="source">The <see cref="T:System.IO.FileSystemWatcher" /> firing the event.</param>
		/// <param name="e">The argument indicates the file that caused the event to be fired.</param>
		/// <remarks>
		/// This handler reloads the configuration from the file when the event is fired.
		/// </remarks>
		private void ConfigWatcherHandler_OnChanged(object source, FileSystemEventArgs e)
		{
			if (_logger.IsDebugEnabled)
			{
				_logger.Debug(string.Concat("ConfigWatcherHandler : ", e.ChangeType, " [", e.Name, "]"));
			}
			_timer.Change(500, -1);
		}

		/// <summary>
		/// Event handler used by <see cref="T:IBatisNet.Common.Utilities.ConfigWatcherHandler" />.
		/// </summary>
		/// <param name="source">The <see cref="T:System.IO.FileSystemWatcher" /> firing the event.</param>
		/// <param name="e">The argument indicates the file that caused the event to be fired.</param>
		/// <remarks>
		/// This handler reloads the configuration from the file when the event is fired.
		/// </remarks>
		private void ConfigWatcherHandler_OnRenamed(object source, RenamedEventArgs e)
		{
			if (_logger.IsDebugEnabled)
			{
				_logger.Debug(string.Concat("ConfigWatcherHandler : ", e.ChangeType, " [", e.OldName, "/", e.Name, "]"));
			}
			_timer.Change(500, -1);
		}
	}
}
