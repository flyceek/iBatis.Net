using IBatisNet.DataMapper.DataExchange;
using System;
using System.Collections.Specialized;
using System.Data;

namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
	public class NullResultMap : IResultMap
	{
		[System.NonSerialized]
		private StringCollection _groupByPropertyNames = new StringCollection();

		[System.NonSerialized]
		private ResultPropertyCollection _properties = new ResultPropertyCollection();

		[System.NonSerialized]
		private ResultPropertyCollection _parameters = new ResultPropertyCollection();

		[System.NonSerialized]
		private ResultPropertyCollection _groupByProperties = new ResultPropertyCollection();

		public StringCollection GroupByPropertyNames
		{
			get
			{
				return this._groupByPropertyNames;
			}
		}

		public ResultPropertyCollection GroupByProperties
		{
			get
			{
				return this._groupByProperties;
			}
		}

		public bool IsInitalized
		{
			get
			{
				throw new System.Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new System.Exception("The method or operation is not implemented.");
			}
		}

		public Discriminator Discriminator
		{
			get
			{
				throw new System.Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new System.Exception("The method or operation is not implemented.");
			}
		}

		public ResultPropertyCollection Properties
		{
			get
			{
				return this._properties;
			}
		}

		public ResultPropertyCollection Parameters
		{
			get
			{
				return this._parameters;
			}
		}

		public string Id
		{
			get
			{
				return "NullResultMap.Id";
			}
		}

		public string ExtendMap
		{
			get
			{
				throw new System.Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new System.Exception("The method or operation is not implemented.");
			}
		}

		public System.Type Class
		{
			get
			{
				throw new System.Exception("The method or operation is not implemented.");
			}
		}

		public IDataExchange DataExchange
		{
			set
			{
				throw new System.Exception("The method or operation is not implemented.");
			}
		}

		public object CreateInstanceOfResult(object[] parameters)
		{
			return null;
		}

		public void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue)
		{
			throw new System.Exception("The method or operation is not implemented.");
		}

		public IResultMap ResolveSubMap(IDataReader dataReader)
		{
			throw new System.Exception("The method or operation is not implemented.");
		}
	}
}
