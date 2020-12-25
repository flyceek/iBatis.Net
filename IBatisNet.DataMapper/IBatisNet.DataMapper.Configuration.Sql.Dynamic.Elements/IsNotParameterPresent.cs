using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isNotParameterPresent", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsNotParameterPresent : SqlTag
	{
		public IsNotParameterPresent(AccessorFactory accessorFactory)
		{
			base.Handler = new IsNotParameterPresentTagHandler(accessorFactory);
		}
	}
}
