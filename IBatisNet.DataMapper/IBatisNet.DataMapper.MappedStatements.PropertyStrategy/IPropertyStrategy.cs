using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Data;

namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
	public interface IPropertyStrategy
	{
		void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object keys);

		object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader);
	}
}
