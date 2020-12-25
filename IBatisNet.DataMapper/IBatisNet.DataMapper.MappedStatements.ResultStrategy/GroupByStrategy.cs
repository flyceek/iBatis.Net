using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.MappedStatements.PropertStrategy;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public sealed class GroupByStrategy : BaseStrategy, IResultStrategy
	{
		public object Process(RequestScope request, ref IDataReader reader, object resultObject)
		{
			object obj = resultObject;
			IResultMap resultMap = request.CurrentResultMap.ResolveSubMap(reader);
			string uniqueKey = base.GetUniqueKey(resultMap, request, reader);
			System.Collections.IDictionary dictionary = request.GetUniqueKeys(resultMap);
			if (dictionary != null && dictionary.Contains(uniqueKey))
			{
				obj = dictionary[uniqueKey];
				for (int i = 0; i < resultMap.Properties.Count; i++)
				{
					ResultProperty resultProperty = resultMap.Properties[i];
					if (resultProperty.PropertyStrategy is IBatisNet.DataMapper.MappedStatements.PropertStrategy.GroupByStrategy)
					{
						resultProperty.PropertyStrategy.Set(request, resultMap, resultProperty, ref obj, reader, null);
					}
				}
				obj = BaseStrategy.SKIP;
			}
			else if (uniqueKey == null || dictionary == null || !dictionary.Contains(uniqueKey))
			{
				if (obj == null)
				{
					obj = resultMap.CreateInstanceOfResult(null);
				}
				for (int i = 0; i < resultMap.Properties.Count; i++)
				{
					ResultProperty resultProperty = resultMap.Properties[i];
					resultProperty.PropertyStrategy.Set(request, resultMap, resultProperty, ref obj, reader, null);
				}
				if (dictionary == null)
				{
					dictionary = new System.Collections.Hashtable();
					request.SetUniqueKeys(resultMap, dictionary);
				}
				dictionary[uniqueKey] = obj;
			}
			return obj;
		}
	}
}
