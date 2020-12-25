using IBatisNet.Common;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Cache;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.SessionStore;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;

namespace IBatisNet.DataMapper
{
	public class SqlMapper : ISqlMapper
	{
		private HybridDictionary _mappedStatements = new HybridDictionary();

		private HybridDictionary _resultMaps = new HybridDictionary();

		private HybridDictionary _parameterMaps = new HybridDictionary();

		private IDataSource _dataSource = null;

		private HybridDictionary _cacheMaps = new HybridDictionary();

		private TypeHandlerFactory _typeHandlerFactory = null;

		private DBHelperParameterCache _dbHelperParameterCache = null;

		private bool _cacheModelsEnabled = false;

		private string _id = string.Empty;

		private ISessionStore _sessionStore = null;

		private IObjectFactory _objectFactory = null;

		private AccessorFactory _accessorFactory = null;

		private DataExchangeFactory _dataExchangeFactory = null;

		public string Id
		{
			get
			{
				return this._id;
			}
		}

		public ISessionStore SessionStore
		{
			set
			{
				this._sessionStore = value;
			}
		}

		public ISqlMapSession LocalSession
		{
			get
			{
				return this._sessionStore.LocalSession;
			}
		}

		public bool IsSessionStarted
		{
			get
			{
				return this._sessionStore.LocalSession != null;
			}
		}

		public DBHelperParameterCache DBHelperParameterCache
		{
			get
			{
				return this._dbHelperParameterCache;
			}
		}

		public DataExchangeFactory DataExchangeFactory
		{
			get
			{
				return this._dataExchangeFactory;
			}
		}

		public TypeHandlerFactory TypeHandlerFactory
		{
			get
			{
				return this._typeHandlerFactory;
			}
		}

		public IObjectFactory ObjectFactory
		{
			get
			{
				return this._objectFactory;
			}
		}

		public AccessorFactory AccessorFactory
		{
			get
			{
				return this._accessorFactory;
			}
		}

		public bool IsCacheModelsEnabled
		{
			get
			{
				return this._cacheModelsEnabled;
			}
			set
			{
				this._cacheModelsEnabled = value;
			}
		}

		public HybridDictionary MappedStatements
		{
			get
			{
				return this._mappedStatements;
			}
		}

		public HybridDictionary ParameterMaps
		{
			get
			{
				return this._parameterMaps;
			}
		}

		public HybridDictionary ResultMaps
		{
			get
			{
				return this._resultMaps;
			}
		}

		public IDataSource DataSource
		{
			get
			{
				return this._dataSource;
			}
			set
			{
				this._dataSource = value;
			}
		}

		public SqlMapper(IObjectFactory objectFactory, AccessorFactory accessorFactory)
		{
			this._typeHandlerFactory = new TypeHandlerFactory();
			this._dbHelperParameterCache = new DBHelperParameterCache();
			this._objectFactory = objectFactory;
			this._accessorFactory = accessorFactory;
			this._dataExchangeFactory = new DataExchangeFactory(this._typeHandlerFactory, this._objectFactory, accessorFactory);
			this._id = HashCodeProvider.GetIdentityHashCode(this).ToString();
			this._sessionStore = SessionStoreFactory.GetSessionStore(this._id);
		}

		public ISqlMapSession OpenConnection()
		{
			if (this._sessionStore.LocalSession != null)
			{
				throw new DataMapperException("SqlMap could not invoke OpenConnection(). A connection is already started. Call CloseConnection first.");
			}
			ISqlMapSession sqlMapSession = this.CreateSqlMapSession();
			this._sessionStore.Store(sqlMapSession);
			return sqlMapSession;
		}

		public ISqlMapSession OpenConnection(string connectionString)
		{
			if (this._sessionStore.LocalSession != null)
			{
				throw new DataMapperException("SqlMap could not invoke OpenConnection(). A connection is already started. Call CloseConnection first.");
			}
			ISqlMapSession sqlMapSession = this.CreateSqlMapSession(connectionString);
			this._sessionStore.Store(sqlMapSession);
			return sqlMapSession;
		}

		public void CloseConnection()
		{
			if (this._sessionStore.LocalSession == null)
			{
				throw new DataMapperException("SqlMap could not invoke CloseConnection(). No connection was started. Call OpenConnection() first.");
			}
			try
			{
				ISqlMapSession localSession = this._sessionStore.LocalSession;
				localSession.CloseConnection();
			}
			catch (System.Exception ex)
			{
				throw new DataMapperException("SqlMapper could not CloseConnection(). Cause :" + ex.Message, ex);
			}
			finally
			{
				this._sessionStore.Dispose();
			}
		}

