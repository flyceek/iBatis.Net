using System;

namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// A <see cref="T:IBatisNet.Common.Utilities.Objects.IObjectFactory" /> implementation that can create objects 
	/// via Activator.CreateInstance
	/// </summary>
	public class ActivatorObjectFactory : IObjectFactory
	{
		/// <summary>
		/// Create a new see <see cref="T:IBatisNet.Common.Utilities.Objects.IObjectFactory" /> instance for a given type
		/// </summary>
		/// <param name="typeToCreate">The type instance to build</param>
		/// <param name="types">The types of the constructor arguments</param>
		/// <returns>Returns a new <see cref="T:IBatisNet.Common.Utilities.Objects.IObjectFactory" /> instance.</returns>
		public IFactory CreateFactory(Type typeToCreate, Type[] types)
		{
			return new ActivatorFactory(typeToCreate);
		}
	}
}
