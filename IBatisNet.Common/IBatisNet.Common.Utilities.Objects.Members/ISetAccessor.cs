namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ISetAccessor" /> interface defines a field/property set accessor.
	/// provides <c>Reflection.Emit</c>-generated <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ISet" /> 
	/// implementations for drastically improved performance over default late-bind 
	/// invoke.
	/// </summary>
	public interface ISetAccessor : IAccessor, ISet
	{
	}
}
