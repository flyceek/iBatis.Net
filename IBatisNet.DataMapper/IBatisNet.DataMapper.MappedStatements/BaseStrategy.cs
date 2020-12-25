using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;
using System.Text;

namespace IBatisNet.DataMapper.MappedStatements
{
	public abstract class BaseStrategy
	{
		private const string KEY_SEPARATOR = "\002";

		public static object SKIP = new object();

		protected string GetUniqueKey(IResultMap resultMap, RequestScope request, IDataReader reader)
		{
			string result;
			if (resultMap.GroupByProperties.Count > 0)
			{
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
				for (int i = 0; i < resultMap.GroupByProperties.Count; i++)
				{
					ResultProperty resultProperty = resultMap.GroupByProperties[i];
					stringBuilder.Append(resultProperty.GetDataBaseValue(reader));
					stringBuilder.Append('-');
				}
				if (stringBuilder.Length < 1)
				{
					result = null;
				}
				else
				{
					stringBuilder.Append("\002");
					result = stringBuilder.ToString();
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		protected bool FillObjectWithReaderAndResultMap(RequestScope request, IDataReader reader, IResultMap resultMap, ref object resultObject)
		{
			bool flag = false;
			bool result;
			if (resultMap.Properties.Count > 0)
			{
				for (int i = 0; i < resultMap.Properties.Count; i++)
				{
					request.IsRowDataFound = false;
					ResultProperty resultProperty = resultMap.Properties[i];
					resultProperty.PropertyStrategy.Set(request, resultMap, resultProperty, ref resultObject, reader, null);
					flag = (flag || request.IsRowDataFound);
				}
				request.IsRowDataFound = flag;
				result = flag;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
