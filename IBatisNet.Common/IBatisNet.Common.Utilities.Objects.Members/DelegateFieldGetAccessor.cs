using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.DelegateFieldGetAccessor" /> class defines a field get accessor and
	/// provides <c>Reflection.Emit</c>-generated <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGet" />  
	/// via the new DynamicMethod (.NET V2).
	/// </summary>
	public sealed class DelegateFieldGetAccessor : BaseAccessor, IGetAccessor, IAccessor, IGet
	{
		private delegate object GetValue(object instance);

		private GetValue _get = null;

		/// <summary>
		/// The field name
		/// </summary>
		private string _fieldName = string.Empty;

		/// <summary>
		/// The class parent type
		/// </summary>
		private Type _fieldType = null;

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
		/// Initializes a new instance of the <see cref="T:DelegateFieldGetAccessor" /> class
		/// for field get access via DynamicMethod.
		/// </summary>
		/// <param name="targetObjectType">Type of the target object.</param>
		/// <param name="fieldName">Name of the field.</param>
		public DelegateFieldGetAccessor(Type targetObjectType, string fieldName)
		{
			_fieldName = fieldName;
			FieldInfo field = targetObjectType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new NotSupportedException($"Field \"{fieldName}\" does not exist for type {targetObjectType}.");
			}
			_fieldType = field.FieldType;
			nullInternal = GetNullInternal(_fieldType);
			DynamicMethod dynamicMethod = new DynamicMethod("GetImplementation", typeof(object), new Type[1]
			{
				typeof(object)
			}, GetType().Module, skipVisibility: false);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldfld, field);
			if (_fieldType.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Box, field.FieldType);
			}
			iLGenerator.Emit(OpCodes.Ret);
			_get = (GetValue)dynamicMethod.CreateDelegate(typeof(GetValue));
		}

		/// <summary>
		/// Gets the field value from the specified target.
		/// </summary>
		/// <param name="target">Target object.</param>
		/// <returns>Property value.</returns>
		public object Get(object target)
		{
			return _get(target);
		}
	}
}
