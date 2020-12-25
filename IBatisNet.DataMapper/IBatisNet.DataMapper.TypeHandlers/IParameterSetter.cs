using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public interface IParameterSetter
	{
		IDataParameter DataParameter
		{
			get;
		}

		object Value
		{
			set;
		}
	}
}
