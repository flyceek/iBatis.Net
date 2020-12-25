using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers.Nullables
{
	public sealed class NullableUInt16TypeHandler : BaseTypeHandler
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

		public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
		{
			ushort? num = (ushort?)parameterValue;
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
				result = new ushort?(System.Convert.ToUInt16(dataReader.GetValue(ordinal)));
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
				result = new ushort?(System.Convert.ToUInt16(dataReader.GetValue(mapping.ColumnIndex)));
			}
			return result;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			return new ushort?(System.Convert.ToUInt16(outputValue));
		}

		public override object ValueOf(System.Type type, string s)
		{
			return new ushort?(ushort.Parse(s));
		}
	}
}
