using IBatisNet.DataMapper.Commands;
using IBatisNet.DataMapper.Configuration.Statements;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IBatisNet.DataMapper.MappedStatements
{
	public interface IMappedStatement
	{
		event ExecuteEventHandler Execute;

		IPreparedCommand PreparedCommand
		{
			get;
		}

		string Id
		{
			get;
		}

		IStatement Statement
		{
			get;
		}

		ISqlMapper SqlMap
		{
			get;
		}

		System.Collections.IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty);

		System.Collections.Generic.IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty);

		System.Collections.Generic.IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate);

		int ExecuteUpdate(ISqlMapSession session, object parameterObject);

		object ExecuteInsert(ISqlMapSession session, object parameterObject);

		void ExecuteQueryForList(ISqlMapSession session, object parameterObject, System.Collections.IList resultObject);

		System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults);

		System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject);

		void ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, System.Collections.Generic.IList<T> resultObject);

		System.Collections.Generic.IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, int skipResults, int maxResults);

		System.Collections.Generic.IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject);

		object ExecuteQueryForObject(ISqlMapSession session, object parameterObject);

		object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject);

		T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject);

		T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject, T resultObject);

		System.Collections.IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate);

		System.Collections.IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate);

		System.Collections.Generic.IList<T> ExecuteQueryForRowDelegate<T>(ISqlMapSession session, object parameterObject, RowDelegate<T> rowDelegate);
	}
}
