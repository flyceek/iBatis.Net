using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isEqual", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsEqual : Conditional
	{
		public IsEqual(AccessorFactory accessorFactory)
		{
			base.Handler = new IsEqualTagHandler(accessorFactory);
		}
	}
}
