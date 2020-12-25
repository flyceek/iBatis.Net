using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class IsNotParameterPresentDeSerializer : IDeSerializer
	{
		private ConfigurationScope _configScope = null;

		public IsNotParameterPresentDeSerializer(ConfigurationScope configScope)
		{
			this._configScope = configScope;
		}

		public SqlTag Deserialize(XmlNode node)
		{
			IsNotParameterPresent isNotParameterPresent = new IsNotParameterPresent(this._configScope.DataExchangeFactory.AccessorFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
			isNotParameterPresent.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
			return isNotParameterPresent;
		}
	}
}
