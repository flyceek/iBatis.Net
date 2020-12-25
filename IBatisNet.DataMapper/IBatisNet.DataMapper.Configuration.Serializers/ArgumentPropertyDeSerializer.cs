using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class ArgumentPropertyDeSerializer
	{
		public static ArgumentProperty Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			ArgumentProperty argumentProperty = new ArgumentProperty();
			NameValueCollection nameValueCollection = NodeUtils.ParseAttributes(node, configScope.Properties);
			argumentProperty.CLRType = NodeUtils.GetStringAttribute(nameValueCollection, "type");
			argumentProperty.CallBackName = NodeUtils.GetStringAttribute(nameValueCollection, "typeHandler");
			argumentProperty.ColumnIndex = NodeUtils.GetIntAttribute(nameValueCollection, "columnIndex", -999999);
			argumentProperty.ColumnName = NodeUtils.GetStringAttribute(nameValueCollection, "column");
			argumentProperty.DbType = NodeUtils.GetStringAttribute(nameValueCollection, "dbType");
			argumentProperty.NestedResultMapName = NodeUtils.GetStringAttribute(nameValueCollection, "resultMapping");
			argumentProperty.NullValue = nameValueCollection["nullValue"];
			argumentProperty.ArgumentName = NodeUtils.GetStringAttribute(nameValueCollection, "argumentName");
			argumentProperty.Select = NodeUtils.GetStringAttribute(nameValueCollection, "select");
			return argumentProperty;
		}
	}
}
