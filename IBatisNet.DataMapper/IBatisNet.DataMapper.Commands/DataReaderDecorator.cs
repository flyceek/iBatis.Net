using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.Commands
{
	public class DataReaderDecorator : IDataReader, System.IDisposable, IDataRecord
	{
		private IDataReader _innerDataReader = null;

		private RequestScope _request = null;

		int IDataReader.Depth
		{
			get
			{
				return this._innerDataReader.Depth;
			}
		}

		bool IDataReader.IsClosed
		{
			get
			{
				return this._innerDataReader.IsClosed;
			}
		}

		int IDataReader.RecordsAffected
		{
			get
			{
				return this._innerDataReader.RecordsAffected;
			}
		}

		int IDataRecord.FieldCount
		{
			get
			{
				return this._innerDataReader.FieldCount;
			}
		}

		object IDataRecord.this[string name]
		{
			get
			{
				return this._innerDataReader[name];
			}
		}

		object IDataRecord.this[int i]
		{
			get
			{
				return this._innerDataReader[i];
			}
		}

		public DataReaderDecorator(IDataReader dataReader, RequestScope request)
		{
			this._innerDataReader = dataReader;
			this._request = request;
		}

		void IDataReader.Close()
		{
			this._innerDataReader.Close();
		}

		DataTable IDataReader.GetSchemaTable()
		{
			return this._innerDataReader.GetSchemaTable();
		}

		bool IDataReader.NextResult()
		{
			this._request.MoveNextResultMap();
			return this._innerDataReader.NextResult();
		}

		bool IDataReader.Read()
		{
			return this._innerDataReader.Read();
		}

		void System.IDisposable.Dispose()
		{
			this._innerDataReader.Dispose();
		}

		bool IDataRecord.GetBoolean(int i)
		{
			bool result = false;
			if (!this._innerDataReader.IsDBNull(i))
			{
				bool.TryParse(this._innerDataReader[i].ToString(), out result);
			}
			return result;
		}

		byte IDataRecord.GetByte(int i)
		{
			byte result = 0;
			if (!this._innerDataReader.IsDBNull(i))
			{
				byte.TryParse(this._innerDataReader[i].ToString(), out result);
			}
			return result;
		}

		long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return this._innerDataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		char IDataRecord.GetChar(int i)
		{
			char result = '\0';
			if (!this._innerDataReader.IsDBNull(i))
			{
				char.TryParse(this._innerDataReader[i].ToString(), out result);
			}
			return result;
		}

		long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return this._innerDataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		IDataReader IDataRecord.GetData(int i)
		{
			return this._innerDataReader.GetData(i);
		}

		string IDataRecord.GetDataTypeName(int i)
		{
			return this._innerDataReader.GetDataTypeName(i);
		}

		System.DateTime IDataRecord.GetDateTime(int i)
		{
			System.DateTime minValue = System.DateTime.MinValue;
			if (!this._innerDataReader.IsDBNull(i))
			{
				System.DateTime.TryParse(this._innerDataReader[i].ToString(), out minValue);
			}
			return minValue;
		}

		decimal IDataRecord.GetDecimal(int i)
		{
			decimal result = 0m;
			if (!this._innerDataReader.IsDBNull(i))
			{
				decimal.TryParse(this._innerDataReader[i].ToString(), out result);
			}
			return result;
		}

		double IDataRecord.GetDouble(int i)
		{
			double result = 0.0;
			if (!this._innerDataReader.IsDBNull(i))
			{
				double.TryParse(this._innerDataReader[i].ToString(), out result);
			}
			return result;
		}

		System.Type IDataRecord.GetFieldType(int i)
		{
			return this._innerDataReader.GetFieldType(i);
		}

		float IDataRecord.GetFloat(int i)
		{
			float result = 0f;
			if (!this._innerDataReader.IsDBNull(i))
			{
				float.TryParse(this._innerDataReader[i].ToString(), out result);
			}
			return result;
		}

		System.Guid IDataRecord.GetGuid(int i)
		{
			return this._innerDataReader.GetGuid(i);
		}

		short IDataRecord.GetInt16(int i)
		{
			short result = 0;
			if (!this._innerDataReader.IsDBNull(i))
			{
				short.TryParse(this._innerDataReader[i].ToString(), out result);
			}
			return result;
		}

		int IDataRecord.GetInt32(int i)
		{
			int result = 0;
			if (!this._innerDataReader.IsDBNull(i))
			{
				int.TryParse(this._innerDataReader[i].ToString(), out result);
			}
			return result;
		}

		long IDataRecord.GetInt64(int i)
		{
			long result = 0L;
			if (!this._innerDataReader.IsDBNull(i))
			{
				long.TryParse(this._innerDataReader[i].ToString(), out result);
			}
			return result;
		}

		string IDataRecord.GetName(int i)
		{
			string result = null;
			if (!this._innerDataReader.IsDBNull(i))
			{
				result = this._innerDataReader[i].ToString();
			}
			return result;
		}

		int IDataRecord.GetOrdinal(string name)
		{
			return this._innerDataReader.GetOrdinal(name);
		}

		string IDataRecord.GetString(int i)
		{
			string result = string.Empty;
			if (!this._innerDataReader.IsDBNull(i))
			{
				result = this._innerDataReader[i].ToString();
			}
			return result;
		}

		object IDataRecord.GetValue(int i)
		{
			return this._innerDataReader.GetValue(i);
		}

		int IDataRecord.GetValues(object[] values)
		{
			return this._innerDataReader.GetValues(values);
		}

		bool IDataRecord.IsDBNull(int i)
		{
			return this._innerDataReader.FieldCount <= i || this._innerDataReader.IsDBNull(i);
		}
	}
}
