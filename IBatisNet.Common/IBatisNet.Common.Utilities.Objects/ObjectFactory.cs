using System;
using System.Reflection;
using IBatisNet.Common.Logging;

namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// A factory to create objects 
	/// </summary>
	public class ObjectFactory : IObjectFactory
	{
		private IObjectFactory _objectFactory = null;

		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="allowCodeGeneration"></param>
		public ObjectFactory(bool allowCodeGeneration)
		{
			if (allowCodeGeneration)
			{
				if (Environment.Version.Major >= 2)
				{
					_objectFactory = new DelegateObjectFactory();
				}
				else
				{
					_objectFactory = new EmitObjectFactory();
				}
			}
			else
			{
				_objectFactory = new ActivatorObjectFactory();
			}
		}

		/// <summary>
		/// Create a new factory instance for a given type
		/// </summary>
		/// <param name="typeToCreate">The type instance to build</param>
		/// <param name="types">The types of the constructor arguments</param>
		/// <returns>Returns a new instance factory</returns>
		public IFactory CreateFactory(Type typeToCreate, Type[] types)
		{
			if (_logger.IsDebugEnabled)
			{
				return new FactoryLogAdapter(typeToCreate, types, _objectFactory.CreateFactory(typeToCreate, types));
			}
			return _objectFactory.CreateFactory(typeToCreate, types);
		}
	}
}
