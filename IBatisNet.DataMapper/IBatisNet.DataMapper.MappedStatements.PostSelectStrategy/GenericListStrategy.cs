using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
	public sealed class GenericListStrategy : IPostSelectStrategy
	{
		public void Execute(PostBindind postSelect, RequestScope request)
		{
			System.Type[] genericArguments = postSelect.ResultProperty.SetAccessor.MemberType.GetGenericArguments();
			System.Type typeFromHandle = typeof(System.Collections.Generic.IList<>);
			System.Type type = typeFromHandle.MakeGenericType(genericArguments);
			System.Type type2 = postSelect.ResultProperty.SetAccessor.MemberType.GetGenericArguments()[0];
			System.Type type3 = postSelect.Statement.GetType();
			System.Type[] array = new System.Type[]
			{
				typeof(SqlMapSession),
				typeof(object)
			};
			System.Reflection.MethodInfo[] methods = type3.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod);
			System.Reflection.MethodInfo methodInfo = null;
			System.Reflection.MethodInfo[] array2 = methods;
			for (int i = 0; i < array2.Length; i++)
			{
				System.Reflection.MethodInfo methodInfo2 = array2[i];
				if (methodInfo2.IsGenericMethod && methodInfo2.Name == "ExecuteQueryForList" && methodInfo2.GetParameters().Length == 2)
				{
					methodInfo = methodInfo2;
					break;
				}
			}
			System.Reflection.MethodInfo methodInfo3 = methodInfo.MakeGenericMethod(new System.Type[]
			{
				type2
			});
			object[] parameters = new object[]
			{
				request.Session,
				postSelect.Keys
			};
			object value = methodInfo3.Invoke(postSelect.Statement, parameters);
			postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, value);
		}
	}
}
