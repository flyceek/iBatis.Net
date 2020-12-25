using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class GuidTypeHandler : BaseTypeHandler
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
				result = new System.Guid(dataReader.GetGuid(ordinal).ToString());
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
				result = new System.Guid(dataReader.GetValue(mapping.ColumnIndex).ToString());
			}
			return result;
		}

		public override object ValueOf(System.Type type, string s)
		{
			return new System.Guid(s);
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			return new System.Guid(outputValue.ToString());
		}
	}
}
