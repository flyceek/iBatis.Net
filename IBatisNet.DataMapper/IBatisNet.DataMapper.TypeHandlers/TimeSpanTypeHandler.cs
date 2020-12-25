using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class TimeSpanTypeHandler : BaseTypeHandler
	{
		public override bool IsSimpleType
		{
			get
			{
				return true;
			}
		}

		public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
		{
			dataParameter.Value = ((System.TimeSpan)parameterValue).Ticks;
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
				result = new System.TimeSpan(System.Convert.ToInt64(dataReader.GetValue(ordinal)));
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
				result = new System.TimeSpan(System.Convert.ToInt64(dataReader.GetValue(mapping.ColumnIndex)));
			}
			return result;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			return new System.TimeSpan(System.Convert.ToInt64(outputValue));
		}

		public override object ValueOf(System.Type type, string s)
		{
			return System.TimeSpan.Parse(s);
		}
	}
}
