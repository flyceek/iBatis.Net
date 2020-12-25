using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isLessThan", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsLessThan : Conditional
	{
		public IsLessThan(AccessorFactory accessorFactory)
		{
			base.Handler = new IsLessThanTagHandler(accessorFactory);
		}
	}
}
