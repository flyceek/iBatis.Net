using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsNotEmptyDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsNotEmptyDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsNotEmpty isNotEmpty = new IsNotEmpty(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isNotEmpty.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isNotEmpty.Property = NodeUtils.GetStringAttribute(attributes, "property");
			return isNotEmpty;
		}
	}
}
