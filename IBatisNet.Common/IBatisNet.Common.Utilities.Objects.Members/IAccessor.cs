using System;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IAccessor" /> interface defines a field/property contract.
	/// </summary>
	public interface IAccessor
	{
		/// <summary>
		/// Gets the member name.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets the type of this member (field or property).
		/// </summary>
		Type MemberType
		{
			get;
		}
	}
}
