using IBatisNet.DataMapper.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace IBatisNet.DataMapper.Proxy
{
	public class LazyFactoryBuilder
	{
		private System.Collections.IDictionary _factory = new HybridDictionary();

		public LazyFactoryBuilder()
		{
			this._factory[typeof(System.Collections.IList)] = new LazyListFactory();
			this._factory[typeof(System.Collections.Generic.IList<>)] = new LazyListGenericFactory();
		}

		public void Register(System.Type type, string memberName, ILazyFactory factory)
		{
		}

		public ILazyFactory GetLazyFactory(System.Type type)
		{
			ILazyFactory result;
			if (type.IsInterface)
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IList<>))
				{
					result = (this._factory[type.GetGenericTypeDefinition()] as ILazyFactory);
				}
				else
				{
					if (!(type == typeof(System.Collections.IList)))
					{
						throw new DataMapperException("Cannot proxy others interfaces than IList or IList<>.");
					}
					result = (this._factory[type] as ILazyFactory);
				}
			}
			else
			{
				result = new LazyLoadProxyFactory();
			}
			return result;
		}
	}
}
