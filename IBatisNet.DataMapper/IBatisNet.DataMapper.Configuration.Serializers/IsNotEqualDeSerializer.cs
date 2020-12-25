using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsNotEqualDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsNotEqualDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsNotEqual isNotEqual = new IsNotEqual(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isNotEqual.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isNotEqual.Property = NodeUtils.GetStringAttribute(attributes, "property");
			isNotEqual.CompareProperty = NodeUtils.GetStringAttribute(attributes, "compareProperty");
			isNotEqual.CompareValue = NodeUtils.GetStringAttribute(attributes, "compareValue");
			return isNotEqual;
		}
	}
}
