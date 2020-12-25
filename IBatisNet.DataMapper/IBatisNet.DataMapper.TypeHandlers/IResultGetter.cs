using System;
using System.Data;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public interface IResultGetter
	{
		IDataReader DataReader
		{
			get;
		}

		object Value
		{
			get;
		}
	}
}
