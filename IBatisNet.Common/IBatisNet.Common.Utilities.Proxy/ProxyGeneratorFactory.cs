using System;
using Castle.DynamicProxy;
using IBatisNet.Common.Logging;

namespace IBatisNet.Common.Utilities.Proxy
{
	/// <summary>
	///  A Factory for getting the ProxyGenerator.
	/// </summary>
	[CLSCompliant(false)]
	public sealed class ProxyGeneratorFactory
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ProxyGeneratorFactory));

		private static ProxyGenerator _generator = (ProxyGenerator)(object)new CachedProxyGenerator();

		private ProxyGeneratorFactory()
		{
		}

		/// <summary></summary>
		public static ProxyGenerator GetProxyGenerator()
		{
			return _generator;
		}
	}
}
