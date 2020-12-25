using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;

namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
	public sealed class ArgumentStrategyFactory
	{
		private static IArgumentStrategy _defaultStrategy;

		private static IArgumentStrategy _resultMapStrategy;

		private static IArgumentStrategy _selectArrayStrategy;

		private static IArgumentStrategy _selectGenericListStrategy;

		private static IArgumentStrategy _selectListStrategy;

		private static IArgumentStrategy _selectObjectStrategy;

		static ArgumentStrategyFactory()
		{
			ArgumentStrategyFactory._defaultStrategy = null;
			ArgumentStrategyFactory._resultMapStrategy = null;
			ArgumentStrategyFactory._selectArrayStrategy = null;
			ArgumentStrategyFactory._selectGenericListStrategy = null;
			ArgumentStrategyFactory._selectListStrategy = null;
			ArgumentStrategyFactory._selectObjectStrategy = null;
			ArgumentStrategyFactory._defaultStrategy = new DefaultStrategy();
			ArgumentStrategyFactory._resultMapStrategy = new ResultMapStrategy();
			ArgumentStrategyFactory._selectArrayStrategy = new SelectArrayStrategy();
			ArgumentStrategyFactory._selectListStrategy = new SelectListStrategy();
			ArgumentStrategyFactory._selectObjectStrategy = new SelectObjectStrategy();
			ArgumentStrategyFactory._selectGenericListStrategy = new SelectGenericListStrategy();
		}

		public static IArgumentStrategy Get(ArgumentProperty mapping)
		{
			IArgumentStrategy result;
			if (mapping.Select.Length == 0 && mapping.NestedResultMap == null)
			{
				result = ArgumentStrategyFactory._defaultStrategy;
			}
			else if (mapping.NestedResultMap != null)
			{
				result = ArgumentStrategyFactory._resultMapStrategy;
			}
			else
			{
				result = new SelectStrategy(mapping, ArgumentStrategyFactory._selectArrayStrategy, ArgumentStrategyFactory._selectGenericListStrategy, ArgumentStrategyFactory._selectListStrategy, ArgumentStrategyFactory._selectObjectStrategy);
			}
			return result;
		}
	}
}
