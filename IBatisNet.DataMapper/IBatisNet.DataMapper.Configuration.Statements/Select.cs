using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	[XmlRoot("select", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class Select : Statement
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
