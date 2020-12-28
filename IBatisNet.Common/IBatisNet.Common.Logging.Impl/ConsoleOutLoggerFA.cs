using System;
using System.Collections;
using System.Collections.Specialized;

namespace IBatisNet.Common.Logging.Impl
{
	/// <summary>
	/// Factory for creating <see cref="T:IBatisNet.Common.Logging.ILog" /> instances that write data to <see cref="P:System.Console.Out" />.
	/// </summary>
	public class ConsoleOutLoggerFA : ILoggerFactoryAdapter
	{
		private Hashtable _logs = Hashtable.Synchronized(new Hashtable());

		private LogLevel _Level = LogLevel.All;

		private bool _showDateTime = true;

		private bool _showLogName = true;

		private string _dateTimeFormat = string.Empty;

		/// <summary>
		/// Looks for level, showDateTime, showLogName, dateTimeFormat items from 
		/// <paramref name="properties" /> for use when the GetLogger methods are called.
		/// </summary>
		/// <param name="properties">Contains user supplied configuration information.</param>
		public ConsoleOutLoggerFA(NameValueCollection properties)
		{
			try
			{
				_Level = (LogLevel)Enum.Parse(typeof(LogLevel), properties["level"], ignoreCase: true);
			}
			catch (Exception)
			{
				_Level = LogLevel.All;
			}
			try
			{
				_showDateTime = bool.Parse(properties["showDateTime"]);
			}
			catch (Exception)
			{
				_showDateTime = true;
			}
			try
			{
				_showLogName = bool.Parse(properties["showLogName"]);
			}
			catch (Exception)
			{
				_showLogName = true;
			}
			_dateTimeFormat = properties["dateTimeFormat"];
		}

		/// <summary>
		/// Get a ILog instance by <see cref="T:System.Type" />.
		/// </summary>
		/// <param name="type">Usually the <see cref="T:System.Type" /> of the current class.</param>
		/// <returns>An ILog instance that will write data to <see cref="P:System.Console.Out" />.</returns>
		public ILog GetLogger(Type type)
		{
			return GetLogger(type.FullName);
		}

		/// <summary>
		/// Get a ILog instance by name.
		/// </summary>
		/// <param name="name">Usually a <see cref="T:System.Type" />'s Name or FullName property.</param>
		/// <returns>An ILog instance that will write data to <see cref="P:System.Console.Out" />.</returns>
		public ILog GetLogger(string name)
		{
			ILog log = _logs[name] as ILog;
			if (log == null)
			{
				log = new ConsoleOutLogger(name, _Level, _showDateTime, _showLogName, _dateTimeFormat);
				_logs.Add(name, log);
			}
			return log;
		}
	}
}
