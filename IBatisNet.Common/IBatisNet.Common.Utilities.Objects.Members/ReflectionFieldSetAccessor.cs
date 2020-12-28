using System;
using System.Reflection;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ReflectionFieldSetAccessor" /> class provides an reflection set access   
	/// to a field of a specified target class.
	/// </summary>
	public sealed class ReflectionFieldSetAccessor : ISetAccessor, IAccessor, ISet
	{
		private FieldInfo _fieldInfo = null;

		/// <summary>
		/// Gets the member name.
		/// </summary>
		public string Name => _fieldInfo.Name;

		/// <summary>
		/// Gets the type of this member, such as field, property.
		/// </summary>
		public Type MemberType => _fieldInfo.FieldType;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ReflectionFieldSetAccessor" /> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="fieldName">Name of the field.</param>
		public ReflectionFieldSetAccessor(Type targetType, string fieldName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			_fieldInfo = (FieldInfo)instance.GetGetter(fieldName);
		}

		/// <summary>
		/// Sets the value for the field of the specified target.
		/// </summary>
		/// <param name="target">Object to set the property on.</param>
		/// <param name="value">Property value.</param>
		public void Set(object target, object value)
		{
			_fieldInfo.SetValue(target, value);
		}
	}
}
