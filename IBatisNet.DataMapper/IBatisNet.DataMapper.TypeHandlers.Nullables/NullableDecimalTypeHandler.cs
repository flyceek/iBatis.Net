using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;
using System.Globalization;

namespace IBatisNet.DataMapper.TypeHandlers.Nullables
{
	public class NullableDecimalTypeHandler : BaseTypeHandler
	{
		public override bool IsSimpleType
		{
			get
			{
				return true;
			}
		}

		public override object NullValue
		{
			get
			{
				return null;
			}
		}

		public sealed override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
		{
			decimal? num = (decimal?)parameterValue;
			if (num.HasValue)
			{
				dataParameter.Value = num.Value;
			}
			else
			{
				dataParameter.Value = System.DBNull.Value;
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
				result = new decimal?(dataReader.GetDecimal(ordinal));
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
				result = new decimal?(dataReader.GetDecimal(mapping.ColumnIndex));
			}
			return result;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			return new char?(System.Convert.ToChar(outputValue));
		}

		public override object ValueOf(System.Type type, string s)
		{
			System.Globalization.CultureInfo provider = new System.Globalization.CultureInfo("en-US");
			return new decimal?(decimal.Parse(s, provider));
		}
	}
}
