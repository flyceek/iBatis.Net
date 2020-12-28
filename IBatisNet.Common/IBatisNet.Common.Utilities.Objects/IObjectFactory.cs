using System;

namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// A factory that can create objects 
	/// </summary>
	public interface IObjectFactory
	{
		/// <summary>
		/// Create a new <see cref="T:IBatisNet.Common.Utilities.Objects.IFactory" /> instance for a given type
		/// </summary>
		/// <param name="typeToCreate">The type instance to build</param>
		/// <param name="types">The types of the constructor arguments</param>
		/// <returns>Returns a new see cref="IFactory"/&gt; instance</returns>
		IFactory CreateFactory(Type typeToCreate, Type[] types);
	}
}
