using System;
using System.Reflection;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ReflectionPropertySetAccessor" /> class provides an reflection set access   
	/// to a property of a specified target class.
	/// </summary>
	public sealed class ReflectionPropertySetAccessor : ISetAccessor, IAccessor, ISet
	{
		private PropertyInfo _propertyInfo = null;

		private string _propertyName = string.Empty;

		private Type _targetType = null;

		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string Name => _propertyInfo.Name;

		/// <summary>
		/// Gets the type of this property.
		/// </summary>
		public Type MemberType => _propertyInfo.PropertyType;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ReflectionPropertySetAccessor" /> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="propertyName">Name of the property.</param>
		public ReflectionPropertySetAccessor(Type targetType, string propertyName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			_propertyInfo = (PropertyInfo)instance.GetSetter(propertyName);
			_targetType = targetType;
			_propertyName = propertyName;
		}

		/// <summary>
		/// Sets the value for the property of the specified target.
		/// </summary>
		/// <param name="target">Object to set the property on.</param>
		/// <param name="value">Property value.</param>
		public void Set(object target, object value)
		{
			if (_propertyInfo.CanWrite)
			{
				_propertyInfo.SetValue(target, value, null);
				return;
			}
			throw new NotSupportedException($"Property \"{_propertyName}\" on type {_targetType} doesn't have a set method.");
		}
	}
}
