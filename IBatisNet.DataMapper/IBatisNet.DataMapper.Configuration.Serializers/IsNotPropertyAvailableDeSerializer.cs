using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsNotPropertyAvailableDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsNotPropertyAvailableDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsNotPropertyAvailable isNotPropertyAvailable = new IsNotPropertyAvailable(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isNotPropertyAvailable.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isNotPropertyAvailable.Property = NodeUtils.GetStringAttribute(attributes, "property");
			return isNotPropertyAvailable;
		}
	}
}
