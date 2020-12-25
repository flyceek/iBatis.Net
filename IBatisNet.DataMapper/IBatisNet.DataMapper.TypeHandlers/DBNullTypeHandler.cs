using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class DBNullTypeHandler : BaseTypeHandler
	{
		public override bool IsSimpleType
		{
			get
			{
				return false;
			}
		}

		public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
		{
			return System.DBNull.Value;
		}

		public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
		{
			return System.DBNull.Value;
		}

		public override object GetDataBaseValue(object outputValue, System.Type parameterType)
		{
			return System.DBNull.Value;
		}

		public override object ValueOf(System.Type type, string s)
		{
			return System.DBNull.Value;
		}

		public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
		{
			dataParameter.Value = System.DBNull.Value;
		}
	}
}
