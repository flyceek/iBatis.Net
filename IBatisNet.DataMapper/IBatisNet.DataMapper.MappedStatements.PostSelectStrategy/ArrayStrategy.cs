using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;

namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
	public sealed class ArrayStrategy : IPostSelectStrategy
	{
		public void Execute(PostBindind postSelect, RequestScope request)
		{
			System.Collections.IList list = postSelect.Statement.ExecuteQueryForList(request.Session, postSelect.Keys);
			System.Type elementType = postSelect.ResultProperty.SetAccessor.MemberType.GetElementType();
			System.Array array = System.Array.CreateInstance(elementType, list.Count);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				array.SetValue(list[i], i);
			}
			postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, array);
		}
	}
}
