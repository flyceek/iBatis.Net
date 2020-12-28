using System;
using System.Configuration;
using IBatisNet.Common.Logging.Impl;

namespace IBatisNet.Common.Logging
{
	/// <summary>
	/// Uses the specified <see cref="T:IBatisNet.Common.Logging.ILoggerFactoryAdapter" /> to create <see cref="T:IBatisNet.Common.Logging.ILog" /> instances
	/// that are used to log messages. Inspired by log4net.
	/// </summary>
	public sealed class LogManager
	{
		private static ILoggerFactoryAdapter _adapter = null;

		private static object _loadLock = new object();

		private static readonly string IBATIS_SECTION_LOGGING = "iBATIS/logging";

		/// <summary>
		/// Gets or sets the adapter.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The IBatisNet.Common assembly ships with the following built-in <see cref="T:IBatisNet.Common.Logging.ILoggerFactoryAdapter" /> implementations:
		/// </para>
		/// <list type="table">
		/// <item><term><see cref="T:IBatisNet.Common.Logging.Impl.ConsoleOutLoggerFA" /></term><description>Writes output to Console.Out</description></item>
		/// <item><term><see cref="T:IBatisNet.Common.Logging.Impl.TraceLoggerFA" /></term><description>Writes output to the System.Diagnostics.Trace sub-system</description></item>
		/// <item><term><see cref="T:IBatisNet.Common.Logging.Impl.NoOpLoggerFA" /></term><description>Ignores all messages</description></item>
		/// </list>
		/// </remarks>
		/// <value>The adapter.</value>
		public static ILoggerFactoryAdapter Adapter
		{
			get
			{
				if (_adapter == null)
				{
					lock (_loadLock)
					{
						if (_adapter == null)
						{
							_adapter = BuildLoggerFactoryAdapter();
						}
					}
				}
				return _adapter;
			}
			set
			{
				lock (_loadLock)
				{
					_adapter = value;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Logging.LogManager" /> class. 
		/// </summary>
		/// <remarks>
		/// Uses a private access modifier to prevent instantiation of this class.
		/// </remarks>
		private LogManager()
		{
		}

		/// <summary>
		/// Gets the logger.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static ILog GetLogger(Type type)
		{
			return Adapter.GetLogger(type);
		}

		/// <summary>
		/// Gets the logger.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static ILog GetLogger(string name)
		{
			return Adapter.GetLogger(name);
		}

		/// <summary>
		/// Builds the logger factory adapter.
		/// </summary>
		/// <returns></returns>
		private static ILoggerFactoryAdapter BuildLoggerFactoryAdapter()
		{
			LogSetting logSetting = null;
			ILoggerFactoryAdapter loggerFactoryAdapter;
			ILog logger;
			try
			{
				logSetting = (LogSetting)ConfigurationManager.GetSection(IBATIS_SECTION_LOGGING);
			}
			catch (Exception exception)
			{
				loggerFactoryAdapter = BuildDefaultLoggerFactoryAdapter();
				logger = loggerFactoryAdapter.GetLogger(typeof(LogManager));
				logger.Warn("Unable to read configuration. Using default logger.", exception);
				return loggerFactoryAdapter;
			}
			if (logSetting != null && !typeof(ILoggerFactoryAdapter).IsAssignableFrom(logSetting.FactoryAdapterType))
			{
				loggerFactoryAdapter = BuildDefaultLoggerFactoryAdapter();
				logger = loggerFactoryAdapter.GetLogger(typeof(LogManager));
				logger.Warn("Type " + logSetting.FactoryAdapterType.FullName + " does not implement ILoggerFactoryAdapter. Using default logger");
				return loggerFactoryAdapter;
			}
			ILoggerFactoryAdapter loggerFactoryAdapter2 = null;
			if (logSetting != null)
			{
				if (logSetting.Properties.Count > 0)
				{
					try
					{
						object[] args = new object[1]
						{
							logSetting.Properties
						};
						loggerFactoryAdapter2 = (ILoggerFactoryAdapter)Activator.CreateInstance(logSetting.FactoryAdapterType, args);
					}
					catch (Exception exception)
					{
						loggerFactoryAdapter = BuildDefaultLoggerFactoryAdapter();
						logger = loggerFactoryAdapter.GetLogger(typeof(LogManager));
						logger.Warn("Unable to create instance of type " + logSetting.FactoryAdapterType.FullName + ". Using default logger.", exception);
						return loggerFactoryAdapter;
					}
				}
				else
				{
					try
					{
						loggerFactoryAdapter2 = (ILoggerFactoryAdapter)Activator.CreateInstance(logSetting.FactoryAdapterType);
					}
					catch (Exception exception)
					{
						loggerFactoryAdapter = BuildDefaultLoggerFactoryAdapter();
						logger = loggerFactoryAdapter.GetLogger(typeof(LogManager));
						logger.Warn("Unable to create instance of type " + logSetting.FactoryAdapterType.FullName + ". Using default logger.", exception);
						return loggerFactoryAdapter;
					}
				}
				return loggerFactoryAdapter2;
			}
			loggerFactoryAdapter = BuildDefaultLoggerFactoryAdapter();
			logger = loggerFactoryAdapter.GetLogger(typeof(LogManager));
			logger.Warn("Unable to read configuration IBatisNet/logging. Using default logger.");
			return loggerFactoryAdapter;
		}

		/// <summary>
		/// Builds the default logger factory adapter.
		/// </summary>
		/// <returns></returns>
		private static ILoggerFactoryAdapter BuildDefaultLoggerFactoryAdapter()
		{
			return new NoOpLoggerFA();
		}
	}
}
