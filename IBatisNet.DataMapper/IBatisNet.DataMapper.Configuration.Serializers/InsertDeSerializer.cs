using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class InsertDeSerializer
	{
		public static Insert Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			Insert insert = new Insert();
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
			insert.CacheModelName = NodeUtils.GetStringAttribute(attributes, "cacheModel");
			insert.ExtendStatement = NodeUtils.GetStringAttribute(attributes, "extends");
			insert.Id = NodeUtils.GetStringAttribute(attributes, "id");
			insert.ParameterClassName = NodeUtils.GetStringAttribute(attributes, "parameterClass");
			insert.ParameterMapName = NodeUtils.GetStringAttribute(attributes, "parameterMap");
			insert.ResultClassName = NodeUtils.GetStringAttribute(attributes, "resultClass");
			insert.ResultMapName = NodeUtils.GetStringAttribute(attributes, "resultMap");
			insert.AllowRemapping = NodeUtils.GetBooleanAttribute(attributes, "remapResults", false);
			int count = node.ChildNodes.Count;
			for (int i = 0; i < count; i++)
			{
				if (node.ChildNodes[i].LocalName == "generate")
				{
					Generate generate = new Generate();
					NameValueCollection nameValueCollection = NodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
					generate.By = NodeUtils.GetStringAttribute(nameValueCollection, "by");
					generate.Table = NodeUtils.GetStringAttribute(nameValueCollection, "table");
					insert.Generate = generate;
				}
				else if (node.ChildNodes[i].LocalName == "selectKey")
				{
					SelectKey selectKey = new SelectKey();
					NameValueCollection nameValueCollection = NodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
					selectKey.PropertyName = NodeUtils.GetStringAttribute(nameValueCollection, "property");
					selectKey.SelectKeyType = InsertDeSerializer.ReadSelectKeyType(nameValueCollection["type"]);
					selectKey.ResultClassName = NodeUtils.GetStringAttribute(nameValueCollection, "resultClass");
					insert.SelectKey = selectKey;
				}
			}
			return insert;
		}

		private static SelectKeyType ReadSelectKeyType(string s)
		{
			if (s != null)
			{
				SelectKeyType result;
				if (!(s == "pre"))
				{
					if (!(s == "post"))
					{
						goto IL_2A;
					}
					result = SelectKeyType.post;
				}
				else
				{
					result = SelectKeyType.pre;
				}
				return result;
			}
			IL_2A:
			throw new ConfigurationException("Unknown selectKey type : '" + s + "'");
		}
	}
}
