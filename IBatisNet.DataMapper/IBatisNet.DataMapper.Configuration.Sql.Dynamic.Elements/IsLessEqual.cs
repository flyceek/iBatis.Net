using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("isLessEqual", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class IsLessEqual : Conditional
	{
		public IsLessEqual(AccessorFactory accessorFactory)
		{
			base.Handler = new IsLessEqualTagHandler(accessorFactory);
		}
	}
}
