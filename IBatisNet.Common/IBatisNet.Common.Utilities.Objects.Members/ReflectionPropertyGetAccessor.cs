using System;
using System.Reflection;

namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// The <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ReflectionPropertyGetAccessor" /> class provides an reflection get access   
	/// to a property of a specified target class.
	/// </summary>
	public sealed class ReflectionPropertyGetAccessor : IGetAccessor, IAccessor, IGet
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
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ReflectionPropertyGetAccessor" /> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="propertyName">Name of the property.</param>
		public ReflectionPropertyGetAccessor(Type targetType, string propertyName)
		{
			ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
			_propertyInfo = (PropertyInfo)instance.GetGetter(propertyName);
			_targetType = targetType;
			_propertyName = propertyName;
		}

		/// <summary>
		/// Gets the value stored in the property for 
		/// the specified target.
		/// </summary>
		/// <param name="target">Object to retrieve the property from.</param>
		/// <returns>Property value.</returns>
		public object Get(object target)
		{
			if (_propertyInfo.CanRead)
			{
				return _propertyInfo.GetValue(target, null);
			}
			throw new NotSupportedException($"Property \"{_propertyName}\" on type {_targetType} doesn't have a get method.");
		}
	}
}
