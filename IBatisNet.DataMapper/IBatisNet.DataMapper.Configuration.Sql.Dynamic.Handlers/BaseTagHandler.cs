using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public abstract class BaseTagHandler : ISqlTagHandler
	{
		public const int SKIP_BODY = 0;

		public const int INCLUDE_BODY = 1;

		public const int REPEAT_BODY = 2;

		private AccessorFactory _accessorFactory = null;

		public AccessorFactory AccessorFactory
		{
			get
			{
				return this._accessorFactory;
			}
		}

		public virtual bool IsPostParseRequired
		{
			get
			{
				return false;
			}
		}

		public BaseTagHandler(AccessorFactory accessorFactory)
		{
			this._accessorFactory = accessorFactory;
		}

		public virtual int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject)
		{
			return 1;
		}

		public virtual int DoEndFragment(SqlTagContext ctx, SqlTag tag, object parameterObject, System.Text.StringBuilder bodyContent)
		{
			return 1;
		}

		public virtual void DoPrepend(SqlTagContext ctx, SqlTag tag, object parameterObject, System.Text.StringBuilder bodyContent)
		{
			if (tag.IsPrependAvailable)
			{
				if (bodyContent.ToString().Trim().Length > 0)
				{
					if (ctx.IsOverridePrepend && tag == ctx.FirstNonDynamicTagWithPrepend)
					{
						ctx.IsOverridePrepend = false;
					}
					else
					{
						bodyContent.Insert(0, tag.Prepend);
					}
				}
				else if (ctx.FirstNonDynamicTagWithPrepend != null)
				{
					ctx.FirstNonDynamicTagWithPrepend = null;
					ctx.IsOverridePrepend = true;
				}
			}
		}
	}
}
