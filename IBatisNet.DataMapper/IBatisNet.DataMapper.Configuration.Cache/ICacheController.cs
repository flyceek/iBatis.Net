using System;
using System.Collections;

namespace IBatisNet.DataMapper.Configuration.Cache
{
	public interface ICacheController
	{
		object this[object key]
		{
			get;
			set;
		}

		object Remove(object key);

		void Flush();

		void Configure(System.Collections.IDictionary properties);
	}
}
