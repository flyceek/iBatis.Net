using IBatisNet.DataMapper.Commands;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
	public sealed class SelectArrayStrategy : IArgumentStrategy
	{
		public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
		{
			IMappedStatement mappedStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);
			reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
			System.Collections.IList list = mappedStatement.ExecuteQueryForList(request.Session, keys);
			System.Type elementType = mapping.MemberType.GetElementType();
			System.Array array = System.Array.CreateInstance(elementType, list.Count);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				array.SetValue(list[i], i);
			}
			return array;
		}
	}
}
