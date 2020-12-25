using System;
using System.Web;

namespace IBatisNet.DataMapper.SessionStore
{
	public sealed class SessionStoreFactory
	{
		public static ISessionStore GetSessionStore(string sqlMapperId)
		{
			ISessionStore result;
			if (HttpContext.Current == null)
			{
				result = new CallContextSessionStore(sqlMapperId);
			}
			else
			{
				result = new WebSessionStore(sqlMapperId);
			}
			return result;
		}
	}
}
