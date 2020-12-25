using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public interface IDeSerializer
	{
		SqlTag Deserialize(XmlNode node);
	}
}
