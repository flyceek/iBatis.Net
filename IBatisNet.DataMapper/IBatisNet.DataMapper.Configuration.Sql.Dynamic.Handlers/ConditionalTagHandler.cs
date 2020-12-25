using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Exceptions;
using System;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public abstract class ConditionalTagHandler : BaseTagHandler
	{
		public const long NOT_COMPARABLE = -9223372036854775808L;

		public ConditionalTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
		{
		}

		public abstract bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject);

		public override int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject)
		{
			int result;
			if (this.IsCondition(ctx, tag, parameterObject))
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public override int DoEndFragment(SqlTagContext ctx, SqlTag tag, object parameterObject, System.Text.StringBuilder bodyContent)
		{
			return 1;
		}

		protected long Compare(SqlTagContext ctx, SqlTag sqlTag, object parameterObject)
		{
			Conditional conditional = (Conditional)sqlTag;
			string property = conditional.Property;
			string compareProperty = conditional.CompareProperty;
			string compareValue = conditional.CompareValue;
			object obj;
			System.Type type;
			if (property != null && property.Length > 0)
			{
				obj = ObjectProbe.GetMemberValue(parameterObject, property, base.AccessorFactory);
				type = obj.GetType();
			}
			else
			{
				obj = parameterObject;
				if (obj != null)
				{
					type = parameterObject.GetType();
				}
				else
				{
					type = typeof(object);
				}
			}
			long result;
			if (compareProperty != null && compareProperty.Length > 0)
			{
				object memberValue = ObjectProbe.GetMemberValue(parameterObject, compareProperty, base.AccessorFactory);
				result = this.CompareValues(type, obj, memberValue);
			}
			else
			{
				if (compareValue == null || !(compareValue != ""))
				{
					throw new DataMapperException("Error comparing in conditional fragment.  Uknown 'compare to' values.");
				}
				result = this.CompareValues(type, obj, compareValue);
			}
			return result;
		}

		protected long CompareValues(System.Type type, object value1, object value2)
		{
			long result;
			if (value1 == null || value2 == null)
			{
				result = ((value1 == value2) ? 0L : -9223372036854775808L);
			}
			else
			{
				if (value2.GetType() != type)
				{
					value2 = this.ConvertValue(type, value2.ToString());
				}
				if (value2 is string && type != typeof(string))
				{
					value1 = value1.ToString();
				}
				if (!(value1 is System.IComparable) || !(value2 is System.IComparable))
				{
					value1 = value1.ToString();
					value2 = value2.ToString();
				}
				result = (long)((System.IComparable)value1).CompareTo(value2);
			}
			return result;
		}

		protected object ConvertValue(System.Type type, string value)
		{
			object result;
			if (type == typeof(string))
			{
				result = value;
			}
			else if (type == typeof(bool))
			{
				result = System.Convert.ToBoolean(value);
			}
			else if (type == typeof(byte))
			{
				result = System.Convert.ToByte(value);
			}
			else if (type == typeof(char))
			{
				result = System.Convert.ToChar(value.Substring(0, 1));
			}
			else
			{
				if (type == typeof(System.DateTime))
				{
					try
					{
						result = System.Convert.ToDateTime(value);
						return result;
					}
					catch (System.Exception ex)
					{
						throw new DataMapperException("Error parsing date. Cause: " + ex.Message, ex);
					}
				}
				if (type == typeof(decimal))
				{
					result = System.Convert.ToDecimal(value);
				}
				else if (type == typeof(double))
				{
					result = System.Convert.ToDouble(value);
				}
				else if (type == typeof(short))
				{
					result = System.Convert.ToInt16(value);
				}
				else if (type == typeof(int))
				{
					result = System.Convert.ToInt32(value);
				}
				else if (type == typeof(long))
				{
					result = System.Convert.ToInt64(value);
				}
				else if (type == typeof(float))
				{
					result = System.Convert.ToSingle(value);
				}
				else
				{
					result = value;
				}
			}
			return result;
		}
	}
}
