using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isGreaterThan", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsGreaterThan : Conditional
	{
		public IsGreaterThan(AccessorFactory accessorFactory)
		{
			base.Handler = new IsGreaterThanTagHandler(accessorFactory);
		}
	}
}
