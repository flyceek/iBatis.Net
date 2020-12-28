using System;
using System.Reflection;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ReflectionFieldGetAccessor" /> class provides an reflection get access   
	/// to a field of a specified target class.
	/// </summary>
	public sealed class ReflectionFieldGetAccessor : IGetAccessor, IAccessor, IGet
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
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ReflectionFieldGetAccessor" /> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="fieldName">Name of the field.</param>
		public ReflectionFieldGetAccessor(Type targetType, string fieldName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			_fieldInfo = (FieldInfo)instance.GetGetter(fieldName);
		}

		/// <summary>
		/// Gets the value stored in the field for the specified target.       
		/// </summary>
		/// <param name="target">Object to retrieve the field/property from.</param>
		/// <returns>The field alue.</returns>
		public object Get(object target)
		{
			return _fieldInfo.GetValue(target);
		}
	}
}
