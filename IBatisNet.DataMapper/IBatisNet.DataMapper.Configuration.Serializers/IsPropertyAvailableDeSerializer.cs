using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsPropertyAvailableDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsPropertyAvailableDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsPropertyAvailable isPropertyAvailable = new IsPropertyAvailable(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isPropertyAvailable.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isPropertyAvailable.Property = NodeUtils.GetStringAttribute(attributes, "property");
			return isPropertyAvailable;
		}
	}
}
