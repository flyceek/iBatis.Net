using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public interface ISqlTagHandler
	{
		bool IsPostParseRequired
		{
			get;
		}

		int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject);

		int DoEndFragment(SqlTagContext ctx, SqlTag tag, object parameterObject, System.Text.StringBuilder bodyContent);

		void DoPrepend(SqlTagContext ctx, SqlTag tag, object parameterObject, System.Text.StringBuilder bodyContent);
	}
}
