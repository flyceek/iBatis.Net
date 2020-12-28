using System;
using System.Globalization;
using System.Text;

namespace IBatisNet.Common.Logging.Impl
{
	/// <summary>
	/// Sends log messages to <see cref="P:System.Console.Out" />.
	/// </summary>
	public class ConsoleOutLogger : AbstractLogger
	{
		private bool _showDateTime = false;

		private bool _showLogName = false;

		private string _logName = string.Empty;

		private LogLevel _currentLogLevel = LogLevel.All;

		private string _dateTimeFormat = string.Empty;

		private bool _hasDateTimeFormat = false;

		/// <summary>
		/// Creates and initializes a logger that writes messages to <see cref="P:System.Console.Out" />.
		/// </summary>
		/// <param name="logName">The name, usually type name of the calling class, of the logger.</param>
		/// <param name="logLevel">The current logging threshold. Messages recieved that are beneath this threshold will not be logged.</param>
		/// <param name="showDateTime">Include the current time in the log message.</param>
		/// <param name="showLogName">Include the instance name in the log message.</param>
		/// <param name="dateTimeFormat">The date and time format to use in the log message.</param>
		public ConsoleOutLogger(string logName, LogLevel logLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
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
		/// Do the actual logging by constructing the log message using a <see cref="T:System.Text.StringBuilder" /> then
		/// sending the output to <see cref="P:System.Console.Out" />.
		/// </summary>
		/// <param name="level">The <see cref="T:IBatisNet.Common.Logging.LogLevel" /> of the message.</param>
		/// <param name="message">The log message.</param>
		/// <param name="e">An optional <see cref="T:System.Exception" /> associated with the message.</param>
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
			Console.Out.WriteLine(stringBuilder.ToString());
		}

		/// <summary>
		/// Determines if the given log level is currently enabled.
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