		public ISqlMapSession BeginTransaction()
		{
			if (this._sessionStore.LocalSession != null)
			{
				throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
			}
			ISqlMapSession sqlMapSession = this.CreateSqlMapSession();
			this._sessionStore.Store(sqlMapSession);
			sqlMapSession.BeginTransaction();
			return sqlMapSession;
		}

		public ISqlMapSession BeginTransaction(string connectionString)
		{
			if (this._sessionStore.LocalSession != null)
			{
				throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
			}
			ISqlMapSession sqlMapSession = this.CreateSqlMapSession(connectionString);
			this._sessionStore.Store(sqlMapSession);
			sqlMapSession.BeginTransaction(connectionString);
			return sqlMapSession;
		}

		public ISqlMapSession BeginTransaction(bool openConnection)
		{
			ISqlMapSession sqlMapSession;
			if (openConnection)
			{
				sqlMapSession = this.BeginTransaction();
			}
			else
			{
				sqlMapSession = this._sessionStore.LocalSession;
				if (sqlMapSession == null)
				{
					throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A session must be Open. Call OpenConnection() first.");
				}
				sqlMapSession.BeginTransaction(openConnection);
			}
			return sqlMapSession;
		}

		public ISqlMapSession BeginTransaction(IsolationLevel isolationLevel)
		{
			if (this._sessionStore.LocalSession != null)
			{
				throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
			}
			ISqlMapSession sqlMapSession = this.CreateSqlMapSession();
			this._sessionStore.Store(sqlMapSession);
			sqlMapSession.BeginTransaction(isolationLevel);
			return sqlMapSession;
		}

		public ISqlMapSession BeginTransaction(string connectionString, IsolationLevel isolationLevel)
		{
			if (this._sessionStore.LocalSession != null)
			{
				throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
			}
			ISqlMapSession sqlMapSession = this.CreateSqlMapSession(connectionString);
			this._sessionStore.Store(sqlMapSession);
			sqlMapSession.BeginTransaction(connectionString, isolationLevel);
			return sqlMapSession;
		}

		public ISqlMapSession BeginTransaction(bool openNewConnection, IsolationLevel isolationLevel)
		{
			ISqlMapSession sqlMapSession;
			if (openNewConnection)
			{
				sqlMapSession = this.BeginTransaction(isolationLevel);
			}
			else
			{
				sqlMapSession = this._sessionStore.LocalSession;
				if (sqlMapSession == null)
				{
					throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A session must be Open. Call OpenConnection() first.");
				}
				sqlMapSession.BeginTransaction(openNewConnection, isolationLevel);
			}
			return sqlMapSession;
		}

		public ISqlMapSession BeginTransaction(string connectionString, bool openNewConnection, IsolationLevel isolationLevel)
		{
			ISqlMapSession sqlMapSession;
			if (openNewConnection)
			{
				sqlMapSession = this.BeginTransaction(connectionString, isolationLevel);
			}
			else
			{
				sqlMapSession = this._sessionStore.LocalSession;
				if (sqlMapSession == null)
				{
					throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A session must be Open. Call OpenConnection() first.");
				}
				sqlMapSession.BeginTransaction(connectionString, openNewConnection, isolationLevel);
			}
			return sqlMapSession;
		}

		public void CommitTransaction()
		{
			if (this._sessionStore.LocalSession == null)
			{
				throw new DataMapperException("SqlMap could not invoke CommitTransaction(). No Transaction was started. Call BeginTransaction() first.");
			}
			try
			{
				ISqlMapSession localSession = this._sessionStore.LocalSession;
				localSession.CommitTransaction();
			}
			finally
			{
				this._sessionStore.Dispose();
			}
		}

		public void CommitTransaction(bool closeConnection)
		{
			if (this._sessionStore.LocalSession == null)
			{
				throw new DataMapperException("SqlMap could not invoke CommitTransaction(). No Transaction was started. Call BeginTransaction() first.");
			}
			try
			{
				ISqlMapSession localSession = this._sessionStore.LocalSession;
				localSession.CommitTransaction(closeConnection);
			}
			finally
			{
				if (closeConnection)
				{
					this._sessionStore.Dispose();
				}
			}
		}

		public void RollBackTransaction()
		{
			if (this._sessionStore.LocalSession == null)
			{
				throw new DataMapperException("SqlMap could not invoke RollBackTransaction(). No Transaction was started. Call BeginTransaction() first.");
			}
			try
			{
				ISqlMapSession localSession = this._sessionStore.LocalSession;
				localSession.RollBackTransaction();
			}
			finally
			{
				this._sessionStore.Dispose();
			}
		}

