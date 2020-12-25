using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsEqualDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsEqualDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsEqual isEqual = new IsEqual(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isEqual.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isEqual.Property = NodeUtils.GetStringAttribute(attributes, "property");
			isEqual.CompareProperty = NodeUtils.GetStringAttribute(attributes, "compareProperty");
			isEqual.CompareValue = NodeUtils.GetStringAttribute(attributes, "compareValue");
			return isEqual;
		}
	}
}
