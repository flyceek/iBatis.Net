using System;

namespace IBatisNet.DataMapper.SessionStore
{
	public interface ISessionStore
	{
		ISqlMapSession LocalSession
		{
			get;
		}

		void Store(ISqlMapSession session);

		void Dispose();
	}
}
