using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Static
{
	public sealed class StaticSql : ISql
	{
		private IStatement _statement = null;

		private PreparedStatement _preparedStatement = null;

		private DataExchangeFactory _dataExchangeFactory = null;

		public StaticSql(IScope scope, IStatement statement)
		{
			this._statement = statement;
			this._dataExchangeFactory = scope.DataExchangeFactory;
		}

		public RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session)
		{
			return new RequestScope(this._dataExchangeFactory, session, this._statement)
			{
				PreparedStatement = this._preparedStatement,
				MappedStatement = mappedStatement
			};
		}

		public void BuildPreparedStatement(ISqlMapSession session, string sqlStatement)
		{
			RequestScope request = new RequestScope(this._dataExchangeFactory, session, this._statement);
			PreparedStatementFactory preparedStatementFactory = new PreparedStatementFactory(session, request, this._statement, sqlStatement);
			this._preparedStatement = preparedStatementFactory.Prepare();
		}
	}
}
