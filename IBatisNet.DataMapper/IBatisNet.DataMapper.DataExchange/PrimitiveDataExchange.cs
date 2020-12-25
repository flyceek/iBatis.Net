using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;

namespace IBatisNet.DataMapper.DataExchange
{
	public sealed class PrimitiveDataExchange : BaseDataExchange
	{
		public PrimitiveDataExchange(DataExchangeFactory dataExchangeFactory) : base(dataExchangeFactory)
		{
		}

		public override object GetData(ParameterProperty mapping, object parameterObject)
		{
			object result;
			if (mapping.IsComplexMemberName)
			{
				result = ObjectProbe.GetMemberValue(parameterObject, mapping.PropertyName, base.DataExchangeFactory.AccessorFactory);
			}
			else
			{
				result = parameterObject;
			}
			return result;
		}

		public override void SetData(ref object target, ResultProperty mapping, object dataBaseValue)
		{
			target = dataBaseValue;
		}

		public override void SetData(ref object target, ParameterProperty mapping, object dataBaseValue)
		{
			target = dataBaseValue;
		}
	}
}
