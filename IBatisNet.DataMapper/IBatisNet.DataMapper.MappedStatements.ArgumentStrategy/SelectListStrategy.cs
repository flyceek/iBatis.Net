using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Commands;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
	public sealed class SelectListStrategy : IArgumentStrategy
	{
		public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
		{
			IMappedStatement mappedStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);
			object result;
			if (mapping.MemberType == typeof(System.Collections.IList))
			{
				reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
				result = mappedStatement.ExecuteQueryForList(request.Session, keys);
			}
			else
			{
				reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
				IFactory factory = request.DataExchangeFactory.ObjectFactory.CreateFactory(mapping.MemberType, System.Type.EmptyTypes);
				object obj = factory.CreateInstance(null);
				mappedStatement.ExecuteQueryForList(request.Session, keys, (System.Collections.IList)obj);
				result = obj;
			}
			return result;
		}
	}
}
