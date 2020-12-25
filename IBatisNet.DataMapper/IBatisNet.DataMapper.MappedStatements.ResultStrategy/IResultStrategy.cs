using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
	public interface IResultStrategy
	{
		object Process(RequestScope request, ref IDataReader reader, object resultObject);
	}
}
