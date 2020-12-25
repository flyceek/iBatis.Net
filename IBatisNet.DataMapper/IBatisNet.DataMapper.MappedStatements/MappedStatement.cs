using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Commands;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.MappedStatements.PostSelectStrategy;
using IBatisNet.DataMapper.MappedStatements.ResultStrategy;
using IBatisNet.DataMapper.Scope;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IBatisNet.DataMapper.MappedStatements
{
	public class MappedStatement : IMappedStatement
	{
		internal const int NO_MAXIMUM_RESULTS = -1;

		internal const int NO_SKIPPED_RESULTS = -1;

		private IStatement _statement = null;

		private ISqlMapper _sqlMap = null;

		private IPreparedCommand _preparedCommand = null;

		private IResultStrategy _resultStrategy = null;

		public event ExecuteEventHandler Execute;

		public IPreparedCommand PreparedCommand
		{
			get
			{
				return this._preparedCommand;
			}
		}

		public string Id
		{
			get
			{
				return this._statement.Id;
			}
		}

		public IStatement Statement
		{
			get
			{
				return this._statement;
			}
		}

		public ISqlMapper SqlMap
		{
			get
			{
				return this._sqlMap;
			}
		}

		internal MappedStatement(ISqlMapper sqlMap, IStatement statement)
		{
			this._sqlMap = sqlMap;
			this._statement = statement;
			this._preparedCommand = PreparedCommandFactory.GetPreparedCommand(false);
			this._resultStrategy = ResultStrategyFactory.Get(this._statement);
		}

		private void RetrieveOutputParameters(RequestScope request, ISqlMapSession session, IDbCommand command, object result)
		{
			if (request.ParameterMap != null)
			{
				int count = request.ParameterMap.PropertiesList.Count;
				for (int i = 0; i < count; i++)
				{
					ParameterProperty property = request.ParameterMap.GetProperty(i);
					if (property.Direction == ParameterDirection.Output || property.Direction == ParameterDirection.InputOutput)
					{
						string parameterName = string.Empty;
						if (!session.DataSource.DbProvider.UseParameterPrefixInParameter)
						{
							parameterName = property.ColumnName;
						}
						else
						{
							parameterName = session.DataSource.DbProvider.ParameterPrefix + property.ColumnName;
						}
						if (property.TypeHandler == null)
						{
							lock (property)
							{
								if (property.TypeHandler == null)
								{
									System.Type memberTypeForGetter = ObjectProbe.GetMemberTypeForGetter(result, property.PropertyName);
									property.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(memberTypeForGetter);
								}
							}
						}
						IDataParameter dataParameter = (IDataParameter)command.Parameters[parameterName];
						object value = dataParameter.Value;
						bool flag2 = value == System.DBNull.Value;
						object obj2;
						if (flag2)
						{
							if (property.HasNullValue)
							{
								obj2 = property.TypeHandler.ValueOf(property.GetAccessor.MemberType, property.NullValue);
							}
							else
							{
								obj2 = property.TypeHandler.NullValue;
							}
						}
						else
						{
							obj2 = property.TypeHandler.GetDataBaseValue(dataParameter.Value, result.GetType());
						}
						request.IsRowDataFound = (request.IsRowDataFound || obj2 != null);
						request.ParameterMap.SetOutputParameter(ref result, property, obj2);
					}
				}
			}
		}

		public virtual object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
		{
			return this.ExecuteQueryForObject(session, parameterObject, null);
		}

		public virtual object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForObject(requestScope, session, parameterObject, resultObject);
		}

		internal object RunQueryForObject(RequestScope request, ISqlMapSession session, object parameterObject, object resultObject)
		{
			object result = resultObject;
			using (IDbCommand iDbCommand = request.IDbCommand)
			{
				IDataReader dataReader = iDbCommand.ExecuteReader();
				try
				{
					while (dataReader.Read())
					{
						object obj = this._resultStrategy.Process(request, ref dataReader, resultObject);
						if (obj != BaseStrategy.SKIP)
						{
							result = obj;
						}
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					dataReader.Close();
					dataReader.Dispose();
				}
				this.ExecutePostSelect(request);
				this.RetrieveOutputParameters(request, session, iDbCommand, parameterObject);
			}
			this.RaiseExecuteEvent();
			return result;
		}

		public virtual T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject)
		{
			return this.ExecuteQueryForObject<T>(session, parameterObject, default(T));
		}

		public virtual T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject, T resultObject)
		{
			T t = default(T);
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForObject<T>(requestScope, session, parameterObject, resultObject);
		}

		internal T RunQueryForObject<T>(RequestScope request, ISqlMapSession session, object parameterObject, T resultObject)
		{
			T result = resultObject;
			using (IDbCommand iDbCommand = request.IDbCommand)
			{
				IDataReader dataReader = iDbCommand.ExecuteReader();
				try
				{
					while (dataReader.Read())
					{
						object obj = this._resultStrategy.Process(request, ref dataReader, resultObject);
						if (obj != BaseStrategy.SKIP)
						{
							result = (T)((object)obj);
						}
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					dataReader.Close();
					dataReader.Dispose();
				}
				this.ExecutePostSelect(request);
				this.RetrieveOutputParameters(request, session, iDbCommand, parameterObject);
			}
			this.RaiseExecuteEvent();
			return result;
		}

		public virtual System.Collections.IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			if (rowDelegate == null)
			{
				throw new DataMapperException("A null RowDelegate was passed to QueryForRowDelegate.");
			}
			return this.RunQueryForList(requestScope, session, parameterObject, null, rowDelegate);
		}

		public virtual System.Collections.IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			if (rowDelegate == null)
			{
				throw new DataMapperException("A null DictionaryRowDelegate was passed to QueryForMapWithRowDelegate.");
			}
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForMap(requestScope, session, parameterObject, keyProperty, valueProperty, rowDelegate);
		}

		public virtual System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForList(requestScope, session, parameterObject, null, null);
		}

		public virtual System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForList(requestScope, session, parameterObject, skipResults, maxResults);
		}

		internal System.Collections.IList RunQueryForList(RequestScope request, ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
		{
			System.Collections.IList list = null;
			using (IDbCommand iDbCommand = request.IDbCommand)
			{
				if (this._statement.ListClass == null)
				{
					list = new System.Collections.ArrayList();
				}
				else
				{
					list = this._statement.CreateInstanceOfListClass();
				}
				IDataReader dataReader = iDbCommand.ExecuteReader();
				try
				{
					for (int i = 0; i < skipResults; i++)
					{
						if (!dataReader.Read())
						{
							break;
						}
					}
					int num = 0;
					while ((maxResults == -1 || num < maxResults) && dataReader.Read())
					{
						object obj = this._resultStrategy.Process(request, ref dataReader, null);
						if (obj != BaseStrategy.SKIP)
						{
							list.Add(obj);
						}
						num++;
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					dataReader.Close();
					dataReader.Dispose();
				}
				this.ExecutePostSelect(request);
				this.RetrieveOutputParameters(request, session, iDbCommand, parameterObject);
			}
			return list;
		}

		internal System.Collections.IList RunQueryForList(RequestScope request, ISqlMapSession session, object parameterObject, System.Collections.IList resultObject, RowDelegate rowDelegate)
		{
			System.Collections.IList list = resultObject;
			using (IDbCommand iDbCommand = request.IDbCommand)
			{
				if (resultObject == null)
				{
					if (this._statement.ListClass == null)
					{
						list = new System.Collections.ArrayList();
					}
					else
					{
						list = this._statement.CreateInstanceOfListClass();
					}
				}
				IDataReader dataReader = iDbCommand.ExecuteReader();
				try
				{
					do
					{
						if (rowDelegate == null)
						{
							while (dataReader.Read())
							{
								object obj = this._resultStrategy.Process(request, ref dataReader, null);
								if (obj != BaseStrategy.SKIP)
								{
									list.Add(obj);
								}
							}
						}
						else
						{
							while (dataReader.Read())
							{
								object obj = this._resultStrategy.Process(request, ref dataReader, null);
								rowDelegate(obj, parameterObject, list);
							}
						}
					}
					while (dataReader.NextResult());
				}
				catch
				{
					throw;
				}
				finally
				{
					dataReader.Close();
					dataReader.Dispose();
				}
				this.ExecutePostSelect(request);
				this.RetrieveOutputParameters(request, session, iDbCommand, parameterObject);
			}
			return list;
		}

		public virtual void ExecuteQueryForList(ISqlMapSession session, object parameterObject, System.Collections.IList resultObject)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			this.RunQueryForList(requestScope, session, parameterObject, resultObject, null);
		}

		public virtual System.Collections.Generic.IList<T> ExecuteQueryForRowDelegate<T>(ISqlMapSession session, object parameterObject, RowDelegate<T> rowDelegate)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			if (rowDelegate == null)
			{
				throw new DataMapperException("A null RowDelegate was passed to QueryForRowDelegate.");
			}
			return this.RunQueryForList<T>(requestScope, session, parameterObject, null, rowDelegate);
		}

		public virtual System.Collections.Generic.IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForList<T>(requestScope, session, parameterObject, null, null);
		}

		public virtual System.Collections.Generic.IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForList<T>(requestScope, session, parameterObject, skipResults, maxResults);
		}

		internal System.Collections.Generic.IList<T> RunQueryForList<T>(RequestScope request, ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
		{
			System.Collections.Generic.IList<T> list = null;
			using (IDbCommand iDbCommand = request.IDbCommand)
			{
				if (this._statement.ListClass == null)
				{
					list = new System.Collections.Generic.List<T>();
				}
				else
				{
					list = this._statement.CreateInstanceOfGenericListClass<T>();
				}
				IDataReader dataReader = iDbCommand.ExecuteReader();
				try
				{
					for (int i = 0; i < skipResults; i++)
					{
						if (!dataReader.Read())
						{
							break;
						}
					}
					int num = 0;
					while ((maxResults == -1 || num < maxResults) && dataReader.Read())
					{
						object obj = this._resultStrategy.Process(request, ref dataReader, null);
						if (obj != BaseStrategy.SKIP)
						{
							list.Add((T)((object)obj));
						}
						num++;
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					dataReader.Close();
					dataReader.Dispose();
				}
				this.ExecutePostSelect(request);
				this.RetrieveOutputParameters(request, session, iDbCommand, parameterObject);
			}
			return list;
		}

		internal System.Collections.Generic.IList<T> RunQueryForList<T>(RequestScope request, ISqlMapSession session, object parameterObject, System.Collections.Generic.IList<T> resultObject, RowDelegate<T> rowDelegate)
		{
			System.Collections.Generic.IList<T> list = resultObject;
			using (IDbCommand iDbCommand = request.IDbCommand)
			{
				if (resultObject == null)
				{
					if (this._statement.ListClass == null)
					{
						list = new System.Collections.Generic.List<T>();
					}
					else
					{
						list = this._statement.CreateInstanceOfGenericListClass<T>();
					}
				}
				IDataReader dataReader = iDbCommand.ExecuteReader();
				try
				{
					do
					{
						if (rowDelegate == null)
						{
							while (dataReader.Read())
							{
								object obj = this._resultStrategy.Process(request, ref dataReader, null);
								if (obj != BaseStrategy.SKIP)
								{
									list.Add((T)((object)obj));
								}
							}
						}
						else
						{
							while (dataReader.Read())
							{
								T t = (T)((object)this._resultStrategy.Process(request, ref dataReader, null));
								rowDelegate(t, parameterObject, list);
							}
						}
					}
					while (dataReader.NextResult());
				}
				catch
				{
					throw;
				}
				finally
				{
					dataReader.Close();
					dataReader.Dispose();
				}
				this.ExecutePostSelect(request);
				this.RetrieveOutputParameters(request, session, iDbCommand, parameterObject);
			}
			return list;
		}

		public virtual void ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, System.Collections.Generic.IList<T> resultObject)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			this.RunQueryForList<T>(requestScope, session, parameterObject, resultObject, null);
		}

		public virtual int ExecuteUpdate(ISqlMapSession session, object parameterObject)
		{
			int result = 0;
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			using (IDbCommand iDbCommand = requestScope.IDbCommand)
			{
				result = iDbCommand.ExecuteNonQuery();
				this.RetrieveOutputParameters(requestScope, session, iDbCommand, parameterObject);
			}
			this.RaiseExecuteEvent();
			return result;
		}

		public virtual object ExecuteInsert(ISqlMapSession session, object parameterObject)
		{
			object obj = null;
			SelectKey selectKey = null;
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			if (this._statement is Insert)
			{
				selectKey = ((Insert)this._statement).SelectKey;
			}
			if (selectKey != null && !selectKey.isAfter)
			{
				IMappedStatement mappedStatement = this._sqlMap.GetMappedStatement(selectKey.Id);
				obj = mappedStatement.ExecuteQueryForObject(session, parameterObject);
				ObjectProbe.SetMemberValue(parameterObject, selectKey.PropertyName, obj, requestScope.DataExchangeFactory.ObjectFactory, requestScope.DataExchangeFactory.AccessorFactory);
			}
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			using (IDbCommand iDbCommand = requestScope.IDbCommand)
			{
				if (this._statement is Insert)
				{
					iDbCommand.ExecuteNonQuery();
				}
				else if (this._statement is Procedure && this._statement.ResultClass != null && this._sqlMap.TypeHandlerFactory.IsSimpleType(this._statement.ResultClass))
				{
					IDataParameter dataParameter = iDbCommand.CreateParameter();
					dataParameter.Direction = ParameterDirection.ReturnValue;
					iDbCommand.Parameters.Add(dataParameter);
					iDbCommand.ExecuteNonQuery();
					obj = dataParameter.Value;
					ITypeHandler typeHandler = this._sqlMap.TypeHandlerFactory.GetTypeHandler(this._statement.ResultClass);
					obj = typeHandler.GetDataBaseValue(obj, this._statement.ResultClass);
				}
				else
				{
					obj = iDbCommand.ExecuteScalar();
					if (this._statement.ResultClass != null && this._sqlMap.TypeHandlerFactory.IsSimpleType(this._statement.ResultClass))
					{
						ITypeHandler typeHandler = this._sqlMap.TypeHandlerFactory.GetTypeHandler(this._statement.ResultClass);
						obj = typeHandler.GetDataBaseValue(obj, this._statement.ResultClass);
					}
				}
				if (selectKey != null && selectKey.isAfter)
				{
					IMappedStatement mappedStatement = this._sqlMap.GetMappedStatement(selectKey.Id);
					obj = mappedStatement.ExecuteQueryForObject(session, parameterObject);
					ObjectProbe.SetMemberValue(parameterObject, selectKey.PropertyName, obj, requestScope.DataExchangeFactory.ObjectFactory, requestScope.DataExchangeFactory.AccessorFactory);
				}
				this.RetrieveOutputParameters(requestScope, session, iDbCommand, parameterObject);
			}
			this.RaiseExecuteEvent();
			return obj;
		}

		public virtual System.Collections.IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForMap(requestScope, session, parameterObject, keyProperty, valueProperty, null);
		}

		internal System.Collections.IDictionary RunQueryForMap(RequestScope request, ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
		{
			System.Collections.IDictionary dictionary = new System.Collections.Hashtable();
			using (IDbCommand iDbCommand = request.IDbCommand)
			{
				IDataReader dataReader = iDbCommand.ExecuteReader();
				try
				{
					if (rowDelegate == null)
					{
						while (dataReader.Read())
						{
							object obj = this._resultStrategy.Process(request, ref dataReader, null);
							object memberValue = ObjectProbe.GetMemberValue(obj, keyProperty, request.DataExchangeFactory.AccessorFactory);
							object value = obj;
							if (valueProperty != null)
							{
								value = ObjectProbe.GetMemberValue(obj, valueProperty, request.DataExchangeFactory.AccessorFactory);
							}
							dictionary.Add(memberValue, value);
						}
					}
					else
					{
						while (dataReader.Read())
						{
							object obj = this._resultStrategy.Process(request, ref dataReader, null);
							object memberValue = ObjectProbe.GetMemberValue(obj, keyProperty, request.DataExchangeFactory.AccessorFactory);
							object value = obj;
							if (valueProperty != null)
							{
								value = ObjectProbe.GetMemberValue(obj, valueProperty, request.DataExchangeFactory.AccessorFactory);
							}
							rowDelegate(memberValue, value, parameterObject, dictionary);
						}
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					dataReader.Close();
					dataReader.Dispose();
				}
				this.ExecutePostSelect(request);
			}
			return dictionary;
		}

		public virtual System.Collections.Generic.IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForDictionary<K, V>(requestScope, session, parameterObject, keyProperty, valueProperty, null);
		}

		public virtual System.Collections.Generic.IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
		{
			RequestScope requestScope = this._statement.Sql.GetRequestScope(this, parameterObject, session);
			if (rowDelegate == null)
			{
				throw new DataMapperException("A null DictionaryRowDelegate was passed to QueryForDictionary.");
			}
			this._preparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			return this.RunQueryForDictionary<K, V>(requestScope, session, parameterObject, keyProperty, valueProperty, rowDelegate);
		}

		internal System.Collections.Generic.IDictionary<K, V> RunQueryForDictionary<K, V>(RequestScope request, ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
		{
			System.Collections.Generic.IDictionary<K, V> dictionary = new System.Collections.Generic.Dictionary<K, V>();
			using (IDbCommand iDbCommand = request.IDbCommand)
			{
				IDataReader dataReader = iDbCommand.ExecuteReader();
				try
				{
					if (rowDelegate == null)
					{
						while (dataReader.Read())
						{
							object obj = this._resultStrategy.Process(request, ref dataReader, null);
							K key = (K)((object)ObjectProbe.GetMemberValue(obj, keyProperty, request.DataExchangeFactory.AccessorFactory));
							V value = default(V);
							if (valueProperty != null)
							{
								value = (V)((object)ObjectProbe.GetMemberValue(obj, valueProperty, request.DataExchangeFactory.AccessorFactory));
							}
							else
							{
								value = (V)((object)obj);
							}
							dictionary.Add(key, value);
						}
					}
					else
					{
						while (dataReader.Read())
						{
							object obj = this._resultStrategy.Process(request, ref dataReader, null);
							K key = (K)((object)ObjectProbe.GetMemberValue(obj, keyProperty, request.DataExchangeFactory.AccessorFactory));
							V value = default(V);
							if (valueProperty != null)
							{
								value = (V)((object)ObjectProbe.GetMemberValue(obj, valueProperty, request.DataExchangeFactory.AccessorFactory));
							}
							else
							{
								value = (V)((object)obj);
							}
							rowDelegate(key, value, parameterObject, dictionary);
						}
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					dataReader.Close();
					dataReader.Dispose();
				}
				this.ExecutePostSelect(request);
			}
			return dictionary;
		}

		private void ExecutePostSelect(RequestScope request)
		{
			while (request.QueueSelect.Count > 0)
			{
				PostBindind postBindind = request.QueueSelect.Dequeue() as PostBindind;
				PostSelectStrategyFactory.Get(postBindind.Method).Execute(postBindind, request);
			}
		}

		private void RaiseExecuteEvent()
		{
			ExecuteEventArgs executeEventArgs = new ExecuteEventArgs();
			executeEventArgs.StatementName = this._statement.Id;
			if (this.Execute != null)
			{
				this.Execute(this, executeEventArgs);
			}
		}

		public override string ToString()
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append("\tMappedStatement: " + this.Id);
			stringBuilder.Append(System.Environment.NewLine);
			if (this._statement.ParameterMap != null)
			{
				stringBuilder.Append(this._statement.ParameterMap.Id);
			}
			return stringBuilder.ToString();
		}
	}
}
