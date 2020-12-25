using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsNotNullDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsNotNullDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsNotNull isNotNull = new IsNotNull(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isNotNull.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isNotNull.Property = NodeUtils.GetStringAttribute(attributes, "property");
			return isNotNull;
		}
	}
}
