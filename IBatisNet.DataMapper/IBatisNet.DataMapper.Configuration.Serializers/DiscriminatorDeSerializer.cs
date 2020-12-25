using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class DiscriminatorDeSerializer
	{
		public static Discriminator Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			Discriminator discriminator = new Discriminator();
			NameValueCollection nameValueCollection = NodeUtils.ParseAttributes(node, configScope.Properties);
			discriminator.CallBackName = NodeUtils.GetStringAttribute(nameValueCollection, "typeHandler");
			discriminator.CLRType = NodeUtils.GetStringAttribute(nameValueCollection, "type");
			discriminator.ColumnIndex = NodeUtils.GetIntAttribute(nameValueCollection, "columnIndex", -999999);
			discriminator.ColumnName = NodeUtils.GetStringAttribute(nameValueCollection, "column");
			discriminator.DbType = NodeUtils.GetStringAttribute(nameValueCollection, "dbType");
			discriminator.NullValue = nameValueCollection["nullValue"];
			return discriminator;
		}
	}
}
