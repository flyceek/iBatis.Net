namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ISet" /> interface defines a field/property set contrat to set the
	/// value on a field or property.
	/// </summary>
	public interface ISet
	{
		/// <summary>
		/// Sets the value for the field/property of the specified target.
		/// </summary>
		/// <param name="target">Object to set the field/property on.</param>
		/// <param name="value">Value.</param>
		void Set(object target, object value);
	}
}
