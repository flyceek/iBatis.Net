using IBatisNet.Common.Utilities;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Alias
{
	[XmlRoot("typeAlias", Namespace = "http://ibatis.apache.org/dataMapper")]
	[System.Serializable]
	public class TypeAlias
	{
		[System.NonSerialized]
		private string _name = string.Empty;

		[System.NonSerialized]
		private string _className = string.Empty;

		[System.NonSerialized]
		private System.Type _class = null;

		[XmlAttribute("alias")]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value == null || value.Length < 1)
				{
					throw new System.ArgumentNullException("The name attribute is mandatory in the typeAlias ");
				}
				this._name = value;
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

		[XmlAttribute("type")]
		public string ClassName
		{
			get
			{
				return this._className;
			}
			set
			{
				if (value == null || value.Length < 1)
				{
					throw new System.ArgumentNullException("The class attribute is mandatory in the typeAlias " + this._name);
				}
				this._className = value;
			}
		}

		public TypeAlias()
		{
		}

		public TypeAlias(System.Type type)
		{
			this._class = type;
		}

		public void Initialize()
		{
			this._class = TypeUtils.ResolveType(this._className);
		}
	}
}
