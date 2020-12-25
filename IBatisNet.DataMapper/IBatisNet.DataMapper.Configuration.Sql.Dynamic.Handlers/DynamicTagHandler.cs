using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public sealed class DynamicTagHandler : BaseTagHandler
	{
		public DynamicTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
		{
		}

		public override int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject)
		{
			ctx.FirstNonDynamicTagWithPrepend = null;
			if (tag.IsPrependAvailable)
			{
				ctx.IsOverridePrepend = true;
			}
			return 1;
		}
	}
}
