using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class ResultGetterImpl : IResultGetter
	{
		private int _columnIndex = -2147483648;

		private string _columnName = string.Empty;

		private object _outputValue = null;

		private IDataReader _dataReader = null;

		public IDataReader DataReader
		{
			get
			{
				return this._dataReader;
			}
		}

		public object Value
		{
			get
			{
				object result;
				if (this._columnName.Length > 0)
				{
					int ordinal = this._dataReader.GetOrdinal(this._columnName);
					result = this._dataReader.GetValue(ordinal);
				}
				else if (this._columnIndex >= 0)
				{
					result = this._dataReader.GetValue(this._columnIndex);
				}
				else
				{
					result = this._outputValue;
				}
				return result;
			}
		}

		public ResultGetterImpl(IDataReader dataReader, int columnIndex)
		{
			this._columnIndex = columnIndex;
			this._dataReader = dataReader;
		}

		public ResultGetterImpl(IDataReader dataReader, string columnName)
		{
			this._columnName = columnName;
			this._dataReader = dataReader;
		}

		public ResultGetterImpl(object outputValue)
		{
			this._outputValue = outputValue;
		}
	}
}
