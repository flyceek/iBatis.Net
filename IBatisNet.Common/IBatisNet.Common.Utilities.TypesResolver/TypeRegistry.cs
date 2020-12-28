using System;
using System.Collections;

namespace IBatisNet.Common.Utilities.TypesResolver
{
	/// <summary> 
	/// Provides access to a central registry of aliased <see cref="T:System.Type" />s.
	/// </summary>
	/// <remarks>
	/// <p>
	/// Simplifies configuration by allowing aliases to be used instead of
	/// fully qualified type names.
	/// </p>
	/// <p>
	/// Comes 'pre-loaded' with a number of convenience alias' for the more
	/// common types; an example would be the '<c>int</c>' (or '<c>Integer</c>'
	/// for Visual Basic.NET developers) alias for the <see cref="T:System.Int32" />
	/// type.
	/// </p>
	/// </remarks>
	public class TypeRegistry
	{
		/// <summary>
		/// The alias around the 'list' type.
		/// </summary>
		public const string ArrayListAlias1 = "arraylist";

		/// <summary>
		/// Another alias around the 'list' type.
		/// </summary>
		public const string ArrayListAlias2 = "list";

		/// <summary>
		/// Another alias around the 'bool' type.
		/// </summary>
		public const string BoolAlias = "bool";

		/// <summary>
		/// The alias around the 'bool' type.
		/// </summary>
		public const string BooleanAlias = "boolean";

		/// <summary>
		/// The alias around the 'byte' type.
		/// </summary>
		public const string ByteAlias = "byte";

		/// <summary>
		/// The alias around the 'char' type.
		/// </summary>
		public const string CharAlias = "char";

		/// <summary>
		/// The alias around the 'DateTime' type.
		/// </summary>
		public const string DateAlias1 = "datetime";

		/// <summary>
		/// Another alias around the 'DateTime' type.
		/// </summary>
		public const string DateAlias2 = "date";

		/// <summary>
		/// The alias around the 'decimal' type.
		/// </summary>
		public const string DecimalAlias = "decimal";

		/// <summary>
		/// The alias around the 'double' type.
		/// </summary>
		public const string DoubleAlias = "double";

		/// <summary>
		/// The alias around the 'float' type.
		/// </summary>
		public const string FloatAlias = "float";

		/// <summary>
		/// Another alias around the 'float' type.
		/// </summary>
		public const string SingleAlias = "single";

		/// <summary>
		/// The alias around the 'guid' type.
		/// </summary>
		public const string GuidAlias = "guid";

		/// <summary>
		/// The alias around the 'Hashtable' type.
		/// </summary>
		public const string HashtableAlias1 = "hashtable";

		/// <summary>
		/// Another alias around the 'Hashtable' type.
		/// </summary>
		public const string HashtableAlias2 = "map";

		/// <summary>
		/// Another alias around the 'Hashtable' type.
		/// </summary>
		public const string HashtableAlias3 = "hashmap";

		/// <summary>
		/// The alias around the 'short' type.
		/// </summary>
		public const string Int16Alias1 = "int16";

		/// <summary>
		/// Another alias around the 'short' type.
		/// </summary>
		public const string Int16Alias2 = "short";

		/// <summary>
		/// The alias around the 'int' type.
		/// </summary>
		public const string Int32Alias1 = "int32";

		/// <summary>
		/// Another alias around the 'int' type.
		/// </summary>
		public const string Int32Alias2 = "int";

		/// <summary>
		/// Another alias around the 'int' type.
		/// </summary>
		public const string Int32Alias3 = "integer";

		/// <summary>
		/// The alias around the 'long' type.
		/// </summary>
		public const string Int64Alias1 = "int64";

		/// <summary>
		/// Another alias around the 'long' type.
		/// </summary>
		public const string Int64Alias2 = "long";

		/// <summary>
		/// The alias around the 'unsigned short' type.
		/// </summary>
		public const string UInt16Alias1 = "uint16";

		/// <summary>
		/// Another alias around the 'unsigned short' type.
		/// </summary>
		public const string UInt16Alias2 = "ushort";

