using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.MappedStatements.PropertStrategy;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
	public sealed class PropertyStrategyFactory
	{
		private static IPropertyStrategy _defaultStrategy;

		private static IPropertyStrategy _resultMapStrategy;

		private static IPropertyStrategy _groupByStrategy;

		private static IPropertyStrategy _selectArrayStrategy;

		private static IPropertyStrategy _selectGenericListStrategy;

		private static IPropertyStrategy _selectListStrategy;

		private static IPropertyStrategy _selectObjectStrategy;

		static PropertyStrategyFactory()
		{
			PropertyStrategyFactory._defaultStrategy = null;
			PropertyStrategyFactory._resultMapStrategy = null;
			PropertyStrategyFactory._groupByStrategy = null;
			PropertyStrategyFactory._selectArrayStrategy = null;
			PropertyStrategyFactory._selectGenericListStrategy = null;
			PropertyStrategyFactory._selectListStrategy = null;
			PropertyStrategyFactory._selectObjectStrategy = null;
			PropertyStrategyFactory._defaultStrategy = new DefaultStrategy();
			PropertyStrategyFactory._resultMapStrategy = new ResultMapStrategy();
			PropertyStrategyFactory._groupByStrategy = new GroupByStrategy();
			PropertyStrategyFactory._selectArrayStrategy = new SelectArrayStrategy();
			PropertyStrategyFactory._selectListStrategy = new SelectListStrategy();
			PropertyStrategyFactory._selectObjectStrategy = new SelectObjectStrategy();
			PropertyStrategyFactory._selectGenericListStrategy = new SelectGenericListStrategy();
		}

		public static IPropertyStrategy Get(ResultProperty mapping)
		{
			IPropertyStrategy result;
			if (mapping.Select.Length == 0 && mapping.NestedResultMap == null)
			{
				result = PropertyStrategyFactory._defaultStrategy;
			}
			else if (mapping.NestedResultMap != null)
			{
				if (mapping.NestedResultMap.GroupByPropertyNames.Count > 0)
				{
					result = PropertyStrategyFactory._groupByStrategy;
				}
				else if (mapping.MemberType.IsGenericType && typeof(System.Collections.Generic.IList<>).IsAssignableFrom(mapping.MemberType.GetGenericTypeDefinition()))
				{
					result = PropertyStrategyFactory._groupByStrategy;
				}
				else if (typeof(System.Collections.IList).IsAssignableFrom(mapping.MemberType))
				{
					result = PropertyStrategyFactory._groupByStrategy;
				}
				else
				{
					result = PropertyStrategyFactory._resultMapStrategy;
				}
			}
			else
			{
				result = new SelectStrategy(mapping, PropertyStrategyFactory._selectArrayStrategy, PropertyStrategyFactory._selectGenericListStrategy, PropertyStrategyFactory._selectListStrategy, PropertyStrategyFactory._selectObjectStrategy);
			}
			return result;
		}
	}
}
