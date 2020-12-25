using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Xml;
using IBatisNet.DataMapper.Configuration.Alias;
using IBatisNet.DataMapper.Scope;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.DataMapper.Configuration.Serializers
{
	public sealed class TypeHandlerDeSerializer
	{
		public static void Deserialize(XmlNode node, ConfigurationScope configScope)
		{
			TypeHandler typeHandler = new TypeHandler();
			NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
			typeHandler.CallBackName = NodeUtils.GetStringAttribute(attributes, "callback");
			typeHandler.ClassName = NodeUtils.GetStringAttribute(attributes, "type");
			typeHandler.DbType = NodeUtils.GetStringAttribute(attributes, "dbType");
			typeHandler.Initialize();
			configScope.ErrorContext.MoreInfo = "Check the callback attribute '" + typeHandler.CallBackName + "' (must be a classname).";
			System.Type type = configScope.SqlMapper.TypeHandlerFactory.GetType(typeHandler.CallBackName);
			object obj = System.Activator.CreateInstance(type);
			ITypeHandler handler;
			if (obj is ITypeHandlerCallback)
			{
				handler = new CustomTypeHandler((ITypeHandlerCallback)obj);
			}
			else
			{
				if (!(obj is ITypeHandler))
				{
					throw new ConfigurationException("The callBack type is not a valid implementation of ITypeHandler or ITypeHandlerCallback");
				}
				handler = (ITypeHandler)obj;
			}
			configScope.ErrorContext.MoreInfo = string.Concat(new string[]
			{
				"Check the type attribute '",
				typeHandler.ClassName,
				"' (must be a class name) or the dbType '",
				typeHandler.DbType,
				"' (must be a DbType type name)."
			});
			if (typeHandler.DbType != null && typeHandler.DbType.Length > 0)
			{
				configScope.DataExchangeFactory.TypeHandlerFactory.Register(TypeUtils.ResolveType(typeHandler.ClassName), typeHandler.DbType, handler);
			}
			else
			{
				configScope.DataExchangeFactory.TypeHandlerFactory.Register(TypeUtils.ResolveType(typeHandler.ClassName), handler);
			}
		}
	}
}
