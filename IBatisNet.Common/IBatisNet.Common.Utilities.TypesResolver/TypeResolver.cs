using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace IBatisNet.Common.Utilities.TypesResolver
{
	/// <summary>
	/// Resolves a <see cref="T:System.Type" /> by name.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The rationale behind the creation of this class is to centralise the
	/// resolution of type names to <see cref="T:System.Type" /> instances beyond that
	/// offered by the plain vanilla System.Type.GetType method call.
	/// </p>
	/// </remarks>
	/// <version>$Id: TypeResolver.cs,v 1.5 2004/09/28 07:51:47 springboy Exp $</version>
	public class TypeResolver : ITypeResolver
	{
		/// <summary>
		/// Holder for the generic arguments when using type parameters.
		/// </summary>
		/// <remarks>
		/// <p>
		/// Type parameters can be applied to classes, interfaces, 
		/// structures, methods, delegates, etc...
		/// </p>
		/// </remarks>
		internal class GenericArgumentsInfo
		{
			/// <summary>
			/// The generic arguments prefix.
			/// </summary>
			public const string GENERIC_ARGUMENTS_PREFIX = "[[";

			/// <summary>
			/// The generic arguments suffix.
			/// </summary>
			public const string GENERIC_ARGUMENTS_SUFFIX = "]]";

			/// <summary>
			/// The character that separates a list of generic arguments.
			/// </summary>
			public const string GENERIC_ARGUMENTS_SEPARATOR = "],[";

			private string _unresolvedGenericTypeName = string.Empty;

			private string[] _unresolvedGenericArguments = null;

			private static readonly Regex generic = new Regex("`\\d*\\[\\[", RegexOptions.Compiled);

			/// <summary>
			/// The (unresolved) generic type name portion 
			/// of the original value when parsing a generic type.
			/// </summary>
			public string GenericTypeName => _unresolvedGenericTypeName;

			/// <summary>
			/// Is the string value contains generic arguments ?
			/// </summary>
			/// <remarks>
			/// <p>
			/// A generic argument can be a type parameter or a type argument.
			/// </p>
			/// </remarks>
			public bool ContainsGenericArguments => _unresolvedGenericArguments != null && _unresolvedGenericArguments.Length > 0;

			/// <summary>
			/// Is generic arguments only contains type parameters ?
			/// </summary>
			public bool IsGenericDefinition
			{
				get
				{
					if (_unresolvedGenericArguments == null)
					{
						return false;
					}
					string[] unresolvedGenericArguments = _unresolvedGenericArguments;
					foreach (string text in unresolvedGenericArguments)
					{
						if (text.Length > 0)
						{
							return false;
						}
					}
					return true;
				}
			}

			/// <summary>
			/// Creates a new instance of the GenericArgumentsInfo class.
			/// </summary>
			/// <param name="value">
			/// The string value to parse looking for a generic definition
			/// and retrieving its generic arguments.
			/// </param>
			public GenericArgumentsInfo(string value)
			{
				ParseGenericArguments(value);
			}

			/// <summary>
			/// Returns an array of unresolved generic arguments types.
			/// </summary>
			/// <remarks>
			/// <p>
			/// A empty string represents a type parameter that 
			/// did not have been substituted by a specific type.
			/// </p>
			/// </remarks>
			/// <returns>
			/// An array of strings that represents the unresolved generic 
			/// arguments types or an empty array if not generic.
			/// </returns>
			public string[] GetGenericArguments()
			{
				if (_unresolvedGenericArguments == null)
				{
					return new string[0];
				}
				return _unresolvedGenericArguments;
			}

			private void ParseGenericArguments(string originalString)
			{
				if (!generic.IsMatch(originalString))
				{
					_unresolvedGenericTypeName = originalString;
					return;
				}
				int num = originalString.IndexOf("[[");
				int num2 = originalString.LastIndexOf("]]");
				if (num2 != -1)
				{
					SplitGenericArguments(originalString.Substring(num + 1, num2 - num));
					_unresolvedGenericTypeName = originalString.Remove(num, num2 - num + 2);
				}
			}

			private void SplitGenericArguments(string originalArgs)
			{
				IList<string> list = new List<string>();
				if (originalArgs.Contains("],["))
				{
					list = Parse(originalArgs);
				}
				else
				{
					string item = originalArgs.Substring(1, originalArgs.Length - 2).Trim();
					list.Add(item);
				}
				_unresolvedGenericArguments = new string[list.Count];
				list.CopyTo(_unresolvedGenericArguments, 0);
			}

			private IList<string> Parse(string args)
			{
				StringBuilder stringBuilder = new StringBuilder();
				IList<string> list = new List<string>();
				TextReader textReader = new StringReader(args);
				int num = 0;
				bool flag = false;
				do
				{
					char c = (char)textReader.Read();
					switch (c)
					{
					case '[':
						num++;
						flag = true;
						break;
					case ']':
						num--;
						break;
					}
					stringBuilder.Append(c);
					if (flag && num == 0)
					{
						string text = stringBuilder.ToString();
						text = text.Substring(1, text.Length - 2);
						list.Add(text);
						textReader.Read();
						stringBuilder = new StringBuilder();
					}
				}
				while (textReader.Peek() != -1);
				return list;
			}
		}

		/// <summary>
		/// Holds data about a <see cref="T:System.Type" /> and it's
		/// attendant <see cref="T:System.Reflection.Assembly" />.
		/// </summary>
		internal class TypeAssemblyInfo
		{
			/// <summary>
			/// The string that separates a <see cref="T:System.Type" /> name
			/// from the name of it's attendant <see cref="T:System.Reflection.Assembly" />
			/// in an assembly qualified type name.
			/// </summary>
			public const string TYPE_ASSEMBLY_SEPARATOR = ",";

			public const string NULLABLE_TYPE = "System.Nullable";

			public const string NULLABLE_TYPE_ASSEMBLY_SEPARATOR = "]],";

			private string _unresolvedAssemblyName = string.Empty;

			private string _unresolvedTypeName = string.Empty;

			/// <summary>
			/// The (unresolved) type name portion of the original type name.
			/// </summary>
			public string TypeName => _unresolvedTypeName;

			/// <summary>
			/// The (unresolved, possibly partial) name of the attandant assembly.
			/// </summary>
			public string AssemblyName => _unresolvedAssemblyName;

			/// <summary>
			/// Is the type name being resolved assembly qualified?
			/// </summary>
			public bool IsAssemblyQualified => HasText(AssemblyName);

			/// <summary>
			/// Creates a new instance of the TypeAssemblyInfo class.
			/// </summary>
			/// <param name="unresolvedTypeName">
			/// The unresolved name of a <see cref="T:System.Type" />.
			/// </param>
			public TypeAssemblyInfo(string unresolvedTypeName)
			{
				SplitTypeAndAssemblyNames(unresolvedTypeName);
			}

			private bool HasText(string target)
			{
				if (target == null)
				{
					return false;
				}
				return HasLength(target.Trim());
			}

			private bool HasLength(string target)
			{
				return target != null && target.Length > 0;
			}

			private void SplitTypeAndAssemblyNames(string originalTypeName)
			{
				if (originalTypeName.StartsWith("System.Nullable"))
				{
					int num = originalTypeName.IndexOf("]],");
					if (num < 0)
					{
						_unresolvedTypeName = originalTypeName;
						return;
					}
					_unresolvedTypeName = originalTypeName.Substring(0, num + 2).Trim();
					_unresolvedAssemblyName = originalTypeName.Substring(num + 3).Trim();
				}
				else
				{
					int num = originalTypeName.IndexOf(",");
					if (num < 0)
					{
						_unresolvedTypeName = originalTypeName;
						return;
					}
					_unresolvedTypeName = originalTypeName.Substring(0, num).Trim();
					_unresolvedAssemblyName = originalTypeName.Substring(num + 1).Trim();
				}
			}
		}

		private const string NULLABLE_TYPE = "System.Nullable";

		/// <summary>
		/// Resolves the supplied <paramref name="typeName" /> to a
		/// <see cref="T:System.Type" /> instance.
		/// </summary>
		/// <param name="typeName">
		/// The unresolved name of a <see cref="T:System.Type" />.
		/// </param>
		/// <returns>
		/// A resolved <see cref="T:System.Type" /> instance.
		/// </returns>
		/// <exception cref="T:System.TypeLoadException">
		/// If the supplied <paramref name="typeName" /> could not be resolved
		/// to a <see cref="T:System.Type" />.
		/// </exception>
		public virtual Type Resolve(string typeName)
		{
			Type type = ResolveGenericType(typeName.Replace(" ", string.Empty));
			if (type == null)
			{
				type = ResolveType(typeName.Replace(" ", string.Empty));
			}
			return type;
		}

		/// <summary>
		/// Resolves the supplied generic <paramref name="typeName" />,
		/// substituting recursively all its type parameters., 
		/// to a <see cref="T:System.Type" /> instance.
		/// </summary>
		/// <param name="typeName">
		/// The (possibly generic) name of a <see cref="T:System.Type" />.
		/// </param>
		/// <returns>
		/// A resolved <see cref="T:System.Type" /> instance.
		/// </returns>
		/// <exception cref="T:System.TypeLoadException">
		/// If the supplied <paramref name="typeName" /> could not be resolved
		/// to a <see cref="T:System.Type" />.
		/// </exception>
		private Type ResolveGenericType(string typeName)
		{
			if (typeName == null || typeName.Trim().Length == 0)
			{
				throw BuildTypeLoadException(typeName);
			}
			if (typeName.StartsWith("System.Nullable"))
			{
				return null;
			}
			GenericArgumentsInfo genericArgumentsInfo = new GenericArgumentsInfo(typeName);
			Type type = null;
			try
			{
				if (genericArgumentsInfo.ContainsGenericArguments)
				{
					type = TypeUtils.ResolveType(genericArgumentsInfo.GenericTypeName);
					if (!genericArgumentsInfo.IsGenericDefinition)
					{
						string[] genericArguments = genericArgumentsInfo.GetGenericArguments();
						Type[] array = new Type[genericArguments.Length];
						for (int i = 0; i < genericArguments.Length; i++)
						{
							array[i] = TypeUtils.ResolveType(genericArguments[i]);
						}
						type = type.MakeGenericType(array);
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is TypeLoadException)
				{
					throw;
				}
				throw BuildTypeLoadException(typeName, ex);
			}
			return type;
		}

		/// <summary>
		/// Resolves the supplied <paramref name="typeName" /> to a
		/// <see cref="T:System.Type" />
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
		/// If the supplied <paramref name="typeName" /> could not be resolved
		/// to a <see cref="T:System.Type" />.
		/// </exception>
		private Type ResolveType(string typeName)
		{
			if (typeName == null || typeName.Trim().Length == 0)
			{
				throw BuildTypeLoadException(typeName);
			}
			TypeAssemblyInfo typeAssemblyInfo = new TypeAssemblyInfo(typeName);
			Type type = null;
			try
			{
				type = (typeAssemblyInfo.IsAssemblyQualified ? LoadTypeDirectlyFromAssembly(typeAssemblyInfo) : LoadTypeByIteratingOverAllLoadedAssemblies(typeAssemblyInfo));
			}
			catch (Exception ex)
			{
				throw BuildTypeLoadException(typeName, ex);
			}
			if (type == null)
			{
				throw BuildTypeLoadException(typeName);
			}
			return type;
		}

		/// <summary>
		/// Uses <see cref="M:System.Reflection.Assembly.LoadWithPartialName(System.String)" />
		/// to load an <see cref="T:System.Reflection.Assembly" /> and then the attendant
		/// <see cref="T:System.Type" /> referred to by the <paramref name="typeInfo" />
		/// parameter.
		/// </summary>
		/// <remarks>
		/// <p>
		/// <see cref="M:System.Reflection.Assembly.LoadWithPartialName(System.String)" /> is
		/// deprecated in .NET 2.0, but is still used here (even when this class is
		/// compiled for .NET 2.0);
		/// <see cref="M:System.Reflection.Assembly.LoadWithPartialName(System.String)" /> will
		/// still resolve (non-.NET Framework) local assemblies when given only the
		/// display name of an assembly (the behaviour for .NET Framework assemblies
		/// and strongly named assemblies is documented in the docs for the
		/// <see cref="M:System.Reflection.Assembly.LoadWithPartialName(System.String)" /> method).
		/// </p>
		/// </remarks>
		/// <param name="typeInfo">
		/// The assembly and type to be loaded.
		/// </param>
		/// <returns>
		/// A <see cref="T:System.Type" />, or <see lang="null" />.
		/// </returns>
		/// <exception cref="T:System.Exception">
		/// <see cref="M:System.Reflection.Assembly.LoadWithPartialName(System.String)" />
		/// </exception>
		private static Type LoadTypeDirectlyFromAssembly(TypeAssemblyInfo typeInfo)
		{
			Type result = null;
			Assembly assembly = null;
			assembly = Assembly.Load(typeInfo.AssemblyName);
			if (assembly != null)
			{
				result = assembly.GetType(typeInfo.TypeName, throwOnError: true, ignoreCase: true);
			}
			return result;
		}

		/// <summary>
		/// Check all assembly
		/// to load the attendant <see cref="T:System.Type" /> referred to by 
		/// the <paramref name="typeInfo" /> parameter.
		/// </summary>
		/// <param name="typeInfo">
		/// The type to be loaded.
		/// </param>
		/// <returns>
		/// A <see cref="T:System.Type" />, or <see lang="null" />.
		/// </returns>
		private static Type LoadTypeByIteratingOverAllLoadedAssemblies(TypeAssemblyInfo typeInfo)
		{
			Type type = null;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				type = assembly.GetType(typeInfo.TypeName, throwOnError: false, ignoreCase: false);
				if (type != null)
				{
					break;
				}
			}
			return type;
		}

		private static TypeLoadException BuildTypeLoadException(string typeName)
		{
			return new TypeLoadException("Could not load type from string value '" + typeName + "'.");
		}

		private static TypeLoadException BuildTypeLoadException(string typeName, Exception ex)
		{
			return new TypeLoadException("Could not load type from string value '" + typeName + "'.", ex);
		}
	}
}
