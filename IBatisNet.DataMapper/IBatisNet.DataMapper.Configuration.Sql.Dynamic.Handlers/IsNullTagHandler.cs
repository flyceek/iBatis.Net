using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public class IsNullTagHandler : ConditionalTagHandler
	{
		public IsNullTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
		{
		}

		public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
		{
			bool result;
			if (parameterObject == null)
			{
				result = true;
			}
			else
			{
				string property = ((BaseTag)tag).Property;
				object obj;
				if (property != null && property.Length > 0)
				{
					obj = ObjectProbe.GetMemberValue(parameterObject, property, base.AccessorFactory);
				}
				else
				{
					obj = parameterObject;
				}
				result = (obj == null);
			}
			return result;
		}
	}
}
