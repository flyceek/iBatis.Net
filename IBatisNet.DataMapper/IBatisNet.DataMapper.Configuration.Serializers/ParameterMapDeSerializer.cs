using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class ParameterMapDeSerializer
	{
		public static ParameterMap Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			ParameterMap parameterMap = new ParameterMap(configScope.DataExchangeFactory);
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
			configScope.ErrorContext.MoreInfo = "ParameterMap DeSerializer";
			parameterMap.ExtendMap = NodeUtils.GetStringAttribute(attributes, "extends");
			parameterMap.Id = NodeUtils.GetStringAttribute(attributes, "id");
			parameterMap.ClassName = NodeUtils.GetStringAttribute(attributes, "class");
			configScope.ErrorContext.MoreInfo = "Initialize ParameterMap";
			configScope.NodeContext = node;
			parameterMap.Initialize(configScope.DataSource.DbProvider.UsePositionalParameters, configScope);
			parameterMap.BuildProperties(configScope);
			configScope.ErrorContext.MoreInfo = string.Empty;
			return parameterMap;
		}
	}
}
