using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public class IsParameterPresentTagHandler : ConditionalTagHandler
	{
		public IsParameterPresentTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
		{
		}

		public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
		{
			return parameterObject != null;
		}
	}
}
