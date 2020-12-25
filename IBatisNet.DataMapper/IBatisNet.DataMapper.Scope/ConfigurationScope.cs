using IBatisNet.Common;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace IBatisNet.DataMapper.Scope
{
	public class ConfigurationScope : IScope
	{
		public const string EMPTY_PARAMETER_MAP = "iBATIS.Empty.ParameterMap";

		private ErrorContext _errorContext = null;

		private HybridDictionary _providers = new HybridDictionary();

		private HybridDictionary _sqlIncludes = new HybridDictionary();

		private NameValueCollection _properties = new NameValueCollection();

		private XmlDocument _sqlMapConfigDocument = null;

		private XmlDocument _sqlMapDocument = null;

		private XmlNode _nodeContext = null;

		private bool _useConfigFileWatcher = false;

		private bool _useStatementNamespaces = false;

		private bool _isCacheModelsEnabled = false;

		private bool _useReflectionOptimizer = true;

		private bool _validateSqlMap = false;

		private bool _isCallFromDao = false;

		private ISqlMapper _sqlMapper = null;

		private string _sqlMapNamespace = null;

		private DataSource _dataSource = null;

		private bool _isXmlValid = true;

		private XmlNamespaceManager _nsmgr = null;

		private HybridDictionary _cacheModelFlushOnExecuteStatements = new HybridDictionary();

		public HybridDictionary SqlIncludes
		{
			get
			{
				return this._sqlIncludes;
			}
		}

		public XmlNamespaceManager XmlNamespaceManager
		{
			get
			{
				return this._nsmgr;
			}
			set
			{
				this._nsmgr = value;
			}
		}

		public bool ValidateSqlMap
		{
			get
			{
				return this._validateSqlMap;
			}
			set
			{
				this._validateSqlMap = value;
			}
		}

		public bool IsXmlValid
		{
			get
			{
				return this._isXmlValid;
			}
			set
			{
				this._isXmlValid = value;
			}
		}

		public string SqlMapNamespace
		{
			get
			{
				return this._sqlMapNamespace;
			}
			set
			{
				this._sqlMapNamespace = value;
			}
		}

		public ISqlMapper SqlMapper
		{
			get
			{
				return this._sqlMapper;
			}
			set
			{
				this._sqlMapper = value;
			}
		}

		public DataExchangeFactory DataExchangeFactory
		{
			get
			{
				return this._sqlMapper.DataExchangeFactory;
			}
		}

		public bool IsCallFromDao
		{
			get
			{
				return this._isCallFromDao;
			}
			set
			{
				this._isCallFromDao = value;
			}
		}

		public bool IsCacheModelsEnabled
		{
			get
			{
				return this._isCacheModelsEnabled;
			}
			set
			{
				this._isCacheModelsEnabled = value;
			}
		}

		public DataSource DataSource
		{
			get
			{
				return this._dataSource;
			}
			set
			{
				this._dataSource = value;
			}
		}

		public XmlNode NodeContext
		{
			get
			{
				return this._nodeContext;
			}
			set
			{
				this._nodeContext = value;
			}
		}

		public XmlDocument SqlMapConfigDocument
		{
			get
			{
				return this._sqlMapConfigDocument;
			}
			set
			{
				this._sqlMapConfigDocument = value;
			}
		}

		public XmlDocument SqlMapDocument
		{
			get
			{
				return this._sqlMapDocument;
			}
			set
			{
				this._sqlMapDocument = value;
			}
		}

		public bool UseConfigFileWatcher
		{
			get
			{
				return this._useConfigFileWatcher;
			}
			set
			{
				this._useConfigFileWatcher = value;
			}
		}

		public bool UseStatementNamespaces
		{
			get
			{
				return this._useStatementNamespaces;
			}
			set
			{
				this._useStatementNamespaces = value;
			}
		}

		public ErrorContext ErrorContext
		{
			get
			{
				return this._errorContext;
			}
		}

		public HybridDictionary Providers
		{
			get
			{
				return this._providers;
			}
		}

		public NameValueCollection Properties
		{
			get
			{
				return this._properties;
			}
		}

		public bool UseReflectionOptimizer
		{
			get
			{
				return this._useReflectionOptimizer;
			}
			set
			{
				this._useReflectionOptimizer = value;
			}
		}

		public HybridDictionary CacheModelFlushOnExecuteStatements
		{
			get
			{
				return this._cacheModelFlushOnExecuteStatements;
			}
			set
			{
				this._cacheModelFlushOnExecuteStatements = value;
			}
		}

		public ConfigurationScope()
		{
			this._errorContext = new ErrorContext();
			this._providers.Clear();
		}

		public string ApplyNamespace(string id)
		{
			string result = id;
			if (this._sqlMapNamespace != null && this._sqlMapNamespace.Length > 0 && id != null && id.Length > 0 && id.IndexOf(".") < 0)
			{
				result = this._sqlMapNamespace + "." + id;
			}
			return result;
		}

		public ITypeHandler ResolveTypeHandler(System.Type clazz, string memberName, string clrType, string dbType, bool forSetter)
		{
			ITypeHandler result = null;
			if (clazz == null)
			{
				result = this.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
			}
			else if (typeof(System.Collections.IDictionary).IsAssignableFrom(clazz))
			{
				if (clrType == null || clrType.Length == 0)
				{
					result = this.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
				}
				else
				{
					try
					{
						System.Type type = TypeUtils.ResolveType(clrType);
						result = this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
					}
					catch (System.Exception ex)
					{
						throw new ConfigurationErrorsException("Error. Could not set TypeHandler.  Cause: " + ex.Message, ex);
					}
				}
			}
			else if (this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(clazz, dbType) != null)
			{
				result = this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(clazz, dbType);
			}
			else if (clrType == null || clrType.Length == 0)
			{
				System.Type type;
				if (forSetter)
				{
					type = ObjectProbe.GetMemberTypeForSetter(clazz, memberName);
				}
				else
				{
					type = ObjectProbe.GetMemberTypeForGetter(clazz, memberName);
				}
				result = this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
			}
			else
			{
				try
				{
					System.Type type = TypeUtils.ResolveType(clrType);
					result = this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
				}
				catch (System.Exception ex)
				{
					throw new ConfigurationErrorsException("Error. Could not set TypeHandler.  Cause: " + ex.Message, ex);
				}
			}
			return result;
		}
	}
}
