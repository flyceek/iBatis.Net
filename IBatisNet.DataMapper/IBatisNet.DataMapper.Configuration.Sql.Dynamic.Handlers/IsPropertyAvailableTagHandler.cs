using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public class IsPropertyAvailableTagHandler : ConditionalTagHandler
	{
		public IsPropertyAvailableTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
		{
		}

		public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
		{
			return parameterObject != null && ObjectProbe.HasReadableProperty(parameterObject, ((BaseTag)tag).Property);
		}
	}
}
