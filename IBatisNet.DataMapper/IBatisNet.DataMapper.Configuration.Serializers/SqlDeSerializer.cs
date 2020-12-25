using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class SqlDeSerializer
	{
		public static void Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
			string text = NodeUtils.GetStringAttribute(attributes, "id");
			if (configScope.UseStatementNamespaces)
			{
				text = configScope.ApplyNamespace(text);
			}
			if (configScope.SqlIncludes.Contains(text))
			{
				throw new ConfigurationException("Duplicate <sql>-include '" + text + "' found.");
			}
			configScope.SqlIncludes.Add(text, node);
		}
	}
}
