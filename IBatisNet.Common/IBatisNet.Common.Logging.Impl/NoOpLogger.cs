using System;

namespace IBatisNet.Common.Logging.Impl
{
	/// <summary>
	/// Silently ignores all log messages.
	/// </summary>
	public sealed class NoOpLogger : ILog
	{
		/// <summary>
		/// Always returns <see langword="false" />.
		/// </summary>
		public bool IsDebugEnabled => false;

		/// <summary>
		/// Always returns <see langword="false" />.
		/// </summary>
		public bool IsErrorEnabled => false;

		/// <summary>
		/// Always returns <see langword="false" />.
		/// </summary>
		public bool IsFatalEnabled => false;

		/// <summary>
		/// Always returns <see langword="false" />.
		/// </summary>
		public bool IsInfoEnabled => false;

		/// <summary>
		/// Always returns <see langword="false" />.
		/// </summary>
		public bool IsWarnEnabled => false;

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		public void Debug(object message)
		{
		}

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public void Debug(object message, Exception e)
		{
		}

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		public void Error(object message)
		{
		}

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public void Error(object message, Exception e)
		{
		}

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		public void Fatal(object message)
		{
		}

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public void Fatal(object message, Exception e)
		{
		}

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		public void Info(object message)
		{
		}

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public void Info(object message, Exception e)
		{
		}

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		public void Warn(object message)
		{
		}

		/// <summary>
		/// Ignores message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public void Warn(object message, Exception e)
		{
		}
	}
}
