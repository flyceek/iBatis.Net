using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public sealed class SimpleTypeStrategy : IResultStrategy
	{
		public object Process(RequestScope request, ref IDataReader reader, object resultObject)
		{
			object obj = resultObject;
			AutoResultMap autoResultMap = request.CurrentResultMap as AutoResultMap;
			if (obj == null)
			{
				obj = autoResultMap.CreateInstanceOfResultClass();
			}
			if (!autoResultMap.IsInitalized)
			{
				lock (autoResultMap)
				{
					if (!autoResultMap.IsInitalized)
					{
						ResultProperty resultProperty = new ResultProperty();
						resultProperty.PropertyName = "value";
						resultProperty.ColumnIndex = 0;
						resultProperty.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(obj.GetType());
						resultProperty.PropertyStrategy = PropertyStrategyFactory.Get(resultProperty);
						autoResultMap.Properties.Add(resultProperty);
						autoResultMap.DataExchange = request.DataExchangeFactory.GetDataExchangeForClass(typeof(int));
						autoResultMap.IsInitalized = true;
					}
				}
			}
			autoResultMap.Properties[0].PropertyStrategy.Set(request, autoResultMap, autoResultMap.Properties[0], ref obj, reader, null);
			return obj;
		}
	}
}
