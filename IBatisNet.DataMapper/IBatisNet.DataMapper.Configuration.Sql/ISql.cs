using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql
{
	public interface ISql
	{
		RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session);
	}
}
