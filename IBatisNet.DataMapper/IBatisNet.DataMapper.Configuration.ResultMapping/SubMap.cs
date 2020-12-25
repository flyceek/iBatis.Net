using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
	[XmlRoot("subMap", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class SubMap
	{
		[System.NonSerialized]
		private string _discriminatorValue = string.Empty;

		[System.NonSerialized]
		private string _resultMapName = string.Empty;

		[System.NonSerialized]
		private IResultMap _resultMap = null;

		[XmlAttribute("value")]
		public string DiscriminatorValue
		{
			get
			{
				return this._discriminatorValue;
			}
		}

		[XmlAttribute("resultMapping")]
		public string ResultMapName
		{
			get
			{
				return this._resultMapName;
			}
		}

		[XmlIgnore]
		public IResultMap ResultMap
		{
			get
			{
				return this._resultMap;
			}
			set
			{
				this._resultMap = value;
			}
		}

		public SubMap(string discriminatorValue, string resultMapName)
		{
			this._discriminatorValue = discriminatorValue;
			this._resultMapName = resultMapName;
		}
	}
}
