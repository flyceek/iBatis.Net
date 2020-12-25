using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using IBatisNet.DataMapper.Configuration.Sql.SimpleDynamic;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic
{
	internal sealed class DynamicSql : ISql, IDynamicParent
	{
		private System.Collections.IList _children = new System.Collections.ArrayList();

		private IStatement _statement = null;

		private bool _usePositionalParameters = false;

		private InlineParameterMapParser _paramParser = null;

		private DataExchangeFactory _dataExchangeFactory = null;

		internal DynamicSql(ConfigurationScope configScope, IStatement statement)
		{
			this._statement = statement;
			this._usePositionalParameters = configScope.DataSource.DbProvider.UsePositionalParameters;
			this._dataExchangeFactory = configScope.DataExchangeFactory;
		}

		public void AddChild(ISqlChild child)
		{
			this._children.Add(child);
		}

		public RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session)
		{
			RequestScope requestScope = new RequestScope(this._dataExchangeFactory, session, this._statement);
			this._paramParser = new InlineParameterMapParser();
			string sqlStatement = this.Process(requestScope, parameterObject);
			requestScope.PreparedStatement = this.BuildPreparedStatement(session, requestScope, sqlStatement);
			requestScope.MappedStatement = mappedStatement;
			return requestScope;
		}

		private string Process(RequestScope request, object parameterObject)
		{
			SqlTagContext sqlTagContext = new SqlTagContext();
			System.Collections.IList children = this._children;
			this.ProcessBodyChildren(request, sqlTagContext, parameterObject, children);
			ParameterMap parameterMap = new ParameterMap(request.DataExchangeFactory);
			parameterMap.Id = this._statement.Id + "-InlineParameterMap";
			parameterMap.Initialize(this._usePositionalParameters, request);
			parameterMap.Class = this._statement.ParameterClass;
			System.Collections.IList parameterMappings = sqlTagContext.GetParameterMappings();
			int count = parameterMappings.Count;
			for (int i = 0; i < count; i++)
			{
				parameterMap.AddParameterProperty((ParameterProperty)parameterMappings[i]);
			}
			request.ParameterMap = parameterMap;
			string text = sqlTagContext.BodyText;
			if (SimpleDynamicSql.IsSimpleDynamicSql(text))
			{
				text = new SimpleDynamicSql(request, text, this._statement).GetSql(parameterObject);
			}
			return text;
		}

		private void ProcessBodyChildren(RequestScope request, SqlTagContext ctx, object parameterObject, System.Collections.IList localChildren)
		{
			System.Text.StringBuilder writer = ctx.GetWriter();
			this.ProcessBodyChildren(request, ctx, parameterObject, localChildren.GetEnumerator(), writer);
		}

		private void ProcessBodyChildren(RequestScope request, SqlTagContext ctx, object parameterObject, System.Collections.IEnumerator localChildren, System.Text.StringBuilder buffer)
		{
			while (localChildren.MoveNext())
			{
				ISqlChild sqlChild = (ISqlChild)localChildren.Current;
				if (sqlChild is SqlText)
				{
					SqlText sqlText = (SqlText)sqlChild;
					string text = sqlText.Text;
					if (sqlText.IsWhiteSpace)
					{
						buffer.Append(text);
					}
					else
					{
						buffer.Append(" ");
						buffer.Append(text);
						ParameterProperty[] parameters = sqlText.Parameters;
						if (parameters != null)
						{
							int num = parameters.Length;
							for (int i = 0; i < num; i++)
							{
								ctx.AddParameterMapping(parameters[i]);
							}
						}
					}
				}
				else if (sqlChild is SqlTag)
				{
					SqlTag sqlTag = (SqlTag)sqlChild;
					ISqlTagHandler handler = sqlTag.Handler;
					int num2;
					do
					{
						System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
						num2 = handler.DoStartFragment(ctx, sqlTag, parameterObject);
						if (num2 != 0)
						{
							if (ctx.IsOverridePrepend && ctx.FirstNonDynamicTagWithPrepend == null && sqlTag.IsPrependAvailable && !(sqlTag.Handler is DynamicTagHandler))
							{
								ctx.FirstNonDynamicTagWithPrepend = sqlTag;
							}
							this.ProcessBodyChildren(request, ctx, parameterObject, sqlTag.GetChildrenEnumerator(), stringBuilder);
							num2 = handler.DoEndFragment(ctx, sqlTag, parameterObject, stringBuilder);
							handler.DoPrepend(ctx, sqlTag, parameterObject, stringBuilder);
							if (num2 != 0)
							{
								if (stringBuilder.Length > 0)
								{
									if (handler.IsPostParseRequired)
									{
										SqlText sqlText = this._paramParser.ParseInlineParameterMap(request, null, stringBuilder.ToString());
										buffer.Append(sqlText.Text);
										ParameterProperty[] parameters2 = sqlText.Parameters;
										if (parameters2 != null)
										{
											int num = parameters2.Length;
											for (int i = 0; i < num; i++)
											{
												ctx.AddParameterMapping(parameters2[i]);
											}
										}
									}
									else
									{
										buffer.Append(" ");
										buffer.Append(stringBuilder.ToString());
									}
									if (sqlTag.IsPrependAvailable && sqlTag == ctx.FirstNonDynamicTagWithPrepend)
									{
										ctx.IsOverridePrepend = false;
									}
								}
							}
						}
					}
					while (num2 == 2);
				}
			}
		}

		private PreparedStatement BuildPreparedStatement(ISqlMapSession session, RequestScope request, string sqlStatement)
		{
			PreparedStatementFactory preparedStatementFactory = new PreparedStatementFactory(session, request, this._statement, sqlStatement);
			return preparedStatementFactory.Prepare();
		}
	}
}
