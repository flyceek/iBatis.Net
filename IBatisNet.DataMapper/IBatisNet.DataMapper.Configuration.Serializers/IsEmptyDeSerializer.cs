using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsEmptyDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsEmptyDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsEmpty isEmpty = new IsEmpty(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isEmpty.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isEmpty.Property = NodeUtils.GetStringAttribute(attributes, "property");
			return isEmpty;
		}
	}
}
