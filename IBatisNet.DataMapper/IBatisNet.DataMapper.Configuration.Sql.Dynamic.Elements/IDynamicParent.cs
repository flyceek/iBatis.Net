using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	public interface IDynamicParent
	{
		void AddChild(ISqlChild child);
	}
}
