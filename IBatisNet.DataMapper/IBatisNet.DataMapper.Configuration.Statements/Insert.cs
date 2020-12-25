using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	[XmlRoot("insert", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class Insert : Statement
	{
		[System.NonSerialized]
		private SelectKey _selectKey = null;

		[System.NonSerialized]
		private Generate _generate = null;

		[XmlIgnore]
		public override string ExtendStatement
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[XmlElement("selectKey", typeof(SelectKey))]
		public SelectKey SelectKey
		{
			get
			{
				return this._selectKey;
			}
			set
			{
				this._selectKey = value;
			}
		}

		[XmlElement("generate", typeof(Generate))]
		public Generate Generate
		{
			get
			{
				return this._generate;
			}
			set
			{
				this._generate = value;
			}
		}
	}
}
