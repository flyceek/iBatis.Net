using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;

namespace IBatisNet.DataMapper.DataExchange
{
	public sealed class DotNetObjectDataExchange : BaseDataExchange
	{
		private System.Type _parameterClass = null;

		public DotNetObjectDataExchange(System.Type parameterClass, DataExchangeFactory dataExchangeFactory) : base(dataExchangeFactory)
		{
			this._parameterClass = parameterClass;
		}

		public override object GetData(ParameterProperty mapping, object parameterObject)
		{
			object result;
			if (mapping.IsComplexMemberName || this._parameterClass != parameterObject.GetType())
			{
				result = ObjectProbe.GetMemberValue(parameterObject, mapping.PropertyName, base.DataExchangeFactory.AccessorFactory);
			}
			else
			{
				result = mapping.GetAccessor.Get(parameterObject);
			}
			return result;
		}

		public override void SetData(ref object target, ResultProperty mapping, object dataBaseValue)
		{
			System.Type type = target.GetType();
			if (type != this._parameterClass && !type.IsSubclassOf(this._parameterClass) && !this._parameterClass.IsAssignableFrom(type))
			{
				throw new System.ArgumentException(string.Concat(new object[]
				{
					"Could not set value in class '",
					target.GetType(),
					"' for property '",
					mapping.PropertyName,
					"' of type '",
					mapping.MemberType,
					"'"
				}));
			}
			if (mapping.IsComplexMemberName)
			{
				ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue, base.DataExchangeFactory.ObjectFactory, base.DataExchangeFactory.AccessorFactory);
			}
			else
			{
				mapping.SetAccessor.Set(target, dataBaseValue);
			}
		}

		public override void SetData(ref object target, ParameterProperty mapping, object dataBaseValue)
		{
			if (mapping.IsComplexMemberName)
			{
				ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue, base.DataExchangeFactory.ObjectFactory, base.DataExchangeFactory.AccessorFactory);
			}
			else
			{
				ISetAccessorFactory setAccessorFactory = base.DataExchangeFactory.AccessorFactory.SetAccessorFactory;
				ISetAccessor setAccessor = setAccessorFactory.CreateSetAccessor(this._parameterClass, mapping.PropertyName);
				setAccessor.Set(target, dataBaseValue);
			}
		}
	}
}
