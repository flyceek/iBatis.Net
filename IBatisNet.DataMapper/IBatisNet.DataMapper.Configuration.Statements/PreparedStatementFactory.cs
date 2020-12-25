using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	public class PreparedStatementFactory
	{
		private PreparedStatement _preparedStatement = null;

		private string _parameterPrefix = string.Empty;

		private IStatement _statement = null;

		private ISqlMapSession _session = null;

		private string _commandText = string.Empty;

		private RequestScope _request = null;

		private HybridDictionary _propertyDbParameterMap = new HybridDictionary();

		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public PreparedStatementFactory(ISqlMapSession session, RequestScope request, IStatement statement, string commandText)
		{
			this._session = session;
			this._request = request;
			this._statement = statement;
			this._commandText = commandText;
		}

		public PreparedStatement Prepare()
		{
			this._preparedStatement = new PreparedStatement();
			this._parameterPrefix = this._session.DataSource.DbProvider.ParameterPrefix;
			this._preparedStatement.PreparedSql = this._commandText;
			if (this._statement.CommandType == CommandType.Text)
			{
				if (this._request.ParameterMap != null)
				{
					this.CreateParametersForTextCommand();
					this.EvaluateParameterMap();
				}
			}
			else if (this._statement.CommandType == CommandType.StoredProcedure)
			{
				if (this._request.ParameterMap == null)
				{
					throw new DataMapperException("A procedure statement tag must have a parameterMap attribute, which is not the case for the procedure '" + this._statement.Id + ".");
				}
				if (this._session.DataSource.DbProvider.UseDeriveParameters)
				{
					this.DiscoverParameter(this._session);
				}
				else
				{
					this.CreateParametersForProcedureCommand();
				}
				if (this._session.DataSource.DbProvider.IsObdc)
				{
					System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("{ call ");
					stringBuilder.Append(this._commandText);
					if (this._preparedStatement.DbParameters.Length > 0)
					{
						stringBuilder.Append(" (");
						int num = this._preparedStatement.DbParameters.Length - 1;
						for (int i = 0; i < num; i++)
						{
							stringBuilder.Append("?,");
						}
						stringBuilder.Append("?) }");
					}
					this._preparedStatement.PreparedSql = stringBuilder.ToString();
				}
			}
			if (PreparedStatementFactory._logger.IsDebugEnabled)
			{
				PreparedStatementFactory._logger.Debug(string.Concat(new string[]
				{
					"Statement Id: [",
					this._statement.Id,
					"] Prepared SQL: [",
					this._preparedStatement.PreparedSql,
					"]"
				}));
			}
			return this._preparedStatement;
		}

		private void DiscoverParameter(ISqlMapSession session)
		{
			IDataParameter[] spParameterSet = session.SqlMapper.DBHelperParameterCache.GetSpParameterSet(session, this._commandText);
			this._preparedStatement.DbParameters = new IDbDataParameter[spParameterSet.Length];
			int length = session.DataSource.DbProvider.ParameterPrefix.Length;
			for (int i = 0; i < spParameterSet.Length; i++)
			{
				IDbDataParameter dbDataParameter = (IDbDataParameter)spParameterSet[i];
				if (!session.DataSource.DbProvider.UseParameterPrefixInParameter)
				{
					if (dbDataParameter.ParameterName.StartsWith(session.DataSource.DbProvider.ParameterPrefix))
					{
						dbDataParameter.ParameterName = dbDataParameter.ParameterName.Substring(length);
					}
				}
				this._preparedStatement.DbParametersName.Add(dbDataParameter.ParameterName);
				this._preparedStatement.DbParameters[i] = dbDataParameter;
			}
			IDbDataParameter[] array = new IDbDataParameter[spParameterSet.Length];
			for (int i = 0; i < this._statement.ParameterMap.Properties.Count; i++)
			{
				array[i] = this.Search(session, this._preparedStatement.DbParameters, this._statement.ParameterMap.Properties[i], i);
			}
			this._preparedStatement.DbParameters = array;
		}

		private IDbDataParameter Search(ISqlMapSession session, IDbDataParameter[] parameters, ParameterProperty property, int index)
		{
			IDbDataParameter result;
			if (property.ColumnName.Length > 0)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					string text = parameters[i].ParameterName;
					if (session.DataSource.DbProvider.UseParameterPrefixInParameter)
					{
						if (text.StartsWith(session.DataSource.DbProvider.ParameterPrefix))
						{
							int length = session.DataSource.DbProvider.ParameterPrefix.Length;
							text = text.Substring(length);
						}
					}
					if (property.ColumnName.Equals(text))
					{
						result = parameters[i];
						return result;
					}
				}
				throw new System.IndexOutOfRangeException(string.Concat(new string[]
				{
					"The parameter '",
					property.ColumnName,
					"' does not exist in the stored procedure '",
					this._statement.Id,
					"'. Check your parameterMap."
				}));
			}
			result = parameters[index];
			return result;
		}

		private void CreateParametersForTextCommand()
		{
			string parameterName = string.Empty;
			string parameterDbTypeProperty = this._session.DataSource.DbProvider.ParameterDbTypeProperty;
			System.Type parameterDbType = this._session.DataSource.DbProvider.ParameterDbType;
			ParameterPropertyCollection parameterPropertyCollection;
			if (this._session.DataSource.DbProvider.UsePositionalParameters)
			{
				parameterPropertyCollection = this._request.ParameterMap.Properties;
			}
			else
			{
				parameterPropertyCollection = this._request.ParameterMap.PropertiesList;
			}
			this._preparedStatement.DbParameters = new IDbDataParameter[parameterPropertyCollection.Count];
			for (int i = 0; i < parameterPropertyCollection.Count; i++)
			{
				ParameterProperty parameterProperty = parameterPropertyCollection[i];
				if (this._session.DataSource.DbProvider.UseParameterPrefixInParameter)
				{
					parameterName = this._parameterPrefix + "param" + i;
				}
				else
				{
					parameterName = "param" + i;
				}
				IDbDataParameter dbDataParameter = this._session.CreateDataParameter();
				if (parameterProperty.DbType != null && parameterProperty.DbType.Length > 0)
				{
					object memberValue = System.Enum.Parse(parameterDbType, parameterProperty.DbType, true);
					ObjectProbe.SetMemberValue(dbDataParameter, parameterDbTypeProperty, memberValue, this._request.DataExchangeFactory.ObjectFactory, this._request.DataExchangeFactory.AccessorFactory);
				}
				if (this._session.DataSource.DbProvider.SetDbParameterSize)
				{
					if (parameterProperty.Size != -1)
					{
						dbDataParameter.Size = parameterProperty.Size;
					}
				}
				if (this._session.DataSource.DbProvider.SetDbParameterPrecision)
				{
					dbDataParameter.Precision = parameterProperty.Precision;
				}
				if (this._session.DataSource.DbProvider.SetDbParameterScale)
				{
					dbDataParameter.Scale = parameterProperty.Scale;
				}
				dbDataParameter.Direction = parameterProperty.Direction;
				dbDataParameter.ParameterName = parameterName;
				this._preparedStatement.DbParametersName.Add(parameterProperty.PropertyName);
				this._preparedStatement.DbParameters[i] = dbDataParameter;
				if (!this._session.DataSource.DbProvider.UsePositionalParameters)
				{
					this._propertyDbParameterMap.Add(parameterProperty, dbDataParameter);
				}
			}
		}

		private void CreateParametersForProcedureCommand()
		{
			string parameterName = string.Empty;
			string parameterDbTypeProperty = this._session.DataSource.DbProvider.ParameterDbTypeProperty;
			System.Type parameterDbType = this._session.DataSource.DbProvider.ParameterDbType;
			ParameterPropertyCollection parameterPropertyCollection;
			if (this._session.DataSource.DbProvider.UsePositionalParameters)
			{
				parameterPropertyCollection = this._request.ParameterMap.Properties;
			}
			else
			{
				parameterPropertyCollection = this._request.ParameterMap.PropertiesList;
			}
			this._preparedStatement.DbParameters = new IDbDataParameter[parameterPropertyCollection.Count];
			for (int i = 0; i < parameterPropertyCollection.Count; i++)
			{
				ParameterProperty parameterProperty = parameterPropertyCollection[i];
				if (this._session.DataSource.DbProvider.UseParameterPrefixInParameter)
				{
					parameterName = this._parameterPrefix + parameterProperty.ColumnName;
				}
				else
				{
					parameterName = parameterProperty.ColumnName;
				}
				IDbDataParameter dbDataParameter = this._session.CreateCommand(this._statement.CommandType).CreateParameter();
				if (parameterProperty.DbType != null && parameterProperty.DbType.Length > 0)
				{
					object memberValue = System.Enum.Parse(parameterDbType, parameterProperty.DbType, true);
					ObjectProbe.SetMemberValue(dbDataParameter, parameterDbTypeProperty, memberValue, this._request.DataExchangeFactory.ObjectFactory, this._request.DataExchangeFactory.AccessorFactory);
				}
				if (this._session.DataSource.DbProvider.SetDbParameterSize)
				{
					if (parameterProperty.Size != -1)
					{
						dbDataParameter.Size = parameterProperty.Size;
					}
				}
				if (this._session.DataSource.DbProvider.SetDbParameterPrecision)
				{
					dbDataParameter.Precision = parameterProperty.Precision;
				}
				if (this._session.DataSource.DbProvider.SetDbParameterScale)
				{
					dbDataParameter.Scale = parameterProperty.Scale;
				}
				dbDataParameter.Direction = parameterProperty.Direction;
				dbDataParameter.ParameterName = parameterName;
				this._preparedStatement.DbParametersName.Add(parameterProperty.PropertyName);
				this._preparedStatement.DbParameters[i] = dbDataParameter;
				if (!this._session.DataSource.DbProvider.UsePositionalParameters)
				{
					this._propertyDbParameterMap.Add(parameterProperty, dbDataParameter);
				}
			}
		}

		private void EvaluateParameterMap()
		{
			string text = "?";
			int num = 0;
			string value = string.Empty;
			StringTokenizer stringTokenizer = new StringTokenizer(this._commandText, text, true);
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			System.Collections.IEnumerator enumerator = stringTokenizer.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string value2 = (string)enumerator.Current;
				if (text.Equals(value2))
				{
					ParameterProperty key = this._request.ParameterMap.Properties[num];
					if (this._session.DataSource.DbProvider.UsePositionalParameters)
					{
						if (this._parameterPrefix.Equals(":"))
						{
							value = ":" + num;
						}
						else
						{
							value = "?";
						}
					}
					else
					{
						IDataParameter dataParameter = (IDataParameter)this._propertyDbParameterMap[key];
						if (this._session.DataSource.DbProvider.UseParameterPrefixInParameter)
						{
							if (this._session.DataSource.DbProvider.Name.IndexOf("ByteFx") >= 0)
							{
								value = this._parameterPrefix + dataParameter.ParameterName;
							}
							else
							{
								value = dataParameter.ParameterName;
							}
						}
						else
						{
							value = this._parameterPrefix + dataParameter.ParameterName;
						}
					}
					stringBuilder.Append(" ");
					stringBuilder.Append(value);
					value = string.Empty;
					num++;
				}
				else
				{
					stringBuilder.Append(value2);
				}
			}
			this._preparedStatement.PreparedSql = stringBuilder.ToString();
		}
	}
}
