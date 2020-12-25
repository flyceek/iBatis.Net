using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class ResultMapDeSerializer
	{
		public static ResultMap Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			NameValueCollection nameValueCollection = NodeUtils.ParseAttributes(node, configScope.Properties);
			ResultMap resultMap = new ResultMap(configScope, nameValueCollection["id"], nameValueCollection["class"], nameValueCollection["extends"], nameValueCollection["groupBy"]);
			configScope.ErrorContext.MoreInfo = "initialize ResultMap";
			resultMap.Initialize(configScope);
			return resultMap;
		}
	}
}
