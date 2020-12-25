using IBatisNet.Common.Exceptions;
using System;
using System.Web;

namespace IBatisNet.DataMapper.SessionStore
{
	public class WebSessionStore : AbstractSessionStore
	{
		public override ISqlMapSession LocalSession
		{
			get
			{
				HttpContext httpContext = WebSessionStore.ObtainSessionContext();
				return httpContext.Items[this.sessionName] as SqlMapSession;
			}
		}

		public WebSessionStore(string sqlMapperId) : base(sqlMapperId)
		{
		}

		public override void Store(ISqlMapSession session)
		{
			HttpContext httpContext = WebSessionStore.ObtainSessionContext();
			httpContext.Items[this.sessionName] = session;
		}

		public override void Dispose()
		{
			HttpContext httpContext = WebSessionStore.ObtainSessionContext();
			httpContext.Items.Remove(this.sessionName);
		}

		private static HttpContext ObtainSessionContext()
		{
			HttpContext current = HttpContext.Current;
			if (current == null)
			{
				throw new IBatisNetException("WebSessionStore: Could not obtain reference to HttpContext");
			}
			return current;
		}
	}
}
