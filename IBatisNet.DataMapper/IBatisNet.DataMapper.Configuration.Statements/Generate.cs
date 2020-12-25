using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	[XmlRoot("generate", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class Generate : Statement
	{
		[System.NonSerialized]
		private string _table = string.Empty;

		[System.NonSerialized]
		private string _by = string.Empty;

		[XmlAttribute("table")]
		public string Table
		{
			get
			{
				return this._table;
			}
			set
			{
				this._table = value;
			}
		}

		[XmlAttribute("by")]
		public string By
		{
			get
			{
				return this._by;
			}
			set
			{
				this._by = value;
			}
		}
	}
}
