using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isNull", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsNull : BaseTag
	{
		public IsNull(AccessorFactory accessorFactory)
		{
			base.Handler = new IsNullTagHandler(accessorFactory);
		}
	}
}
