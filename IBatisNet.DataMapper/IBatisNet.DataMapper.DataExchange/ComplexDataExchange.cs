using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;

namespace IBatisNet.DataMapper.DataExchange
{
	public sealed class ComplexDataExchange : BaseDataExchange
	{
		public ComplexDataExchange(DataExchangeFactory dataExchangeFactory) : base(dataExchangeFactory)
		{
		}

		public override object GetData(ParameterProperty mapping, object parameterObject)
		{
			object result;
			if (parameterObject != null)
			{
				if (base.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(parameterObject.GetType()))
				{
					result = parameterObject;
				}
				else
				{
					result = ObjectProbe.GetMemberValue(parameterObject, mapping.PropertyName, base.DataExchangeFactory.AccessorFactory);
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		public override void SetData(ref object target, ResultProperty mapping, object dataBaseValue)
		{
			ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue, base.DataExchangeFactory.ObjectFactory, base.DataExchangeFactory.AccessorFactory);
		}

		public override void SetData(ref object target, ParameterProperty mapping, object dataBaseValue)
		{
			ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue, base.DataExchangeFactory.ObjectFactory, base.DataExchangeFactory.AccessorFactory);
		}
	}
}
