using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Configuration.Statements;
using System;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public sealed class ResultStrategyFactory
	{
		private static IResultStrategy _resultClassStrategy;

		private static IResultStrategy _mapStrategy;

		private static IResultStrategy _objectStrategy;

		static ResultStrategyFactory()
		{
			ResultStrategyFactory._resultClassStrategy = null;
			ResultStrategyFactory._mapStrategy = null;
			ResultStrategyFactory._objectStrategy = null;
			ResultStrategyFactory._mapStrategy = new MapStrategy();
			ResultStrategyFactory._resultClassStrategy = new ResultClassStrategy();
			ResultStrategyFactory._objectStrategy = new ObjectStrategy();
		}

		public static IResultStrategy Get(IStatement statement)
		{
			IResultStrategy result;
			if (statement.ResultsMap.Count > 0)
			{
				if (statement.ResultsMap[0] is ResultMap)
				{
					result = ResultStrategyFactory._mapStrategy;
				}
				else
				{
					result = ResultStrategyFactory._resultClassStrategy;
				}
			}
			else
			{
				result = ResultStrategyFactory._objectStrategy;
			}
			return result;
		}
	}
}
