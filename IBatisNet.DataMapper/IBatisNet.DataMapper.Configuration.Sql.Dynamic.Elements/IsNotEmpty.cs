using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isNotEmpty", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsNotEmpty : BaseTag
	{
		public IsNotEmpty(AccessorFactory accessorFactory)
		{
			base.Handler = new IsNotEmptyTagHandler(accessorFactory);
		}
	}
}