		/// <summary>
		/// The alias around the 'unsigned int' type.
		/// </summary>
		public const string UInt32Alias1 = "uint32";

		/// <summary>
		/// Another alias around the 'unsigned int' type.
		/// </summary>
		public const string UInt32Alias2 = "uint";

		/// <summary>
		/// The alias around the 'unsigned long' type.
		/// </summary>
		public const string UInt64Alias1 = "uint64";

		/// <summary>
		/// Another alias around the 'unsigned long' type.
		/// </summary>
		public const string UInt64Alias2 = "ulong";

		/// <summary>
		/// The alias around the 'SByte' type.
		/// </summary>
		public const string SByteAlias = "sbyte";

		/// <summary>
		/// The alias around the 'string' type.
		/// </summary>
		public const string StringAlias = "string";

		/// <summary>
		/// The alias around the 'TimeSpan' type.
		/// </summary>
		public const string TimeSpanAlias = "timespan";

		/// <summary>
		/// The alias around the 'int?' type.
		/// </summary>
		public const string NullableInt32Alias = "int?";

		/// <summary>
		/// The alias around the 'int?[]' array type.
		/// </summary>
		public const string NullableInt32ArrayAlias = "int?[]";

		/// <summary>
		/// The alias around the 'decimal?' type.
		/// </summary>
		public const string NullableDecimalAlias = "decimal?";

		/// <summary>
		/// The alias around the 'decimal?[]' array type.
		/// </summary>
		public const string NullableDecimalArrayAlias = "decimal?[]";

		/// <summary>
		/// The alias around the 'char?' type.
		/// </summary>
		public const string NullableCharAlias = "char?";

		/// <summary>
		/// The alias around the 'char?[]' array type.
		/// </summary>
		public const string NullableCharArrayAlias = "char?[]";

		/// <summary>
		/// The alias around the 'long?' type.
		/// </summary>
		public const string NullableInt64Alias = "long?";

		/// <summary>
		/// The alias around the 'long?[]' array type.
		/// </summary>
		public const string NullableInt64ArrayAlias = "long?[]";

		/// <summary>
		/// The alias around the 'short?' type.
		/// </summary>
		public const string NullableInt16Alias = "short?";

		/// <summary>
		/// The alias around the 'short?[]' array type.
		/// </summary>
		public const string NullableInt16ArrayAlias = "short?[]";

		/// <summary>
		/// The alias around the 'unsigned int?' type.
		/// </summary>
		public const string NullableUInt32Alias = "uint?";

		/// <summary>
		/// The alias around the 'unsigned long?' type.
		/// </summary>
		public const string NullableUInt64Alias = "ulong?";

		/// <summary>
		/// The alias around the 'ulong?[]' array type.
		/// </summary>
		public const string NullableUInt64ArrayAlias = "ulong?[]";

		/// <summary>
		/// The alias around the 'uint?[]' array type.
		/// </summary>
		public const string NullableUInt32ArrayAlias = "uint?[]";

		/// <summary>
		/// The alias around the 'unsigned short?' type.
		/// </summary>
		public const string NullableUInt16Alias = "ushort?";

		/// <summary>
		/// The alias around the 'ushort?[]' array type.
		/// </summary>
		public const string NullableUInt16ArrayAlias = "ushort?[]";

		/// <summary>
		/// The alias around the 'double?' type.
		/// </summary>
		public const string NullableDoubleAlias = "double?";

		/// <summary>
		/// The alias around the 'double?[]' array type.
		/// </summary>
		public const string NullableDoubleArrayAlias = "double?[]";

		/// <summary>
		/// The alias around the 'float?' type.
		/// </summary>
		public const string NullableFloatAlias = "float?";

		/// <summary>
		/// The alias around the 'float?[]' array type.
		/// </summary>
		public const string NullableFloatArrayAlias = "float?[]";

		/// <summary>
		/// The alias around the 'bool?' type.
		/// </summary>
		public const string NullableBoolAlias = "bool?";

		/// <summary>
		/// The alias around the 'bool?[]' array type.
		/// </summary>
		public const string NullableBoolArrayAlias = "bool?[]";

