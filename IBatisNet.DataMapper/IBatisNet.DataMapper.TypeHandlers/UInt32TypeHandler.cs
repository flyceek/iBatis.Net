using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class UInt32TypeHandler : BaseTypeHandler
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
				uint num = 0u;
				if (dataReader.GetValue(ordinal) != null)
				{
					uint.TryParse(dataReader.GetValue(ordinal).ToString(), out num);
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
				uint num = 0u;
				if (dataReader.GetValue(mapping.ColumnIndex) != null)
				{
					uint.TryParse(dataReader.GetValue(mapping.ColumnIndex).ToString(), out num);
				}
				result = num;
			}
			return result;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			uint num = 0u;
			if (outputValue != null)
			{
				uint.TryParse(outputValue.ToString(), out num);
			}
			return num;
		}

		public override object ValueOf(System.Type type, string s)
		{
			uint num = 0u;
			if (s != null)
			{
				uint.TryParse(s.ToString(), out num);
			}
			return num;
		}
	}
}
