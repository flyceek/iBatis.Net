using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Collections;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
	[System.Serializable]
	public abstract class SqlTag : ISqlChild, IDynamicParent
	{
		[System.NonSerialized]
		private string _prepend = string.Empty;

		[System.NonSerialized]
		private ISqlTagHandler _handler = null;

		[System.NonSerialized]
		private SqlTag _parent = null;

		[System.NonSerialized]
		private System.Collections.IList _children = new System.Collections.ArrayList();

		[XmlIgnore]
		public SqlTag Parent
		{
			get
			{
				return this._parent;
			}
			set
			{
				this._parent = value;
			}
		}

		[XmlAttribute("prepend")]
		public string Prepend
		{
			get
			{
				return this._prepend;
			}
			set
			{
				this._prepend = value;
			}
		}

		[XmlIgnore]
		public ISqlTagHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		public bool IsPrependAvailable
		{
			get
			{
				return this._prepend != null && this._prepend.Length > 0;
			}
		}

		public System.Collections.IEnumerator GetChildrenEnumerator()
		{
			return this._children.GetEnumerator();
		}

		public void AddChild(ISqlChild child)
		{
			if (child is SqlTag)
			{
				((SqlTag)child).Parent = this;
			}
			this._children.Add(child);
		}
	}
}
