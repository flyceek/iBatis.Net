using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public abstract class BaseTypeHandler : ITypeHandler
	{
		public abstract bool IsSimpleType
		{
			get;
		}

		public virtual object NullValue
		{
			get
			{
				return null;
			}
		}

		public abstract object GetValueByName(ResultProperty mapping, IDataReader dataReader);

		public abstract object GetValueByIndex(ResultProperty mapping, IDataReader dataReader);

		public abstract object GetDataBaseValue(object outputValue, System.Type parameterType);

		public abstract object ValueOf(System.Type type, string s);

		public virtual void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
		{
			if (parameterValue != null)
			{
				dataParameter.Value = parameterValue;
			}
			else
			{
				dataParameter.Value = System.DBNull.Value;
			}
		}

		public virtual bool Equals(object obj, string str)
		{
			bool result;
			if (obj == null || str == null)
			{
				result = ((string)obj == str);
			}
			else
			{
				object obj2 = this.ValueOf(obj.GetType(), str);
				result = obj.Equals(obj2);
			}
			return result;
		}
	}
}
