using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	[XmlRoot("delete", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class Delete : Statement
	{
		[System.NonSerialized]
		private Generate _generate = null;

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
