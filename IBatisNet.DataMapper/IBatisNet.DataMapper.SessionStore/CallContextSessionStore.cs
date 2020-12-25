using System;
using System.Runtime.Remoting.Messaging;

namespace IBatisNet.DataMapper.SessionStore
{
	public class CallContextSessionStore : AbstractSessionStore
	{
		public override ISqlMapSession LocalSession
		{
			get
			{
				return System.Runtime.Remoting.Messaging.CallContext.GetData(this.sessionName) as SqlMapSession;
			}
		}

		public CallContextSessionStore(string sqlMapperId) : base(sqlMapperId)
		{
		}

		public override void Store(ISqlMapSession session)
		{
			System.Runtime.Remoting.Messaging.CallContext.SetData(this.sessionName, session);
		}

		public override void Dispose()
		{
			System.Runtime.Remoting.Messaging.CallContext.SetData(this.sessionName, null);
		}
	}
}
