using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.PropertStrategy
{
	public sealed class GroupByStrategy : BaseStrategy, IPropertyStrategy
	{
		private static IPropertyStrategy _resultMapStrategy;

		static GroupByStrategy()
		{
			GroupByStrategy._resultMapStrategy = null;
			GroupByStrategy._resultMapStrategy = new ResultMapStrategy();
		}

		public void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object keys)
		{
			this.Get(request, resultMap, mapping, ref target, reader);
		}

		public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
		{
			object obj = ObjectProbe.GetMemberValue(target, mapping.PropertyName, request.DataExchangeFactory.AccessorFactory);
			if (obj == null)
			{
				obj = mapping.ListFactory.CreateInstance(null);
				mapping.SetAccessor.Set(target, obj);
			}
			System.Collections.IList list = (System.Collections.IList)obj;
			object obj2 = null;
			IResultMap resultMap2 = mapping.NestedResultMap.ResolveSubMap(reader);
			if (resultMap2.GroupByProperties.Count > 0)
			{
				string uniqueKey = base.GetUniqueKey(resultMap2, request, reader);
				System.Collections.IDictionary dictionary = request.GetUniqueKeys(resultMap2);
				if (dictionary != null && dictionary.Contains(uniqueKey))
				{
					obj2 = dictionary[uniqueKey];
					if (obj2 != null)
					{
						for (int i = 0; i < resultMap2.Properties.Count; i++)
						{
							ResultProperty resultProperty = resultMap2.Properties[i];
							if (resultProperty.PropertyStrategy is GroupByStrategy)
							{
								resultProperty.PropertyStrategy.Set(request, resultMap2, resultProperty, ref obj2, reader, null);
							}
						}
					}
					obj2 = BaseStrategy.SKIP;
				}
				else if (uniqueKey == null || dictionary == null || !dictionary.Contains(uniqueKey))
				{
					obj2 = GroupByStrategy._resultMapStrategy.Get(request, resultMap, mapping, ref target, reader);
					if (dictionary == null)
					{
						dictionary = new System.Collections.Hashtable();
						request.SetUniqueKeys(resultMap2, dictionary);
					}
					dictionary[uniqueKey] = obj2;
				}
			}
			else
			{
				obj2 = GroupByStrategy._resultMapStrategy.Get(request, resultMap, mapping, ref target, reader);
			}
			if (obj2 != null && obj2 != BaseStrategy.SKIP)
			{
				list.Add(obj2);
			}
			return obj2;
		}
	}
}
