using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isPropertyAvailable", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsPropertyAvailable : BaseTag
	{
		public IsPropertyAvailable(AccessorFactory accessorFactory)
		{
			base.Handler = new IsPropertyAvailableTagHandler(accessorFactory);
		}
	}
}
