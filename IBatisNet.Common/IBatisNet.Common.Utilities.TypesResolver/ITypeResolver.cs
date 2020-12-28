using System;

namespace IBatisNet.Common.Utilities.TypesResolver
{
	/// <summary>
	/// Resolves a <see cref="T:System.Type" /> by name.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The rationale behind the creation of this interface is to centralise
	/// the resolution of type names to <see cref="T:System.Type" /> instances
	/// beyond that offered by the plain vanilla
	/// <see cref="M:System.Type.GetType(System.String)" /> method call.
	/// </p>
	/// </remarks>
	public interface ITypeResolver
	{
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
		Type Resolve(string typeName);
	}
}
