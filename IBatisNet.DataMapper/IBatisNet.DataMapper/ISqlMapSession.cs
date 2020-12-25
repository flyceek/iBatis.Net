using IBatisNet.Common;
using System;

namespace IBatisNet.DataMapper
{
	public interface ISqlMapSession : IDalSession, System.IDisposable
	{
		ISqlMapper SqlMapper
		{
			get;
		}

		void CreateConnection();

		void CreateConnection(string connectionString);
	}
}