		public void RollBackTransaction(bool closeConnection)
		{
			if (this._sessionStore.LocalSession == null)
			{
				throw new DataMapperException("SqlMap could not invoke RollBackTransaction(). No Transaction was started. Call BeginTransaction() first.");
			}
			try
			{
				ISqlMapSession localSession = this._sessionStore.LocalSession;
				localSession.RollBackTransaction(closeConnection);
			}
			finally
			{
				if (closeConnection)
				{
					this._sessionStore.Dispose();
				}
			}
		}

		public object QueryForObject(string statementName, object parameterObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			object result;
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForObject(sqlMapSession, parameterObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public object QueryForObject(string statementName, object parameterObject, object resultObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			object result = null;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForObject(sqlMapSession, parameterObject, resultObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public T QueryForObject<T>(string statementName, object parameterObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			T result;
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForObject<T>(sqlMapSession, parameterObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public T QueryForObject<T>(string statementName, object parameterObject, T instanceObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			T result = default(T);
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForObject<T>(sqlMapSession, parameterObject, instanceObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public System.Collections.IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty)
		{
			return this.QueryForMap(statementName, parameterObject, keyProperty);
		}

		public System.Collections.IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty, string valueProperty)
		{
			return this.QueryForMap(statementName, parameterObject, keyProperty, valueProperty);
		}

		public System.Collections.IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty)
		{
			return this.QueryForMap(statementName, parameterObject, keyProperty, null);
		}

		public System.Collections.IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty, string valueProperty)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			System.Collections.IDictionary result = null;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForMap(sqlMapSession, parameterObject, keyProperty, valueProperty);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public System.Collections.IList QueryForList(string statementName, object parameterObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			System.Collections.IList result;
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForList(sqlMapSession, parameterObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public System.Collections.IList QueryForList(string statementName, object parameterObject, int skipResults, int maxResults)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			System.Collections.IList result;
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForList(sqlMapSession, parameterObject, skipResults, maxResults);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public void QueryForList(string statementName, object parameterObject, System.Collections.IList resultObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			if (resultObject == null)
			{
				throw new DataMapperException("resultObject parameter must be instantiated before being passed to SqlMapper.QueryForList");
			}
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				mappedStatement.ExecuteQueryForList(sqlMapSession, parameterObject, resultObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
		}

		public System.Collections.Generic.IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			System.Collections.Generic.IDictionary<K, V> result = null;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForDictionary<K, V>(sqlMapSession, parameterObject, keyProperty, valueProperty);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public System.Collections.Generic.IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty)
		{
			return this.QueryForDictionary<K, V>(statementName, parameterObject, keyProperty, null);
		}

		public System.Collections.Generic.IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			System.Collections.Generic.IDictionary<K, V> result = null;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForDictionary<K, V>(sqlMapSession, parameterObject, keyProperty, valueProperty, rowDelegate);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public System.Collections.Generic.IList<T> QueryForList<T>(string statementName, object parameterObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			System.Collections.Generic.IList<T> result;
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForList<T>(sqlMapSession, parameterObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public System.Collections.Generic.IList<T> QueryForList<T>(string statementName, object parameterObject, int skipResults, int maxResults)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			System.Collections.Generic.IList<T> result;
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForList<T>(sqlMapSession, parameterObject, skipResults, maxResults);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public void QueryForList<T>(string statementName, object parameterObject, System.Collections.Generic.IList<T> resultObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			if (resultObject == null)
			{
				throw new DataMapperException("resultObject parameter must be instantiated before being passed to SqlMapper.QueryForList");
			}
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				mappedStatement.ExecuteQueryForList<T>(sqlMapSession, parameterObject, resultObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
		}

		[System.Obsolete("This method will be remove in future version.", false)]
		public PaginatedList QueryForPaginatedList(string statementName, object parameterObject, int pageSize)
		{
			IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
			return new PaginatedList(mappedStatement, parameterObject, pageSize);
		}

		public System.Collections.IList QueryWithRowDelegate(string statementName, object parameterObject, RowDelegate rowDelegate)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			System.Collections.IList result = null;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForRowDelegate(sqlMapSession, parameterObject, rowDelegate);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public System.Collections.Generic.IList<T> QueryWithRowDelegate<T>(string statementName, object parameterObject, RowDelegate<T> rowDelegate)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			System.Collections.Generic.IList<T> result = null;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForRowDelegate<T>(sqlMapSession, parameterObject, rowDelegate);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public System.Collections.IDictionary QueryForMapWithRowDelegate(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			System.Collections.IDictionary result = null;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteQueryForMapWithRowDelegate(sqlMapSession, parameterObject, keyProperty, valueProperty, rowDelegate);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public object Insert(string statementName, object parameterObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			object result = null;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteInsert(sqlMapSession, parameterObject);
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public int Update(string statementName, object parameterObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			int result = 0;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteUpdate(sqlMapSession, parameterObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public int Delete(string statementName, object parameterObject)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._sessionStore.LocalSession;
			int result = 0;
			if (sqlMapSession == null)
			{
				sqlMapSession = this.CreateSqlMapSession();
				flag = true;
			}
			try
			{
				IMappedStatement mappedStatement = this.GetMappedStatement(statementName);
				result = mappedStatement.ExecuteUpdate(sqlMapSession, parameterObject);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		public IMappedStatement GetMappedStatement(string id)
		{
			if (!this._mappedStatements.Contains(id))
			{
				throw new DataMapperException("This SQL map does not contain a MappedStatement named " + id);
			}
			return (IMappedStatement)this._mappedStatements[id];
		}

		public void AddMappedStatement(string key, IMappedStatement mappedStatement)
		{
			if (this._mappedStatements.Contains(key))
			{
				throw new DataMapperException("This SQL map already contains a MappedStatement named " + mappedStatement.Id);
			}
			this._mappedStatements.Add(key, mappedStatement);
		}

		public ParameterMap GetParameterMap(string name)
		{
			if (!this._parameterMaps.Contains(name))
			{
				throw new DataMapperException("This SQL map does not contain an ParameterMap named " + name + ".  ");
			}
			return (ParameterMap)this._parameterMaps[name];
		}

		public void AddParameterMap(ParameterMap parameterMap)
		{
			if (this._parameterMaps.Contains(parameterMap.Id))
			{
				throw new DataMapperException("This SQL map already contains an ParameterMap named " + parameterMap.Id);
			}
			this._parameterMaps.Add(parameterMap.Id, parameterMap);
		}

		public IResultMap GetResultMap(string name)
		{
			if (!this._resultMaps.Contains(name))
			{
				throw new DataMapperException("This SQL map does not contain an ResultMap named " + name);
			}
			return (ResultMap)this._resultMaps[name];
		}

		public void AddResultMap(IResultMap resultMap)
		{
			if (this._resultMaps.Contains(resultMap.Id))
			{
				throw new DataMapperException("This SQL map already contains an ResultMap named " + resultMap.Id);
			}
			this._resultMaps.Add(resultMap.Id, resultMap);
		}

		public void FlushCaches()
		{
			System.Collections.IDictionaryEnumerator enumerator = this._cacheMaps.GetEnumerator();
			while (enumerator.MoveNext())
			{
				((CacheModel)enumerator.Value).Flush();
			}
		}

		public void AddCache(CacheModel cache)
		{
			if (this._cacheMaps.Contains(cache.Id))
			{
				throw new DataMapperException("This SQL map already contains an Cache named " + cache.Id);
			}
			this._cacheMaps.Add(cache.Id, cache);
		}

		public CacheModel GetCache(string name)
		{
			if (!this._cacheMaps.Contains(name))
			{
				throw new DataMapperException("This SQL map does not contain an Cache named " + name);
			}
			return (CacheModel)this._cacheMaps[name];
		}

		public string GetDataCacheStats()
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append(System.Environment.NewLine);
			stringBuilder.Append("Cache Data Statistics");
			stringBuilder.Append(System.Environment.NewLine);
			stringBuilder.Append("=====================");
			stringBuilder.Append(System.Environment.NewLine);
			System.Collections.IDictionaryEnumerator enumerator = this._mappedStatements.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IMappedStatement mappedStatement = (IMappedStatement)enumerator.Value;
				stringBuilder.Append(mappedStatement.Id);
				stringBuilder.Append(": ");
				if (mappedStatement is CachingStatement)
				{
					double dataCacheHitRatio = ((CachingStatement)mappedStatement).GetDataCacheHitRatio();
					if (dataCacheHitRatio != -1.0)
					{
						stringBuilder.Append(System.Math.Round(dataCacheHitRatio * 100.0));
						stringBuilder.Append("%");
					}
					else
					{
						stringBuilder.Append("No Cache.");
					}
				}
				else
				{
					stringBuilder.Append("No Cache.");
				}
				stringBuilder.Append(System.Environment.NewLine);
			}
			return stringBuilder.ToString();
		}

		public ISqlMapSession CreateSqlMapSession()
		{
			ISqlMapSession sqlMapSession = new SqlMapSession(this);
			sqlMapSession.CreateConnection();
			return sqlMapSession;
		}

		public ISqlMapSession CreateSqlMapSession(string connectionString)
		{
			ISqlMapSession sqlMapSession = new SqlMapSession(this);
			sqlMapSession.CreateConnection(connectionString);
			return sqlMapSession;
		}
	}
}
