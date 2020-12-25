using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;

namespace IBatisNet.DataMapper.DataExchange
{
	public abstract class BaseDataExchange : IDataExchange
	{
		private DataExchangeFactory _dataExchangeFactory = null;

		public DataExchangeFactory DataExchangeFactory
		{
			get
			{
				return this._dataExchangeFactory;
			}
		}

		public BaseDataExchange(DataExchangeFactory dataExchangeFactory)
		{
			this._dataExchangeFactory = dataExchangeFactory;
		}

		public abstract object GetData(ParameterProperty mapping, object parameterObject);

		public abstract void SetData(ref object target, ResultProperty mapping, object dataBaseValue);

		public abstract void SetData(ref object target, ParameterProperty mapping, object dataBaseValue);
	}
}
