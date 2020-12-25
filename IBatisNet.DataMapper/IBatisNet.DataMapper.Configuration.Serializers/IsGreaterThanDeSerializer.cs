using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsGreaterThanDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsGreaterThanDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsGreaterThan isGreaterThan = new IsGreaterThan(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isGreaterThan.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isGreaterThan.Property = NodeUtils.GetStringAttribute(attributes, "property");
			isGreaterThan.CompareProperty = NodeUtils.GetStringAttribute(attributes, "compareProperty");
			isGreaterThan.CompareValue = NodeUtils.GetStringAttribute(attributes, "compareValue");
			return isGreaterThan;
		}
	}
}
