using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[System.Serializable]
	public abstract class Conditional : BaseTag
	{
		[System.NonSerialized]
		private string _compareValue = string.Empty;

		[System.NonSerialized]
		private string _compareProperty = string.Empty;

		[XmlAttribute("compareProperty")]
		public string CompareProperty
		{
			get
			{
				return this._compareProperty;
			}
			set
			{
				this._compareProperty = value;
			}
		}

		[XmlAttribute("compareValue")]
		public string CompareValue
		{
			get
			{
				return this._compareValue;
			}
			set
			{
				this._compareValue = value;
			}
		}
	}
}
