using System;
using System.Collections;
using System.Collections.Specialized;

namespace IBatisNet.Common.Logging.Impl
{
	/// <summary>
	/// Summary description for  TraceLoggerFA.
	/// </summary>
	public class TraceLoggerFA : ILoggerFactoryAdapter
	{
		private Hashtable _logs = Hashtable.Synchronized(new Hashtable());

		private LogLevel _Level = LogLevel.All;

		private bool _showDateTime = true;

		private bool _showLogName = true;

		private string _dateTimeFormat = string.Empty;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="properties"></param>
		public TraceLoggerFA(NameValueCollection properties)
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
		/// Get a ILog instance by type 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ILog GetLogger(Type type)
		{
			return GetLogger(type.FullName);
		}

		/// <summary>
		/// Get a ILog instance by type name 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ILog GetLogger(string name)
		{
			ILog log = _logs[name] as ILog;
			if (log == null)
			{
				log = new TraceLogger(name, _Level, _showDateTime, _showLogName, _dateTimeFormat);
				_logs.Add(name, log);
			}
			return log;
		}
	}
}
