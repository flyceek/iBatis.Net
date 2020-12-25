using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public sealed class AutoMapStrategy : IResultStrategy
	{
		private AutoResultMap InitializeAutoResultMap(RequestScope request, ref IDataReader reader, ref object resultObject)
		{
			AutoResultMap autoResultMap = request.CurrentResultMap as AutoResultMap;
			if (request.Statement.AllowRemapping)
			{
				autoResultMap = autoResultMap.Clone();
				ResultPropertyCollection value = ReaderAutoMapper.Build(request.DataExchangeFactory, reader, ref resultObject);
				autoResultMap.Properties.AddRange(value);
			}
			else if (!autoResultMap.IsInitalized)
			{
				lock (autoResultMap)
				{
					if (!autoResultMap.IsInitalized)
					{
						ResultPropertyCollection value = ReaderAutoMapper.Build(request.DataExchangeFactory, reader, ref resultObject);
						autoResultMap.Properties.AddRange(value);
						autoResultMap.IsInitalized = true;
					}
				}
			}
			return autoResultMap;
		}

		public object Process(RequestScope request, ref IDataReader reader, object resultObject)
		{
			object obj = resultObject;
			if (obj == null)
			{
				obj = (request.CurrentResultMap as AutoResultMap).CreateInstanceOfResultClass();
			}
			AutoResultMap autoResultMap = this.InitializeAutoResultMap(request, ref reader, ref obj);
			for (int i = 0; i < autoResultMap.Properties.Count; i++)
			{
				ResultProperty resultProperty = autoResultMap.Properties[i];
				autoResultMap.SetValueOfProperty(ref obj, resultProperty, resultProperty.GetDataBaseValue(reader));
			}
			return obj;
		}
	}
}
