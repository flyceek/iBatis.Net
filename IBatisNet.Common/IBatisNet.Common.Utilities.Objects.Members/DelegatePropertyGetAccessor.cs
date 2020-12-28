using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.DelegatePropertyGetAccessor" /> class defines a get property accessor and
	/// provides <c>Reflection.Emit</c>-generated <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGet" /> 
	/// via the new DynamicMethod (.NET V2).
	/// </summary>
	public sealed class DelegatePropertyGetAccessor : BaseAccessor, IGetAccessor, IAccessor, IGet
	{
		private delegate object GetValue(object instance);

		private GetValue _get = null;

		/// <summary>
		/// The property type
		/// </summary>
		private Type _propertyType = null;

		private bool _canRead = false;

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
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.DelegatePropertyGetAccessor" /> class
		/// for get property access via DynamicMethod.
		/// </summary>
		/// <param name="targetObjectType">Type of the target object.</param>
		/// <param name="propertyName">Name of the property.</param>
		public DelegatePropertyGetAccessor(Type targetObjectType, string propertyName)
		{
			targetType = targetObjectType;
			base.propertyName = propertyName;
			PropertyInfo propertyInfo = GetPropertyInfo(targetObjectType);
			if (propertyInfo == null)
			{
				propertyInfo = targetType.GetProperty(propertyName);
			}
			if (propertyInfo == null)
			{
				throw new NotSupportedException($"Property \"{propertyName}\" does not exist for type {targetType}.");
			}
			_propertyType = propertyInfo.PropertyType;
			_canRead = propertyInfo.CanRead;
			nullInternal = GetNullInternal(_propertyType);
			if (propertyInfo.CanRead)
			{
				DynamicMethod dynamicMethod = new DynamicMethod("GetImplementation", typeof(object), new Type[1]
				{
					typeof(object)
				}, GetType().Module, skipVisibility: true);
				ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
				MethodInfo getMethod = propertyInfo.GetGetMethod();
				iLGenerator.DeclareLocal(typeof(object));
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Castclass, targetObjectType);
				iLGenerator.EmitCall(OpCodes.Callvirt, getMethod, null);
				if (getMethod.ReturnType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, getMethod.ReturnType);
				}
				iLGenerator.Emit(OpCodes.Stloc_0);
				iLGenerator.Emit(OpCodes.Ldloc_0);
				iLGenerator.Emit(OpCodes.Ret);
				_get = (GetValue)dynamicMethod.CreateDelegate(typeof(GetValue));
			}
		}

		/// <summary>
		/// Gets the field value from the specified target.
		/// </summary>
		/// <param name="target">Target object.</param>
		/// <returns>Property value.</returns>
		public object Get(object target)
		{
			if (_canRead)
			{
				return _get(target);
			}
			throw new NotSupportedException($"Property \"{propertyName}\" on type {targetType} doesn't have a get method.");
		}
	}
}
