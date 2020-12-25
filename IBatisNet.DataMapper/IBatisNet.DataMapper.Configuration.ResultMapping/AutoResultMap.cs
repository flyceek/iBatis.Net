using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.DataExchange;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
	public class AutoResultMap : IResultMap
	{
		[System.NonSerialized]
		private bool _isInitalized = false;

		[System.NonSerialized]
		private System.Type _resultClass = null;

		[System.NonSerialized]
		private IFactory _resultClassFactory = null;

		[System.NonSerialized]
		private ResultPropertyCollection _properties = new ResultPropertyCollection();

		[System.NonSerialized]
		private IDataExchange _dataExchange = null;

		[XmlIgnore]
		public StringCollection GroupByPropertyNames
		{
			get
			{
				throw new System.NotImplementedException("The property 'GroupByPropertyNames' is not implemented.");
			}
		}

		[XmlIgnore]
		public ResultPropertyCollection Properties
		{
			get
			{
				return this._properties;
			}
		}

		public ResultPropertyCollection GroupByProperties
		{
			get
			{
				throw new System.NotImplementedException("The property 'GroupByProperties' is not implemented.");
			}
		}

		[XmlIgnore]
		public ResultPropertyCollection Parameters
		{
			get
			{
				throw new System.NotImplementedException("The property 'Parameters' is not implemented.");
			}
		}

		public bool IsInitalized
		{
			get
			{
				return this._isInitalized;
			}
			set
			{
				this._isInitalized = value;
			}
		}

		public string Id
		{
			get
			{
				return this._resultClass.Name;
			}
		}

		public System.Type Class
		{
			get
			{
				return this._resultClass;
			}
		}

		public IDataExchange DataExchange
		{
			set
			{
				this._dataExchange = value;
			}
		}

		public AutoResultMap(System.Type resultClass, IFactory resultClassFactory, IDataExchange dataExchange)
		{
			this._resultClass = resultClass;
			this._resultClassFactory = resultClassFactory;
			this._dataExchange = dataExchange;
		}

		public object CreateInstanceOfResult(object[] parameters)
		{
			return this.CreateInstanceOfResultClass();
		}

		public void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue)
		{
			this._dataExchange.SetData(ref target, property, dataBaseValue);
		}

		public IResultMap ResolveSubMap(IDataReader dataReader)
		{
			return this;
		}

		public AutoResultMap Clone()
		{
			return new AutoResultMap(this._resultClass, this._resultClassFactory, this._dataExchange);
		}

		public object CreateInstanceOfResultClass()
		{
			object result;
			if (this._resultClass.IsPrimitive || this._resultClass == typeof(string))
			{
				System.TypeCode typeCode = System.Type.GetTypeCode(this._resultClass);
				result = TypeUtils.InstantiatePrimitiveType(typeCode);
			}
			else if (this._resultClass.IsValueType)
			{
				if (this._resultClass == typeof(System.DateTime))
				{
					result = default(System.DateTime);
				}
				else if (this._resultClass == typeof(decimal))
				{
					result = 0m;
				}
				else if (this._resultClass == typeof(System.Guid))
				{
					result = System.Guid.Empty;
				}
				else if (this._resultClass == typeof(System.TimeSpan))
				{
					result = new System.TimeSpan(0L);
				}
				else
				{
					if (!this._resultClass.IsGenericType || !typeof(System.Nullable<>).IsAssignableFrom(this._resultClass.GetGenericTypeDefinition()))
					{
						throw new System.NotImplementedException("Unable to instanciate value type");
					}
					result = TypeUtils.InstantiateNullableType(this._resultClass);
				}
			}
			else
			{
				result = this._resultClassFactory.CreateInstance(null);
			}
			return result;
		}
	}
}
