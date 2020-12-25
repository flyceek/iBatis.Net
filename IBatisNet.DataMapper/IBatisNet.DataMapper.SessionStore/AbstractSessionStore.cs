using System;

namespace IBatisNet.DataMapper.SessionStore
{
	public abstract class AbstractSessionStore : System.MarshalByRefObject, ISessionStore
	{
		private const string KEY = "_IBATIS_LOCAL_SQLMAP_SESSION_";

		protected string sessionName = string.Empty;

		public abstract ISqlMapSession LocalSession
		{
			get;
		}

		public AbstractSessionStore(string sqlMapperId)
		{
			this.sessionName = "_IBATIS_LOCAL_SQLMAP_SESSION_" + sqlMapperId;
		}

		public abstract void Store(ISqlMapSession session);

		public abstract void Dispose();
	}
}
