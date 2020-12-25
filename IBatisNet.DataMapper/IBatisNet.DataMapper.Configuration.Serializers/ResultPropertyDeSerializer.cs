using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class ResultPropertyDeSerializer
	{
		public static ResultProperty Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			ResultProperty resultProperty = new ResultProperty();
			NameValueCollection nameValueCollection = NodeUtils.ParseAttributes(node, configScope.Properties);
			resultProperty.CLRType = NodeUtils.GetStringAttribute(nameValueCollection, "type");
			resultProperty.CallBackName = NodeUtils.GetStringAttribute(nameValueCollection, "typeHandler");
			resultProperty.ColumnIndex = NodeUtils.GetIntAttribute(nameValueCollection, "columnIndex", -999999);
			resultProperty.ColumnName = NodeUtils.GetStringAttribute(nameValueCollection, "column");
			resultProperty.DbType = NodeUtils.GetStringAttribute(nameValueCollection, "dbType");
			resultProperty.IsLazyLoad = NodeUtils.GetBooleanAttribute(nameValueCollection, "lazyLoad", false);
			resultProperty.NestedResultMapName = NodeUtils.GetStringAttribute(nameValueCollection, "resultMapping");
			resultProperty.NullValue = nameValueCollection["nullValue"];
			resultProperty.PropertyName = NodeUtils.GetStringAttribute(nameValueCollection, "property");
			resultProperty.Select = NodeUtils.GetStringAttribute(nameValueCollection, "select");
			return resultProperty;
		}
	}
}
