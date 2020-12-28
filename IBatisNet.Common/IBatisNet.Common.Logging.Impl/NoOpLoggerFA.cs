using System;
using System.Collections.Specialized;

namespace IBatisNet.Common.Logging.Impl
{
	/// <summary>
	/// Factory for creating "no operation" loggers that do nothing and whose Is*Enabled properties always 
	/// return false.
	/// </summary>
	/// <remarks>
	/// This factory creates a single instance of <see cref="T:IBatisNet.Common.Logging.Impl.NoOpLogger" /> and always returns that 
	/// instance whenever an <see cref="T:IBatisNet.Common.Logging.ILog" /> instance is requested.
	/// </remarks>
	public sealed class NoOpLoggerFA : ILoggerFactoryAdapter
	{
		private ILog _nopLogger = new NoOpLogger();

		/// <summary>
		/// Constructor
		/// </summary>
		public NoOpLoggerFA()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public NoOpLoggerFA(NameValueCollection properties)
		{
		}

		/// <summary>
		/// Get a ILog instance by type 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ILog GetLogger(Type type)
		{
			return _nopLogger;
		}

		/// <summary>
		/// Get a ILog instance by type name 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		ILog ILoggerFactoryAdapter.GetLogger(string name)
		{
			return _nopLogger;
		}
	}
}
