using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public sealed class IsLessThanTagHandler : ConditionalTagHandler
	{
		public IsLessThanTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
		{
		}

		public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
		{
			long num = base.Compare(ctx, tag, parameterObject);
			return num < 0L && num != -9223372036854775808L;
		}
	}
}
