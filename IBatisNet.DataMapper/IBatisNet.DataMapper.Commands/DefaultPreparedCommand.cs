using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Text;

namespace IBatisNet.DataMapper.Commands
{
	internal class DefaultPreparedCommand : IPreparedCommand
	{
		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public void Create(RequestScope request, ISqlMapSession session, IStatement statement, object parameterObject)
		{
			request.IDbCommand = new DbCommandDecorator(session.CreateCommand(statement.CommandType), request);
			request.IDbCommand.CommandText = request.PreparedStatement.PreparedSql;
			if (DefaultPreparedCommand._logger.IsDebugEnabled)
			{
				DefaultPreparedCommand._logger.Debug(string.Concat(new string[]
				{
					"Statement Id: [",
					statement.Id,
					"] PreparedStatement : [",
					request.IDbCommand.CommandText,
					"]"
				}));
			}
			this.ApplyParameterMap(session, request.IDbCommand, request, statement, parameterObject);
		}

		protected virtual void ApplyParameterMap(ISqlMapSession session, IDbCommand command, RequestScope request, IStatement statement, object parameterObject)
		{
			StringCollection dbParametersName = request.PreparedStatement.DbParametersName;
			IDbDataParameter[] dbParameters = request.PreparedStatement.DbParameters;
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			System.Text.StringBuilder stringBuilder2 = new System.Text.StringBuilder();
			int count = dbParametersName.Count;
			for (int i = 0; i < count; i++)
			{
				IDbDataParameter dbDataParameter = dbParameters[i];
				IDbDataParameter dbDataParameter2 = command.CreateParameter();
				ParameterProperty property = request.ParameterMap.GetProperty(i);
				if (DefaultPreparedCommand._logger.IsDebugEnabled)
				{
					stringBuilder.Append(dbDataParameter.ParameterName);
					stringBuilder.Append("=[");
					stringBuilder2.Append(dbDataParameter.ParameterName);
					stringBuilder2.Append("=[");
				}
				if (command.CommandType == CommandType.StoredProcedure)
				{
					if (request.ParameterMap == null)
					{
						throw new DataMapperException("A procedure statement tag must alway have a parameterMap attribute, which is not the case for the procedure '" + statement.Id + "'.");
					}
					if (property.DirectionAttribute.Length == 0)
					{
						property.Direction = dbDataParameter.Direction;
					}
					dbDataParameter.Direction = property.Direction;
				}
				if (DefaultPreparedCommand._logger.IsDebugEnabled)
				{
					stringBuilder.Append(property.PropertyName);
					stringBuilder.Append(",");
				}
				request.ParameterMap.SetParameter(property, dbDataParameter2, parameterObject);
				dbDataParameter2.Direction = dbDataParameter.Direction;
				if (request.ParameterMap != null)
				{
					if (property.DbType != null && property.DbType.Length > 0)
					{
						string parameterDbTypeProperty = session.DataSource.DbProvider.ParameterDbTypeProperty;
						object memberValue = ObjectProbe.GetMemberValue(dbDataParameter, parameterDbTypeProperty, request.DataExchangeFactory.AccessorFactory);
						ObjectProbe.SetMemberValue(dbDataParameter2, parameterDbTypeProperty, memberValue, request.DataExchangeFactory.ObjectFactory, request.DataExchangeFactory.AccessorFactory);
					}
				}
				if (DefaultPreparedCommand._logger.IsDebugEnabled)
				{
					if (dbDataParameter2.Value == System.DBNull.Value)
					{
						stringBuilder.Append("null");
						stringBuilder.Append("], ");
						stringBuilder2.Append("System.DBNull, null");
						stringBuilder2.Append("], ");
					}
					else
					{
						stringBuilder.Append(dbDataParameter2.Value.ToString());
						stringBuilder.Append("], ");
						stringBuilder2.Append(dbDataParameter2.DbType.ToString());
						stringBuilder2.Append(", ");
						stringBuilder2.Append(dbDataParameter2.Value.GetType().ToString());
						stringBuilder2.Append("], ");
					}
				}
				if (session.DataSource.DbProvider.SetDbParameterSize)
				{
					if (dbDataParameter.Size > 0)
					{
						dbDataParameter2.Size = dbDataParameter.Size;
					}
				}
				if (session.DataSource.DbProvider.SetDbParameterPrecision)
				{
					dbDataParameter2.Precision = dbDataParameter.Precision;
				}
				if (session.DataSource.DbProvider.SetDbParameterScale)
				{
					dbDataParameter2.Scale = dbDataParameter.Scale;
				}
				dbDataParameter2.ParameterName = dbDataParameter.ParameterName;
				command.Parameters.Add(dbDataParameter2);
			}
			if (DefaultPreparedCommand._logger.IsDebugEnabled && dbParametersName.Count > 0)
			{
				DefaultPreparedCommand._logger.Debug(string.Concat(new string[]
				{
					"Statement Id: [",
					statement.Id,
					"] Parameters: [",
					stringBuilder.ToString(0, stringBuilder.Length - 2),
					"]"
				}));
				DefaultPreparedCommand._logger.Debug(string.Concat(new string[]
				{
					"Statement Id: [",
					statement.Id,
					"] Types: [",
					stringBuilder2.ToString(0, stringBuilder2.Length - 2),
					"]"
				}));
			}
		}
	}
}
