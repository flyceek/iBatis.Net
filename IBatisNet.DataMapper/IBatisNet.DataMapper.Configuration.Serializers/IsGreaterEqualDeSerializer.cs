using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsGreaterEqualDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsGreaterEqualDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsGreaterEqual isGreaterEqual = new IsGreaterEqual(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isGreaterEqual.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isGreaterEqual.Property = NodeUtils.GetStringAttribute(attributes, "property");
			isGreaterEqual.CompareProperty = NodeUtils.GetStringAttribute(attributes, "compareProperty");
			isGreaterEqual.CompareValue = NodeUtils.GetStringAttribute(attributes, "compareValue");
			return isGreaterEqual;
		}
	}
}
