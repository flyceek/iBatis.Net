using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public sealed class ResultClassStrategy : IResultStrategy
	{
		private static IResultStrategy _simpleTypeStrategy = null;

		private static IResultStrategy _dictionaryStrategy = null;

		private static IResultStrategy _listStrategy = null;

		private static IResultStrategy _autoMapStrategy = null;

		public ResultClassStrategy()
		{
			ResultClassStrategy._simpleTypeStrategy = new SimpleTypeStrategy();
			ResultClassStrategy._dictionaryStrategy = new DictionaryStrategy();
			ResultClassStrategy._listStrategy = new ListStrategy();
			ResultClassStrategy._autoMapStrategy = new AutoMapStrategy();
		}

		public object Process(RequestScope request, ref IDataReader reader, object resultObject)
		{
			object result;
			if (request.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(request.CurrentResultMap.Class))
			{
				result = ResultClassStrategy._simpleTypeStrategy.Process(request, ref reader, resultObject);
			}
			else if (typeof(System.Collections.IDictionary).IsAssignableFrom(request.CurrentResultMap.Class))
			{
				result = ResultClassStrategy._dictionaryStrategy.Process(request, ref reader, resultObject);
			}
			else if (typeof(System.Collections.IList).IsAssignableFrom(request.CurrentResultMap.Class))
			{
				result = ResultClassStrategy._listStrategy.Process(request, ref reader, resultObject);
			}
			else
			{
				result = ResultClassStrategy._autoMapStrategy.Process(request, ref reader, resultObject);
			}
			return result;
		}
	}
}
