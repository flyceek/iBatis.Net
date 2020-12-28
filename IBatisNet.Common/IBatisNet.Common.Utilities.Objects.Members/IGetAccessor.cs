namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGetAccessor" /> interface defines a field/property get accessor.
	/// provides <c>Reflection.Emit</c>-generated <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGet" />
	/// implementations for drastically improved performance over default late-bind 
	/// invoke.
	/// </summary>
	public interface IGetAccessor : IAccessor, IGet
	{
	}
}
