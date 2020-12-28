using System;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// Factory contact to build <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ISetAccessor" /> for a type.
	/// </summary>
	public interface ISetAccessorFactory
	{
		/// <summary>
		/// Generate an <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ISetAccessor" /> instance.
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="name">Field or Property name.</param>
		/// <returns>null if the generation fail</returns>
		ISetAccessor CreateSetAccessor(Type targetType, string name);
	}
}
