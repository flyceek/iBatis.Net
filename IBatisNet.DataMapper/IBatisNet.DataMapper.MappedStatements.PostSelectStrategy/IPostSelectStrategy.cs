using IBatisNet.DataMapper.Scope;
using System;

namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
	public interface IPostSelectStrategy
	{
		void Execute(PostBindind postSelect, RequestScope request);
	}
}
