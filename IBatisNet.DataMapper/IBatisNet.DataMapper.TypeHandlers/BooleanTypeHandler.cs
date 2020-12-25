using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class BooleanTypeHandler : BaseTypeHandler
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
				bool flag = false;
				if (dataReader.GetValue(ordinal) != null)
				{
					bool.TryParse(dataReader.GetValue(ordinal).ToString(), out flag);
				}
				result = flag;
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
				bool flag = false;
				if (dataReader.GetValue(mapping.ColumnIndex) != null)
				{
					bool.TryParse(dataReader.GetValue(mapping.ColumnIndex).ToString(), out flag);
				}
				result = flag;
			}
			return result;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			bool flag = false;
			if (outputValue != null)
			{
				bool.TryParse(outputValue.ToString(), out flag);
			}
			return flag;
		}

		public override object ValueOf(System.Type type, string s)
		{
			bool flag = false;
			if (s != null)
			{
				bool.TryParse(s.ToString(), out flag);
			}
			return flag;
		}
	}
}
