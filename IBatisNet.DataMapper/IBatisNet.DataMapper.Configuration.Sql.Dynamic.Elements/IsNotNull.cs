using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isNotNull", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsNotNull : BaseTag
	{
		public IsNotNull(AccessorFactory accessorFactory)
		{
			base.Handler = new IsNotNullTagHandler(accessorFactory);
		}
	}
}
