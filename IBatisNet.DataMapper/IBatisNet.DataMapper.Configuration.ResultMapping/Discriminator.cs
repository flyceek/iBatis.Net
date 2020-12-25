using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
	[XmlRoot("discriminator", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class Discriminator
	{
		[System.NonSerialized]
		private ResultProperty _mapping = null;

		[System.NonSerialized]
		private HybridDictionary _resultMaps = null;

		[System.NonSerialized]
		private System.Collections.ArrayList _subMaps = null;

		[System.NonSerialized]
		private string _nullValue = string.Empty;

		[System.NonSerialized]
		private string _columnName = string.Empty;

		[System.NonSerialized]
		private int _columnIndex = -999999;

		[System.NonSerialized]
		private string _dbType = string.Empty;

		[System.NonSerialized]
		private string _clrType = string.Empty;

		[System.NonSerialized]
		private string _callBackName = string.Empty;

		[XmlAttribute("typeHandler")]
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

		[XmlAttribute("type")]
		public string CLRType
		{
			get
			{
				return this._clrType;
			}
			set
			{
				this._clrType = value;
			}
		}

		[XmlAttribute("columnIndex")]
		public int ColumnIndex
		{
			get
			{
				return this._columnIndex;
			}
			set
			{
				this._columnIndex = value;
			}
		}

		[XmlAttribute("column")]
		public string ColumnName
		{
			get
			{
				return this._columnName;
			}
			set
			{
				this._columnName = value;
			}
		}

		[XmlAttribute("nullValue")]
		public string NullValue
		{
			get
			{
				return this._nullValue;
			}
			set
			{
				this._nullValue = value;
			}
		}

		[XmlIgnore]
		public ResultProperty ResultProperty
		{
			get
			{
				return this._mapping;
			}
		}

		public Discriminator()
		{
			this._resultMaps = new HybridDictionary();
			this._subMaps = new System.Collections.ArrayList();
		}

		public void SetMapping(ConfigurationScope configScope, System.Type resultClass)
		{
			configScope.ErrorContext.MoreInfo = "Initialize discriminator mapping";
			this._mapping = new ResultProperty();
			this._mapping.ColumnName = this._columnName;
			this._mapping.ColumnIndex = this._columnIndex;
			this._mapping.CLRType = this._clrType;
			this._mapping.CallBackName = this._callBackName;
			this._mapping.DbType = this._dbType;
			this._mapping.NullValue = this._nullValue;
			this._mapping.Initialize(configScope, resultClass);
		}

		public void Initialize(ConfigurationScope configScope)
		{
			int count = this._subMaps.Count;
			for (int i = 0; i < count; i++)
			{
				SubMap subMap = this._subMaps[i] as SubMap;
				this._resultMaps.Add(subMap.DiscriminatorValue, configScope.SqlMapper.GetResultMap(subMap.ResultMapName));
			}
		}

		public void Add(SubMap subMap)
		{
			this._subMaps.Add(subMap);
		}

		public IResultMap GetSubMap(string discriminatorValue)
		{
			return this._resultMaps[discriminatorValue] as ResultMap;
		}
	}
}
