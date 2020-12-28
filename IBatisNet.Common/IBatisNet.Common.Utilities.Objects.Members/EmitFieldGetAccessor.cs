using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.EmitFieldGetAccessor" /> class provides an IL-based get access   
	/// to a field of a specified target class.
	/// </summary>
	/// <remarks>Will Throw FieldAccessException on private field</remarks>
	public sealed class EmitFieldGetAccessor : BaseAccessor, IGetAccessor, IAccessor, IGet
	{
		private const BindingFlags VISIBILITY = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		/// <summary>
		/// The field name
		/// </summary>
		private string _fieldName = string.Empty;

		/// <summary>
		/// The class parent type
		/// </summary>
		private Type _fieldType = null;

		/// <summary>
		/// The IL emitted IGet
		/// </summary>
		private IGet _emittedGet = null;

		private Type _targetType = null;

		/// <summary>
		/// Gets the field's name.
		/// </summary>
		/// <value></value>
		public string Name => _fieldName;

		/// <summary>
		/// Gets the field's type.
		/// </summary>
		/// <value></value>
		public Type MemberType => _fieldType;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.EmitFieldGetAccessor" /> class.
		/// </summary>
		/// <param name="targetObjectType">Type of the target object.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="assemblyBuilder">The assembly builder.</param>
		/// <param name="moduleBuilder">The module builder.</param>
		public EmitFieldGetAccessor(Type targetObjectType, string fieldName, AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
		{
			_targetType = targetObjectType;
			_fieldName = fieldName;
			FieldInfo field = _targetType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new NotSupportedException($"Field \"{fieldName}\" does not exist for type {targetObjectType}.");
			}
			_fieldType = field.FieldType;
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
			_emittedGet = assemblyBuilder.CreateInstance("GetFor" + _targetType.FullName + _fieldName) as IGet;
			nullInternal = GetNullInternal(_fieldType);
			if (_emittedGet == null)
			{
				throw new NotSupportedException(string.Format("Unable to create a get field accessor for '{0}' field on class  '{0}'.", _fieldName, _fieldType));
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
			TypeBuilder typeBuilder = moduleBuilder.DefineType("GetFor" + _targetType.FullName + _fieldName, TypeAttributes.Public | TypeAttributes.Sealed);
			typeBuilder.AddInterfaceImplementation(typeof(IGet));
			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
			Type[] parameterTypes = new Type[1]
			{
				typeof(object)
			};
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual, typeof(object), parameterTypes);
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			FieldInfo field = _targetType.GetField(_fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Ldfld, field);
				if (_fieldType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, field.FieldType);
				}
				iLGenerator.Emit(OpCodes.Ret);
			}
			else
			{
				iLGenerator.ThrowException(typeof(MissingMethodException));
			}
			typeBuilder.CreateType();
		}

		/// <summary>
		/// Gets the value stored in the field for the specified target.
		/// </summary>
		/// <param name="target">Object to retrieve the field from.</param>
		/// <returns>The value.</returns>
		public object Get(object target)
		{
			return _emittedGet.Get(target);
		}
	}
}
