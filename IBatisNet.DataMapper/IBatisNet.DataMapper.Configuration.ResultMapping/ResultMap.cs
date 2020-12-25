using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Configuration.Serializers;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
	[XmlRoot("resultMap", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class ResultMap : IResultMap
	{
		private const string XML_RESULT = "result";

		private const string XML_CONSTRUCTOR_ARGUMENT = "constructor/argument";

		private const string XML_DISCRIMNATOR = "discriminator";

		private const string XML_SUBMAP = "subMap";

		public static System.Reflection.BindingFlags ANY_VISIBILITY_INSTANCE = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

		private static IResultMap _nullResultMap = null;

		[System.NonSerialized]
		private bool _isInitalized = true;

		[System.NonSerialized]
		private string _id = string.Empty;

		[System.NonSerialized]
		private string _className = string.Empty;

		[System.NonSerialized]
		private string _extendMap = string.Empty;

		[System.NonSerialized]
		private System.Type _class = null;

		[System.NonSerialized]
		private StringCollection _groupByPropertyNames = new StringCollection();

		[System.NonSerialized]
		private ResultPropertyCollection _properties = new ResultPropertyCollection();

		[System.NonSerialized]
		private ResultPropertyCollection _groupByProperties = new ResultPropertyCollection();

		[System.NonSerialized]
		private ResultPropertyCollection _parameters = new ResultPropertyCollection();

		[System.NonSerialized]
		private Discriminator _discriminator = null;

		[System.NonSerialized]
		private string _sqlMapNameSpace = string.Empty;

		[System.NonSerialized]
		private IFactory _objectFactory = null;

		[System.NonSerialized]
		private DataExchangeFactory _dataExchangeFactory = null;

		[System.NonSerialized]
		private IDataExchange _dataExchange = null;

		[XmlIgnore]
		public StringCollection GroupByPropertyNames
		{
			get
			{
				return this._groupByPropertyNames;
			}
		}

		public bool IsInitalized
		{
			get
			{
				return true;
			}
			set
			{
				this._isInitalized = value;
			}
		}

		[XmlIgnore]
		public Discriminator Discriminator
		{
			get
			{
				return this._discriminator;
			}
			set
			{
				this._discriminator = value;
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

		[XmlIgnore]
		public ResultPropertyCollection GroupByProperties
		{
			get
			{
				return this._groupByProperties;
			}
		}

		[XmlIgnore]
		public ResultPropertyCollection Parameters
		{
			get
			{
				return this._parameters;
			}
		}

		[XmlAttribute("id")]
		public string Id
		{
			get
			{
				return this._id;
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
		public System.Type Class
		{
			get
			{
				return this._class;
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

		public ResultMap(ConfigurationScope configScope, string id, string className, string extendMap, string groupBy)
		{
			ResultMap._nullResultMap = new NullResultMap();
			this._dataExchangeFactory = configScope.DataExchangeFactory;
			this._sqlMapNameSpace = configScope.SqlMapNamespace;
			if (id == null || id.Length < 1)
			{
				throw new System.ArgumentNullException("The id attribute is mandatory in a ResultMap tag.");
			}
			this._id = configScope.ApplyNamespace(id);
			if (className == null || className.Length < 1)
			{
				throw new System.ArgumentNullException("The class attribute is mandatory in the ResultMap tag id:" + this._id);
			}
			this._className = className;
			this._extendMap = extendMap;
			if (groupBy != null && groupBy.Length > 0)
			{
				string[] array = groupBy.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					string value = array[i].Trim();
					this._groupByPropertyNames.Add(value);
				}
			}
		}

		public void Initialize(ConfigurationScope configScope)
		{
			try
			{
				this._class = configScope.SqlMapper.TypeHandlerFactory.GetType(this._className);
				this._dataExchange = this._dataExchangeFactory.GetDataExchangeForClass(this._class);
				this.GetChildNode(configScope);
				for (int i = 0; i < this._groupByProperties.Count; i++)
				{
					string text = this.GroupByPropertyNames[i];
					if (!this._properties.Contains(text))
					{
						throw new ConfigurationException(string.Format("Could not configure ResultMap named \"{0}\". Check the groupBy attribute. Cause: there's no result property named \"{1}\".", this._id, text));
					}
				}
			}
			catch (System.Exception ex)
			{
				throw new ConfigurationException(string.Format("Could not configure ResultMap named \"{0}\", Cause: {1}", this._id, ex.Message), ex);
			}
		}

		public void InitializeGroupByProperties()
		{
			for (int i = 0; i < this.GroupByPropertyNames.Count; i++)
			{
				ResultProperty value = this.Properties.FindByPropertyName(this.GroupByPropertyNames[i]);
				this.GroupByProperties.Add(value);
			}
		}

		private void GetChildNode(ConfigurationScope configScope)
		{
			XmlNodeList xmlNodeList = configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("constructor/argument"), configScope.XmlNamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				System.Type[] array = new System.Type[xmlNodeList.Count];
				string[] array2 = new string[xmlNodeList.Count];
				for (int i = 0; i < xmlNodeList.Count; i++)
				{
					ArgumentProperty argumentProperty = ArgumentPropertyDeSerializer.Deserialize(xmlNodeList[i], configScope);
					this._parameters.Add(argumentProperty);
					array2[i] = argumentProperty.ArgumentName;
				}
				System.Reflection.ConstructorInfo constructor = this.GetConstructor(this._class, array2);
				for (int i = 0; i < this._parameters.Count; i++)
				{
					ArgumentProperty argumentProperty = (ArgumentProperty)this._parameters[i];
					configScope.ErrorContext.MoreInfo = "initialize argument property : " + argumentProperty.ArgumentName;
					argumentProperty.Initialize(configScope, constructor);
					array[i] = argumentProperty.MemberType;
				}
				this._objectFactory = configScope.SqlMapper.ObjectFactory.CreateFactory(this._class, array);
			}
			else if (System.Type.GetTypeCode(this._class) == System.TypeCode.Object)
			{
				this._objectFactory = configScope.SqlMapper.ObjectFactory.CreateFactory(this._class, System.Type.EmptyTypes);
			}
			foreach (XmlNode node in configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("result"), configScope.XmlNamespaceManager))
			{
				ResultProperty resultProperty = ResultPropertyDeSerializer.Deserialize(node, configScope);
				configScope.ErrorContext.MoreInfo = "initialize result property: " + resultProperty.PropertyName;
				resultProperty.Initialize(configScope, this._class);
				this._properties.Add(resultProperty);
			}
			XmlNode xmlNode = configScope.NodeContext.SelectSingleNode(DomSqlMapBuilder.ApplyMappingNamespacePrefix("discriminator"), configScope.XmlNamespaceManager);
			if (xmlNode != null)
			{
				configScope.ErrorContext.MoreInfo = "initialize discriminator";
				this.Discriminator = DiscriminatorDeSerializer.Deserialize(xmlNode, configScope);
				this.Discriminator.SetMapping(configScope, this._class);
			}
			if (configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("subMap"), configScope.XmlNamespaceManager).Count > 0 && this.Discriminator == null)
			{
				throw new ConfigurationException("The discriminator is null, but somehow a subMap was reached.  This is a bug.");
			}
			foreach (XmlNode node in configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("subMap"), configScope.XmlNamespaceManager))
			{
				configScope.ErrorContext.MoreInfo = "initialize subMap";
				SubMap subMap = SubMapDeSerializer.Deserialize(node, configScope);
				this.Discriminator.Add(subMap);
			}
		}

		public void SetObjectFactory(ConfigurationScope configScope)
		{
			System.Type[] array = new System.Type[this._parameters.Count];
			for (int i = 0; i < this._parameters.Count; i++)
			{
				ArgumentProperty argumentProperty = (ArgumentProperty)this._parameters[i];
				array[i] = argumentProperty.MemberType;
			}
			this._objectFactory = configScope.SqlMapper.ObjectFactory.CreateFactory(this._class, array);
		}

		private System.Reflection.ConstructorInfo GetConstructor(System.Type type, string[] parametersName)
		{
			System.Reflection.ConstructorInfo[] constructors = type.GetConstructors(ResultMap.ANY_VISIBILITY_INSTANCE);
			System.Reflection.ConstructorInfo[] array = constructors;
			for (int i = 0; i < array.Length; i++)
			{
				System.Reflection.ConstructorInfo constructorInfo = array[i];
				System.Reflection.ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (parameters.Length == parametersName.Length)
				{
					bool flag = true;
					for (int j = 0; j < parameters.Length; j++)
					{
						if (!(parameters[j].Name == parametersName[j]))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return constructorInfo;
					}
				}
			}
			throw new DataMapperException("Cannot find an appropriate constructor which map parameters in class: " + type.Name);
		}

		public object CreateInstanceOfResult(object[] parameters)
		{
			System.TypeCode typeCode = System.Type.GetTypeCode(this._class);
			object result;
			if (typeCode == System.TypeCode.Object)
			{
				result = this._objectFactory.CreateInstance(parameters);
			}
			else
			{
				result = TypeUtils.InstantiatePrimitiveType(typeCode);
			}
			return result;
		}

		public void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue)
		{
			this._dataExchange.SetData(ref target, property, dataBaseValue);
		}

		public IResultMap ResolveSubMap(IDataReader dataReader)
		{
			IResultMap resultMap = this;
			if (this._discriminator != null)
			{
				ResultProperty resultProperty = this._discriminator.ResultProperty;
				object dataBaseValue = resultProperty.GetDataBaseValue(dataReader);
				if (dataBaseValue != null)
				{
					resultMap = this._discriminator.GetSubMap(dataBaseValue.ToString());
					if (resultMap == null)
					{
						resultMap = this;
					}
					else if (resultMap != this)
					{
						resultMap = resultMap.ResolveSubMap(dataReader);
					}
				}
				else
				{
					resultMap = ResultMap._nullResultMap;
				}
			}
			return resultMap;
		}
	}
}
