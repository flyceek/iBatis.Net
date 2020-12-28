using System;
using System.Collections;
using System.Collections.Specialized;

namespace IBatisNet.Common.Utilities.TypesResolver
{
	/// <summary>
	/// Resolves (instantiates) a <see cref="T:System.Type" /> by it's (possibly
	/// assembly qualified) name, and caches the <see cref="T:System.Type" />
	/// instance against the type name.
	/// </summary>
	public class CachedTypeResolver : ITypeResolver
	{
		/// <summary>
		/// The cache, mapping type names (<see cref="T:System.String" /> instances) against
		/// <see cref="T:System.Type" /> instances.
		/// </summary>
		private IDictionary _typeCache = new HybridDictionary();

		private ITypeResolver _typeResolver = null;

		/// <summary>
		/// Creates a new instance of the <see cref="T:IBatisNet.Common.Utilities.TypesResolver.CachedTypeResolver" /> class.
		/// </summary>
		/// <param name="typeResolver">
		/// The <see cref="T:IBatisNet.Common.Utilities.TypesResolver.ITypeResolver" /> that this instance will delegate
		/// actual <see cref="T:System.Type" /> resolution to if a <see cref="T:System.Type" />
		/// cannot be found in this instance's <see cref="T:System.Type" /> cache.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// If the supplied <paramref name="typeResolver" /> is <see langword="null" />.
		/// </exception>
		public CachedTypeResolver(ITypeResolver typeResolver)
		{
			_typeResolver = typeResolver;
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
		public Type Resolve(string typeName)
		{
			if (typeName == null || typeName.Trim().Length == 0)
			{
				throw new TypeLoadException("Could not load type from string value '" + typeName + "'.");
			}
			Type type = null;
			try
			{
				type = _typeCache[typeName] as Type;
				if (type == null)
				{
					type = _typeResolver.Resolve(typeName);
					_typeCache[typeName] = type;
				}
			}
			catch (Exception ex)
			{
				if (ex is TypeLoadException)
				{
					throw;
				}
				throw new TypeLoadException("Could not load type from string value '" + typeName + "'.", ex);
			}
			return type;
		}
	}
}
