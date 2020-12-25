using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Configuration.Cache;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Configuration.Sql;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	[XmlRoot("statement", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class Statement : IStatement
	{
		[System.NonSerialized]
		private bool _allowRemapping = false;

		[System.NonSerialized]
		private string _id = string.Empty;

		[System.NonSerialized]
		private string _resultMapName = string.Empty;

		[System.NonSerialized]
		private ResultMapCollection _resultsMap = new ResultMapCollection();

		[System.NonSerialized]
		private string _parameterMapName = string.Empty;

		[System.NonSerialized]
		private ParameterMap _parameterMap = null;

		[System.NonSerialized]
		private string _resultClassName = string.Empty;

		[System.NonSerialized]
		private System.Type _resultClass = null;

		[System.NonSerialized]
		private string _parameterClassName = string.Empty;

		[System.NonSerialized]
		private System.Type _parameterClass = null;

		[System.NonSerialized]
		private string _listClassName = string.Empty;

		[System.NonSerialized]
		private System.Type _listClass = null;

		[System.NonSerialized]
		private string _cacheModelName = string.Empty;

		[System.NonSerialized]
		private CacheModel _cacheModel = null;

		[System.NonSerialized]
		private ISql _sql = null;

		[System.NonSerialized]
		private string _extendStatement = string.Empty;

		[System.NonSerialized]
		private IFactory _listClassFactory = null;

		[XmlAttribute("remapResults")]
		public bool AllowRemapping
		{
			get
			{
				return this._allowRemapping;
			}
			set
			{
				this._allowRemapping = value;
			}
		}

		[XmlAttribute("extends")]
		public virtual string ExtendStatement
		{
			get
			{
				return this._extendStatement;
			}
			set
			{
				this._extendStatement = value;
			}
		}

		[XmlAttribute("cacheModel")]
		public string CacheModelName
		{
			get
			{
				return this._cacheModelName;
			}
			set
			{
				this._cacheModelName = value;
			}
		}

		[XmlIgnore]
		public bool HasCacheModel
		{
			get
			{
				return this._cacheModelName.Length > 0;
			}
		}

		[XmlIgnore]
		public CacheModel CacheModel
		{
			get
			{
				return this._cacheModel;
			}
			set
			{
				this._cacheModel = value;
			}
		}

		[XmlAttribute("listClass")]
		public string ListClassName
		{
			get
			{
				return this._listClassName;
			}
			set
			{
				this._listClassName = value;
			}
		}

		[XmlIgnore]
		public System.Type ListClass
		{
			get
			{
				return this._listClass;
			}
		}

		[XmlAttribute("resultClass")]
		public string ResultClassName
		{
			get
			{
				return this._resultClassName;
			}
			set
			{
				this._resultClassName = value;
			}
		}

		[XmlIgnore]
		public System.Type ResultClass
		{
			get
			{
				return this._resultClass;
			}
		}

		[XmlAttribute("parameterClass")]
		public string ParameterClassName
		{
			get
			{
				return this._parameterClassName;
			}
			set
			{
				this._parameterClassName = value;
			}
		}

		[XmlIgnore]
		public System.Type ParameterClass
		{
			get
			{
				return this._parameterClass;
			}
		}

		[XmlAttribute("id")]
		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				if (value == null || value.Length < 1)
				{
					throw new DataMapperException("The id attribute is required in a statement tag.");
				}
				this._id = value;
			}
		}

		[XmlIgnore]
		public ISql Sql
		{
			get
			{
				return this._sql;
			}
			set
			{
				if (value == null)
				{
					throw new DataMapperException("The sql statement query text is required in the statement tag " + this._id);
				}
				this._sql = value;
			}
		}

		[XmlAttribute("resultMap")]
		public string ResultMapName
		{
			get
			{
				return this._resultMapName;
			}
			set
			{
				this._resultMapName = value;
			}
		}

		[XmlAttribute("parameterMap")]
		public string ParameterMapName
		{
			get
			{
				return this._parameterMapName;
			}
			set
			{
				this._parameterMapName = value;
			}
		}

		[XmlIgnore]
		public ResultMapCollection ResultsMap
		{
			get
			{
				return this._resultsMap;
			}
		}

		[XmlIgnore]
		public ParameterMap ParameterMap
		{
			get
			{
				return this._parameterMap;
			}
			set
			{
				this._parameterMap = value;
			}
		}

		[XmlIgnore]
		public virtual CommandType CommandType
		{
			get
			{
				return CommandType.Text;
			}
		}

		internal virtual void Initialize(ConfigurationScope configurationScope)
		{
			if (this._resultMapName.Length > 0)
			{
				string[] array = this._resultMapName.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					string name = configurationScope.ApplyNamespace(array[i].Trim());
					this._resultsMap.Add(configurationScope.SqlMapper.GetResultMap(name));
				}
			}
			if (this._parameterMapName.Length > 0)
			{
				this._parameterMap = configurationScope.SqlMapper.GetParameterMap(this._parameterMapName);
			}
			if (this._resultClassName.Length > 0)
			{
				string[] array2 = this._resultClassName.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array2.Length; i++)
				{
					this._resultClass = configurationScope.SqlMapper.TypeHandlerFactory.GetType(array2[i].Trim());
					IFactory resultClassFactory = null;
					if (System.Type.GetTypeCode(this._resultClass) == System.TypeCode.Object && !this._resultClass.IsValueType)
					{
						resultClassFactory = configurationScope.SqlMapper.ObjectFactory.CreateFactory(this._resultClass, System.Type.EmptyTypes);
					}
					IDataExchange dataExchangeForClass = configurationScope.DataExchangeFactory.GetDataExchangeForClass(this._resultClass);
					IResultMap value = new AutoResultMap(this._resultClass, resultClassFactory, dataExchangeForClass);
					this._resultsMap.Add(value);
				}
			}
			if (this._parameterClassName.Length > 0)
			{
				this._parameterClass = configurationScope.SqlMapper.TypeHandlerFactory.GetType(this._parameterClassName);
			}
			if (this._listClassName.Length > 0)
			{
				this._listClass = configurationScope.SqlMapper.TypeHandlerFactory.GetType(this._listClassName);
				this._listClassFactory = configurationScope.SqlMapper.ObjectFactory.CreateFactory(this._listClass, System.Type.EmptyTypes);
			}
		}

		public System.Collections.IList CreateInstanceOfListClass()
		{
			return (System.Collections.IList)this._listClassFactory.CreateInstance(null);
		}

		public System.Collections.Generic.IList<T> CreateInstanceOfGenericListClass<T>()
		{
			return (System.Collections.Generic.IList<T>)this._listClassFactory.CreateInstance(null);
		}
	}
}
