using IBatisNet.Common;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Cache;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.SessionStore;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Threading.Tasks;

namespace IBatisNet.DataMapper
{
	public interface ISqlMapper
	{
		string Id
		{
			get;
		}

		ISessionStore SessionStore
		{
			set;
		}

		bool IsSessionStarted
		{
			get;
		}

		ISqlMapSession LocalSession
		{
			get;
		}

		DBHelperParameterCache DBHelperParameterCache
		{
			get;
		}

		bool IsCacheModelsEnabled
		{
			get;
			set;
		}

		DataExchangeFactory DataExchangeFactory
		{
			get;
		}

		TypeHandlerFactory TypeHandlerFactory
		{
			get;
		}

		IObjectFactory ObjectFactory
		{
			get;
		}

		AccessorFactory AccessorFactory
		{
			get;
		}

		HybridDictionary ParameterMaps
		{
			get;
		}

		HybridDictionary ResultMaps
		{
			get;
		}

		HybridDictionary MappedStatements
		{
			get;
		}

		IDataSource DataSource
		{
			get;
			set;
		}

		ISqlMapSession CreateSqlMapSession();

		ParameterMap GetParameterMap(string name);

		void AddParameterMap(ParameterMap parameterMap);

		IResultMap GetResultMap(string name);

		void AddResultMap(IResultMap resultMap);

		CacheModel GetCache(string name);

		void AddCache(CacheModel cache);

		void AddMappedStatement(string key, IMappedStatement mappedStatement);

		ISqlMapSession BeginTransaction();

		ISqlMapSession BeginTransaction(bool openConnection);

		ISqlMapSession BeginTransaction(string connectionString);

		ISqlMapSession BeginTransaction(bool openNewConnection, IsolationLevel isolationLevel);

		ISqlMapSession BeginTransaction(string connectionString, bool openNewConnection, IsolationLevel isolationLevel);

		ISqlMapSession BeginTransaction(IsolationLevel isolationLevel);

		ISqlMapSession BeginTransaction(string connectionString, IsolationLevel isolationLevel);

		void CloseConnection();

		void CommitTransaction(bool closeConnection);

		void CommitTransaction();

		int Delete(string statementName, object parameterObject);

		void FlushCaches();

		string GetDataCacheStats();

		IMappedStatement GetMappedStatement(string id);

		object Insert(string statementName, object parameterObject);

		ISqlMapSession OpenConnection();

		ISqlMapSession OpenConnection(string connectionString);

		System.Collections.IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty, string valueProperty);

		System.Collections.IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty);

		void QueryForList(string statementName, object parameterObject, System.Collections.IList resultObject);

		System.Collections.IList QueryForList(string statementName, object parameterObject);

		System.Collections.IList QueryForList(string statementName, object parameterObject, int skipResults, int maxResults);

		Task<System.Collections.Generic.IList<T>> QueryForListAsync<T>(string statementName, object parameterObject);

		System.Collections.IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty);

		System.Collections.IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty, string valueProperty);

		System.Collections.IDictionary QueryForMapWithRowDelegate(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate);

		object QueryForObject(string statementName, object parameterObject, object resultObject);

		object QueryForObject(string statementName, object parameterObject);

		[System.Obsolete("This method will be remove in future version.", false)]
		PaginatedList QueryForPaginatedList(string statementName, object parameterObject, int pageSize);

		System.Collections.IList QueryWithRowDelegate(string statementName, object parameterObject, RowDelegate rowDelegate);

		void RollBackTransaction();

		void RollBackTransaction(bool closeConnection);

		int Update(string statementName, object parameterObject);

		System.Collections.Generic.IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty);

		System.Collections.Generic.IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty);

		System.Collections.Generic.IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate);

		T QueryForObject<T>(string statementName, object parameterObject, T instanceObject);

		T QueryForObject<T>(string statementName, object parameterObject);

		Task<T> QueryForObjectAsync<T>(string statementName, object parameterObject);

		System.Collections.Generic.IList<T> QueryForList<T>(string statementName, object parameterObject);

		void QueryForList<T>(string statementName, object parameterObject, System.Collections.Generic.IList<T> resultObject);

		System.Collections.Generic.IList<T> QueryForList<T>(string statementName, object parameterObject, int skipResults, int maxResults);

		System.Collections.Generic.IList<T> QueryWithRowDelegate<T>(string statementName, object parameterObject, RowDelegate<T> rowDelegate);
	}
}