		private static IDictionary _types;

		/// <summary>
		/// Creates a new instance of the <see cref="T:IBatisNet.Common.Utilities.TypesResolver.TypeRegistry" /> class.
		/// </summary>
		/// <remarks>
		/// <p>
		/// This is a utility class, and as such has no publicly visible
		/// constructors.
		/// </p>
		/// </remarks>
		private TypeRegistry()
		{
		}

		/// <summary>
		/// Initialises the static properties of the TypeAliasResolver class.
		/// </summary>
		static TypeRegistry()
		{
			_types = new Hashtable();
			_types["arraylist"] = typeof(ArrayList);
			_types["list"] = typeof(ArrayList);
			_types["bool"] = typeof(bool);
			_types["boolean"] = typeof(bool);
			_types["byte"] = typeof(byte);
			_types["char"] = typeof(char);
			_types["datetime"] = typeof(DateTime);
			_types["date"] = typeof(DateTime);
			_types["decimal"] = typeof(decimal);
			_types["double"] = typeof(double);
			_types["float"] = typeof(float);
			_types["single"] = typeof(float);
			_types["guid"] = typeof(Guid);
			_types["hashtable"] = typeof(Hashtable);
			_types["map"] = typeof(Hashtable);
			_types["hashmap"] = typeof(Hashtable);
			_types["int16"] = typeof(short);
			_types["short"] = typeof(short);
			_types["int32"] = typeof(int);
			_types["int"] = typeof(int);
			_types["integer"] = typeof(int);
			_types["int64"] = typeof(long);
			_types["long"] = typeof(long);
			_types["uint16"] = typeof(ushort);
			_types["ushort"] = typeof(ushort);
			_types["uint32"] = typeof(uint);
			_types["uint"] = typeof(uint);
			_types["uint64"] = typeof(ulong);
			_types["ulong"] = typeof(ulong);
			_types["sbyte"] = typeof(sbyte);
			_types["string"] = typeof(string);
			_types["timespan"] = typeof(string);
			_types["int?"] = typeof(int?);
			_types["int?[]"] = typeof(int?[]);
			_types["decimal?"] = typeof(decimal?);
			_types["decimal?[]"] = typeof(decimal?[]);
			_types["char?"] = typeof(char?);
			_types["char?[]"] = typeof(char?[]);
			_types["long?"] = typeof(long?);
			_types["long?[]"] = typeof(long?[]);
			_types["short?"] = typeof(short?);
			_types["short?[]"] = typeof(short?[]);
			_types["uint?"] = typeof(uint?);
			_types["uint?[]"] = typeof(uint?[]);
			_types["ulong?"] = typeof(ulong?);
			_types["ulong?[]"] = typeof(ulong?[]);
			_types["ushort?"] = typeof(ushort?);
			_types["ushort?[]"] = typeof(ushort?[]);
			_types["double?"] = typeof(double?);
			_types["double?[]"] = typeof(double?[]);
			_types["float?"] = typeof(float?);
			_types["float?[]"] = typeof(float?[]);
			_types["bool?"] = typeof(bool?);
			_types["bool?[]"] = typeof(bool?[]);
		}

		/// <summary> 
		/// Resolves the supplied <paramref name="alias" /> to a <see cref="T:System.Type" />. 
		/// </summary> 
		/// <param name="alias">
		/// The alias to resolve.
		/// </param>
		/// <returns>
		/// The <see cref="T:System.Type" /> the supplied <paramref name="alias" /> was
		/// associated with, or <see lang="null" /> if no <see cref="T:System.Type" /> 
		/// was previously registered for the supplied <paramref name="alias" />.
		/// </returns>
		/// <remarks>The alis name will be convert in lower character before the resolution.</remarks>
		/// <exception cref="T:System.ArgumentNullException">
		/// If the supplied <paramref name="alias" /> is <see langword="null" /> or
		/// contains only whitespace character(s).
		/// </exception>
		public static Type ResolveType(string alias)
		{
			return (Type)_types[alias.ToLower()];
		}
	}
}
