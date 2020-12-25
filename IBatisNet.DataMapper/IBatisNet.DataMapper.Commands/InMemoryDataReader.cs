using IBatisNet.DataMapper.Exceptions;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace IBatisNet.DataMapper.Commands
{
	public class InMemoryDataReader : IDataReader, System.IDisposable, IDataRecord
	{
		private class InMemoryResultSet
		{
			private readonly object[][] _records = null;

			private int _fieldCount = 0;

			private string[] _fieldsName = null;

			private System.Type[] _fieldsType = null;

			private StringDictionary _fieldsNameLookup = new StringDictionary();

			private string[] _dataTypeName = null;

			public int FieldCount
			{
				get
				{
					return this._fieldCount;
				}
			}

			public int RecordCount
			{
				get
				{
					return this._records.Length;
				}
			}

			public InMemoryResultSet(IDataReader reader, bool isMidstream)
			{
				System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
				this._fieldCount = reader.FieldCount;
				this._fieldsName = new string[this._fieldCount];
				this._fieldsType = new System.Type[this._fieldCount];
				this._dataTypeName = new string[this._fieldCount];
				bool flag = true;
				while (isMidstream || reader.Read())
				{
					if (flag)
					{
						for (int i = 0; i < reader.FieldCount; i++)
						{
							string name = reader.GetName(i);
							this._fieldsName[i] = name;
							if (!this._fieldsNameLookup.ContainsKey(name))
							{
								this._fieldsNameLookup.Add(name, i.ToString());
							}
							this._fieldsType[i] = reader.GetFieldType(i);
							this._dataTypeName[i] = reader.GetDataTypeName(i);
						}
					}
					flag = false;
					object[] array = new object[this._fieldCount];
					reader.GetValues(array);
					arrayList.Add(array);
					isMidstream = false;
				}
				this._records = (object[][])arrayList.ToArray(typeof(object[]));
			}

			public object GetValue(int rowIndex, int colIndex)
			{
				return this._records[rowIndex][colIndex];
			}

			public System.Type GetFieldType(int colIndex)
			{
				return this._fieldsType[colIndex];
			}

			public string GetName(int colIndex)
			{
				return this._fieldsName[colIndex];
			}

			public int GetOrdinal(string colName)
			{
				if (this._fieldsNameLookup.ContainsKey(colName))
				{
					return System.Convert.ToInt32(this._fieldsNameLookup[colName]);
				}
				throw new System.IndexOutOfRangeException(string.Format("No column with the specified name was found: {0}.", colName));
			}

			public string GetDataTypeName(int colIndex)
			{
				return this._dataTypeName[colIndex];
			}

			public int GetValues(int rowIndex, object[] values)
			{
				System.Array.Copy(this._records[rowIndex], 0, values, 0, this._fieldCount);
				return this._fieldCount;
			}
		}

		private int _currentRowIndex = 0;

		private int _currentResultIndex = 0;

		private bool _isClosed = false;

		private InMemoryDataReader.InMemoryResultSet[] _results = null;

		public int RecordsAffected
		{
			get
			{
				throw new System.NotImplementedException("InMemoryDataReader only used for select IList statements !");
			}
		}

		public bool IsClosed
		{
			get
			{
				return this._isClosed;
			}
		}

		public int Depth
		{
			get
			{
				return this._currentResultIndex;
			}
		}

		public object this[string name]
		{
			get
			{
				return this[this.GetOrdinal(name)];
			}
		}

		public object this[int fieldIndex]
		{
			get
			{
				return this.GetValue(fieldIndex);
			}
		}

		public int FieldCount
		{
			get
			{
				return this.CurrentResultSet.FieldCount;
			}
		}

		private InMemoryDataReader.InMemoryResultSet CurrentResultSet
		{
			get
			{
				return this._results[this._currentResultIndex];
			}
		}

		public InMemoryDataReader(IDataReader reader)
		{
			System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
			try
			{
				this._currentResultIndex = 0;
				this._currentRowIndex = 0;
				arrayList.Add(new InMemoryDataReader.InMemoryResultSet(reader, true));
				while (reader.NextResult())
				{
					arrayList.Add(new InMemoryDataReader.InMemoryResultSet(reader, false));
				}
				this._results = (InMemoryDataReader.InMemoryResultSet[])arrayList.ToArray(typeof(InMemoryDataReader.InMemoryResultSet));
			}
			catch (System.Exception inner)
			{
				throw new DataMapperException("There was a problem converting an IDataReader to an InMemoryDataReader", inner);
			}
			finally
			{
				reader.Close();
				reader.Dispose();
			}
		}

		public bool NextResult()
		{
			this._currentResultIndex++;
			bool result;
			if (this._currentResultIndex >= this._results.Length)
			{
				this._currentResultIndex--;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public void Close()
		{
			this._isClosed = true;
		}

		public bool Read()
		{
			this._currentRowIndex++;
			bool result;
			if (this._currentRowIndex >= this._results[this._currentResultIndex].RecordCount)
			{
				this._currentRowIndex--;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public DataTable GetSchemaTable()
		{
			throw new System.NotImplementedException("GetSchemaTable() is not implemented, cause not use.");
		}

		public void Dispose()
		{
			this._isClosed = true;
			this._results = null;
		}

		public int GetInt32(int fieldIndex)
		{
			return (int)this.GetValue(fieldIndex);
		}

		public object GetValue(int fieldIndex)
		{
			return this.CurrentResultSet.GetValue(this._currentRowIndex, fieldIndex);
		}

		public bool IsDBNull(int fieldIndex)
		{
			return this.GetValue(fieldIndex) == System.DBNull.Value;
		}

		public long GetBytes(int fieldIndex, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			object value = this.GetValue(fieldIndex);
			if (!(value is byte[]))
			{
				throw new System.InvalidCastException("Type is " + value.GetType().ToString());
			}
			long result;
			if (buffer == null)
			{
				result = (long)((byte[])value).Length;
			}
			else
			{
				int num = (int)((long)((byte[])value).Length - dataIndex);
				if (num < length)
				{
					length = num;
				}
				System.Array.Copy((byte[])value, (int)dataIndex, buffer, bufferIndex, length);
				result = (long)length;
			}
			return result;
		}

		public byte GetByte(int fieldIndex)
		{
			return (byte)this.GetValue(fieldIndex);
		}

		public System.Type GetFieldType(int fieldIndex)
		{
			return this.CurrentResultSet.GetFieldType(fieldIndex);
		}

		public decimal GetDecimal(int fieldIndex)
		{
			return (decimal)this.GetValue(fieldIndex);
		}

		public int GetValues(object[] values)
		{
			return this.CurrentResultSet.GetValues(this._currentRowIndex, values);
		}

		public string GetName(int fieldIndex)
		{
			return this.CurrentResultSet.GetName(fieldIndex);
		}

		public long GetInt64(int fieldIndex)
		{
			return (long)this.GetValue(fieldIndex);
		}

		public double GetDouble(int fieldIndex)
		{
			return (double)this.GetValue(fieldIndex);
		}

		public bool GetBoolean(int fieldIndex)
		{
			return (bool)this.GetValue(fieldIndex);
		}

		public System.Guid GetGuid(int fieldIndex)
		{
			return (System.Guid)this.GetValue(fieldIndex);
		}

		public System.DateTime GetDateTime(int fieldIndex)
		{
			return (System.DateTime)this.GetValue(fieldIndex);
		}

		public int GetOrdinal(string colName)
		{
			return this.CurrentResultSet.GetOrdinal(colName);
		}

		public string GetDataTypeName(int fieldIndex)
		{
			return this.CurrentResultSet.GetDataTypeName(fieldIndex);
		}

		public float GetFloat(int fieldIndex)
		{
			return (float)this.GetValue(fieldIndex);
		}

		public IDataReader GetData(int fieldIndex)
		{
			throw new System.NotImplementedException("GetData(int) is not implemented, cause not use.");
		}

		public long GetChars(int fieldIndex, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			object value = this.GetValue(fieldIndex);
			char[] array;
			if (value is char[])
			{
				array = (char[])value;
			}
			else
			{
				if (!(value is string))
				{
					throw new System.InvalidCastException("Type is " + value.GetType().ToString());
				}
				array = ((string)value).ToCharArray();
			}
			long result;
			if (buffer == null)
			{
				result = (long)array.Length;
			}
			else
			{
				System.Array.Copy(array, (int)dataIndex, buffer, bufferIndex, length);
				result = (long)array.Length - dataIndex;
			}
			return result;
		}

		public string GetString(int fieldIndex)
		{
			return (string)this.GetValue(fieldIndex);
		}

		public char GetChar(int fieldIndex)
		{
			return (char)this.GetValue(fieldIndex);
		}

		public short GetInt16(int fieldIndex)
		{
			return (short)this.GetValue(fieldIndex);
		}
	}
}
