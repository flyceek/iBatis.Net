using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class ObjectTypeHandler : BaseTypeHandler
	{
		public override bool IsSimpleType
		{
			get
			{
				return false;
			}
		}

		public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
		{
			int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
			object value;
			if (dataReader.IsDBNull(ordinal))
			{
				value = System.DBNull.Value;
			}
			else
			{
				value = dataReader.GetValue(ordinal);
			}
			return value;
		}

		public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
		{
			object value;
			if (dataReader.IsDBNull(mapping.ColumnIndex))
			{
				value = System.DBNull.Value;
			}
			else
			{
				value = dataReader.GetValue(mapping.ColumnIndex);
			}
			return value;
		}

		public override object ValueOf(System.Type type, string s)
		{
			return s;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			return outputValue;
		}
	}
}
