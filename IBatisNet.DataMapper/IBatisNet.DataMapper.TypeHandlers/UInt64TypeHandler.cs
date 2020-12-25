using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class UInt64TypeHandler : BaseTypeHandler
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
				ulong num = 0uL;
				if (dataReader.GetValue(ordinal) != null)
				{
					ulong.TryParse(dataReader.GetValue(ordinal).ToString(), out num);
				}
				result = num;
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
				ulong num = 0uL;
				if (dataReader.GetValue(mapping.ColumnIndex) != null)
				{
					ulong.TryParse(dataReader.GetValue(mapping.ColumnIndex).ToString(), out num);
				}
				result = num;
			}
			return result;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			ulong num = 0uL;
			if (outputValue != null)
			{
				ulong.TryParse(outputValue.ToString(), out num);
			}
			return num;
		}

		public override object ValueOf(System.Type type, string s)
		{
			ulong num = 0uL;
			if (s != null)
			{
				ulong.TryParse(s.ToString(), out num);
			}
			return num;
		}
	}
}
