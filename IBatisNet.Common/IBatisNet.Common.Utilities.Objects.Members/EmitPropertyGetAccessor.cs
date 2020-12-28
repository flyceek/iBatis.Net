using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.EmitPropertyGetAccessor" /> class provides an IL-based get access   
	/// to a property of a specified target class.
	/// </summary>
	public sealed class EmitPropertyGetAccessor : BaseAccessor, IGetAccessor, IAccessor, IGet
	{
		/// <summary>
		/// The property name
		/// </summary>
		private string _propertyName = string.Empty;

		/// <summary>
		/// The property type
		/// </summary>
		private Type _propertyType = null;

		/// <summary>
		/// The class parent type
		/// </summary>
		private Type _targetType = null;

		private bool _canRead = false;

		/// <summary>
		/// The IL emitted IGet
		/// </summary>
		private IGet _emittedGet = null;

		/// <summary>
		/// Gets the property's name.
		/// </summary>
		/// <value></value>
		public string Name => _propertyName;

		/// <summary>
		/// Gets the property's type.
		/// </summary>
		/// <value></value>
		public Type MemberType => _propertyType;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.EmitPropertyGetAccessor" /> class.
		/// </summary>
		/// <param name="targetObjectType">Type of the target object.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="assemblyBuilder">The <see cref="T:System.Reflection.Emit.AssemblyBuilder" />.</param>
		/// <param name="moduleBuilder">The <see cref="T:System.Reflection.Emit.ModuleBuilder" />.</param>
		public EmitPropertyGetAccessor(Type targetObjectType, string propertyName, AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
		{
			_targetType = targetObjectType;
			_propertyName = propertyName;
			PropertyInfo property = _targetType.GetProperty(propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			if (property == null)
			{
				property = _targetType.GetProperty(propertyName);
			}
			if (property == null)
			{
				throw new NotSupportedException($"Property \"{propertyName}\" does not exist for type {_targetType}.");
			}
			_propertyType = property.PropertyType;
			_canRead = property.CanRead;
			EmitIL(assemblyBuilder, moduleBuilder);
		}

		/// <summary>
		/// This method create a new type oject for the the property accessor class 
		/// that will provide dynamic access.
		/// </summary>
		/// <param name="assemblyBuilder">The assembly builder.</param>
		/// <param name="moduleBuilder">The module builder.</param>
		private void EmitIL(AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
		{
			EmitType(moduleBuilder);
			_emittedGet = assemblyBuilder.CreateInstance("GetFor" + _targetType.FullName + _propertyName) as IGet;
			nullInternal = GetNullInternal(_propertyType);
			if (_emittedGet == null)
			{
				throw new NotSupportedException($"Unable to create a get property accessor for \"{_propertyType}\".");
			}
		}

		/// <summary>
		/// Create an type that will provide the set access method.
		/// </summary>
		/// <remarks>
		///  new ReflectionPermission(PermissionState.Unrestricted).Assert();
		///  CodeAccessPermission.RevertAssert();
		/// </remarks>
		/// <param name="moduleBuilder">The module builder.</param>
		private void EmitType(ModuleBuilder moduleBuilder)
		{
			TypeBuilder typeBuilder = moduleBuilder.DefineType("GetFor" + _targetType.FullName + _propertyName, TypeAttributes.Public | TypeAttributes.Sealed);
			typeBuilder.AddInterfaceImplementation(typeof(IGet));
			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
			Type[] parameterTypes = new Type[1]
			{
				typeof(object)
			};
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual, typeof(object), parameterTypes);
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			if (_canRead)
			{
				MethodInfo method = _targetType.GetMethod("get_" + _propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
				if (method == null)
				{
					method = _targetType.GetMethod("get_" + _propertyName);
				}
				iLGenerator.DeclareLocal(typeof(object));
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Castclass, _targetType);
				iLGenerator.EmitCall(OpCodes.Call, method, null);
				if (method.ReturnType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, method.ReturnType);
				}
				iLGenerator.Emit(OpCodes.Stloc_0);
				iLGenerator.Emit(OpCodes.Ldloc_0);
				iLGenerator.Emit(OpCodes.Ret);
			}
			else
			{
				iLGenerator.ThrowException(typeof(MissingMethodException));
			}
			typeBuilder.CreateType();
		}

		/// <summary>
		/// Gets the property value from the specified target.
		/// </summary>
		/// <param name="target">Target object.</param>
		/// <returns>Property value.</returns>
		public object Get(object target)
		{
			if (_canRead)
			{
				return _emittedGet.Get(target);
			}
			throw new NotSupportedException($"Property \"{_propertyName}\" on type {_targetType} doesn't have a get method.");
		}
	}
}
