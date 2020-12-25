using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers.Nullables
{
	public sealed class NullableSByteTypeHandler : BaseTypeHandler
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
			byte? b = (byte?)parameterValue;
			if (b.HasValue)
			{
				dataParameter.Value = b.Value;
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
				result = new sbyte?(System.Convert.ToSByte(dataReader.GetValue(ordinal)));
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
				result = new sbyte?(System.Convert.ToSByte(dataReader.GetValue(mapping.ColumnIndex)));
			}
			return result;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			return new sbyte?(System.Convert.ToSByte(outputValue));
		}

		public override object ValueOf(System.Type type, string s)
		{
			return new sbyte?(System.Convert.ToSByte(s));
		}
	}
}
