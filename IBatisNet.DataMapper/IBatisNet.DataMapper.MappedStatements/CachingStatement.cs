using IBatisNet.DataMapper.Commands;
using IBatisNet.DataMapper.Configuration.Cache;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements
{
	public sealed class CachingStatement : IMappedStatement
	{
		private MappedStatement _mappedStatement = null;

		public event ExecuteEventHandler Execute;

		public IPreparedCommand PreparedCommand
		{
			get
			{
				return this._mappedStatement.PreparedCommand;
			}
		}

		public string Id
		{
			get
			{
				return this._mappedStatement.Id;
			}
		}

		public IStatement Statement
		{
			get
			{
				return this._mappedStatement.Statement;
			}
		}

		public ISqlMapper SqlMap
		{
			get
			{
				return this._mappedStatement.SqlMap;
			}
		}

		public CachingStatement(MappedStatement statement)
		{
			this._mappedStatement = statement;
		}

		public System.Collections.IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
		{
			System.Collections.IDictionary dictionary = new System.Collections.Hashtable();
			RequestScope requestScope = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
			this._mappedStatement.PreparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			CacheKey cacheKey = this.GetCacheKey(requestScope);
			cacheKey.Update("ExecuteQueryForMap");
			if (keyProperty != null)
			{
				cacheKey.Update(keyProperty);
			}
			if (valueProperty != null)
			{
				cacheKey.Update(valueProperty);
			}
			dictionary = (this.Statement.CacheModel[cacheKey] as System.Collections.IDictionary);
			if (dictionary == null)
			{
				dictionary = this._mappedStatement.RunQueryForMap(requestScope, session, parameterObject, keyProperty, valueProperty, null);
				this.Statement.CacheModel[cacheKey] = dictionary;
			}
			return dictionary;
		}

		public System.Collections.Generic.IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
		{
			System.Collections.Generic.IDictionary<K, V> dictionary = new System.Collections.Generic.Dictionary<K, V>();
			RequestScope requestScope = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
			this._mappedStatement.PreparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			CacheKey cacheKey = this.GetCacheKey(requestScope);
			cacheKey.Update("ExecuteQueryForMap");
			if (keyProperty != null)
			{
				cacheKey.Update(keyProperty);
			}
			if (valueProperty != null)
			{
				cacheKey.Update(valueProperty);
			}
			dictionary = (this.Statement.CacheModel[cacheKey] as System.Collections.Generic.IDictionary<K, V>);
			if (dictionary == null)
			{
				dictionary = this._mappedStatement.RunQueryForDictionary<K, V>(requestScope, session, parameterObject, keyProperty, valueProperty, null);
				this.Statement.CacheModel[cacheKey] = dictionary;
			}
			return dictionary;
		}

		public System.Collections.Generic.IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
		{
			return this._mappedStatement.ExecuteQueryForDictionary<K, V>(session, parameterObject, keyProperty, valueProperty, rowDelegate);
		}

		public int ExecuteUpdate(ISqlMapSession session, object parameterObject)
		{
			return this._mappedStatement.ExecuteUpdate(session, parameterObject);
		}

		public object ExecuteInsert(ISqlMapSession session, object parameterObject)
		{
			return this._mappedStatement.ExecuteInsert(session, parameterObject);
		}

		public void ExecuteQueryForList(ISqlMapSession session, object parameterObject, System.Collections.IList resultObject)
		{
			this._mappedStatement.ExecuteQueryForList(session, parameterObject, resultObject);
		}

		public System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
		{
			RequestScope requestScope = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
			this._mappedStatement.PreparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			CacheKey cacheKey = this.GetCacheKey(requestScope);
			cacheKey.Update("ExecuteQueryForList");
			cacheKey.Update(skipResults);
			cacheKey.Update(maxResults);
			System.Collections.IList list = this.Statement.CacheModel[cacheKey] as System.Collections.IList;
			if (list == null)
			{
				list = this._mappedStatement.RunQueryForList(requestScope, session, parameterObject, skipResults, maxResults);
				this.Statement.CacheModel[cacheKey] = list;
			}
			return list;
		}

		public System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject)
		{
			return this.ExecuteQueryForList(session, parameterObject, -1, -1);
		}

		public void ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, System.Collections.Generic.IList<T> resultObject)
		{
			this._mappedStatement.ExecuteQueryForList<T>(session, parameterObject, resultObject);
		}

		public System.Collections.Generic.IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
		{
			RequestScope requestScope = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
			this._mappedStatement.PreparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			CacheKey cacheKey = this.GetCacheKey(requestScope);
			cacheKey.Update("ExecuteQueryForList");
			cacheKey.Update(skipResults);
			cacheKey.Update(maxResults);
			System.Collections.Generic.IList<T> list = this.Statement.CacheModel[cacheKey] as System.Collections.Generic.IList<T>;
			if (list == null)
			{
				list = this._mappedStatement.RunQueryForList<T>(requestScope, session, parameterObject, skipResults, maxResults);
				this.Statement.CacheModel[cacheKey] = list;
			}
			return list;
		}

		public System.Collections.Generic.IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject)
		{
			return this.ExecuteQueryForList<T>(session, parameterObject, -1, -1);
		}

		public object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
		{
			return this.ExecuteQueryForObject(session, parameterObject, null);
		}

		public object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
		{
			RequestScope requestScope = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
			this._mappedStatement.PreparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			CacheKey cacheKey = this.GetCacheKey(requestScope);
			cacheKey.Update("ExecuteQueryForObject");
			object obj = this.Statement.CacheModel[cacheKey];
			if (obj == CacheModel.NULL_OBJECT)
			{
				obj = null;
			}
			else if (obj == null)
			{
				obj = this._mappedStatement.RunQueryForObject(requestScope, session, parameterObject, resultObject);
				this.Statement.CacheModel[cacheKey] = obj;
			}
			return obj;
		}

		public T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject)
		{
			return this.ExecuteQueryForObject<T>(session, parameterObject, default(T));
		}

		public T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject, T resultObject)
		{
			T t = default(T);
			RequestScope requestScope = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
			this._mappedStatement.PreparedCommand.Create(requestScope, session, this.Statement, parameterObject);
			CacheKey cacheKey = this.GetCacheKey(requestScope);
			cacheKey.Update("ExecuteQueryForObject");
			object obj = this.Statement.CacheModel[cacheKey];
			if (obj is T)
			{
				t = (T)((object)obj);
			}
			else if (obj == CacheModel.NULL_OBJECT)
			{
				t = default(T);
			}
			else
			{
				t = this._mappedStatement.RunQueryForObject<T>(requestScope, session, parameterObject, resultObject);
				this.Statement.CacheModel[cacheKey] = t;
			}
			return t;
		}

		public System.Collections.IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
		{
			return this._mappedStatement.ExecuteQueryForRowDelegate(session, parameterObject, rowDelegate);
		}

		public System.Collections.Generic.IList<T> ExecuteQueryForRowDelegate<T>(ISqlMapSession session, object parameterObject, RowDelegate<T> rowDelegate)
		{
			return this._mappedStatement.ExecuteQueryForRowDelegate<T>(session, parameterObject, rowDelegate);
		}

		public System.Collections.IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
		{
			return this._mappedStatement.ExecuteQueryForMapWithRowDelegate(session, parameterObject, keyProperty, valueProperty, rowDelegate);
		}

		public double GetDataCacheHitRatio()
		{
			double result;
			if (this._mappedStatement.Statement.CacheModel != null)
			{
				result = this._mappedStatement.Statement.CacheModel.HitRatio;
			}
			else
			{
				result = -1.0;
			}
			return result;
		}

		private CacheKey GetCacheKey(RequestScope request)
		{
			CacheKey cacheKey = new CacheKey();
			int count = request.IDbCommand.Parameters.Count;
			for (int i = 0; i < count; i++)
			{
				IDataParameter dataParameter = (IDataParameter)request.IDbCommand.Parameters[i];
				if (dataParameter.Value != null)
				{
					cacheKey.Update(dataParameter.Value);
				}
			}
			cacheKey.Update(this._mappedStatement.Id);
			cacheKey.Update(this._mappedStatement.SqlMap.DataSource.ConnectionString);
			cacheKey.Update(request.IDbCommand.CommandText);
			CacheModel cacheModel = this._mappedStatement.Statement.CacheModel;
			if (!cacheModel.IsReadOnly && !cacheModel.IsSerializable)
			{
				cacheKey.Update(request);
			}
			return cacheKey;
		}
	}
}
