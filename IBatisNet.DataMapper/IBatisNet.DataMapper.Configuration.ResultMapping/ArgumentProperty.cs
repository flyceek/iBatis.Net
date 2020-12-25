using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper.MappedStatements.ArgumentStrategy;
using IBatisNet.DataMapper.Scope;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
	[XmlRoot("argument", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class ArgumentProperty : ResultProperty
	{
		[System.NonSerialized]
		private string _argumentName = string.Empty;

		[System.NonSerialized]
		private System.Type _argumentType = null;

		[System.NonSerialized]
		private IArgumentStrategy _argumentStrategy = null;

		[XmlIgnore]
		public override IArgumentStrategy ArgumentStrategy
		{
			get
			{
				return this._argumentStrategy;
			}
			set
			{
				this._argumentStrategy = value;
			}
		}

		[XmlAttribute("argumentName")]
		public string ArgumentName
		{
			get
			{
				return this._argumentName;
			}
			set
			{
				if (value == null || value.Length < 1)
				{
					throw new System.ArgumentNullException("The name attribute is mandatory in a argument tag.");
				}
				this._argumentName = value;
			}
		}

		[XmlAttribute("lazyLoad")]
		public override bool IsLazyLoad
		{
			get
			{
				return false;
			}
			set
			{
				throw new System.InvalidOperationException("Argument property cannot be lazy load.");
			}
		}

		[XmlIgnore]
		public override System.Type MemberType
		{
			get
			{
				return this._argumentType;
			}
		}

		public void Initialize(ConfigurationScope configScope, System.Reflection.ConstructorInfo constructorInfo)
		{
			System.Reflection.ParameterInfo[] parameters = constructorInfo.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				bool flag = parameters[i].Name == this._argumentName;
				if (flag)
				{
					this._argumentType = parameters[i].ParameterType;
					break;
				}
			}
			if (base.CallBackName != null && base.CallBackName.Length > 0)
			{
				configScope.ErrorContext.MoreInfo = string.Concat(new string[]
				{
					"Argument property (",
					this._argumentName,
					"), check the typeHandler attribute '",
					base.CallBackName,
					"' (must be a ITypeHandlerCallback implementation)."
				});
				try
				{
					System.Type type = configScope.SqlMapper.TypeHandlerFactory.GetType(base.CallBackName);
					ITypeHandlerCallback callback = (ITypeHandlerCallback)System.Activator.CreateInstance(type);
					base.TypeHandler = new CustomTypeHandler(callback);
				}
				catch (System.Exception ex)
				{
					throw new ConfigurationErrorsException("Error occurred during custom type handler configuration.  Cause: " + ex.Message, ex);
				}
			}
			else
			{
				configScope.ErrorContext.MoreInfo = "Argument property (" + this._argumentName + ") set the typeHandler attribute.";
				base.TypeHandler = this.ResolveTypeHandler(configScope, this._argumentType, base.CLRType, base.DbType);
			}
		}

		public ITypeHandler ResolveTypeHandler(ConfigurationScope configScope, System.Type argumenType, string clrType, string dbType)
		{
			ITypeHandler result = null;
			if (argumenType == null)
			{
				result = configScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
			}
			else if (typeof(System.Collections.IDictionary).IsAssignableFrom(argumenType))
			{
				if (clrType == null || clrType.Length == 0)
				{
					result = configScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
				}
				else
				{
					try
					{
						System.Type type = TypeUtils.ResolveType(clrType);
						result = configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
					}
					catch (System.Exception ex)
					{
						throw new ConfigurationErrorsException("Error. Could not set TypeHandler.  Cause: " + ex.Message, ex);
					}
				}
			}
			else if (configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(argumenType, dbType) != null)
			{
				result = configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(argumenType, dbType);
			}
			else if (clrType == null || clrType.Length == 0)
			{
				result = configScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
			}
			else
			{
				try
				{
					System.Type type = TypeUtils.ResolveType(clrType);
					result = configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
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
