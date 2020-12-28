using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// A <see cref="T:IBatisNet.Common.Utilities.Objects.IObjectFactory" /> implementation that can create objects via IL code
	/// </summary>
	public sealed class EmitObjectFactory : IObjectFactory
	{
		private IDictionary _cachedfactories = new HybridDictionary();

		private FactoryBuilder _factoryBuilder = null;

		private object _padlock = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.EmitObjectFactory" /> class.
		/// </summary>
		public EmitObjectFactory()
		{
			_factoryBuilder = new FactoryBuilder();
		}

		/// <summary>
		/// Create a new <see cref="T:IBatisNet.Common.Utilities.Objects.IFactory" /> instance for a given type
		/// </summary>
		/// <param name="typeToCreate">The type instance to build</param>
		/// <param name="types">The types of the constructor arguments</param>
		/// <returns>Returns a new <see cref="T:IBatisNet.Common.Utilities.Objects.IFactory" /> instance.</returns>
		public IFactory CreateFactory(Type typeToCreate, Type[] types)
		{
			string key = GenerateKey(typeToCreate, types);
			IFactory factory = _cachedfactories[key] as IFactory;
			if (factory == null)
			{
				lock (_padlock)
				{
					factory = _cachedfactories[key] as IFactory;
					if (factory == null)
					{
						factory = _factoryBuilder.CreateFactory(typeToCreate, types);
						_cachedfactories[key] = factory;
					}
				}
			}
			return factory;
		}

		/// <summary>
		/// Generates the key for a cache entry.
		/// </summary>
		/// <param name="typeToCreate">The type instance to build.</param>
		/// <param name="arguments">The types of the constructor arguments</param>
		/// <returns>The key for a cache entry.</returns>
		private string GenerateKey(Type typeToCreate, object[] arguments)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(typeToCreate.ToString());
			stringBuilder.Append(".");
			if (arguments != null && arguments.Length != 0)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					stringBuilder.Append(".").Append(arguments[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
