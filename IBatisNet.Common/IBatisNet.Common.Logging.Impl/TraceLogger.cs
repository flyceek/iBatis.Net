#define TRACE
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace IBatisNet.Common.Logging.Impl
{
	/// <summary>
	/// Logger that sends output to the <see cref="T:System.Diagnostics.Trace" /> sub-system.
	/// </summary>
	public class TraceLogger : AbstractLogger
	{
		private bool _showDateTime = false;

		private bool _showLogName = false;

		private string _logName = string.Empty;

		private LogLevel _currentLogLevel = LogLevel.All;

		private string _dateTimeFormat = string.Empty;

		private bool _hasDateTimeFormat = false;

		/// <summary>
		/// Creates a new instance of the TraceLogger.
		/// </summary>
		/// <param name="logName">The name for this instance (usually the fully qualified class name).</param>
		/// <param name="logLevel">
		/// The logging threshold. Log messages created with a <see cref="T:IBatisNet.Common.Logging.LogLevel" />
		/// beneath this threshold will not be logged.
		/// </param>
		/// <param name="showDateTime">Include the current time in the log message </param>
		/// <param name="showLogName">Include the instance name in the log message</param>
		/// <param name="dateTimeFormat">The date and time format to use in the log message </param>
		public TraceLogger(string logName, LogLevel logLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
		{
			_logName = logName;
			_currentLogLevel = logLevel;
			_showDateTime = showDateTime;
			_showLogName = showLogName;
			_dateTimeFormat = dateTimeFormat;
			if (_dateTimeFormat != null && _dateTimeFormat.Length > 0)
			{
				_hasDateTimeFormat = true;
			}
		}

		/// <summary>
		/// Responsible for assembling and writing the log message to the tracing sub-system.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="e"></param>
		protected override void Write(LogLevel level, object message, Exception e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (_showDateTime)
			{
				if (_hasDateTimeFormat)
				{
					stringBuilder.Append(DateTime.Now.ToString(_dateTimeFormat, CultureInfo.InvariantCulture));
				}
				else
				{
					stringBuilder.Append(DateTime.Now);
				}
				stringBuilder.Append(" ");
			}
			stringBuilder.Append($"[{level.ToString().ToUpper()}]".PadRight(8));
			if (_showLogName)
			{
				stringBuilder.Append(_logName).Append(" - ");
			}
			stringBuilder.Append(message.ToString());
			if (e != null)
			{
				stringBuilder.Append(Environment.NewLine).Append(e.ToString());
			}
			Trace.WriteLine(stringBuilder.ToString());
		}

		/// <summary>
		/// Is the given log level currently enabled ?
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		protected override bool IsLevelEnabled(LogLevel level)
		{
			int currentLogLevel = (int)_currentLogLevel;
			return (int)level >= currentLogLevel;
		}
	}
}
