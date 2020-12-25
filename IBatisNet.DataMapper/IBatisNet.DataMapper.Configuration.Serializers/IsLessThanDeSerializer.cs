using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsLessThanDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsLessThanDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsLessThan isLessThan = new IsLessThan(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isLessThan.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isLessThan.Property = NodeUtils.GetStringAttribute(attributes, "property");
			isLessThan.CompareProperty = NodeUtils.GetStringAttribute(attributes, "compareProperty");
			isLessThan.CompareValue = NodeUtils.GetStringAttribute(attributes, "compareValue");
			return isLessThan;
		}
	}
}
