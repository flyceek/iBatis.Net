using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public sealed class ParameterSetterImpl : IParameterSetter
	{
		private IDataParameter _dataParameter = null;

		public IDataParameter DataParameter
		{
			get
			{
				return this._dataParameter;
			}
		}

		public object Value
		{
			set
			{
				this._dataParameter.Value = value;
			}
		}

		public ParameterSetterImpl(IDataParameter dataParameter)
		{
			this._dataParameter = dataParameter;
		}
	}
}
