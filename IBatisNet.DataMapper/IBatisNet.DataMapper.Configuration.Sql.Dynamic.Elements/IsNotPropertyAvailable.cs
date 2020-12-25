using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isNotPropertyAvailable", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsNotPropertyAvailable : BaseTag
	{
		public IsNotPropertyAvailable(AccessorFactory accessorFactory)
		{
			base.Handler = new IsNotPropertyAvailableTagHandler(accessorFactory);
		}
	}
}
