using IBatisNet.DataMapper.DataExchange;
using System;

namespace IBatisNet.DataMapper.Scope
{
	public interface IScope
	{
		ErrorContext ErrorContext
		{
			get;
		}

		DataExchangeFactory DataExchangeFactory
		{
			get;
		}
	}
}
