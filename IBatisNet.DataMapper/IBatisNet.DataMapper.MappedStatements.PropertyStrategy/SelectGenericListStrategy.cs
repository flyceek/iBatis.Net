using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
	public sealed class SelectGenericListStrategy : IPropertyStrategy
	{
		public void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object keys)
		{
			IMappedStatement mappedStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);
			PostBindind postBindind = new PostBindind();
			postBindind.Statement = mappedStatement;
			postBindind.Keys = keys;
			postBindind.Target = target;
			postBindind.ResultProperty = mapping;
			if (mapping.IsLazyLoad)
			{
				object value = mapping.LazyFactory.CreateProxy(mappedStatement, keys, target, mapping.SetAccessor);
				mapping.SetAccessor.Set(target, value);
			}
			else
			{
				if (mapping.SetAccessor.MemberType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IList<>))
				{
					postBindind.Method = PostBindind.ExecuteMethod.ExecuteQueryForGenericIList;
				}
				request.QueueSelect.Enqueue(postBindind);
			}
		}

		public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
		{
			throw new System.NotSupportedException("Get method on ResultMapStrategy is not supported");
		}
	}
}
