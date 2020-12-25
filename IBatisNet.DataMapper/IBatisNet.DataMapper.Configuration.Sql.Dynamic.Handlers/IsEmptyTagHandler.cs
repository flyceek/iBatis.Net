using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Collections;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public class IsEmptyTagHandler : ConditionalTagHandler
	{
		public IsEmptyTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
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
				if (obj is System.Collections.ICollection)
				{
					result = (obj == null || ((System.Collections.ICollection)obj).Count < 1);
				}
				else if (obj != null && typeof(System.Array).IsAssignableFrom(obj.GetType()))
				{
					result = (((System.Array)obj).GetLength(0) == 0);
				}
				else
				{
					result = (obj == null || System.Convert.ToString(obj).Equals(""));
				}
			}
			return result;
		}
	}
}
