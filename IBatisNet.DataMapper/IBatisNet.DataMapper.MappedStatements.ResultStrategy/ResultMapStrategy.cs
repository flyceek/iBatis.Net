using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public sealed class ResultMapStrategy : BaseStrategy, IResultStrategy
	{
		public object Process(RequestScope request, ref IDataReader reader, object resultObject)
		{
			object obj = resultObject;
			IResultMap resultMap = request.CurrentResultMap.ResolveSubMap(reader);
			if (obj == null)
			{
				object[] array = null;
				if (resultMap.Parameters.Count > 0)
				{
					array = new object[resultMap.Parameters.Count];
					for (int i = 0; i < resultMap.Parameters.Count; i++)
					{
						ResultProperty resultProperty = resultMap.Parameters[i];
						array[i] = resultProperty.ArgumentStrategy.GetValue(request, resultProperty, ref reader, null);
					}
				}
				obj = resultMap.CreateInstanceOfResult(array);
			}
			for (int i = 0; i < resultMap.Properties.Count; i++)
			{
				ResultProperty resultProperty2 = resultMap.Properties[i];
				resultProperty2.PropertyStrategy.Set(request, resultMap, resultProperty2, ref obj, reader, null);
			}
			return obj;
		}
	}
}
