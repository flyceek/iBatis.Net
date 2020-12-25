using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public sealed class IsNotEmptyTagHandler : IsEmptyTagHandler
	{
		public IsNotEmptyTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
		{
		}

		public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
		{
			return !base.IsCondition(ctx, tag, parameterObject);
		}
	}
}
