using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class UnknownTypeHandler : BaseTypeHandler
	{
		private TypeHandlerFactory _factory = null;

		public override bool IsSimpleType
		{
			get
			{
				return true;
			}
		}

		public UnknownTypeHandler(TypeHandlerFactory factory)
		{
			this._factory = factory;
		}

		public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
		{
			if (parameterValue != null)
			{
				ITypeHandler typeHandler = this._factory.GetTypeHandler(parameterValue.GetType(), dbType);
				typeHandler.SetParameter(dataParameter, parameterValue, dbType);
			}
			else
			{
				dataParameter.Value = System.DBNull.Value;
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

		public override bool Equals(object obj, string str)
		{
			bool result;
			if (obj == null || str == null)
			{
				result = ((string)obj == str);
			}
			else
			{
				ITypeHandler typeHandler = this._factory.GetTypeHandler(obj.GetType());
				object obj2 = typeHandler.ValueOf(obj.GetType(), str);
				result = obj.Equals(obj2);
			}
			return result;
		}
	}
}
