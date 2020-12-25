using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("dynamic", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class Dynamic : SqlTag
	{
		public Dynamic(AccessorFactory accessorFactory)
		{
			base.Handler = new DynamicTagHandler(accessorFactory);
		}
	}
}
