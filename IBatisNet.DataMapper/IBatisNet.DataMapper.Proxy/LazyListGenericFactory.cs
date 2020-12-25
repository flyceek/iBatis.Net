using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.MappedStatements;
using System;

namespace IBatisNet.DataMapper.Proxy
{
	public class LazyListGenericFactory : ILazyFactory
	{
		public object CreateProxy(IMappedStatement mappedStatement, object param, object target, ISetAccessor setAccessor)
		{
			System.Type type = setAccessor.MemberType.GetGenericArguments()[0];
			System.Type typeFromHandle = typeof(LazyListGeneric<>);
			System.Type typeToCreate = typeFromHandle.MakeGenericType(new System.Type[]
			{
				type
			});
			System.Type[] types = new System.Type[]
			{
				typeof(IMappedStatement),
				typeof(object),
				typeof(object),
				typeof(ISetAccessor)
			};
			IFactory factory = mappedStatement.SqlMap.DataExchangeFactory.ObjectFactory.CreateFactory(typeToCreate, types);
			object[] parameters = new object[]
			{
				mappedStatement,
				param,
				target,
				setAccessor
			};
			return factory.CreateInstance(parameters);
		}
	}
}
