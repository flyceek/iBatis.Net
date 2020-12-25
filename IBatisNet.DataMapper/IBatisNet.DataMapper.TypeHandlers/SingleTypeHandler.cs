using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class SingleTypeHandler : BaseTypeHandler
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
				result = dataReader.GetFloat(ordinal);
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
				result = dataReader.GetFloat(mapping.ColumnIndex);
			}
			return result;
		}

		public override object ValueOf(System.Type type, string s)
		{
			float num = 0f;
			if (s != null)
			{
				float.TryParse(s.ToString(), out num);
			}
			return num;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			float num = 0f;
			if (outputValue != null)
			{
				float.TryParse(outputValue.ToString(), out num);
			}
			return num;
		}
	}
}
