using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.MappedStatements;
using System;

namespace IBatisNet.DataMapper.Proxy
{
	public class LazyListFactory : ILazyFactory
	{
		public object CreateProxy(IMappedStatement mappedStatement, object param, object target, ISetAccessor setAccessor)
		{
			return new LazyList(mappedStatement, param, target, setAccessor);
		}
	}
}
