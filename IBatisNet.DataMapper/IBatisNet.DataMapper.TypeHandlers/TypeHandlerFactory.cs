using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper.Configuration.Alias;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.TypeHandlers.Nullables;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public class TypeHandlerFactory
	{
		private const string NULL = "_NULL_TYPE_";

		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private System.Collections.IDictionary _typeHandlerMap = new HybridDictionary();

		private ITypeHandler _unknownTypeHandler = null;

		private System.Collections.IDictionary _typeAliasMaps = new HybridDictionary();

		public TypeHandlerFactory()
		{
			ITypeHandler handler = new DBNullTypeHandler();
			this.Register(typeof(System.DBNull), handler);
			handler = new BooleanTypeHandler();
			this.Register(typeof(bool), handler);
			handler = new ByteTypeHandler();
			this.Register(typeof(byte), handler);
			handler = new CharTypeHandler();
			this.Register(typeof(char), handler);
			handler = new DateTimeTypeHandler();
			this.Register(typeof(System.DateTime), handler);
			handler = new DecimalTypeHandler();
			this.Register(typeof(decimal), handler);
			handler = new DoubleTypeHandler();
			this.Register(typeof(double), handler);
			handler = new Int16TypeHandler();
			this.Register(typeof(short), handler);
			handler = new Int32TypeHandler();
			this.Register(typeof(int), handler);
			handler = new Int64TypeHandler();
			this.Register(typeof(long), handler);
			handler = new SingleTypeHandler();
			this.Register(typeof(float), handler);
			handler = new StringTypeHandler();
			this.Register(typeof(string), handler);
			handler = new GuidTypeHandler();
			this.Register(typeof(System.Guid), handler);
			handler = new TimeSpanTypeHandler();
			this.Register(typeof(System.TimeSpan), handler);
			handler = new ByteArrayTypeHandler();
			this.Register(typeof(byte[]), handler);
			handler = new ObjectTypeHandler();
			this.Register(typeof(object), handler);
			handler = new EnumTypeHandler();
			this.Register(typeof(System.Enum), handler);
			handler = new UInt16TypeHandler();
			this.Register(typeof(ushort), handler);
			handler = new UInt32TypeHandler();
			this.Register(typeof(uint), handler);
			handler = new UInt64TypeHandler();
			this.Register(typeof(ulong), handler);
			handler = new SByteTypeHandler();
			this.Register(typeof(sbyte), handler);
			handler = new NullableBooleanTypeHandler();
			this.Register(typeof(bool?), handler);
			handler = new NullableByteTypeHandler();
			this.Register(typeof(byte?), handler);
			handler = new NullableCharTypeHandler();
			this.Register(typeof(char?), handler);
			handler = new NullableDateTimeTypeHandler();
			this.Register(typeof(System.DateTime?), handler);
			handler = new NullableDecimalTypeHandler();
			this.Register(typeof(decimal?), handler);
			handler = new NullableDoubleTypeHandler();
			this.Register(typeof(double?), handler);
			handler = new NullableGuidTypeHandler();
			this.Register(typeof(System.Guid?), handler);
			handler = new NullableInt16TypeHandler();
			this.Register(typeof(short?), handler);
			handler = new NullableInt32TypeHandler();
			this.Register(typeof(int?), handler);
			handler = new NullableInt64TypeHandler();
			this.Register(typeof(long?), handler);
			handler = new NullableSingleTypeHandler();
			this.Register(typeof(float?), handler);
			handler = new NullableUInt16TypeHandler();
			this.Register(typeof(ushort?), handler);
			handler = new NullableUInt32TypeHandler();
			this.Register(typeof(uint?), handler);
			handler = new NullableUInt64TypeHandler();
			this.Register(typeof(ulong?), handler);
			handler = new NullableSByteTypeHandler();
			this.Register(typeof(sbyte?), handler);
			handler = new NullableTimeSpanTypeHandler();
			this.Register(typeof(System.TimeSpan?), handler);
			this._unknownTypeHandler = new UnknownTypeHandler(this);
		}

		public ITypeHandler GetTypeHandler(System.Type type)
		{
			return this.GetTypeHandler(type, null);
		}

		public ITypeHandler GetTypeHandler(System.Type type, string dbType)
		{
			ITypeHandler privateTypeHandler;
			if (type.IsEnum)
			{
				privateTypeHandler = this.GetPrivateTypeHandler(typeof(System.Enum), dbType);
			}
			else
			{
				privateTypeHandler = this.GetPrivateTypeHandler(type, dbType);
			}
			return privateTypeHandler;
		}

		private ITypeHandler GetPrivateTypeHandler(System.Type type, string dbType)
		{
			System.Collections.IDictionary dictionary = (System.Collections.IDictionary)this._typeHandlerMap[type];
			ITypeHandler typeHandler = null;
			if (dictionary != null)
			{
				if (dbType == null)
				{
					typeHandler = (ITypeHandler)dictionary["_NULL_TYPE_"];
				}
				else
				{
					typeHandler = (ITypeHandler)dictionary[dbType];
					if (typeHandler == null)
					{
						typeHandler = (ITypeHandler)dictionary["_NULL_TYPE_"];
					}
				}
				if (typeHandler == null)
				{
					throw new DataMapperException(string.Format("Type handler for {0} not registered.", type.Name));
				}
			}
			return typeHandler;
		}

		public void Register(System.Type type, ITypeHandler handler)
		{
			this.Register(type, null, handler);
		}

		public void Register(System.Type type, string dbType, ITypeHandler handler)
		{
			HybridDictionary hybridDictionary = (HybridDictionary)this._typeHandlerMap[type];
			if (hybridDictionary == null)
			{
				hybridDictionary = new HybridDictionary();
				this._typeHandlerMap.Add(type, hybridDictionary);
			}
			if (dbType == null)
			{
				if (TypeHandlerFactory._logger.IsInfoEnabled)
				{
					ITypeHandler typeHandler = (ITypeHandler)hybridDictionary["_NULL_TYPE_"];
					if (typeHandler != null)
					{
						CustomTypeHandler customTypeHandler = handler as CustomTypeHandler;
						string text = string.Empty;
						if (customTypeHandler != null)
						{
							text = customTypeHandler.Callback.ToString();
						}
						else
						{
							text = handler.ToString();
						}
						TypeHandlerFactory._logger.Info(string.Concat(new string[]
						{
							"Replacing type handler [",
							typeHandler.ToString(),
							"] with [",
							text,
							"]."
						}));
					}
				}
				hybridDictionary["_NULL_TYPE_"] = handler;
			}
			else
			{
				hybridDictionary.Add(dbType, handler);
			}
		}

		public ITypeHandler GetUnkownTypeHandler()
		{
			return this._unknownTypeHandler;
		}

		public bool IsSimpleType(System.Type type)
		{
			bool result = false;
			if (type != null)
			{
				ITypeHandler typeHandler = this.GetTypeHandler(type, null);
				if (typeHandler != null)
				{
					result = typeHandler.IsSimpleType;
				}
			}
			return result;
		}

		internal TypeAlias GetTypeAlias(string name)
		{
			TypeAlias result;
			if (this._typeAliasMaps.Contains(name))
			{
				result = (TypeAlias)this._typeAliasMaps[name];
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal System.Type GetType(string className)
		{
			TypeAlias typeAlias = this.GetTypeAlias(className);
			System.Type result;
			if (typeAlias != null)
			{
				result = typeAlias.Class;
			}
			else
			{
				result = TypeUtils.ResolveType(className);
			}
			return result;
		}

		internal void AddTypeAlias(string key, TypeAlias typeAlias)
		{
			if (this._typeAliasMaps.Contains(key))
			{
				throw new DataMapperException(string.Concat(new string[]
				{
					" Alias name conflict occurred.  The type alias '",
					key,
					"' is already mapped to the value '",
					typeAlias.ClassName,
					"'."
				}));
			}
			this._typeAliasMaps.Add(key, typeAlias);
		}
	}
}
