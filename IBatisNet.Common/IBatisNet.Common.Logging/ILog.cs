using System;

namespace IBatisNet.Common.Logging
{
	/// <summary>
	/// A simple logging interface abstracting logging APIs. Inspired by log4net.
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// Checks if this logger is enabled for the <see cref="F:IBatisNet.Common.Logging.LogLevel.Debug" /> level.
		/// </summary>
		bool IsDebugEnabled
		{
			get;
		}

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="F:IBatisNet.Common.Logging.LogLevel.Error" /> level.
		/// </summary>
		bool IsErrorEnabled
		{
			get;
		}

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="F:IBatisNet.Common.Logging.LogLevel.Fatal" /> level.
		/// </summary>
		bool IsFatalEnabled
		{
			get;
		}

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="F:IBatisNet.Common.Logging.LogLevel.Info" /> level.
		/// </summary>
		bool IsInfoEnabled
		{
			get;
		}

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="F:IBatisNet.Common.Logging.LogLevel.Warn" /> level.
		/// </summary>
		bool IsWarnEnabled
		{
			get;
		}

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Debug" /> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Debug(object message);

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Debug" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		void Debug(object message, Exception exception);

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Error" /> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Error(object message);

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Error" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		void Error(object message, Exception exception);

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Fatal" /> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Fatal(object message);

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Fatal" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		void Fatal(object message, Exception exception);

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Info" /> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Info(object message);

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Info" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		void Info(object message, Exception exception);

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Warn" /> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Warn(object message);

		/// <summary>
		/// Log a message object with the <see cref="F:IBatisNet.Common.Logging.LogLevel.Warn" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		void Warn(object message, Exception exception);
	}
}
