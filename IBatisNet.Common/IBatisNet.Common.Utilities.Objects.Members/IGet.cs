namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGet" /> interface defines a field/property get contrat to get the
	/// value of a field or a property.
	/// </summary>
	public interface IGet
	{
		/// <summary>
		/// Gets the value stored in the field/property for the specified target.
		/// </summary>
		/// <param name="target">Object to retrieve the field/property from.</param>
		/// <returns>The value.</returns>
		object Get(object target);
	}
}
