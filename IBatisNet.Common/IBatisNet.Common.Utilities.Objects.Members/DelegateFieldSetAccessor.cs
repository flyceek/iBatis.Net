using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.DelegateFieldSetAccessor" /> class defines a field get accessor and
	/// provides <c>Reflection.Emit</c>-generated <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ISet" /> 
	/// via the new DynamicMethod (.NET V2).
	/// </summary>
	public sealed class DelegateFieldSetAccessor : BaseAccessor, ISetAccessor, IAccessor, ISet
	{
		private delegate void SetValue(object instance, object value);

		private SetValue _set = null;

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
		/// Initializes a new instance of the <see cref="T:DelegateFieldSetAccessor" /> class
		/// for field get access via DynamicMethod.
		/// </summary>
		/// <param name="targetObjectType">Type of the target object.</param>
		/// <param name="fieldName">Name of the field.</param>
		public DelegateFieldSetAccessor(Type targetObjectType, string fieldName)
		{
			_fieldName = fieldName;
			FieldInfo field = targetObjectType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new NotSupportedException($"Field \"{fieldName}\" does not exist for type {targetObjectType}.");
			}
			_fieldType = field.FieldType;
			nullInternal = GetNullInternal(_fieldType);
			DynamicMethod dynamicMethod = new DynamicMethod("SetImplementation", null, new Type[2]
			{
				typeof(object),
				typeof(object)
			}, GetType().Module, skipVisibility: false);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldarg_1);
			UnboxIfNeeded(field.FieldType, iLGenerator);
			iLGenerator.Emit(OpCodes.Stfld, field);
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
			object obj = value;
			if (obj == null)
			{
				obj = nullInternal;
			}
			_set(target, obj);
		}

		private static void UnboxIfNeeded(Type type, ILGenerator generator)
		{
			if (type.IsValueType)
			{
				generator.Emit(OpCodes.Unbox_Any, type);
			}
		}
	}
}
