using IBatisNet.DataMapper.Configuration.Cache;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Configuration.Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	public interface IStatement
	{
		bool AllowRemapping
		{
			get;
			set;
		}

		string Id
		{
			get;
			set;
		}

		CommandType CommandType
		{
			get;
		}

		string ExtendStatement
		{
			get;
			set;
		}

		ISql Sql
		{
			get;
			set;
		}

		ResultMapCollection ResultsMap
		{
			get;
		}

		ParameterMap ParameterMap
		{
			get;
			set;
		}

		CacheModel CacheModel
		{
			get;
			set;
		}

		string CacheModelName
		{
			get;
			set;
		}

		System.Type ListClass
		{
			get;
		}

		System.Type ResultClass
		{
			get;
		}

		System.Type ParameterClass
		{
			get;
		}

		System.Collections.IList CreateInstanceOfListClass();

		System.Collections.Generic.IList<T> CreateInstanceOfGenericListClass<T>();
	}
}
