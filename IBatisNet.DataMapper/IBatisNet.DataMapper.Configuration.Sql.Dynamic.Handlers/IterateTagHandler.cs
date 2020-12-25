using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public sealed class IterateTagHandler : BaseTagHandler
	{
		public override bool IsPostParseRequired
		{
			get
			{
				return true;
			}
		}

		public IterateTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
		{
		}

		public override int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject)
		{
			IterateContext iterateContext = (IterateContext)ctx.GetAttribute(tag);
			if (iterateContext == null)
			{
				string property = ((BaseTag)tag).Property;
				object collection;
				if (property != null && property.Length > 0)
				{
					collection = ObjectProbe.GetMemberValue(parameterObject, property, base.AccessorFactory);
				}
				else
				{
					collection = parameterObject;
				}
				iterateContext = new IterateContext(collection);
				ctx.AddAttribute(tag, iterateContext);
			}
			int result;
			if (iterateContext != null && iterateContext.HasNext)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public override void DoPrepend(SqlTagContext ctx, SqlTag tag, object parameterObject, System.Text.StringBuilder bodyContent)
		{
			IterateContext iterateContext = (IterateContext)ctx.GetAttribute(tag);
			if (iterateContext.IsFirst)
			{
				base.DoPrepend(ctx, tag, parameterObject, bodyContent);
			}
		}

		public override int DoEndFragment(SqlTagContext ctx, SqlTag tag, object parameterObject, System.Text.StringBuilder bodyContent)
		{
			IterateContext iterateContext = (IterateContext)ctx.GetAttribute(tag);
			int result;
			if (iterateContext.MoveNext())
			{
				string text = ((BaseTag)tag).Property;
				if (text == null)
				{
					text = "";
				}
				string find = text + "[]";
				string replace = string.Concat(new object[]
				{
					text,
					"[",
					iterateContext.Index,
					"]"
				});
				IterateTagHandler.Replace(bodyContent, find, replace);
				if (iterateContext.IsFirst)
				{
					string open = ((Iterate)tag).Open;
					if (open != null)
					{
						bodyContent.Insert(0, open);
						bodyContent.Insert(0, ' ');
					}
				}
				if (!iterateContext.IsLast)
				{
					string conjunction = ((Iterate)tag).Conjunction;
					if (conjunction != null)
					{
						bodyContent.Append(conjunction);
						bodyContent.Append(' ');
					}
				}
				if (iterateContext.IsLast)
				{
					string close = ((Iterate)tag).Close;
					if (close != null)
					{
						bodyContent.Append(close);
					}
				}
				result = 2;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		private static void Replace(System.Text.StringBuilder buffer, string find, string replace)
		{
			int i = buffer.ToString().IndexOf(find);
			int length = find.Length;
			while (i > -1)
			{
				buffer = buffer.Replace(find, replace, i, length);
				i = buffer.ToString().IndexOf(find);
			}
		}
	}
}
