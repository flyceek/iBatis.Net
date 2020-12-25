using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsLessEqualDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsLessEqualDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsLessEqual isLessEqual = new IsLessEqual(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isLessEqual.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			isLessEqual.Property = NodeUtils.GetStringAttribute(attributes, "property");
			isLessEqual.CompareProperty = NodeUtils.GetStringAttribute(attributes, "compareProperty");
			isLessEqual.CompareValue = NodeUtils.GetStringAttribute(attributes, "compareValue");
			return isLessEqual;
		}
	}
}
