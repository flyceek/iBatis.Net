using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isGreaterEqual", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsGreaterEqual : Conditional
	{
		public IsGreaterEqual(AccessorFactory accessorFactory)
		{
			base.Handler = new IsGreaterEqualTagHandler(accessorFactory);
		}
	}
}
