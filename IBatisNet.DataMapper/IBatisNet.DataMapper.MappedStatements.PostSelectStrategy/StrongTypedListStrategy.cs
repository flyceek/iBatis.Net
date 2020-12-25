using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;

namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
	public sealed class StrongTypedListStrategy : IPostSelectStrategy
	{
		public void Execute(PostBindind postSelect, RequestScope request)
		{
			IFactory factory = request.DataExchangeFactory.ObjectFactory.CreateFactory(postSelect.ResultProperty.SetAccessor.MemberType, System.Type.EmptyTypes);
			object obj = factory.CreateInstance(null);
			postSelect.Statement.ExecuteQueryForList(request.Session, postSelect.Keys, (System.Collections.IList)obj);
			postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, obj);
		}
	}
}
