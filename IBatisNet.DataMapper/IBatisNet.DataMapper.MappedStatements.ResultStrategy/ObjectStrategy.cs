using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public sealed class ObjectStrategy : IResultStrategy
	{
		public object Process(RequestScope request, ref IDataReader reader, object resultObject)
		{
			object result = resultObject;
			if (reader.FieldCount == 1)
			{
				result = new ResultProperty
				{
					PropertyName = "value",
					ColumnIndex = 0,
					TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(reader.GetFieldType(0))
				}.GetDataBaseValue(reader);
			}
			else if (reader.FieldCount > 1)
			{
				object[] array = new object[reader.FieldCount];
				int fieldCount = reader.FieldCount;
				for (int i = 0; i < fieldCount; i++)
				{
					array[i] = new ResultProperty
					{
						PropertyName = "value",
						ColumnIndex = i,
						TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(reader.GetFieldType(i))
					}.GetDataBaseValue(reader);
				}
				result = array;
			}
			return result;
		}
	}
}
