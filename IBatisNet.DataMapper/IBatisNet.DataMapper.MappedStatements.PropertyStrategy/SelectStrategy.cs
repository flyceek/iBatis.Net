using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
	public class SelectStrategy : IPropertyStrategy
	{
		private IPropertyStrategy _selectStrategy = null;

		public SelectStrategy(ResultProperty mapping, IPropertyStrategy selectArrayStrategy, IPropertyStrategy selectGenericListStrategy, IPropertyStrategy selectListStrategy, IPropertyStrategy selectObjectStrategy)
		{
			if (mapping.SetAccessor.MemberType.BaseType == typeof(System.Array))
			{
				this._selectStrategy = selectArrayStrategy;
			}
			else if (mapping.SetAccessor.MemberType.IsGenericType && typeof(System.Collections.Generic.IList<>).IsAssignableFrom(mapping.SetAccessor.MemberType.GetGenericTypeDefinition()))
			{
				this._selectStrategy = selectGenericListStrategy;
			}
			else if (typeof(System.Collections.IList).IsAssignableFrom(mapping.SetAccessor.MemberType))
			{
				this._selectStrategy = selectListStrategy;
			}
			else
			{
				this._selectStrategy = selectObjectStrategy;
			}
		}

		public void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object selectKeys)
		{
			string columnName = mapping.ColumnName;
			bool flag = false;
			object keys;
			if (columnName.IndexOf(',') > 0 || columnName.IndexOf('=') > 0)
			{
				System.Collections.IDictionary dictionary = new System.Collections.Hashtable();
				keys = dictionary;
				char[] separator = new char[]
				{
					'=',
					','
				};
				string[] array = columnName.Split(separator);
				if (array.Length % 2 != 0)
				{
					throw new DataMapperException("Invalid composite key string format in '" + mapping.PropertyName + ". It must be: property1=column1,property2=column2,...");
				}
				System.Collections.IEnumerator enumerator = array.GetEnumerator();
				while (!flag && enumerator.MoveNext())
				{
					string key = ((string)enumerator.Current).Trim();
					if (columnName.Contains("="))
					{
						enumerator.MoveNext();
					}
					object value = reader.GetValue(reader.GetOrdinal(((string)enumerator.Current).Trim()));
					dictionary.Add(key, value);
					flag = (value == System.DBNull.Value);
				}
			}
			else
			{
				keys = reader.GetValue(reader.GetOrdinal(columnName));
				flag = reader.IsDBNull(reader.GetOrdinal(columnName));
			}
			if (flag)
			{
				mapping.SetAccessor.Set(target, null);
			}
			else
			{
				this._selectStrategy.Set(request, resultMap, mapping, ref target, reader, keys);
			}
		}

		public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
		{
			throw new System.NotSupportedException("Get method on ResultMapStrategy is not supported");
		}
	}
}
