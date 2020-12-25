using System;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace IBatisNet.DataMapper.SessionStore
{
	public class HybridWebThreadSessionStore : AbstractSessionStore
	{
		public override ISqlMapSession LocalSession
		{
			get
			{
				HttpContext current = HttpContext.Current;
				ISqlMapSession result;
				if (current == null)
				{
					result = (System.Runtime.Remoting.Messaging.CallContext.GetData(this.sessionName) as SqlMapSession);
				}
				else
				{
					result = (current.Items[this.sessionName] as SqlMapSession);
				}
				return result;
			}
		}

		public HybridWebThreadSessionStore(string sqlMapperId) : base(sqlMapperId)
		{
		}

		public override void Store(ISqlMapSession session)
		{
			HttpContext current = HttpContext.Current;
			if (current == null)
			{
				System.Runtime.Remoting.Messaging.CallContext.SetData(this.sessionName, session);
			}
			else
			{
				current.Items[this.sessionName] = session;
			}
		}

		public override void Dispose()
		{
			HttpContext current = HttpContext.Current;
			if (current == null)
			{
				System.Runtime.Remoting.Messaging.CallContext.SetData(this.sessionName, null);
			}
			else
			{
				current.Items.Remove(this.sessionName);
			}
		}
	}
}
