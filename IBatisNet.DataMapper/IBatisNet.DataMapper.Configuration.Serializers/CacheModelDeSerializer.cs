using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Cache;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class CacheModelDeSerializer
	{
		public static CacheModel Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			CacheModel cacheModel = new CacheModel();
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
			cacheModel.Id = NodeUtils.GetStringAttribute(attributes, "id");
			cacheModel.Implementation = NodeUtils.GetStringAttribute(attributes, "implementation");
			if (!string.IsNullOrEmpty(cacheModel.Implementation))
			{
				cacheModel.Implementation = configScope.SqlMapper.TypeHandlerFactory.GetTypeAlias(cacheModel.Implementation).Class.AssemblyQualifiedName;
			}
			cacheModel.IsReadOnly = NodeUtils.GetBooleanAttribute(attributes, "readOnly", true);
			cacheModel.IsSerializable = NodeUtils.GetBooleanAttribute(attributes, "serialize", false);
			cacheModel.ThirdPatyType = NodeUtils.GetStringAttribute(attributes, "thirdpatytype");
			int count = node.ChildNodes.Count;
			for (int i = 0; i < count; i++)
			{
				if (node.ChildNodes[i].LocalName == "flushInterval")
				{
					FlushInterval flushInterval = new FlushInterval();
					NameValueCollection attributes2 = NodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
					flushInterval.Hours = NodeUtils.GetIntAttribute(attributes2, "hours", 0);
					flushInterval.Milliseconds = NodeUtils.GetIntAttribute(attributes2, "milliseconds", 0);
					flushInterval.Minutes = NodeUtils.GetIntAttribute(attributes2, "minutes", 0);
					flushInterval.Seconds = NodeUtils.GetIntAttribute(attributes2, "seconds", 0);
					cacheModel.FlushInterval = flushInterval;
				}
			}
			return cacheModel;
		}
	}
}
