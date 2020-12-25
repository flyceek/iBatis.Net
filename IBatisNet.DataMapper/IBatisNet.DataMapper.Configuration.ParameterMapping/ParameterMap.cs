using IBatisNet.Common.Logging;
using IBatisNet.DataMapper.Configuration.Serializers;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.Scope;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.ParameterMapping
{
	[XmlRoot("parameterMap", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class ParameterMap
	{
		private const string XML_PARAMATER = "parameter";

		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		[System.NonSerialized]
		private string _id = string.Empty;

		[System.NonSerialized]
		private ParameterPropertyCollection _properties = new ParameterPropertyCollection();

		[System.NonSerialized]
		private ParameterPropertyCollection _propertiesList = new ParameterPropertyCollection();

		[System.NonSerialized]
		private System.Collections.Hashtable _propertiesMap = new System.Collections.Hashtable();

		[System.NonSerialized]
		private string _extendMap = string.Empty;

		[System.NonSerialized]
		private bool _usePositionalParameters = false;

		[System.NonSerialized]
		private string _className = string.Empty;

		[System.NonSerialized]
		private System.Type _parameterClass = null;

		[System.NonSerialized]
		private DataExchangeFactory _dataExchangeFactory = null;

		[System.NonSerialized]
		private IDataExchange _dataExchange = null;

		[XmlAttribute("class")]
		public string ClassName
		{
			get
			{
				return this._className;
			}
			set
			{
				if (ParameterMap._logger.IsInfoEnabled)
				{
					if (value == null || value.Length < 1)
					{
						ParameterMap._logger.Info("The class attribute is recommended for better performance in a ParameterMap tag '" + this._id + "'.");
					}
				}
				this._className = value;
			}
		}

		[XmlIgnore]
		public System.Type Class
		{
			get
			{
				return this._parameterClass;
			}
			set
			{
				this._parameterClass = value;
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
					throw new System.ArgumentNullException("The id attribute is mandatory in a ParameterMap tag.");
				}
				this._id = value;
			}
		}

		[XmlIgnore]
		public ParameterPropertyCollection Properties
		{
			get
			{
				return this._properties;
			}
		}

		[XmlIgnore]
		public ParameterPropertyCollection PropertiesList
		{
			get
			{
				return this._propertiesList;
			}
		}

		[XmlAttribute("extends")]
		public string ExtendMap
		{
			get
			{
				return this._extendMap;
			}
			set
			{
				this._extendMap = value;
			}
		}

		[XmlIgnore]
		public IDataExchange DataExchange
		{
			set
			{
				this._dataExchange = value;
			}
		}

		public ParameterMap(DataExchangeFactory dataExchangeFactory)
		{
			this._dataExchangeFactory = dataExchangeFactory;
		}

		public ParameterProperty GetProperty(int index)
		{
			ParameterProperty result;
			if (this._usePositionalParameters)
			{
				result = this._properties[index];
			}
			else
			{
				result = this._propertiesList[index];
			}
			return result;
		}

		public ParameterProperty GetProperty(string name)
		{
			return (ParameterProperty)this._propertiesMap[name];
		}

		public void AddParameterProperty(ParameterProperty property)
		{
			this._propertiesMap[property.PropertyName] = property;
			this._properties.Add(property);
			if (!this._propertiesList.Contains(property))
			{
				this._propertiesList.Add(property);
			}
		}

		public void InsertParameterProperty(int index, ParameterProperty property)
		{
			this._propertiesMap[property.PropertyName] = property;
			this._properties.Insert(index, property);
			if (!this._propertiesList.Contains(property))
			{
				this._propertiesList.Insert(index, property);
			}
		}

		public int GetParameterIndex(string propertyName)
		{
			return System.Convert.ToInt32(propertyName.Replace("[", "").Replace("]", ""));
		}

		public string[] GetPropertyNameArray()
		{
			string[] array = new string[this._propertiesMap.Count];
			for (int i = 0; i < this._propertiesList.Count; i++)
			{
				array[i] = this._propertiesList[i].PropertyName;
			}
			return array;
		}

		public void SetParameter(ParameterProperty mapping, IDataParameter dataParameter, object parameterValue)
		{
			object obj = this._dataExchange.GetData(mapping, parameterValue);
			ITypeHandler typeHandler = mapping.TypeHandler;
			if (mapping.HasNullValue)
			{
				if (typeHandler.Equals(obj, mapping.NullValue))
				{
					obj = null;
				}
			}
			typeHandler.SetParameter(dataParameter, obj, mapping.DbType);
		}

		public void SetOutputParameter(ref object target, ParameterProperty mapping, object dataBaseValue)
		{
			this._dataExchange.SetData(ref target, mapping, dataBaseValue);
		}

		public void Initialize(bool usePositionalParameters, IScope scope)
		{
			this._usePositionalParameters = usePositionalParameters;
			if (this._className.Length > 0)
			{
				this._parameterClass = this._dataExchangeFactory.TypeHandlerFactory.GetType(this._className);
				this._dataExchange = this._dataExchangeFactory.GetDataExchangeForClass(this._parameterClass);
			}
			else
			{
				this._dataExchange = this._dataExchangeFactory.GetDataExchangeForClass(null);
			}
		}

		public void BuildProperties(ConfigurationScope configScope)
		{
			foreach (XmlNode node in configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("parameter"), configScope.XmlNamespaceManager))
			{
				ParameterProperty parameterProperty = ParameterPropertyDeSerializer.Deserialize(node, configScope);
				parameterProperty.Initialize(configScope, this._parameterClass);
				this.AddParameterProperty(parameterProperty);
			}
		}
	}
}
