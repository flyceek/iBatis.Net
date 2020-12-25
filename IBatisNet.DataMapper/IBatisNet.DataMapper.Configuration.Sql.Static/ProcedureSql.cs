using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Static
{
	public sealed class ProcedureSql : ISql
	{
		private IStatement _statement = null;

		private PreparedStatement _preparedStatement = null;

		private string _sqlStatement = string.Empty;

		private object _synRoot = new object();

		private DataExchangeFactory _dataExchangeFactory = null;

		public ProcedureSql(IScope scope, string sqlStatement, IStatement statement)
		{
			this._sqlStatement = sqlStatement;
			this._statement = statement;
			this._dataExchangeFactory = scope.DataExchangeFactory;
		}

		public RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session)
		{
			RequestScope requestScope = new RequestScope(this._dataExchangeFactory, session, this._statement);
			requestScope.PreparedStatement = this.BuildPreparedStatement(session, requestScope, this._sqlStatement);
			requestScope.MappedStatement = mappedStatement;
			return requestScope;
		}

		public PreparedStatement BuildPreparedStatement(ISqlMapSession session, RequestScope request, string commandText)
		{
			if (this._preparedStatement == null)
			{
				lock (this._synRoot)
				{
					if (this._preparedStatement == null)
					{
						PreparedStatementFactory preparedStatementFactory = new PreparedStatementFactory(session, request, this._statement, commandText);
						this._preparedStatement = preparedStatementFactory.Prepare();
					}
				}
			}
			return this._preparedStatement;
		}
	}
}
