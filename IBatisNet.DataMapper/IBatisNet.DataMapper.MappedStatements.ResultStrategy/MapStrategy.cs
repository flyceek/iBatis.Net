using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public sealed class MapStrategy : IResultStrategy
	{
		private static IResultStrategy _resultMapStrategy;

		private static IResultStrategy _groupByStrategy;

		static MapStrategy()
		{
			MapStrategy._resultMapStrategy = null;
			MapStrategy._groupByStrategy = null;
			MapStrategy._resultMapStrategy = new ResultMapStrategy();
			MapStrategy._groupByStrategy = new GroupByStrategy();
		}

		public object Process(RequestScope request, ref IDataReader reader, object resultObject)
		{
			IResultMap resultMap = request.CurrentResultMap.ResolveSubMap(reader);
			object result;
			if (resultMap.GroupByPropertyNames.Count > 0)
			{
				result = MapStrategy._groupByStrategy.Process(request, ref reader, resultObject);
			}
			else
			{
				result = MapStrategy._resultMapStrategy.Process(request, ref reader, resultObject);
			}
			return result;
		}
	}
}
