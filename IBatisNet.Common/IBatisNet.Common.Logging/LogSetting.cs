using System;
using System.Collections.Specialized;

namespace IBatisNet.Common.Logging
{
	/// <summary>
	/// Container used to hold configuration information from config file.
	/// </summary>
	internal class LogSetting
	{
		private Type _factoryAdapterType = null;

		private NameValueCollection _properties = null;

		/// <summary>
		/// The <see cref="T:IBatisNet.Common.Logging.ILoggerFactoryAdapter" /> type that will be used for creating <see cref="T:IBatisNet.Common.Logging.ILog" />
		/// instances.
		/// </summary>
		public Type FactoryAdapterType => _factoryAdapterType;

		/// <summary>
		/// Additional user supplied properties that are passed to the <see cref="P:IBatisNet.Common.Logging.LogSetting.FactoryAdapterType" />'s constructor.
		/// </summary>
		public NameValueCollection Properties => _properties;

		/// <summary>
		///
		/// </summary>
		/// <param name="factoryAdapterType">
		/// The <see cref="T:IBatisNet.Common.Logging.ILoggerFactoryAdapter" /> type 
		/// that will be used for creating <see cref="T:IBatisNet.Common.Logging.ILog" />
		/// </param>
		/// <param name="properties">
		/// Additional user supplied properties that are passed to the 
		/// <paramref name="factoryAdapterType" />'s constructor.
		/// </param>
		public LogSetting(Type factoryAdapterType, NameValueCollection properties)
		{
			_factoryAdapterType = factoryAdapterType;
			_properties = properties;
		}
	}
}
