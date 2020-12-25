using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.Exceptions;
using System;
using System.Collections;

namespace IBatisNet.DataMapper.MappedStatements
{
	public sealed class InsertMappedStatement : MappedStatement
	{
		internal InsertMappedStatement(ISqlMapper sqlMap, IStatement statement) : base(sqlMap, statement)
		{
		}

		public override System.Collections.IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
		{
			throw new DataMapperException("Insert statements cannot be executed as a query for map.");
		}

		public override int ExecuteUpdate(ISqlMapSession session, object parameterObject)
		{
			throw new DataMapperException("Insert statements cannot be executed as a update query.");
		}

		public override void ExecuteQueryForList(ISqlMapSession session, object parameterObject, System.Collections.IList resultObject)
		{
			throw new DataMapperException("Insert statements cannot be executed as a query for list.");
		}

		public override System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
		{
			throw new DataMapperException("Insert statements cannot be executed as a query for list.");
		}

		public override System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject)
		{
			throw new DataMapperException("Insert statements cannot be executed as a query for list.");
		}

		public override System.Collections.IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
		{
			throw new DataMapperException("Insert statements cannot be executed as a query for row delegate.");
		}

		public override System.Collections.IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
		{
			throw new DataMapperException("Insert statements cannot be executed as a query for row delegate.");
		}

		public override object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
		{
			throw new DataMapperException("Insert statements cannot be executed as a query for object.");
		}

		public override object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
		{
			throw new DataMapperException("Insert statements cannot be executed as a query for object.");
		}
	}
}
