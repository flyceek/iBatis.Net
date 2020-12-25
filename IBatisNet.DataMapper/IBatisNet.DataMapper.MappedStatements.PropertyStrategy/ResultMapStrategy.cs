using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
	public sealed class ResultMapStrategy : BaseStrategy, IPropertyStrategy
	{
		public void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object keys)
		{
			object dataBaseValue = this.Get(request, resultMap, mapping, ref target, reader);
			resultMap.SetValueOfProperty(ref target, mapping, dataBaseValue);
		}

		public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
		{
			object[] array = null;
			bool flag = false;
			IResultMap resultMap2 = mapping.NestedResultMap.ResolveSubMap(reader);
			if (resultMap2.Parameters.Count > 0)
			{
				array = new object[resultMap2.Parameters.Count];
				for (int i = 0; i < resultMap2.Parameters.Count; i++)
				{
					ResultProperty resultProperty = resultMap2.Parameters[i];
					array[i] = resultProperty.ArgumentStrategy.GetValue(request, resultProperty, ref reader, null);
					request.IsRowDataFound = (request.IsRowDataFound || array[i] != null);
					flag = (flag || array[i] != null);
				}
			}
			object result = null;
			if (resultMap2.Parameters.Count > 0 && !flag)
			{
				result = null;
			}
			else
			{
				result = resultMap2.CreateInstanceOfResult(array);
				if (!base.FillObjectWithReaderAndResultMap(request, reader, resultMap2, ref result))
				{
					result = null;
				}
			}
			return result;
		}
	}
}
