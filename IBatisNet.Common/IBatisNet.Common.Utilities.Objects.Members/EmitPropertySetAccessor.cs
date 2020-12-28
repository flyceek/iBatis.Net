using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.EmitPropertySetAccessor" /> class provides an IL-based set access   
	/// to a property of a specified target class.
	/// </summary>
	public sealed class EmitPropertySetAccessor : BaseAccessor, ISetAccessor, IAccessor, ISet
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

		private bool _canWrite = false;

		/// <summary>
		/// The IL emitted ISet
		/// </summary>
		private ISet _emittedSet = null;

		/// <summary>
		/// Gets the member name.
		/// </summary>
		/// <value></value>
		public string Name => _propertyName;

		/// <summary>
		/// Gets the type of this member (field or property).
		/// </summary>
		/// <value></value>
		public Type MemberType => _propertyType;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.EmitPropertySetAccessor" /> class.
		/// Generates the implementation for setter methods.
		/// </summary>
		/// <param name="targetObjectType">Type of the target object.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="assemblyBuilder">The <see cref="T:System.Reflection.Emit.AssemblyBuilder" />.</param>
		/// <param name="moduleBuilder">The <see cref="T:System.Reflection.Emit.ModuleBuilder" />.</param>
		public EmitPropertySetAccessor(Type targetObjectType, string propertyName, AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
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
			_canWrite = property.CanWrite;
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
			_emittedSet = assemblyBuilder.CreateInstance("SetFor" + _targetType.FullName + _propertyName) as ISet;
			nullInternal = GetNullInternal(_propertyType);
			if (_emittedSet == null)
			{
				throw new NotSupportedException($"Unable to create a get propert accessor for '{_propertyName}' property on class  '{_propertyType.ToString()}'.");
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
			TypeBuilder typeBuilder = moduleBuilder.DefineType("SetFor" + _targetType.FullName + _propertyName, TypeAttributes.Public | TypeAttributes.Sealed);
			typeBuilder.AddInterfaceImplementation(typeof(ISet));
			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
			Type[] parameterTypes = new Type[2]
			{
				typeof(object),
				typeof(object)
			};
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("Set", MethodAttributes.Public | MethodAttributes.Virtual, null, parameterTypes);
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			if (_canWrite)
			{
				MethodInfo method = _targetType.GetMethod("set_" + _propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
				if (method == null)
				{
					method = _targetType.GetMethod("set_" + _propertyName);
				}
				Type parameterType = method.GetParameters()[0].ParameterType;
				iLGenerator.DeclareLocal(parameterType);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Castclass, _targetType);
				iLGenerator.Emit(OpCodes.Ldarg_2);
				if (parameterType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Unbox, parameterType);
					if (BaseAccessor.typeToOpcode[parameterType] != null)
					{
						OpCode opcode = (OpCode)BaseAccessor.typeToOpcode[parameterType];
						iLGenerator.Emit(opcode);
					}
					else
					{
						iLGenerator.Emit(OpCodes.Ldobj, parameterType);
					}
				}
				else
				{
					iLGenerator.Emit(OpCodes.Castclass, parameterType);
				}
				iLGenerator.EmitCall(OpCodes.Callvirt, method, null);
				iLGenerator.Emit(OpCodes.Ret);
			}
			else
			{
				iLGenerator.ThrowException(typeof(MissingMethodException));
			}
			typeBuilder.CreateType();
		}

		/// <summary>
		/// Sets the property for the specified target.
		/// </summary>
		/// <param name="target">Target object.</param>
		/// <param name="value">Value to set.</param>
		public void Set(object target, object value)
		{
			if (_canWrite)
			{
				object obj = value;
				if (obj == null)
				{
					obj = nullInternal;
				}
				_emittedSet.Set(target, obj);
				return;
			}
			throw new NotSupportedException($"Property \"{_propertyName}\" on type {_targetType} doesn't have a set method.");
		}
	}
}
