using IBatisNet.DataMapper.Scope;
using System;

namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
	public sealed class ObjectStrategy : IPostSelectStrategy
	{
		public void Execute(PostBindind postSelect, RequestScope request)
		{
			object value = postSelect.Statement.ExecuteQueryForObject(request.Session, postSelect.Keys);
			postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, value);
		}
	}
}
