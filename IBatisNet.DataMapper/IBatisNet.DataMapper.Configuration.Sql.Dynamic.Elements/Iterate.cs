using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[XmlRoot("iterate", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public sealed class Iterate : BaseTag
	{
		[System.NonSerialized]
		private string _open = string.Empty;

		[System.NonSerialized]
		private string _close = string.Empty;

		[System.NonSerialized]
		private string _conjunction = string.Empty;

		[XmlAttribute("conjunction")]
		public string Conjunction
		{
			get
			{
				return this._conjunction;
			}
			set
			{
				this._conjunction = value;
			}
		}

		[XmlAttribute("close")]
		public string Close
		{
			get
			{
				return this._close;
			}
			set
			{
				this._close = value;
			}
		}

		[XmlAttribute("open")]
		public string Open
		{
			get
			{
				return this._open;
			}
			set
			{
				this._open = value;
			}
		}

		public Iterate(AccessorFactory accessorFactory)
		{
			base.Handler = new IterateTagHandler(accessorFactory);
		}
	}
}
