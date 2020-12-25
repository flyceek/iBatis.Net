using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class EnumTypeHandler : BaseTypeHandler
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
			if (parameterValue != null)
			{
				dataParameter.Value = System.Convert.ChangeType(parameterValue, System.Enum.GetUnderlyingType(parameterValue.GetType()));
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
				result = System.Enum.Parse(mapping.MemberType, dataReader.GetValue(ordinal).ToString());
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
				result = System.Enum.Parse(mapping.MemberType, dataReader.GetValue(mapping.ColumnIndex).ToString());
			}
			return result;
		}

		public override object ValueOf(System.Type type, string s)
		{
			return System.Enum.Parse(type, s);
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			return System.Enum.Parse(parameterType, outputValue.ToString());
		}
	}
}
