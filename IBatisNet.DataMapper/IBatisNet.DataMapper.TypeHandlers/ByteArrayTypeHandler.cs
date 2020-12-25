using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Exceptions;
using System;
using System.Data;
using System.Text;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class ByteArrayTypeHandler : BaseTypeHandler
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
			if (dataReader.IsDBNull(ordinal) || dataReader.GetBytes(ordinal, 0L, null, 0, 0) == 0L)
			{
				result = System.DBNull.Value;
			}
			else
			{
				result = this.GetValueByIndex(ordinal, dataReader);
			}
			return result;
		}

		public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
		{
			object result;
			if (dataReader.IsDBNull(mapping.ColumnIndex) || dataReader.GetBytes(mapping.ColumnIndex, 0L, null, 0, 0) == 0L)
			{
				result = System.DBNull.Value;
			}
			else
			{
				result = this.GetValueByIndex(mapping.ColumnIndex, dataReader);
			}
			return result;
		}

		private byte[] GetValueByIndex(int columnIndex, IDataReader dataReader)
		{
			int num = (int)dataReader.GetBytes(columnIndex, 0L, null, 0, 0);
			byte[] array = new byte[num];
			dataReader.GetBytes(columnIndex, 0L, array, 0, num);
			return array;
		}

		public override object ValueOf(System.Type type, string s)
		{
			return System.Text.Encoding.Default.GetBytes(s);
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			throw new DataMapperException("NotSupportedException");
		}
	}
}
