using Castle.DynamicProxy;
using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.Common.Utilities.Proxy;
using IBatisNet.DataMapper.MappedStatements;
using System;
using System.Reflection;

namespace IBatisNet.DataMapper.Proxy
{
	public class LazyLoadProxyFactory : ILazyFactory
	{
		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public object CreateProxy(IMappedStatement selectStatement, object param, object target, ISetAccessor setAccessor)
		{
			System.Type memberType = setAccessor.MemberType;
			if (LazyLoadProxyFactory._logger.IsDebugEnabled)
			{
				LazyLoadProxyFactory._logger.Debug(string.Format("Statement '{0}', create proxy for member {1}.", selectStatement.Id, setAccessor.MemberType));
			}
			IInterceptor interceptor = new LazyLoadInterceptor(selectStatement, param, target, setAccessor);
			return ProxyGeneratorFactory.GetProxyGenerator().CreateClassProxy(memberType, interceptor, System.Type.EmptyTypes);
		}
	}
}
