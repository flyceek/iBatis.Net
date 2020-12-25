using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class ParameterPropertyDeSerializer
	{
		public static ParameterProperty Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			ParameterProperty parameterProperty = new ParameterProperty();
			NameValueCollection nameValueCollection = NodeUtils.ParseAttributes(node, configScope.Properties);
			configScope.ErrorContext.MoreInfo = "ParameterPropertyDeSerializer";
			parameterProperty.CallBackName = NodeUtils.GetStringAttribute(nameValueCollection, "typeHandler");
			parameterProperty.CLRType = NodeUtils.GetStringAttribute(nameValueCollection, "type");
			parameterProperty.ColumnName = NodeUtils.GetStringAttribute(nameValueCollection, "column");
			parameterProperty.DbType = NodeUtils.GetStringAttribute(nameValueCollection, "dbType", null);
			parameterProperty.DirectionAttribute = NodeUtils.GetStringAttribute(nameValueCollection, "direction");
			parameterProperty.NullValue = nameValueCollection["nullValue"];
			parameterProperty.PropertyName = NodeUtils.GetStringAttribute(nameValueCollection, "property");
			parameterProperty.Precision = NodeUtils.GetByteAttribute(nameValueCollection, "precision", 0);
			parameterProperty.Scale = NodeUtils.GetByteAttribute(nameValueCollection, "scale", 0);
			parameterProperty.Size = NodeUtils.GetIntAttribute(nameValueCollection, "size", -1);
			return parameterProperty;
		}
	}
}
