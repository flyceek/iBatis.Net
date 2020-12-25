using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class CharTypeHandler : BaseTypeHandler
	{
		public override bool IsSimpleType
		{
			get
			{
				return true;
			}
		}

		public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
		{
			int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
			object result;
			if (dataReader.IsDBNull(ordinal))
			{
				result = System.DBNull.Value;
			}
			else
			{
				result = dataReader.GetString(ordinal)[0];
			}
			return result;
		}

		public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
		{
			object result;
			if (dataReader.IsDBNull(mapping.ColumnIndex))
			{
				result = System.DBNull.Value;
			}
			else
			{
				result = dataReader.GetString(mapping.ColumnIndex)[0];
			}
			return result;
		}

		public override object ValueOf(System.Type type, string s)
		{
			return System.Convert.ToChar(s);
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			return System.Convert.ToChar(outputValue);
		}
	}
}
