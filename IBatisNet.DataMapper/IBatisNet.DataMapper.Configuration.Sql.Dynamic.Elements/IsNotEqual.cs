using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isNotEqual", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsNotEqual : Conditional
	{
		public IsNotEqual(AccessorFactory accessorFactory)
		{
			base.Handler = new IsNotEqualTagHandler(accessorFactory);
		}
	}
}
