using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public interface ITypeHandler
	{
		bool IsSimpleType
		{
			get;
		}

		object NullValue
		{
			get;
		}

		object GetValueByName(ResultProperty mapping, IDataReader dataReader);

		object GetValueByIndex(ResultProperty mapping, IDataReader dataReader);

		object GetDataBaseValue(object outputValue, System.Type parameterType);

		void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType);

		object ValueOf(System.Type type, string s);

		bool Equals(object obj, string str);
	}
}
