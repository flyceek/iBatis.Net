using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class SubMapDeSerializer
	{
		public static SubMap Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
			string stringAttribute = NodeUtils.GetStringAttribute(attributes, "value");
			string resultMapName = configScope.ApplyNamespace(NodeUtils.GetStringAttribute(attributes, "resultMapping"));
			return new SubMap(stringAttribute, resultMapName);
		}
	}
}
