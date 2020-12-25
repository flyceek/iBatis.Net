using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.Exceptions;
using System;
using System.Collections;

namespace IBatisNet.DataMapper.MappedStatements
{
	public sealed class UpdateMappedStatement : MappedStatement
	{
		internal UpdateMappedStatement(ISqlMapper sqlMap, IStatement statement) : base(sqlMap, statement)
		{
		}

		public override System.Collections.IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
		{
			throw new DataMapperException("Update statements cannot be executed as a query for map.");
		}

		public override object ExecuteInsert(ISqlMapSession session, object parameterObject)
		{
			throw new DataMapperException("Update statements cannot be executed as a query insert.");
		}

		public override void ExecuteQueryForList(ISqlMapSession session, object parameterObject, System.Collections.IList resultObject)
		{
			throw new DataMapperException("Update statements cannot be executed as a query for list.");
		}

		public override System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
		{
			throw new DataMapperException("Update statements cannot be executed as a query for list.");
		}

		public override System.Collections.IList ExecuteQueryForList(ISqlMapSession session, object parameterObject)
		{
			throw new DataMapperException("Update statements cannot be executed as a query for list.");
		}

		public override System.Collections.IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
		{
			throw new DataMapperException("Update statement cannot be executed as a query for row delegate.");
		}

		public override System.Collections.IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
		{
			throw new DataMapperException("Update statement cannot be executed as a query for row delegate.");
		}

		public override object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
		{
			throw new DataMapperException("Update statements cannot be executed as a query for object.");
		}

		public override object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
		{
			throw new DataMapperException("Update statements cannot be executed as a query for object.");
		}
	}
}
