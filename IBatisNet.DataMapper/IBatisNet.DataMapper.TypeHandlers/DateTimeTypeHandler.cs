using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class DateTimeTypeHandler : BaseTypeHandler
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
				result = dataReader.GetDateTime(ordinal);
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
				result = dataReader.GetDateTime(mapping.ColumnIndex);
			}
			return result;
		}

		public override object ValueOf(System.Type type, string s)
		{
			System.DateTime minValue = System.DateTime.MinValue;
			if (s != null)
			{
				System.DateTime.TryParse(s.ToString(), out minValue);
			}
			return minValue;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			System.DateTime minValue = System.DateTime.MinValue;
			if (outputValue != null)
			{
				System.DateTime.TryParse(outputValue.ToString(), out minValue);
			}
			return minValue;
		}
	}
}
