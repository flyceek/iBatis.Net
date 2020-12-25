using IBatisNet.Common.Utilities;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Alias
{
	[XmlRoot("typeHandler", Namespace = "http://ibatis.apache.org/dataMapper")]
	[System.Serializable]
	public class TypeHandler
	{
		[System.NonSerialized]
		private string _className = string.Empty;

		[System.NonSerialized]
		private System.Type _class = null;

		[System.NonSerialized]
		private string _dbType = string.Empty;

		[System.NonSerialized]
		private string _callBackName = string.Empty;

		[XmlAttribute("type")]
		public string ClassName
		{
			get
			{
				return this._className;
			}
			set
			{
				this._className = value;
			}
		}

		[XmlIgnore]
		public System.Type Class
		{
			get
			{
				return this._class;
			}
		}

		[XmlAttribute("dbType")]
		public string DbType
		{
			get
			{
				return this._dbType;
			}
			set
			{
				this._dbType = value;
			}
		}

		[XmlAttribute("callback")]
		public string CallBackName
		{
			get
			{
				return this._callBackName;
			}
			set
			{
				this._callBackName = value;
			}
		}

		public void Initialize()
		{
			this._class = TypeUtils.ResolveType(this._className);
		}
	}
}
