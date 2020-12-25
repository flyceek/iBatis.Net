using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public sealed class ListStrategy : IResultStrategy
	{
		public object Process(RequestScope request, ref IDataReader reader, object resultObject)
		{
			object obj = resultObject;
			AutoResultMap autoResultMap = request.CurrentResultMap as AutoResultMap;
			if (obj == null)
			{
				obj = autoResultMap.CreateInstanceOfResultClass();
			}
			int fieldCount = reader.FieldCount;
			for (int i = 0; i < fieldCount; i++)
			{
				ResultProperty resultProperty = new ResultProperty();
				resultProperty.PropertyName = "value";
				resultProperty.ColumnIndex = i;
				resultProperty.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(reader.GetFieldType(i));
				((System.Collections.IList)obj).Add(resultProperty.GetDataBaseValue(reader));
			}
			return obj;
		}
	}
}
