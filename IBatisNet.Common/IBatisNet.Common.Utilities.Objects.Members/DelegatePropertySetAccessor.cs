using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.DelegatePropertySetAccessor" /> class defines a set property accessor and
	/// provides <c>Reflection.Emit</c>-generated <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ISet" /> 
	/// via the new DynamicMethod (.NET V2).
	/// </summary>
	public sealed class DelegatePropertySetAccessor : BaseAccessor, ISetAccessor, IAccessor, ISet
	{
		private delegate void SetValue(object instance, object value);

		private SetValue _set = null;

		/// <summary>
		/// The property type
		/// </summary>
		private Type _propertyType = null;

		private bool _canWrite = false;

		/// <summary>
		/// Gets the property's name.
		/// </summary>
		/// <value></value>
		public string Name => propertyName;

		/// <summary>
		/// Gets the property's type.
		/// </summary>
		/// <value></value>
		public Type MemberType => _propertyType;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.DelegatePropertySetAccessor" /> class
		/// for set property access via DynamicMethod.
		/// </summary>
		/// <param name="targetObjectType">Type of the target object.</param>
		/// <param name="propName">Name of the property.</param>
		public DelegatePropertySetAccessor(Type targetObjectType, string propName)
		{
			targetType = targetObjectType;
			propertyName = propName;
			PropertyInfo propertyInfo = GetPropertyInfo(targetObjectType);
			if (propertyInfo == null)
			{
				throw new NotSupportedException($"Property \"{propertyName}\" does not exist for type {targetType}.");
			}
			_propertyType = propertyInfo.PropertyType;
			_canWrite = propertyInfo.CanWrite;
			nullInternal = GetNullInternal(_propertyType);
			if (!propertyInfo.CanWrite)
			{
				return;
			}
			DynamicMethod dynamicMethod = new DynamicMethod("SetImplementation", null, new Type[2]
			{
				typeof(object),
				typeof(object)
			}, GetType().Module, skipVisibility: true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			MethodInfo setMethod = propertyInfo.GetSetMethod();
			Type parameterType = setMethod.GetParameters()[0].ParameterType;
			iLGenerator.DeclareLocal(parameterType);
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Castclass, targetType);
			iLGenerator.Emit(OpCodes.Ldarg_1);
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
			iLGenerator.EmitCall(OpCodes.Callvirt, setMethod, null);
			iLGenerator.Emit(OpCodes.Ret);
			_set = (SetValue)dynamicMethod.CreateDelegate(typeof(SetValue));
		}

		/// <summary>
		/// Sets the field for the specified target.
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
				_set(target, obj);
				return;
			}
			throw new NotSupportedException($"Property \"{propertyName}\" on type {targetType} doesn't have a set method.");
		}
	}
}
