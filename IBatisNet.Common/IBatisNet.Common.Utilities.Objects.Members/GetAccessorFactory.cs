using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using IBatisNet.Common.Exceptions;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// A factory to build <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGetAccessorFactory" /> for a type.
	/// </summary>
	public class GetAccessorFactory : IGetAccessorFactory
	{
		private delegate IGetAccessor CreatePropertyGetAccessor(Type targetType, string propertyName);

		private delegate IGetAccessor CreateFieldGetAccessor(Type targetType, string fieldName);

		private CreatePropertyGetAccessor _createPropertyGetAccessor = null;

		private CreateFieldGetAccessor _createFieldGetAccessor = null;

		private IDictionary _cachedIGetAccessor = new HybridDictionary();

		private AssemblyBuilder _assemblyBuilder = null;

		private ModuleBuilder _moduleBuilder = null;

		private object _syncObject = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.GetAccessorFactory" /> class.
		/// </summary>
		/// <param name="allowCodeGeneration">if set to <c>true</c> [allow code generation].</param>
		public GetAccessorFactory(bool allowCodeGeneration)
		{
			if (allowCodeGeneration)
			{
				if (Environment.Version.Major >= 2)
				{
					_createPropertyGetAccessor = CreateDynamicPropertyGetAccessor;
					_createFieldGetAccessor = CreateDynamicFieldGetAccessor;
					return;
				}
				AssemblyName assemblyName = new AssemblyName
				{
					Name = "iBATIS.FastGetAccessor" + HashCodeProvider.GetIdentityHashCode(this)
				};
				_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
				_moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName.Name + ".dll");
				_createPropertyGetAccessor = CreatePropertyAccessor;
				_createFieldGetAccessor = CreateFieldAccessor;
			}
			else
			{
				_createPropertyGetAccessor = CreateReflectionPropertyGetAccessor;
				_createFieldGetAccessor = CreateReflectionFieldGetAccessor;
			}
		}

		/// <summary>
		/// Create a Dynamic IGetAccessor instance for a property
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="propertyName">Property name.</param>
		/// <returns>null if the generation fail</returns>
		private IGetAccessor CreateDynamicPropertyGetAccessor(Type targetType, string propertyName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			PropertyInfo propertyInfo = (PropertyInfo)instance.GetGetter(propertyName);
			if (propertyInfo.CanRead)
			{
				MethodInfo methodInfo = null;
				methodInfo = propertyInfo.GetGetMethod();
				if (methodInfo != null)
				{
					return new DelegatePropertyGetAccessor(targetType, propertyName);
				}
				return new ReflectionPropertyGetAccessor(targetType, propertyName);
			}
			throw new NotSupportedException($"Property \"{propertyInfo.Name}\" on type {targetType} cannot be get.");
		}

		/// <summary>
		/// Create a Dynamic IGetAccessor instance for a field
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="fieldName">Property name.</param>
		/// <returns>null if the generation fail</returns>
		private IGetAccessor CreateDynamicFieldGetAccessor(Type targetType, string fieldName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			FieldInfo fieldInfo = (FieldInfo)instance.GetGetter(fieldName);
			if (fieldInfo.IsPublic)
			{
				return new DelegateFieldGetAccessor(targetType, fieldName);
			}
			return new ReflectionFieldGetAccessor(targetType, fieldName);
		}

		/// <summary>
		/// Create a IGetAccessor instance for a property
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="propertyName">Property name.</param>
		/// <returns>null if the generation fail</returns>
		private IGetAccessor CreatePropertyAccessor(Type targetType, string propertyName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			PropertyInfo propertyInfo = (PropertyInfo)instance.GetGetter(propertyName);
			if (propertyInfo.CanRead)
			{
				MethodInfo methodInfo = null;
				methodInfo = propertyInfo.GetGetMethod();
				if (methodInfo != null)
				{
					return new EmitPropertyGetAccessor(targetType, propertyName, _assemblyBuilder, _moduleBuilder);
				}
				return new ReflectionPropertyGetAccessor(targetType, propertyName);
			}
			throw new NotSupportedException($"Property \"{propertyInfo.Name}\" on type {targetType} cannot be get.");
		}

		/// <summary>
		/// Create a IGetAccessor instance for a field
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="fieldName">Field name.</param>
		/// <returns>null if the generation fail</returns>
		private IGetAccessor CreateFieldAccessor(Type targetType, string fieldName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			FieldInfo fieldInfo = (FieldInfo)instance.GetGetter(fieldName);
			if (fieldInfo.IsPublic)
			{
				return new EmitFieldGetAccessor(targetType, fieldName, _assemblyBuilder, _moduleBuilder);
			}
			return new ReflectionFieldGetAccessor(targetType, fieldName);
		}

		/// <summary>
		/// Create a Reflection IGetAccessor instance for a property
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="propertyName">Property name.</param>
		/// <returns>null if the generation fail</returns>
		private IGetAccessor CreateReflectionPropertyGetAccessor(Type targetType, string propertyName)
		{
			return new ReflectionPropertyGetAccessor(targetType, propertyName);
		}

		/// <summary>
		/// Create Reflection IGetAccessor instance for a field
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="fieldName">field name.</param>
		/// <returns>null if the generation fail</returns>
		private IGetAccessor CreateReflectionFieldGetAccessor(Type targetType, string fieldName)
		{
			return new ReflectionFieldGetAccessor(targetType, fieldName);
		}

		/// <summary>
		/// Generate an <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGetAccessor" /> instance.
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="name">Field or Property name.</param>
		/// <returns>null if the generation fail</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IGetAccessor CreateGetAccessor(Type targetType, string name)
		{
			string key = new StringBuilder(targetType.FullName).Append(".").Append(name).ToString();
			if (_cachedIGetAccessor.Contains(key))
			{
				return (IGetAccessor)_cachedIGetAccessor[key];
			}
			IGetAccessor getAccessor = null;
			lock (_syncObject)
			{
				if (!_cachedIGetAccessor.Contains(key))
				{
					ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
					MemberInfo getter = instance.GetGetter(name);
					if (!(getter != null))
					{
						throw new ProbeException($"No property or field named \"{name}\" exists for type {targetType}.");
					}
					if (getter is PropertyInfo)
					{
						getAccessor = _createPropertyGetAccessor(targetType, name);
						_cachedIGetAccessor[key] = getAccessor;
					}
					else
					{
						getAccessor = _createFieldGetAccessor(targetType, name);
						_cachedIGetAccessor[key] = getAccessor;
					}
				}
				else
				{
					getAccessor = (IGetAccessor)_cachedIGetAccessor[key];
				}
			}
			return getAccessor;
		}
	}
}
