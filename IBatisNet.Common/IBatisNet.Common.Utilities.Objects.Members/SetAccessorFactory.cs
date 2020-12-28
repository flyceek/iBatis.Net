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
	/// A factory to build <see cref="T:IBatisNet.Common.Utilities.Objects.Members.SetAccessorFactory" /> for a type.
	/// </summary>
	public class SetAccessorFactory : ISetAccessorFactory
	{
		private delegate ISetAccessor CreatePropertySetAccessor(Type targetType, string propertyName);

		private delegate ISetAccessor CreateFieldSetAccessor(Type targetType, string fieldName);

		private CreatePropertySetAccessor _createPropertySetAccessor = null;

		private CreateFieldSetAccessor _createFieldSetAccessor = null;

		private IDictionary _cachedISetAccessor = new HybridDictionary();

		private AssemblyBuilder _assemblyBuilder = null;

		private ModuleBuilder _moduleBuilder = null;

		private object _syncObject = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.SetAccessorFactory" /> class.
		/// </summary>
		/// <param name="allowCodeGeneration">if set to <c>true</c> [allow code generation].</param>
		public SetAccessorFactory(bool allowCodeGeneration)
		{
			if (allowCodeGeneration)
			{
				if (Environment.Version.Major >= 2)
				{
					_createPropertySetAccessor = CreateDynamicPropertySetAccessor;
					_createFieldSetAccessor = CreateDynamicFieldSetAccessor;
					return;
				}
				AssemblyName assemblyName = new AssemblyName
				{
					Name = "iBATIS.FastSetAccessor" + HashCodeProvider.GetIdentityHashCode(this)
				};
				_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
				_moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName.Name + ".dll");
				_createPropertySetAccessor = CreatePropertyAccessor;
				_createFieldSetAccessor = CreateFieldAccessor;
			}
			else
			{
				_createPropertySetAccessor = CreateReflectionPropertySetAccessor;
				_createFieldSetAccessor = CreateReflectionFieldSetAccessor;
			}
		}

		/// <summary>
		/// Create a Dynamic ISetAccessor instance for a property
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="propertyName">Property name.</param>
		/// <returns>null if the generation fail</returns>
		private ISetAccessor CreateDynamicPropertySetAccessor(Type targetType, string propertyName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			PropertyInfo propertyInfo = (PropertyInfo)instance.GetSetter(propertyName);
			if (propertyInfo.CanWrite)
			{
				MethodInfo methodInfo = null;
				methodInfo = propertyInfo.GetSetMethod();
				if (methodInfo != null)
				{
					return new DelegatePropertySetAccessor(targetType, propertyName);
				}
				return new ReflectionPropertySetAccessor(targetType, propertyName);
			}
			throw new NotSupportedException($"Property \"{propertyInfo.Name}\" on type {targetType} cannot be set.");
		}

		/// <summary>
		/// Create a Dynamic ISetAccessor instance for a field
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="fieldName">field name.</param>
		/// <returns>null if the generation fail</returns>
		private ISetAccessor CreateDynamicFieldSetAccessor(Type targetType, string fieldName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			FieldInfo fieldInfo = (FieldInfo)instance.GetSetter(fieldName);
			if (fieldInfo.IsPublic)
			{
				return new DelegateFieldSetAccessor(targetType, fieldName);
			}
			return new ReflectionFieldSetAccessor(targetType, fieldName);
		}

		/// <summary>
		/// Create a ISetAccessor instance for a property
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="propertyName">Property name.</param>
		/// <returns>null if the generation fail</returns>
		private ISetAccessor CreatePropertyAccessor(Type targetType, string propertyName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			PropertyInfo propertyInfo = (PropertyInfo)instance.GetSetter(propertyName);
			if (propertyInfo.CanWrite)
			{
				MethodInfo methodInfo = null;
				methodInfo = propertyInfo.GetSetMethod();
				if (methodInfo != null)
				{
					return new EmitPropertySetAccessor(targetType, propertyName, _assemblyBuilder, _moduleBuilder);
				}
				return new ReflectionPropertySetAccessor(targetType, propertyName);
			}
			throw new NotSupportedException($"Property \"{propertyInfo.Name}\" on type {targetType} cannot be set.");
		}

		/// <summary>
		/// Create a ISetAccessor instance for a field
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="fieldName">Field name.</param>
		/// <returns>null if the generation fail</returns>
		private ISetAccessor CreateFieldAccessor(Type targetType, string fieldName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			FieldInfo fieldInfo = (FieldInfo)instance.GetSetter(fieldName);
			if (fieldInfo.IsPublic)
			{
				return new EmitFieldSetAccessor(targetType, fieldName, _assemblyBuilder, _moduleBuilder);
			}
			return new ReflectionFieldSetAccessor(targetType, fieldName);
		}

		/// <summary>
		/// Create a Reflection ISetAccessor instance for a property
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="propertyName">Property name.</param>
		/// <returns>null if the generation fail</returns>
		private ISetAccessor CreateReflectionPropertySetAccessor(Type targetType, string propertyName)
		{
			return new ReflectionPropertySetAccessor(targetType, propertyName);
		}

		/// <summary>
		/// Create Reflection ISetAccessor instance for a field
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="fieldName">field name.</param>
		/// <returns>null if the generation fail</returns>
		private ISetAccessor CreateReflectionFieldSetAccessor(Type targetType, string fieldName)
		{
			return new ReflectionFieldSetAccessor(targetType, fieldName);
		}

		/// <summary>
		/// Generate an <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ISetAccessor" /> instance.
		/// </summary>
		/// <param name="targetType">Target object type.</param>
		/// <param name="name">Field or Property name.</param>
		/// <returns>null if the generation fail</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public ISetAccessor CreateSetAccessor(Type targetType, string name)
		{
			string key = new StringBuilder(targetType.FullName).Append(".").Append(name).ToString();
			if (_cachedISetAccessor.Contains(key))
			{
				return (ISetAccessor)_cachedISetAccessor[key];
			}
			ISetAccessor setAccessor = null;
			lock (_syncObject)
			{
				if (!_cachedISetAccessor.Contains(key))
				{
					ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
					MemberInfo setter = instance.GetSetter(name);
					if (!(setter != null))
					{
						throw new ProbeException($"No property or field named \"{name}\" exists for type {targetType}.");
					}
					if (setter is PropertyInfo)
					{
						setAccessor = _createPropertySetAccessor(targetType, name);
						_cachedISetAccessor[key] = setAccessor;
					}
					else
					{
						setAccessor = _createFieldSetAccessor(targetType, name);
						_cachedISetAccessor[key] = setAccessor;
					}
				}
				else
				{
					setAccessor = (ISetAccessor)_cachedISetAccessor[key];
				}
			}
			return setAccessor;
		}
	}
}
