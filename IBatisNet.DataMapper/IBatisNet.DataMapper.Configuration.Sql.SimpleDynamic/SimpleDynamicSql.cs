using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Sql.SimpleDynamic
{
	internal sealed class SimpleDynamicSql : ISql
	{
		private const string ELEMENT_TOKEN = "$";

		private string _simpleSqlStatement = string.Empty;

		private IStatement _statement = null;

		private DataExchangeFactory _dataExchangeFactory = null;

		internal SimpleDynamicSql(IScope scope, string sqlStatement, IStatement statement)
		{
			this._simpleSqlStatement = sqlStatement;
			this._statement = statement;
			this._dataExchangeFactory = scope.DataExchangeFactory;
		}

		public string GetSql(object parameterObject)
		{
			return this.ProcessDynamicElements(parameterObject);
		}

		public static bool IsSimpleDynamicSql(string sqlStatement)
		{
			return sqlStatement != null && sqlStatement.IndexOf("$") > -1;
		}

		private string ProcessDynamicElements(object parameterObject)
		{
			StringTokenizer stringTokenizer = new StringTokenizer(this._simpleSqlStatement, "$", true);
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			string value = null;
			System.Collections.IEnumerator enumerator = stringTokenizer.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current;
				if ("$".Equals(value))
				{
					if ("$".Equals(text))
					{
						stringBuilder.Append("$");
						text = null;
					}
					else
					{
						object obj = null;
						if (parameterObject != null)
						{
							if (this._dataExchangeFactory.TypeHandlerFactory.IsSimpleType(parameterObject.GetType()))
							{
								obj = parameterObject;
							}
							else
							{
								obj = ObjectProbe.GetMemberValue(parameterObject, text, this._dataExchangeFactory.AccessorFactory);
							}
						}
						if (obj != null)
						{
							stringBuilder.Append(obj.ToString());
						}
						enumerator.MoveNext();
						text = (string)enumerator.Current;
						if (!"$".Equals(text))
						{
							throw new DataMapperException("Unterminated dynamic element in sql (" + this._simpleSqlStatement + ").");
						}
						text = null;
					}
				}
				else if (!"$".Equals(text))
				{
					stringBuilder.Append(text);
				}
				value = text;
			}
			return stringBuilder.ToString();
		}

		public RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session)
		{
			string sqlStatement = this.ProcessDynamicElements(parameterObject);
			RequestScope requestScope = new RequestScope(this._dataExchangeFactory, session, this._statement);
			requestScope.PreparedStatement = this.BuildPreparedStatement(session, requestScope, sqlStatement);
			requestScope.MappedStatement = mappedStatement;
			return requestScope;
		}

		private PreparedStatement BuildPreparedStatement(ISqlMapSession session, RequestScope request, string sqlStatement)
		{
			PreparedStatementFactory preparedStatementFactory = new PreparedStatementFactory(session, request, this._statement, sqlStatement);
			return preparedStatementFactory.Prepare();
		}
	}
}
