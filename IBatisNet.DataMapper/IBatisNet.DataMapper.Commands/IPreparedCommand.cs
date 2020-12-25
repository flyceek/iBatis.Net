using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.Scope;
using System;

namespace IBatisNet.DataMapper.Commands
{
	public interface IPreparedCommand
	{
		void Create(RequestScope request, ISqlMapSession session, IStatement statement, object parameterObject);
	}
}
