using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isEmpty", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsEmpty : BaseTag
	{
		public IsEmpty(AccessorFactory accessorFactory)
		{
			base.Handler = new IsEmptyTagHandler(accessorFactory);
		}
	}
}
