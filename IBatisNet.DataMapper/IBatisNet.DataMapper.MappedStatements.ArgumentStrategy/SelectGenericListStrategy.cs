using IBatisNet.DataMapper.Commands;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
	public sealed class SelectGenericListStrategy : IArgumentStrategy
	{
		public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
		{
			IMappedStatement mappedStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);
			reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
			System.Type[] genericArguments = mapping.MemberType.GetGenericArguments();
			System.Type typeFromHandle = typeof(System.Collections.Generic.IList<>);
			System.Type type = typeFromHandle.MakeGenericType(genericArguments);
			System.Type type2 = mapping.MemberType.GetGenericArguments()[0];
			System.Type type3 = mappedStatement.GetType();
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
				keys
			};
			return methodInfo3.Invoke(mappedStatement, parameters);
		}
	}
}
