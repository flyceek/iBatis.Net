using System;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// Factory contact to build <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGetAccessor" /> for a type.
	/// </summary>
	public interface IGetAccessorFactory
	{
		/// <summary>
		/// Generate an <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGetAccessor" /> instance.
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="name">Field or Property name.</param>
		/// <returns>null if the generation fail</returns>
		IGetAccessor CreateGetAccessor(Type targetType, string name);
	}
}
