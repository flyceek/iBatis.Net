using System;
using System.Collections;
using System.Collections.Specialized;

namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
	public sealed class PostSelectStrategyFactory
	{
		private static System.Collections.IDictionary _strategies;

		static PostSelectStrategyFactory()
		{
			PostSelectStrategyFactory._strategies = new HybridDictionary();
			PostSelectStrategyFactory._strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForArrayList, new ArrayStrategy());
			PostSelectStrategyFactory._strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForIList, new ListStrategy());
			PostSelectStrategyFactory._strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForObject, new ObjectStrategy());
			PostSelectStrategyFactory._strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForStrongTypedIList, new StrongTypedListStrategy());
			PostSelectStrategyFactory._strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForGenericIList, new GenericListStrategy());
		}

		public static IPostSelectStrategy Get(PostBindind.ExecuteMethod method)
		{
			return (IPostSelectStrategy)PostSelectStrategyFactory._strategies[method];
		}
	}
}
