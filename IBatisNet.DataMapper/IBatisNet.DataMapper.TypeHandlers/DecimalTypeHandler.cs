using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;
using System.Globalization;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class DecimalTypeHandler : BaseTypeHandler
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
				result = dataReader.GetDecimal(ordinal);
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
				result = dataReader.GetDecimal(mapping.ColumnIndex);
			}
			return result;
		}

		public override object ValueOf(System.Type type, string s)
		{
			System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
			decimal num = 0m;
			if (s != null)
			{
				decimal.TryParse(s.ToString(), out num);
			}
			return num;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			decimal num = 0m;
			if (outputValue != null)
			{
				decimal.TryParse(outputValue.ToString(), out num);
			}
			return num;
		}
	}
}
