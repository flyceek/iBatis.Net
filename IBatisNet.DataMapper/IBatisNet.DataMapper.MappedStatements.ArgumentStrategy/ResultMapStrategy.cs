using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
	public sealed class ResultMapStrategy : BaseStrategy, IArgumentStrategy
	{
		public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
		{
			object[] array = null;
			bool flag = false;
			IResultMap resultMap = mapping.NestedResultMap.ResolveSubMap(reader);
			if (resultMap.Parameters.Count > 0)
			{
				array = new object[resultMap.Parameters.Count];
				for (int i = 0; i < resultMap.Parameters.Count; i++)
				{
					ResultProperty resultProperty = resultMap.Parameters[i];
					array[i] = resultProperty.ArgumentStrategy.GetValue(request, resultProperty, ref reader, null);
					request.IsRowDataFound = (request.IsRowDataFound || array[i] != null);
					flag = (flag || array[i] != null);
				}
			}
			object result = null;
			if (resultMap.Parameters.Count > 0 && !flag)
			{
				result = null;
			}
			else
			{
				result = resultMap.CreateInstanceOfResult(array);
				if (!base.FillObjectWithReaderAndResultMap(request, reader, resultMap, ref result))
				{
					result = null;
				}
			}
			return result;
		}
	}
}
