using IBatisNet.Common;
using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Alias;
using IBatisNet.DataMapper.Configuration.Cache;
using IBatisNet.DataMapper.Configuration.Cache.Fifo;
using IBatisNet.DataMapper.Configuration.Cache.Lru;
using IBatisNet.DataMapper.Configuration.Cache.Memory;
using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Configuration.Serializers;
using IBatisNet.DataMapper.Configuration.Sql;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNet.DataMapper.Configuration.Sql.SimpleDynamic;
using IBatisNet.DataMapper.Configuration.Sql.Static;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.MappedStatements.ArgumentStrategy;
using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
using IBatisNet.DataMapper.Scope;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace IBatisNet.DataMapper.Configuration
{
	public class DomSqlMapBuilder
	{
		private const string PROPERTY_ELEMENT_KEY_ATTRIB = "key";

		private const string PROPERTY_ELEMENT_VALUE_ATTRIB = "value";

		private const string DATAMAPPER_NAMESPACE_PREFIX = "mapper";

		private const string PROVIDERS_NAMESPACE_PREFIX = "provider";

		private const string MAPPING_NAMESPACE_PREFIX = "mapping";

		private const string DATAMAPPER_XML_NAMESPACE = "http://ibatis.apache.org/dataMapper";

		private const string PROVIDER_XML_NAMESPACE = "http://ibatis.apache.org/providers";

		private const string MAPPING_XML_NAMESPACE = "http://ibatis.apache.org/mapping";

		public const string DEFAULT_FILE_CONFIG_NAME = "SqlMap.config";

		private const string DEFAULT_PROVIDER_NAME = "_DEFAULT_PROVIDER_NAME";

		public const string DOT = ".";

		private const string XML_DATAMAPPER_CONFIG_ROOT = "sqlMapConfig";

		private const string XML_CONFIG_SETTINGS = "sqlMapConfig/settings/setting";

		private const string PROVIDERS_FILE_NAME = "providers.config";

		private const string XML_CONFIG_PROVIDERS = "sqlMapConfig/providers";

		private const string XML_PROPERTIES = "properties";

		private const string XML_PROPERTY = "property";

		private const string XML_SETTINGS_ADD = "/*/add";

		private const string XML_GLOBAL_PROPERTIES = "*/add";

		private const string XML_PROVIDER = "providers/provider";

		private const string XML_DATABASE_PROVIDER = "sqlMapConfig/database/provider";

		private const string XML_DATABASE_DATASOURCE = "sqlMapConfig/database/dataSource";

		private const string XML_GLOBAL_TYPEALIAS = "sqlMapConfig/alias/typeAlias";

		private const string XML_GLOBAL_TYPEHANDLER = "sqlMapConfig/typeHandlers/typeHandler";

		private const string XML_SQLMAP = "sqlMapConfig/sqlMaps/sqlMap";

		private const string XML_MAPPING_ROOT = "sqlMap";

		private const string XML_TYPEALIAS = "sqlMap/alias/typeAlias";

		private const string XML_RESULTMAP = "sqlMap/resultMaps/resultMap";

		private const string XML_PARAMETERMAP = "sqlMap/parameterMaps/parameterMap";

		private const string SQL_STATEMENT = "sqlMap/statements/sql";

		private const string XML_STATEMENT = "sqlMap/statements/statement";

		private const string XML_SELECT = "sqlMap/statements/select";

		private const string XML_INSERT = "sqlMap/statements/insert";

		private const string XML_SELECTKEY = "selectKey";

		private const string XML_UPDATE = "sqlMap/statements/update";

		private const string XML_DELETE = "sqlMap/statements/delete";

		private const string XML_PROCEDURE = "sqlMap/statements/procedure";

		private const string XML_CACHE_MODEL = "sqlMap/cacheModels/cacheModel";

		private const string XML_FLUSH_ON_EXECUTE = "flushOnExecute";

		private const string XML_SEARCH_STATEMENT = "sqlMap/statements";

		private const string XML_SEARCH_PARAMETER = "sqlMap/parameterMaps/parameterMap[@id='";

		private const string XML_SEARCH_RESULTMAP = "sqlMap/resultMaps/resultMap[@id='";

		private const string ATR_USE_STATEMENT_NAMESPACES = "useStatementNamespaces";

		private const string ATR_CACHE_MODELS_ENABLED = "cacheModelsEnabled";

		private const string ATR_VALIDATE_SQLMAP = "validateSqlMap";

		private const string ATR_USE_REFLECTION_OPTIMIZER = "useReflectionOptimizer";

		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private ConfigurationScope _configScope = null;

		private DeSerializerFactory _deSerializerFactory = null;

		private InlineParameterMapParser _paramParser = null;

		private IObjectFactory _objectFactory = null;

		private ISetAccessorFactory _setAccessorFactory = null;

		private IGetAccessorFactory _getAccessorFactory = null;

		private ISqlMapper _sqlMapper = null;

		private bool _validateSqlMapConfig = true;

		public NameValueCollection Properties
		{
			set
			{
				this._configScope.Properties.Add(value);
			}
		}

		public ISetAccessorFactory SetAccessorFactory
		{
			set
			{
				this._setAccessorFactory = value;
			}
		}

		public IGetAccessorFactory GetAccessorFactory
		{
			set
			{
				this._getAccessorFactory = value;
			}
		}

		public IObjectFactory ObjectFactory
		{
			set
			{
				this._objectFactory = value;
			}
		}

		public ISqlMapper SqlMapper
		{
			set
			{
				this._sqlMapper = value;
			}
		}

		public bool ValidateSqlMapConfig
		{
			set
			{
				this._validateSqlMapConfig = value;
			}
		}

		public DomSqlMapBuilder()
		{
			this._configScope = new ConfigurationScope();
			this._paramParser = new InlineParameterMapParser();
			this._deSerializerFactory = new DeSerializerFactory(this._configScope);
		}

		public ISqlMapper Configure()
		{
			return this.Configure(Resources.GetConfigAsXmlDocument("SqlMap.config"));
		}

		public ISqlMapper Configure(XmlDocument document)
		{
			return this.Build(document, false);
		}

		public ISqlMapper Configure(string resource)
		{
			XmlDocument document;
			if (resource.StartsWith("file://"))
			{
				document = Resources.GetUrlAsXmlDocument(resource.Remove(0, 7));
			}
			else
			{
				document = Resources.GetResourceAsXmlDocument(resource);
			}
			return this.Build(document, false);
		}

		public ISqlMapper Configure(System.IO.Stream resource)
		{
			XmlDocument streamAsXmlDocument = Resources.GetStreamAsXmlDocument(resource);
			return this.Build(streamAsXmlDocument, false);
		}

		public ISqlMapper Configure(System.IO.FileInfo resource)
		{
			XmlDocument fileInfoAsXmlDocument = Resources.GetFileInfoAsXmlDocument(resource);
			return this.Build(fileInfoAsXmlDocument, false);
		}

		public ISqlMapper Configure(Uri resource)
		{
			XmlDocument uriAsXmlDocument = Resources.GetUriAsXmlDocument(resource);
			return this.Build(uriAsXmlDocument, false);
		}

		public ISqlMapper ConfigureAndWatch(ConfigureHandler configureDelegate)
		{
			return this.ConfigureAndWatch("SqlMap.config", configureDelegate);
		}

		public ISqlMapper ConfigureAndWatch(string resource, ConfigureHandler configureDelegate)
		{
			XmlDocument document;
			if (resource.StartsWith("file://"))
			{
				document = Resources.GetUrlAsXmlDocument(resource.Remove(0, 7));
			}
			else
			{
				document = Resources.GetResourceAsXmlDocument(resource);
			}
			ConfigWatcherHandler.ClearFilesMonitored();
			ConfigWatcherHandler.AddFileToWatch(Resources.GetFileInfo(resource));
			System.Threading.TimerCallback onWhatchedFileChange = new System.Threading.TimerCallback(DomSqlMapBuilder.OnConfigFileChange);
			StateConfig state = default(StateConfig);
			state.FileName = resource;
			state.ConfigureHandler = configureDelegate;
			ISqlMapper result = this.Build(document, true);
			new ConfigWatcherHandler(onWhatchedFileChange, state);
			return result;
		}

		public ISqlMapper ConfigureAndWatch(System.IO.FileInfo resource, ConfigureHandler configureDelegate)
		{
			XmlDocument fileInfoAsXmlDocument = Resources.GetFileInfoAsXmlDocument(resource);
			ConfigWatcherHandler.ClearFilesMonitored();
			ConfigWatcherHandler.AddFileToWatch(resource);
			System.Threading.TimerCallback onWhatchedFileChange = new System.Threading.TimerCallback(DomSqlMapBuilder.OnConfigFileChange);
			StateConfig state = default(StateConfig);
			state.FileName = resource.FullName;
			state.ConfigureHandler = configureDelegate;
			ISqlMapper result = this.Build(fileInfoAsXmlDocument, true);
			new ConfigWatcherHandler(onWhatchedFileChange, state);
			return result;
		}

		public static void OnConfigFileChange(object obj)
		{
			((StateConfig)obj).ConfigureHandler(null);
		}

		private ISqlMapper Build(XmlDocument document, DataSource dataSource, bool useConfigFileWatcher, bool isCallFromDao)
		{
			this._configScope.SqlMapConfigDocument = document;
			this._configScope.DataSource = dataSource;
			this._configScope.IsCallFromDao = isCallFromDao;
			this._configScope.UseConfigFileWatcher = useConfigFileWatcher;
			this._configScope.XmlNamespaceManager = new XmlNamespaceManager(this._configScope.SqlMapConfigDocument.NameTable);
			this._configScope.XmlNamespaceManager.AddNamespace("mapper", "http://ibatis.apache.org/dataMapper");
			this._configScope.XmlNamespaceManager.AddNamespace("provider", "http://ibatis.apache.org/providers");
			this._configScope.XmlNamespaceManager.AddNamespace("mapping", "http://ibatis.apache.org/mapping");
			ISqlMapper sqlMapper;
			try
			{
				if (this._validateSqlMapConfig)
				{
					this.ValidateSchema(document.ChildNodes[1], "SqlMapConfig.xsd");
				}
				this.Initialize();
				sqlMapper = this._configScope.SqlMapper;
			}
			catch (System.Exception inner)
			{
				throw new ConfigurationException(this._configScope.ErrorContext.ToString(), inner);
			}
			return sqlMapper;
		}

		private void ValidateSchema(XmlNode section, string schemaFileName)
		{
			XmlReader xmlReader = null;
			System.IO.Stream stream = null;
			this._configScope.ErrorContext.Activity = "Validate SqlMap config";
			try
			{
				stream = this.GetStream(schemaFileName);
				if (stream == null)
				{
					throw new ConfigurationException("Unable to locate embedded resource [IBatisNet.DataMapper." + schemaFileName + "]. If you are building from source, verfiy the file is marked as an embedded resource.");
				}
				XmlSchema schema = XmlSchema.Read(stream, new ValidationEventHandler(this.ValidationCallBack));
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
				xmlSchemaSet.Add(schema);
				xmlReaderSettings.Schemas = xmlSchemaSet;
				xmlReader = XmlReader.Create(new XmlNodeReader(section), xmlReaderSettings);
				xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(this.ValidationCallBack);
				while (xmlReader.Read())
				{
				}
				if (!this._configScope.IsXmlValid)
				{
					throw new ConfigurationException("Invalid SqlMap.config document. cause :" + this._configScope.ErrorContext.Resource);
				}
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			this._configScope.IsXmlValid = false;
			ErrorContext expr_19 = this._configScope.ErrorContext;
			expr_19.Resource = expr_19.Resource + args.Message + System.Environment.NewLine;
		}

		public ISqlMapper Build(XmlDocument document, DataSource dataSource, bool useConfigFileWatcher, NameValueCollection properties)
		{
			this._configScope.Properties.Add(properties);
			return this.Build(document, dataSource, useConfigFileWatcher, true);
		}

		public ISqlMapper Build(XmlDocument document, bool useConfigFileWatcher)
		{
			return this.Build(document, null, useConfigFileWatcher, false);
		}

		private void Reset()
		{
		}

		private void Initialize()
		{
			this.Reset();
			if (!this._configScope.IsCallFromDao)
			{
				this._configScope.NodeContext = this._configScope.SqlMapConfigDocument.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("sqlMapConfig"), this._configScope.XmlNamespaceManager);
				this.ParseGlobalProperties();
			}
			this._configScope.ErrorContext.Activity = "loading global settings";
			XmlNodeList xmlNodeList = this._configScope.SqlMapConfigDocument.SelectNodes(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/settings/setting"), this._configScope.XmlNamespaceManager);
			System.Collections.IEnumerator enumerator;
			if (xmlNodeList != null)
			{
				enumerator = xmlNodeList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						XmlNode xmlNode = (XmlNode)enumerator.Current;
						if (xmlNode.Attributes["useStatementNamespaces"] != null)
						{
							string value = NodeUtils.ParsePropertyTokens(xmlNode.Attributes["useStatementNamespaces"].Value, this._configScope.Properties);
							this._configScope.UseStatementNamespaces = System.Convert.ToBoolean(value);
						}
						if (xmlNode.Attributes["cacheModelsEnabled"] != null)
						{
							string value = NodeUtils.ParsePropertyTokens(xmlNode.Attributes["cacheModelsEnabled"].Value, this._configScope.Properties);
							this._configScope.IsCacheModelsEnabled = System.Convert.ToBoolean(value);
						}
						if (xmlNode.Attributes["useReflectionOptimizer"] != null)
						{
							string value = NodeUtils.ParsePropertyTokens(xmlNode.Attributes["useReflectionOptimizer"].Value, this._configScope.Properties);
							this._configScope.UseReflectionOptimizer = System.Convert.ToBoolean(value);
						}
						if (xmlNode.Attributes["validateSqlMap"] != null)
						{
							string value = NodeUtils.ParsePropertyTokens(xmlNode.Attributes["validateSqlMap"].Value, this._configScope.Properties);
							this._configScope.ValidateSqlMap = System.Convert.ToBoolean(value);
						}
					}
				}
				finally
				{
					System.IDisposable disposable = enumerator as System.IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (this._objectFactory == null)
			{
				this._objectFactory = new ObjectFactory(this._configScope.UseReflectionOptimizer);
			}
			if (this._setAccessorFactory == null)
			{
				this._setAccessorFactory = new SetAccessorFactory(this._configScope.UseReflectionOptimizer);
			}
			if (this._getAccessorFactory == null)
			{
				this._getAccessorFactory = new GetAccessorFactory(this._configScope.UseReflectionOptimizer);
			}
			if (this._sqlMapper == null)
			{
				AccessorFactory accessorFactory = new AccessorFactory(this._setAccessorFactory, this._getAccessorFactory);
				this._configScope.SqlMapper = new SqlMapper(this._objectFactory, accessorFactory);
			}
			else
			{
				this._configScope.SqlMapper = this._sqlMapper;
			}
			ParameterMap parameterMap = new ParameterMap(this._configScope.DataExchangeFactory);
			parameterMap.Id = "iBATIS.Empty.ParameterMap";
			this._configScope.SqlMapper.AddParameterMap(parameterMap);
			this._configScope.SqlMapper.IsCacheModelsEnabled = this._configScope.IsCacheModelsEnabled;
			TypeAlias typeAlias = new TypeAlias(typeof(MemoryCacheControler));
			typeAlias.Name = "MEMORY";
			this._configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
			typeAlias = new TypeAlias(typeof(LruCacheController));
			typeAlias.Name = "LRU";
			this._configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
			typeAlias = new TypeAlias(typeof(FifoCacheController));
			typeAlias.Name = "FIFO";
			this._configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
			typeAlias = new TypeAlias(typeof(AnsiStringTypeHandler));
			typeAlias.Name = "AnsiStringTypeHandler";
			this._configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
			if (!this._configScope.IsCallFromDao)
			{
				this.GetProviders();
			}
			IDbProvider dbProvider = null;
			if (!this._configScope.IsCallFromDao)
			{
				dbProvider = this.ParseProvider();
				this._configScope.ErrorContext.Reset();
			}
			this._configScope.ErrorContext.Activity = "loading Database DataSource";
			XmlNode xmlNode2 = this._configScope.SqlMapConfigDocument.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/database/dataSource"), this._configScope.XmlNamespaceManager);
			if (xmlNode2 == null)
			{
				if (!this._configScope.IsCallFromDao)
				{
					throw new ConfigurationException("There's no dataSource tag in SqlMap.config.");
				}
				this._configScope.SqlMapper.DataSource = this._configScope.DataSource;
			}
			else
			{
				if (!this._configScope.IsCallFromDao)
				{
					this._configScope.ErrorContext.Resource = xmlNode2.OuterXml.ToString();
					this._configScope.ErrorContext.MoreInfo = "parse DataSource";
					DataSource dataSource = DataSourceDeSerializer.Deserialize(xmlNode2);
					dataSource.DbProvider = dbProvider;
					dataSource.ConnectionString = NodeUtils.ParsePropertyTokens(dataSource.ConnectionString, this._configScope.Properties);
					this._configScope.DataSource = dataSource;
					this._configScope.SqlMapper.DataSource = this._configScope.DataSource;
				}
				else
				{
					this._configScope.SqlMapper.DataSource = this._configScope.DataSource;
				}
				this._configScope.ErrorContext.Reset();
			}
			enumerator = this._configScope.SqlMapConfigDocument.SelectNodes(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/alias/typeAlias"), this._configScope.XmlNamespaceManager).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlNode xmlNode3 = (XmlNode)enumerator.Current;
					this._configScope.ErrorContext.Activity = "loading global Type alias";
					TypeAliasDeSerializer.Deserialize(xmlNode3, this._configScope);
				}
			}
			finally
			{
				System.IDisposable disposable = enumerator as System.IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			this._configScope.ErrorContext.Reset();
			enumerator = this._configScope.SqlMapConfigDocument.SelectNodes(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/typeHandlers/typeHandler"), this._configScope.XmlNamespaceManager).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlNode xmlNode3 = (XmlNode)enumerator.Current;
					try
					{
						this._configScope.ErrorContext.Activity = "loading typeHandler";
						TypeHandlerDeSerializer.Deserialize(xmlNode3, this._configScope);
					}
					catch (System.Exception ex)
					{
						NameValueCollection attributes = NodeUtils.ParseAttributes(xmlNode3, this._configScope.Properties);
						throw new ConfigurationException(string.Format("Error registering TypeHandler class \"{0}\" for handling .Net type \"{1}\" and dbType \"{2}\". Cause: {3}", new object[]
						{
							NodeUtils.GetStringAttribute(attributes, "callback"),
							NodeUtils.GetStringAttribute(attributes, "type"),
							NodeUtils.GetStringAttribute(attributes, "dbType"),
							ex.Message
						}), ex);
					}
				}
			}
			finally
			{
				System.IDisposable disposable = enumerator as System.IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			this._configScope.ErrorContext.Reset();
			enumerator = this._configScope.SqlMapConfigDocument.SelectNodes(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/sqlMaps/sqlMap"), this._configScope.XmlNamespaceManager).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlNode xmlNode3 = (XmlNode)enumerator.Current;
					this._configScope.NodeContext = xmlNode3;
					this.ConfigureSqlMap();
				}
			}
			finally
			{
				System.IDisposable disposable = enumerator as System.IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			System.Collections.IDictionaryEnumerator enumerator2;
			if (this._configScope.IsCacheModelsEnabled)
			{
				enumerator2 = this._configScope.SqlMapper.MappedStatements.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						System.Collections.DictionaryEntry dictionaryEntry = (System.Collections.DictionaryEntry)enumerator2.Current;
						this._configScope.ErrorContext.Activity = "Set CacheModel to statement";
						IMappedStatement mappedStatement = (IMappedStatement)dictionaryEntry.Value;
						if (mappedStatement.Statement.CacheModelName.Length > 0)
						{
							this._configScope.ErrorContext.MoreInfo = "statement : " + mappedStatement.Statement.Id;
							this._configScope.ErrorContext.Resource = "cacheModel : " + mappedStatement.Statement.CacheModelName;
							mappedStatement.Statement.CacheModel = this._configScope.SqlMapper.GetCache(mappedStatement.Statement.CacheModelName);
						}
					}
				}
				finally
				{
					System.IDisposable disposable = enumerator2 as System.IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			this._configScope.ErrorContext.Reset();
			enumerator2 = this._configScope.CacheModelFlushOnExecuteStatements.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					System.Collections.DictionaryEntry dictionaryEntry = (System.Collections.DictionaryEntry)enumerator2.Current;
					string text = (string)dictionaryEntry.Key;
					System.Collections.IList list = (System.Collections.IList)dictionaryEntry.Value;
					if (list != null && list.Count > 0)
					{
						enumerator = list.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								string text2 = (string)enumerator.Current;
								IMappedStatement mappedStatement = this._configScope.SqlMapper.MappedStatements[text2] as IMappedStatement;
								if (mappedStatement != null)
								{
									CacheModel cache = this._configScope.SqlMapper.GetCache(text);
									if (DomSqlMapBuilder._logger.IsDebugEnabled)
									{
										DomSqlMapBuilder._logger.Debug(string.Concat(new string[]
										{
											"Registering trigger statement [",
											mappedStatement.Id,
											"] to cache model [",
											cache.Id,
											"]"
										}));
									}
									cache.RegisterTriggerStatement(mappedStatement);
								}
								else if (DomSqlMapBuilder._logger.IsWarnEnabled)
								{
									DomSqlMapBuilder._logger.Warn(string.Concat(new string[]
									{
										"Unable to register trigger statement [",
										text2,
										"] to cache model [",
										text,
										"]. Statement does not exist."
									}));
								}
							}
						}
						finally
						{
							System.IDisposable disposable = enumerator as System.IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
			}
			finally
			{
				System.IDisposable disposable = enumerator2 as System.IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			enumerator2 = this._configScope.SqlMapper.ResultMaps.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					System.Collections.DictionaryEntry dictionaryEntry = (System.Collections.DictionaryEntry)enumerator2.Current;
					this._configScope.ErrorContext.Activity = "Resolve 'resultMap' attribute on Result Property";
					ResultMap resultMap = (ResultMap)dictionaryEntry.Value;
					for (int i = 0; i < resultMap.Properties.Count; i++)
					{
						ResultProperty resultProperty = resultMap.Properties[i];
						if (resultProperty.NestedResultMapName.Length > 0)
						{
							resultProperty.NestedResultMap = this._configScope.SqlMapper.GetResultMap(resultProperty.NestedResultMapName);
						}
						resultProperty.PropertyStrategy = PropertyStrategyFactory.Get(resultProperty);
					}
					for (int i = 0; i < resultMap.Parameters.Count; i++)
					{
						ResultProperty resultProperty = resultMap.Parameters[i];
						if (resultProperty.NestedResultMapName.Length > 0)
						{
							resultProperty.NestedResultMap = this._configScope.SqlMapper.GetResultMap(resultProperty.NestedResultMapName);
						}
						resultProperty.ArgumentStrategy = ArgumentStrategyFactory.Get((ArgumentProperty)resultProperty);
					}
					if (resultMap.Discriminator != null)
					{
						resultMap.Discriminator.Initialize(this._configScope);
					}
				}
			}
			finally
			{
				System.IDisposable disposable = enumerator2 as System.IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			this._configScope.ErrorContext.Reset();
		}

		private void GetProviders()
		{
			this._configScope.ErrorContext.Activity = "loading Providers";
			XmlNode xmlNode = this._configScope.SqlMapConfigDocument.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/providers"), this._configScope.XmlNamespaceManager);
			XmlDocument xmlDocument;
			if (xmlNode != null)
			{
				xmlDocument = Resources.GetAsXmlDocument(xmlNode, this._configScope.Properties);
			}
			else
			{
				xmlDocument = Resources.GetConfigAsXmlDocument("providers.config");
			}
			foreach (XmlNode xmlNode2 in xmlDocument.SelectNodes(this.ApplyProviderNamespacePrefix("providers/provider"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.Resource = xmlNode2.InnerXml.ToString();
				IDbProvider dbProvider = ProviderDeSerializer.Deserialize(xmlNode2);
				if (dbProvider.IsEnabled)
				{
					this._configScope.ErrorContext.ObjectId = dbProvider.Name;
					this._configScope.ErrorContext.MoreInfo = "initialize provider";
					dbProvider.Initialize();
					this._configScope.Providers.Add(dbProvider.Name, dbProvider);
					if (dbProvider.IsDefault)
					{
						if (this._configScope.Providers["_DEFAULT_PROVIDER_NAME"] != null)
						{
							throw new ConfigurationException(string.Format("Error while configuring the Provider named \"{0}\" There can be only one default Provider.", dbProvider.Name));
						}
						this._configScope.Providers.Add("_DEFAULT_PROVIDER_NAME", dbProvider);
					}
				}
			}
			this._configScope.ErrorContext.Reset();
		}

		private IDbProvider ParseProvider()
		{
			this._configScope.ErrorContext.Activity = "load DataBase Provider";
			XmlNode xmlNode = this._configScope.SqlMapConfigDocument.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/database/provider"), this._configScope.XmlNamespaceManager);
			IDbProvider result;
			if (xmlNode != null)
			{
				this._configScope.ErrorContext.Resource = xmlNode.OuterXml.ToString();
				string text = NodeUtils.ParsePropertyTokens(xmlNode.Attributes["name"].Value, this._configScope.Properties);
				this._configScope.ErrorContext.ObjectId = text;
				if (!this._configScope.Providers.Contains(text))
				{
					throw new ConfigurationException(string.Format("Error while configuring the Provider named \"{0}\". Cause : The provider is not in 'providers.config' or is not enabled.", text));
				}
				result = (IDbProvider)this._configScope.Providers[text];
			}
			else
			{
				if (!this._configScope.Providers.Contains("_DEFAULT_PROVIDER_NAME"))
				{
					throw new ConfigurationException(string.Format("Error while configuring the SqlMap. There is no provider marked default in 'providers.config' file.", new object[0]));
				}
				result = (IDbProvider)this._configScope.Providers["_DEFAULT_PROVIDER_NAME"];
			}
			return result;
		}

		private void ConfigureSqlMap()
		{
			XmlNode nodeContext = this._configScope.NodeContext;
			this._configScope.ErrorContext.Activity = "loading SqlMap";
			this._configScope.ErrorContext.Resource = nodeContext.OuterXml.ToString();
			if (this._configScope.UseConfigFileWatcher)
			{
				if (nodeContext.Attributes["resource"] != null || nodeContext.Attributes["url"] != null)
				{
					ConfigWatcherHandler.AddFileToWatch(Resources.GetFileInfo(Resources.GetValueOfNodeResourceUrl(nodeContext, this._configScope.Properties)));
				}
			}
			this._configScope.SqlMapDocument = Resources.GetAsXmlDocument(nodeContext, this._configScope.Properties);
			if (this._configScope.ValidateSqlMap)
			{
				this.ValidateSchema(this._configScope.SqlMapDocument.ChildNodes[1], "SqlMap.xsd");
			}
			this._configScope.SqlMapNamespace = this._configScope.SqlMapDocument.SelectSingleNode(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap"), this._configScope.XmlNamespaceManager).Attributes["namespace"].Value;
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/alias/typeAlias"), this._configScope.XmlNamespaceManager))
			{
				TypeAliasDeSerializer.Deserialize(xmlNode, this._configScope);
			}
			this._configScope.ErrorContext.MoreInfo = string.Empty;
			this._configScope.ErrorContext.ObjectId = string.Empty;
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/resultMaps/resultMap"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.MoreInfo = "loading ResultMap tag";
				this._configScope.NodeContext = xmlNode;
				this.BuildResultMap();
			}
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/parameterMaps/parameterMap"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.MoreInfo = "loading ParameterMap tag";
				this._configScope.NodeContext = xmlNode;
				this.BuildParameterMap();
			}
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/statements/sql"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.MoreInfo = "loading sql tag";
				this._configScope.NodeContext = xmlNode;
				SqlDeSerializer.Deserialize(xmlNode, this._configScope);
			}
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/statements/statement"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.MoreInfo = "loading statement tag";
				this._configScope.NodeContext = xmlNode;
				Statement statement = StatementDeSerializer.Deserialize(xmlNode, this._configScope);
				statement.CacheModelName = this._configScope.ApplyNamespace(statement.CacheModelName);
				statement.ParameterMapName = this._configScope.ApplyNamespace(statement.ParameterMapName);
				if (this._configScope.UseStatementNamespaces)
				{
					statement.Id = this._configScope.ApplyNamespace(statement.Id);
				}
				this._configScope.ErrorContext.ObjectId = statement.Id;
				statement.Initialize(this._configScope);
				this.ProcessSqlStatement(statement);
				MappedStatement mappedStatement = new MappedStatement(this._configScope.SqlMapper, statement);
				IMappedStatement mappedStatement2 = mappedStatement;
				if (statement.CacheModelName != null && statement.CacheModelName.Length > 0 && this._configScope.IsCacheModelsEnabled)
				{
					mappedStatement2 = new CachingStatement(mappedStatement);
				}
				this._configScope.SqlMapper.AddMappedStatement(mappedStatement2.Id, mappedStatement2);
			}
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/statements/select"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.MoreInfo = "loading select tag";
				this._configScope.NodeContext = xmlNode;
				Select select = SelectDeSerializer.Deserialize(xmlNode, this._configScope);
				select.CacheModelName = this._configScope.ApplyNamespace(select.CacheModelName);
				select.ParameterMapName = this._configScope.ApplyNamespace(select.ParameterMapName);
				if (this._configScope.UseStatementNamespaces)
				{
					select.Id = this._configScope.ApplyNamespace(select.Id);
				}
				this._configScope.ErrorContext.ObjectId = select.Id;
				select.Initialize(this._configScope);
				if (select.Generate != null)
				{
					this.GenerateCommandText(this._configScope, select);
				}
				else
				{
					this.ProcessSqlStatement(select);
				}
				MappedStatement mappedStatement = new SelectMappedStatement(this._configScope.SqlMapper, select);
				IMappedStatement mappedStatement2 = mappedStatement;
				if (select.CacheModelName != null && select.CacheModelName.Length > 0 && this._configScope.IsCacheModelsEnabled)
				{
					mappedStatement2 = new CachingStatement(mappedStatement);
				}
				this._configScope.SqlMapper.AddMappedStatement(mappedStatement2.Id, mappedStatement2);
			}
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/statements/insert"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.MoreInfo = "loading insert tag";
				this._configScope.NodeContext = xmlNode;
				Insert insert = InsertDeSerializer.Deserialize(xmlNode, this._configScope);
				insert.CacheModelName = this._configScope.ApplyNamespace(insert.CacheModelName);
				insert.ParameterMapName = this._configScope.ApplyNamespace(insert.ParameterMapName);
				if (this._configScope.UseStatementNamespaces)
				{
					insert.Id = this._configScope.ApplyNamespace(insert.Id);
				}
				this._configScope.ErrorContext.ObjectId = insert.Id;
				insert.Initialize(this._configScope);
				if (insert.Generate != null)
				{
					this.GenerateCommandText(this._configScope, insert);
				}
				else
				{
					this.ProcessSqlStatement(insert);
				}
				MappedStatement mappedStatement = new InsertMappedStatement(this._configScope.SqlMapper, insert);
				this._configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
				if (insert.SelectKey != null)
				{
					this._configScope.ErrorContext.MoreInfo = "loading selectKey tag";
					this._configScope.NodeContext = xmlNode.SelectSingleNode(DomSqlMapBuilder.ApplyMappingNamespacePrefix("selectKey"), this._configScope.XmlNamespaceManager);
					insert.SelectKey.Id = insert.Id;
					insert.SelectKey.Initialize(this._configScope);
					SelectKey expr_8CB = insert.SelectKey;
					expr_8CB.Id += ".SelectKey";
					this.ProcessSqlStatement(insert.SelectKey);
					mappedStatement = new MappedStatement(this._configScope.SqlMapper, insert.SelectKey);
					this._configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
				}
			}
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/statements/update"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.MoreInfo = "loading update tag";
				this._configScope.NodeContext = xmlNode;
				Update update = UpdateDeSerializer.Deserialize(xmlNode, this._configScope);
				update.CacheModelName = this._configScope.ApplyNamespace(update.CacheModelName);
				update.ParameterMapName = this._configScope.ApplyNamespace(update.ParameterMapName);
				if (this._configScope.UseStatementNamespaces)
				{
					update.Id = this._configScope.ApplyNamespace(update.Id);
				}
				this._configScope.ErrorContext.ObjectId = update.Id;
				update.Initialize(this._configScope);
				if (update.Generate != null)
				{
					this.GenerateCommandText(this._configScope, update);
				}
				else
				{
					this.ProcessSqlStatement(update);
				}
				MappedStatement mappedStatement = new UpdateMappedStatement(this._configScope.SqlMapper, update);
				this._configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
			}
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/statements/delete"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.MoreInfo = "loading delete tag";
				this._configScope.NodeContext = xmlNode;
				Delete delete = DeleteDeSerializer.Deserialize(xmlNode, this._configScope);
				delete.CacheModelName = this._configScope.ApplyNamespace(delete.CacheModelName);
				delete.ParameterMapName = this._configScope.ApplyNamespace(delete.ParameterMapName);
				if (this._configScope.UseStatementNamespaces)
				{
					delete.Id = this._configScope.ApplyNamespace(delete.Id);
				}
				this._configScope.ErrorContext.ObjectId = delete.Id;
				delete.Initialize(this._configScope);
				if (delete.Generate != null)
				{
					this.GenerateCommandText(this._configScope, delete);
				}
				else
				{
					this.ProcessSqlStatement(delete);
				}
				MappedStatement mappedStatement = new DeleteMappedStatement(this._configScope.SqlMapper, delete);
				this._configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
			}
			foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/statements/procedure"), this._configScope.XmlNamespaceManager))
			{
				this._configScope.ErrorContext.MoreInfo = "loading procedure tag";
				this._configScope.NodeContext = xmlNode;
				Procedure procedure = ProcedureDeSerializer.Deserialize(xmlNode, this._configScope);
				procedure.CacheModelName = this._configScope.ApplyNamespace(procedure.CacheModelName);
				procedure.ParameterMapName = this._configScope.ApplyNamespace(procedure.ParameterMapName);
				if (this._configScope.UseStatementNamespaces)
				{
					procedure.Id = this._configScope.ApplyNamespace(procedure.Id);
				}
				this._configScope.ErrorContext.ObjectId = procedure.Id;
				procedure.Initialize(this._configScope);
				this.ProcessSqlStatement(procedure);
				MappedStatement mappedStatement = new MappedStatement(this._configScope.SqlMapper, procedure);
				IMappedStatement mappedStatement2 = mappedStatement;
				if (procedure.CacheModelName != null && procedure.CacheModelName.Length > 0 && this._configScope.IsCacheModelsEnabled)
				{
					mappedStatement2 = new CachingStatement(mappedStatement);
				}
				this._configScope.SqlMapper.AddMappedStatement(mappedStatement2.Id, mappedStatement2);
			}
			if (this._configScope.IsCacheModelsEnabled)
			{
				foreach (XmlNode xmlNode in this._configScope.SqlMapDocument.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/cacheModels/cacheModel"), this._configScope.XmlNamespaceManager))
				{
					CacheModel cacheModel = CacheModelDeSerializer.Deserialize(xmlNode, this._configScope);
					cacheModel.Id = this._configScope.ApplyNamespace(cacheModel.Id);
					foreach (XmlNode xmlNode2 in xmlNode.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("flushOnExecute"), this._configScope.XmlNamespaceManager))
					{
						string text = xmlNode2.Attributes["statement"].Value;
						if (this._configScope.UseStatementNamespaces)
						{
							text = this._configScope.ApplyNamespace(text);
						}
						System.Collections.IList list = (System.Collections.IList)this._configScope.CacheModelFlushOnExecuteStatements[cacheModel.Id];
						if (list == null)
						{
							list = new System.Collections.ArrayList();
						}
						list.Add(text);
						this._configScope.CacheModelFlushOnExecuteStatements[cacheModel.Id] = list;
					}
					foreach (XmlNode xmlNode3 in xmlNode.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("property"), this._configScope.XmlNamespaceManager))
					{
						string value = xmlNode3.Attributes["name"].Value;
						string value2 = xmlNode3.Attributes["value"].Value;
						cacheModel.AddProperty(value, value2);
					}
					cacheModel.Initialize();
					this._configScope.SqlMapper.AddCache(cacheModel);
				}
			}
			this._configScope.ErrorContext.Reset();
		}

		private void ProcessSqlStatement(IStatement statement)
		{
			bool flag = false;
			XmlNode nodeContext = this._configScope.NodeContext;
			DynamicSql dynamicSql = new DynamicSql(this._configScope, statement);
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			if (statement.Id == "DynamicJIRA")
			{
				System.Console.Write("tt");
			}
			this._configScope.ErrorContext.MoreInfo = "process the Sql statement";
			if (statement.ExtendStatement.Length > 0)
			{
				XmlNode xmlNode = this._configScope.SqlMapDocument.SelectSingleNode(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/statements") + "/child::*[@id='" + statement.ExtendStatement + "']", this._configScope.XmlNamespaceManager);
				if (xmlNode == null)
				{
					throw new ConfigurationException(string.Concat(new string[]
					{
						"Unable to find extend statement named '",
						statement.ExtendStatement,
						"' on statement '",
						statement.Id,
						"'.'"
					}));
				}
				nodeContext.InnerXml = xmlNode.InnerXml + nodeContext.InnerXml;
			}
			this._configScope.ErrorContext.MoreInfo = "parse dynamic tags on sql statement";
			flag = this.ParseDynamicTags(nodeContext, dynamicSql, stringBuilder, flag, false, statement);
			if (flag)
			{
				statement.Sql = dynamicSql;
			}
			else
			{
				string sqlStatement = stringBuilder.ToString();
				this.ApplyInlineParemeterMap(statement, sqlStatement);
			}
		}

		private bool ParseDynamicTags(XmlNode commandTextNode, IDynamicParent dynamic, System.Text.StringBuilder sqlBuffer, bool isDynamic, bool postParseRequired, IStatement statement)
		{
			XmlNodeList childNodes = commandTextNode.ChildNodes;
			int count = childNodes.Count;
			for (int i = 0; i < count; i++)
			{
				XmlNode xmlNode = childNodes[i];
				if (xmlNode.NodeType == XmlNodeType.CDATA || xmlNode.NodeType == XmlNodeType.Text)
				{
					string text = xmlNode.InnerText.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ');
					text = NodeUtils.ParsePropertyTokens(text, this._configScope.Properties);
					SqlText sqlText;
					if (postParseRequired)
					{
						sqlText = new SqlText();
						sqlText.Text = text.ToString();
					}
					else
					{
						sqlText = this._paramParser.ParseInlineParameterMap(this._configScope, null, text);
					}
					dynamic.AddChild(sqlText);
					sqlBuffer.Append(text);
				}
				else if (xmlNode.Name == "include")
				{
					NameValueCollection attributes = NodeUtils.ParseAttributes(xmlNode, this._configScope.Properties);
					string stringAttribute = NodeUtils.GetStringAttribute(attributes, "refid");
					XmlNode xmlNode2 = (XmlNode)this._configScope.SqlIncludes[stringAttribute];
					if (xmlNode2 == null)
					{
						string key = this._configScope.ApplyNamespace(stringAttribute);
						xmlNode2 = (XmlNode)this._configScope.SqlIncludes[key];
						if (xmlNode2 == null)
						{
							throw new ConfigurationException("Could not find SQL tag to include with refid '" + stringAttribute + "'");
						}
					}
					isDynamic = this.ParseDynamicTags(xmlNode2, dynamic, sqlBuffer, isDynamic, false, statement);
				}
				else
				{
					string name = xmlNode.Name;
					IDeSerializer deSerializer = this._deSerializerFactory.GetDeSerializer(name);
					if (deSerializer != null)
					{
						isDynamic = true;
						SqlTag sqlTag = deSerializer.Deserialize(xmlNode);
						dynamic.AddChild(sqlTag);
						if (xmlNode.HasChildNodes)
						{
							isDynamic = this.ParseDynamicTags(xmlNode, sqlTag, sqlBuffer, isDynamic, sqlTag.Handler.IsPostParseRequired, statement);
						}
					}
				}
			}
			return isDynamic;
		}

		private void ApplyInlineParemeterMap(IStatement statement, string sqlStatement)
		{
			string text = sqlStatement;
			this._configScope.ErrorContext.MoreInfo = "apply inline parameterMap";
			if (statement.ParameterMap == null)
			{
				SqlText sqlText = this._paramParser.ParseInlineParameterMap(this._configScope, statement, text);
				if (sqlText.Parameters.Length > 0)
				{
					ParameterMap parameterMap = new ParameterMap(this._configScope.DataExchangeFactory);
					parameterMap.Id = statement.Id + "-InLineParameterMap";
					if (statement.ParameterClass != null)
					{
						parameterMap.Class = statement.ParameterClass;
					}
					parameterMap.Initialize(this._configScope.DataSource.DbProvider.UsePositionalParameters, this._configScope);
					if (statement.ParameterClass == null && sqlText.Parameters.Length == 1 && sqlText.Parameters[0].PropertyName == "value")
					{
						parameterMap.DataExchange = this._configScope.DataExchangeFactory.GetDataExchangeForClass(typeof(int));
					}
					statement.ParameterMap = parameterMap;
					int num = sqlText.Parameters.Length;
					for (int i = 0; i < num; i++)
					{
						parameterMap.AddParameterProperty(sqlText.Parameters[i]);
					}
				}
				text = sqlText.Text;
			}
			ISql sql = null;
			text = text.Trim();
			if (SimpleDynamicSql.IsSimpleDynamicSql(text))
			{
				sql = new SimpleDynamicSql(this._configScope, text, statement);
			}
			else if (statement is Procedure)
			{
				sql = new ProcedureSql(this._configScope, text, statement);
			}
			else if (statement is Statement)
			{
				sql = new StaticSql(this._configScope, statement);
				ISqlMapSession session = new SqlMapSession(this._configScope.SqlMapper);
				((StaticSql)sql).BuildPreparedStatement(session, text);
			}
			statement.Sql = sql;
		}

		private void ParseGlobalProperties()
		{
			XmlNode xmlNode = this._configScope.NodeContext.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("properties"), this._configScope.XmlNamespaceManager);
			this._configScope.ErrorContext.Activity = "loading global properties";
			if (xmlNode != null)
			{
				if (xmlNode.HasChildNodes)
				{
					foreach (XmlNode xmlNode2 in xmlNode.SelectNodes(this.ApplyDataMapperNamespacePrefix("property"), this._configScope.XmlNamespaceManager))
					{
						XmlAttribute xmlAttribute = xmlNode2.Attributes["key"];
						XmlAttribute xmlAttribute2 = xmlNode2.Attributes["value"];
						if (xmlAttribute != null && xmlAttribute2 != null)
						{
							this._configScope.Properties.Add(xmlAttribute.Value, xmlAttribute2.Value);
							if (DomSqlMapBuilder._logger.IsDebugEnabled)
							{
								DomSqlMapBuilder._logger.Debug(string.Format("Add property \"{0}\" value \"{1}\"", xmlAttribute.Value, xmlAttribute2.Value));
							}
						}
						else
						{
							XmlDocument asXmlDocument = Resources.GetAsXmlDocument(xmlNode2, this._configScope.Properties);
							foreach (XmlNode xmlNode3 in asXmlDocument.SelectNodes("*/add", this._configScope.XmlNamespaceManager))
							{
								this._configScope.Properties[xmlNode3.Attributes["key"].Value] = xmlNode3.Attributes["value"].Value;
								if (DomSqlMapBuilder._logger.IsDebugEnabled)
								{
									DomSqlMapBuilder._logger.Debug(string.Format("Add property \"{0}\" value \"{1}\"", xmlNode3.Attributes["key"].Value, xmlNode3.Attributes["value"].Value));
								}
							}
						}
					}
				}
				else
				{
					this._configScope.ErrorContext.Resource = xmlNode.OuterXml.ToString();
					XmlDocument asXmlDocument = Resources.GetAsXmlDocument(xmlNode, this._configScope.Properties);
					foreach (XmlNode xmlNode3 in asXmlDocument.SelectNodes("/*/add"))
					{
						this._configScope.Properties[xmlNode3.Attributes["key"].Value] = xmlNode3.Attributes["value"].Value;
						if (DomSqlMapBuilder._logger.IsDebugEnabled)
						{
							DomSqlMapBuilder._logger.Debug(string.Format("Add property \"{0}\" value \"{1}\"", xmlNode3.Attributes["key"].Value, xmlNode3.Attributes["value"].Value));
						}
					}
				}
			}
			this._configScope.ErrorContext.Reset();
		}

		private void GenerateCommandText(ConfigurationScope configScope, IStatement statement)
		{
			string sqlStatement = SqlGenerator.BuildQuery(statement);
			ISql sql = new StaticSql(configScope, statement);
			ISqlMapSession session = new SqlMapSession(configScope.SqlMapper);
			((StaticSql)sql).BuildPreparedStatement(session, sqlStatement);
			statement.Sql = sql;
		}

		private void BuildParameterMap()
		{
			XmlNode nodeContext = this._configScope.NodeContext;
			this._configScope.ErrorContext.MoreInfo = "build ParameterMap";
			string text = this._configScope.ApplyNamespace(nodeContext.Attributes.GetNamedItem("id").Value);
			this._configScope.ErrorContext.ObjectId = text;
			if (!this._configScope.SqlMapper.ParameterMaps.Contains(text))
			{
				ParameterMap parameterMap = ParameterMapDeSerializer.Deserialize(nodeContext, this._configScope);
				parameterMap.Id = this._configScope.ApplyNamespace(parameterMap.Id);
				string extendMap = parameterMap.ExtendMap;
				parameterMap.ExtendMap = this._configScope.ApplyNamespace(parameterMap.ExtendMap);
				if (parameterMap.ExtendMap.Length > 0)
				{
					ParameterMap parameterMap2;
					if (!this._configScope.SqlMapper.ParameterMaps.Contains(parameterMap.ExtendMap))
					{
						XmlNode xmlNode = this._configScope.SqlMapDocument.SelectSingleNode(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/parameterMaps/parameterMap[@id='") + extendMap + "']", this._configScope.XmlNamespaceManager);
						if (xmlNode == null)
						{
							throw new ConfigurationException(string.Concat(new string[]
							{
								"In mapping file '",
								this._configScope.SqlMapNamespace,
								"' the parameterMap '",
								parameterMap.Id,
								"' can not resolve extends attribute '",
								parameterMap.ExtendMap,
								"'"
							}));
						}
						this._configScope.ErrorContext.MoreInfo = "Build parent ParameterMap";
						this._configScope.NodeContext = xmlNode;
						this.BuildParameterMap();
						parameterMap2 = this._configScope.SqlMapper.GetParameterMap(parameterMap.ExtendMap);
					}
					else
					{
						parameterMap2 = this._configScope.SqlMapper.GetParameterMap(parameterMap.ExtendMap);
					}
					int num = 0;
					string[] propertyNameArray = parameterMap2.GetPropertyNameArray();
					for (int i = 0; i < propertyNameArray.Length; i++)
					{
						string name = propertyNameArray[i];
						ParameterProperty parameterProperty = parameterMap2.GetProperty(name).Clone();
						parameterProperty.Initialize(this._configScope, parameterMap.Class);
						parameterMap.InsertParameterProperty(num, parameterProperty);
						num++;
					}
				}
				this._configScope.SqlMapper.AddParameterMap(parameterMap);
			}
		}

		private void BuildResultMap()
		{
			XmlNode nodeContext = this._configScope.NodeContext;
			this._configScope.ErrorContext.MoreInfo = "build ResultMap";
			string text = this._configScope.ApplyNamespace(nodeContext.Attributes.GetNamedItem("id").Value);
			this._configScope.ErrorContext.ObjectId = text;
			if (!this._configScope.SqlMapper.ResultMaps.Contains(text))
			{
				ResultMap resultMap = ResultMapDeSerializer.Deserialize(nodeContext, this._configScope);
				string extendMap = resultMap.ExtendMap;
				resultMap.ExtendMap = this._configScope.ApplyNamespace(resultMap.ExtendMap);
				if (resultMap.ExtendMap != null && resultMap.ExtendMap.Length > 0)
				{
					IResultMap resultMap2;
					if (!this._configScope.SqlMapper.ResultMaps.Contains(resultMap.ExtendMap))
					{
						XmlNode xmlNode = this._configScope.SqlMapDocument.SelectSingleNode(DomSqlMapBuilder.ApplyMappingNamespacePrefix("sqlMap/resultMaps/resultMap[@id='") + extendMap + "']", this._configScope.XmlNamespaceManager);
						if (xmlNode == null)
						{
							throw new ConfigurationException(string.Concat(new string[]
							{
								"In mapping file '",
								this._configScope.SqlMapNamespace,
								"' the resultMap '",
								resultMap.Id,
								"' can not resolve extends attribute '",
								resultMap.ExtendMap,
								"'"
							}));
						}
						this._configScope.ErrorContext.MoreInfo = "Build parent ResultMap";
						this._configScope.NodeContext = xmlNode;
						this.BuildResultMap();
						resultMap2 = this._configScope.SqlMapper.GetResultMap(resultMap.ExtendMap);
					}
					else
					{
						resultMap2 = this._configScope.SqlMapper.GetResultMap(resultMap.ExtendMap);
					}
					for (int i = 0; i < resultMap2.Properties.Count; i++)
					{
						ResultProperty resultProperty = resultMap2.Properties[i].Clone();
						resultProperty.Initialize(this._configScope, resultMap.Class);
						resultMap.Properties.Add(resultProperty);
					}
					if (resultMap.GroupByPropertyNames.Count == 0)
					{
						for (int j = 0; j < resultMap2.GroupByPropertyNames.Count; j++)
						{
							resultMap.GroupByPropertyNames.Add(resultMap2.GroupByPropertyNames[j]);
						}
					}
					if (resultMap.Parameters.Count == 0)
					{
						for (int j = 0; j < resultMap2.Parameters.Count; j++)
						{
							resultMap.Parameters.Add(resultMap2.Parameters[j]);
						}
						if (resultMap.Parameters.Count > 0)
						{
							resultMap.SetObjectFactory(this._configScope);
						}
					}
					for (int j = 0; j < resultMap.GroupByPropertyNames.Count; j++)
					{
						string text2 = resultMap.GroupByPropertyNames[j];
						if (!resultMap.Properties.Contains(text2))
						{
							throw new ConfigurationException(string.Format("Could not configure ResultMap named \"{0}\". Check the groupBy attribute. Cause: there's no result property named \"{1}\".", resultMap.Id, text2));
						}
					}
				}
				resultMap.InitializeGroupByProperties();
				this._configScope.SqlMapper.AddResultMap(resultMap);
			}
		}

		public System.IO.Stream GetStream(string schemaResourceKey)
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("IBatisNet.DataMapper." + schemaResourceKey);
		}

		public string ApplyDataMapperNamespacePrefix(string elementName)
		{
			return "mapper:" + elementName.Replace("/", "/mapper:");
		}

		public string ApplyProviderNamespacePrefix(string elementName)
		{
			return "provider:" + elementName.Replace("/", "/provider:");
		}

		public static string ApplyMappingNamespacePrefix(string elementName)
		{
			return "mapping:" + elementName.Replace("/", "/mapping:");
		}
	}
}
