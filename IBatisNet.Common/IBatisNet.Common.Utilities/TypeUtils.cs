using System;
using System.Collections.Generic;
using IBatisNet.Common.Utilities.TypesResolver;

namespace IBatisNet.Common.Utilities
{
	/// <summary>
	///  Helper methods with regard to type.
	/// </summary>
	/// <remarks>
	/// <p>
	/// Mainly for internal use within the framework.
	/// </p>
	/// </remarks>
	public sealed class TypeUtils
	{
		private static readonly ITypeResolver _internalTypeResolver = new CachedTypeResolver(new TypeResolver());

		/// <summary>
		/// Creates a new instance of the <see cref="T:IBatisNet.Common.Utilities.TypeUtils" /> class.
		/// </summary>
		/// <remarks>
		/// <p>
		/// This is a utility class, and as such exposes no public constructors.
		/// </p>
		/// </remarks>
		private TypeUtils()
		{
		}

		/// <summary>
		/// Resolves the supplied type name into a <see cref="T:System.Type" />
		/// instance.
		/// </summary>
		/// <param name="typeName">
		/// The (possibly partially assembly qualified) name of a
		/// <see cref="T:System.Type" />.
		/// </param>
		/// <returns>
		/// A resolved <see cref="T:System.Type" /> instance.
		/// </returns>
		/// <exception cref="T:System.TypeLoadException">
		/// If the type cannot be resolved.
		/// </exception>
		public static Type ResolveType(string typeName)
		{
			Type type = TypeRegistry.ResolveType(typeName);
			if (type == null)
			{
				type = _internalTypeResolver.Resolve(typeName);
			}
			return type;
		}

		/// <summary>
		/// Instantiate a 'Primitive' Type.
		/// </summary>
		/// <param name="typeCode">a typeCode.</param>
		/// <returns>An object.</returns>
		public static object InstantiatePrimitiveType(TypeCode typeCode)
		{
			object result = null;
			switch (typeCode)
			{
			case TypeCode.Boolean:
				result = false;
				break;
			case TypeCode.Byte:
				result = (byte)0;
				break;
			case TypeCode.Char:
				result = '\0';
				break;
			case TypeCode.DateTime:
				result = default(DateTime);
				break;
			case TypeCode.Decimal:
				result = 0m;
				break;
			case TypeCode.Double:
				result = 0.0;
				break;
			case TypeCode.Int16:
				result = (short)0;
				break;
			case TypeCode.Int32:
				result = 0;
				break;
			case TypeCode.Int64:
				result = 0L;
				break;
			case TypeCode.SByte:
				result = (sbyte)0;
				break;
			case TypeCode.Single:
				result = 0f;
				break;
			case TypeCode.String:
				result = "";
				break;
			case TypeCode.UInt16:
				result = (ushort)0;
				break;
			case TypeCode.UInt32:
				result = 0u;
				break;
			case TypeCode.UInt64:
				result = 0uL;
				break;
			}
			return result;
		}

		/// <summary>
		/// Instantiate a Nullable Type.
		/// </summary>
		/// <param name="type">The nullable type.</param>
		/// <returns>An object.</returns>
		public static object InstantiateNullableType(Type type)
		{
			object result = null;
			if (type == typeof(bool?))
			{
				result = new bool?(false);
			}
			else if (type == typeof(byte?))
			{
				result = new byte?(0);
			}
			else if (type == typeof(char?))
			{
				result = new char?('\0');
			}
			else if (type == typeof(DateTime?))
			{
				result = new DateTime?(DateTime.MinValue);
			}
			else if (type == typeof(decimal?))
			{
				result = new decimal?(decimal.MinValue);
			}
			else if (type == typeof(double?))
			{
				result = new double?(double.MinValue);
			}
			else if (type == typeof(short?))
			{
				result = new short?(short.MinValue);
			}
			else if (type == typeof(int?))
			{
				result = new int?(int.MinValue);
			}
			else if (type == typeof(long?))
			{
				result = new long?(long.MinValue);
			}
			else if (type == typeof(sbyte?))
			{
				result = new sbyte?(sbyte.MinValue);
			}
			else if (type == typeof(float?))
			{
				result = new float?(float.MinValue);
			}
			else if (type == typeof(ushort?))
			{
				result = new ushort?(0);
			}
			else if (type == typeof(uint?))
			{
				result = new uint?(0u);
			}
			else if (type == typeof(ulong?))
			{
				result = new ulong?(0uL);
			}
			return result;
		}

		/// <summary>
		/// Determines whether the specified type is implement generic Ilist interface.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type is implement generic Ilist interface; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsImplementGenericIListInterface(Type type)
		{
			bool flag = false;
			if (!type.IsGenericType)
			{
				flag = false;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
			{
				return true;
			}
			Type[] interfaces = type.GetInterfaces();
			Type[] array = interfaces;
			foreach (Type type2 in array)
			{
				flag = IsImplementGenericIListInterface(type2);
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		/// <summary>
		/// Get type from assembly.
		/// </summary>
		/// <param name="type">typename,assemblyname.</param>
		/// <returns></returns>
		public static Type GetTypeByString(string type)
		{
			switch (type.ToLower())
			{
			case "bool":
				return Type.GetType("System.Boolean", throwOnError: true, ignoreCase: true);
			case "byte":
				return Type.GetType("System.Byte", throwOnError: true, ignoreCase: true);
			case "sbyte":
				return Type.GetType("System.SByte", throwOnError: true, ignoreCase: true);
			case "char":
				return Type.GetType("System.Char", throwOnError: true, ignoreCase: true);
			case "decimal":
				return Type.GetType("System.Decimal", throwOnError: true, ignoreCase: true);
			case "double":
				return Type.GetType("System.Double", throwOnError: true, ignoreCase: true);
			case "float":
				return Type.GetType("System.Single", throwOnError: true, ignoreCase: true);
			case "int":
				return Type.GetType("System.Int32", throwOnError: true, ignoreCase: true);
			case "uint":
				return Type.GetType("System.UInt32", throwOnError: true, ignoreCase: true);
			case "long":
				return Type.GetType("System.Int64", throwOnError: true, ignoreCase: true);
			case "ulong":
				return Type.GetType("System.UInt64", throwOnError: true, ignoreCase: true);
			case "object":
				return Type.GetType("System.Object", throwOnError: true, ignoreCase: true);
			case "short":
				return Type.GetType("System.Int16", throwOnError: true, ignoreCase: true);
			case "ushort":
				return Type.GetType("System.UInt16", throwOnError: true, ignoreCase: true);
			case "string":
				return Type.GetType("System.String", throwOnError: true, ignoreCase: true);
			case "date":
			case "datetime":
				return Type.GetType("System.DateTime", throwOnError: true, ignoreCase: true);
			case "guid":
				return Type.GetType("System.Guid", throwOnError: true, ignoreCase: true);
			default:
				return Type.GetType(type, throwOnError: true, ignoreCase: true);
			}
		}
	}
}
