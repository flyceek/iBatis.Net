using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[System.Serializable]
	public abstract class BaseTag : SqlTag
	{
		[System.NonSerialized]
		private string _property = string.Empty;

		[XmlAttribute("property")]
		public string Property
		{
			get
			{
				return this._property;
			}
			set
			{
				this._property = value;
			}
		}
	}
}
