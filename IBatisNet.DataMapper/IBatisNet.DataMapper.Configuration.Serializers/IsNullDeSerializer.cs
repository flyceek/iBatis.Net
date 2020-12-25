using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsNullDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsNullDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsNull isNull = new IsNull(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isNull.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isNull.Property = NodeUtils.GetStringAttribute(attributes, "property");
			return isNull;
		}
	}
}
