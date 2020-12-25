using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Collections;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public sealed class SqlTagContext
	{
		private System.Collections.Hashtable _attributes = new System.Collections.Hashtable();

		private bool _overridePrepend = false;

		private SqlTag _firstNonDynamicTagWithPrepend = null;

		private System.Collections.ArrayList _parameterMappings = new System.Collections.ArrayList();

		private System.Text.StringBuilder buffer = new System.Text.StringBuilder();

		public string BodyText
		{
			get
			{
				return this.buffer.ToString().Trim();
			}
		}

		public bool IsOverridePrepend
		{
			get
			{
				return this._overridePrepend;
			}
			set
			{
				this._overridePrepend = value;
			}
		}

		public SqlTag FirstNonDynamicTagWithPrepend
		{
			get
			{
				return this._firstNonDynamicTagWithPrepend;
			}
			set
			{
				this._firstNonDynamicTagWithPrepend = value;
			}
		}

		public SqlTagContext()
		{
			this._overridePrepend = false;
		}

		public System.Text.StringBuilder GetWriter()
		{
			return this.buffer;
		}

		public void AddAttribute(object key, object value)
		{
			this._attributes.Add(key, value);
		}

		public object GetAttribute(object key)
		{
			return this._attributes[key];
		}

		public void AddParameterMapping(ParameterProperty mapping)
		{
			this._parameterMappings.Add(mapping);
		}

		public System.Collections.IList GetParameterMappings()
		{
			return this._parameterMappings;
		}
	}
}
