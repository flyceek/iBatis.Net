using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Reflection.Emit;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// Abstract base class for member accessor
	/// </summary>
	public abstract class BaseAccessor
	{
		/// <summary>
		/// The property name
		/// </summary>
		protected string propertyName = string.Empty;

		/// <summary>
		/// The target type
		/// </summary>
		protected Type targetType = null;

		/// <summary>
		/// The null internal value used by this member type 
		/// </summary>
		protected object nullInternal = null;

		/// <summary>
		/// List of type-opCode
		/// </summary>
		protected static IDictionary typeToOpcode;

		/// <summary>
		/// Static constructor
		/// "Initialize a private IDictionary with type-opCode pairs 
		/// </summary>
		static BaseAccessor()
		{
			typeToOpcode = new HybridDictionary();
			typeToOpcode[typeof(sbyte)] = OpCodes.Ldind_I1;
			typeToOpcode[typeof(byte)] = OpCodes.Ldind_U1;
			typeToOpcode[typeof(char)] = OpCodes.Ldind_U2;
			typeToOpcode[typeof(short)] = OpCodes.Ldind_I2;
			typeToOpcode[typeof(ushort)] = OpCodes.Ldind_U2;
			typeToOpcode[typeof(int)] = OpCodes.Ldind_I4;
			typeToOpcode[typeof(uint)] = OpCodes.Ldind_U4;
			typeToOpcode[typeof(long)] = OpCodes.Ldind_I8;
			typeToOpcode[typeof(ulong)] = OpCodes.Ldind_I8;
			typeToOpcode[typeof(bool)] = OpCodes.Ldind_I1;
			typeToOpcode[typeof(double)] = OpCodes.Ldind_R8;
			typeToOpcode[typeof(float)] = OpCodes.Ldind_R4;
		}

		/// <summary>
		/// Gets the property info.
		/// </summary>
		/// <param name="target">The target type.</param>
		/// <returns></returns>
		protected PropertyInfo GetPropertyInfo(Type target)
		{
			PropertyInfo propertyInfo = null;
			propertyInfo = target.GetProperty(propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			if (propertyInfo == null)
			{
				if (target.IsInterface)
				{
					Type[] interfaces = target.GetInterfaces();
					foreach (Type target2 in interfaces)
					{
						propertyInfo = GetPropertyInfo(target2);
						if (propertyInfo != null)
						{
							break;
						}
					}
				}
				else
				{
					propertyInfo = target.GetProperty(propertyName);
				}
			}
			return propertyInfo;
		}

		/// <summary>
		/// Get the null value for a given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected object GetNullInternal(Type type)
		{
			if (type.IsValueType)
			{
				if (type.IsEnum)
				{
					return GetNullInternal(Enum.GetUnderlyingType(type));
				}
				if (type.IsPrimitive)
				{
					if (type == typeof(int))
					{
						return 0;
					}
					if (type == typeof(double))
					{
						return 0.0;
					}
					if (type == typeof(short))
					{
						return (short)0;
					}
					if (type == typeof(sbyte))
					{
						return (sbyte)0;
					}
					if (type == typeof(long))
					{
						return 0L;
					}
					if (type == typeof(byte))
					{
						return (byte)0;
					}
					if (type == typeof(ushort))
					{
						return (ushort)0;
					}
					if (type == typeof(uint))
					{
						return 0u;
					}
					if (type == typeof(ulong))
					{
						return 0uL;
					}
					if (type == typeof(ulong))
					{
						return 0uL;
					}
					if (type == typeof(float))
					{
						return 0f;
					}
					if (type == typeof(bool))
					{
						return false;
					}
					if (type == typeof(char))
					{
						return '\0';
					}
				}
				else
				{
					if (type == typeof(DateTime))
					{
						return DateTime.MinValue;
					}
					if (type == typeof(decimal))
					{
						return 0m;
					}
					if (type == typeof(Guid))
					{
						return Guid.Empty;
					}
					if (type == typeof(TimeSpan))
					{
						return new TimeSpan(0, 0, 0);
					}
				}
			}
			return null;
		}
	}
}
