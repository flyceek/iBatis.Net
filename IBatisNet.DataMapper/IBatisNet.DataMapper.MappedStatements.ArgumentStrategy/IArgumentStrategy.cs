using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
	public interface IArgumentStrategy
	{
		object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys);
	}
}
