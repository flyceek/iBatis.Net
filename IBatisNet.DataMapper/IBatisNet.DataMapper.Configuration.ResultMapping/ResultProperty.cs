using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.MappedStatements.ArgumentStrategy;
using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
using IBatisNet.DataMapper.Proxy;
using IBatisNet.DataMapper.Scope;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
	[XmlRoot("result", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class ResultProperty
	{
		private class ArrayListFactory : IFactory
		{
			public object CreateInstance(object[] parameters)
			{
				return new System.Collections.ArrayList();
			}
		}

		public const int UNKNOWN_COLUMN_INDEX = -999999;

		[System.NonSerialized]
		private ISetAccessor _setAccessor = null;

		[System.NonSerialized]
		private string _nullValue = null;

		[System.NonSerialized]
		private string _propertyName = string.Empty;

		[System.NonSerialized]
		private string _columnName = string.Empty;

		[System.NonSerialized]
		private int _columnIndex = -999999;

		[System.NonSerialized]
		private string _select = string.Empty;

		[System.NonSerialized]
		private string _nestedResultMapName = string.Empty;

		[System.NonSerialized]
		private IResultMap _nestedResultMap = null;

		[System.NonSerialized]
		private string _dbType = null;

		[System.NonSerialized]
		private string _clrType = string.Empty;

		[System.NonSerialized]
		private bool _isLazyLoad = false;

		[System.NonSerialized]
		private ITypeHandler _typeHandler = null;

		[System.NonSerialized]
		private string _callBackName = string.Empty;

		[System.NonSerialized]
		private bool _isComplexMemberName = false;

		[System.NonSerialized]
		private IPropertyStrategy _propertyStrategy = null;

		[System.NonSerialized]
		private ILazyFactory _lazyFactory = null;

		[System.NonSerialized]
		private bool _isIList = false;

		[System.NonSerialized]
		private bool _isGenericIList = false;

		[System.NonSerialized]
		private IFactory _listFactory = null;

		[System.NonSerialized]
		private static readonly IFactory _arrayListFactory = new ResultProperty.ArrayListFactory();

		[XmlIgnore]
		public bool IsGenericIList
		{
			get
			{
				return this._isGenericIList;
			}
		}

		[XmlIgnore]
		public bool IsIList
		{
			get
			{
				return this._isIList;
			}
		}

		[XmlIgnore]
		public IFactory ListFactory
		{
			get
			{
				return this._listFactory;
			}
		}

		[XmlIgnore]
		public ILazyFactory LazyFactory
		{
			get
			{
				return this._lazyFactory;
			}
		}

		[XmlIgnore]
		public virtual IArgumentStrategy ArgumentStrategy
		{
			get
			{
				throw new System.NotImplementedException("Valid on ArgumentProperty");
			}
			set
			{
				throw new System.NotImplementedException("Valid on ArgumentProperty");
			}
		}

		[XmlIgnore]
		public IPropertyStrategy PropertyStrategy
		{
			get
			{
				return this._propertyStrategy;
			}
			set
			{
				this._propertyStrategy = value;
			}
		}

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

		[XmlAttribute("lazyLoad")]
		public virtual bool IsLazyLoad
		{
			get
			{
				return this._isLazyLoad;
			}
			set
			{
				this._isLazyLoad = value;
			}
		}

		[XmlIgnore]
		public ITypeHandler TypeHandler
		{
			get
			{
				if (this._typeHandler == null)
				{
					throw new DataMapperException(string.Format("Error on Result property {0}, type handler for {1} is not registered.", this.PropertyName, this.MemberType.Name));
				}
				return this._typeHandler;
			}
			set
			{
				this._typeHandler = value;
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

		[XmlAttribute("select")]
		public string Select
		{
			get
			{
				return this._select;
			}
			set
			{
				this._select = value;
			}
		}

		[XmlAttribute("resultMapping")]
		public string NestedResultMapName
		{
			get
			{
				return this._nestedResultMapName;
			}
			set
			{
				this._nestedResultMapName = value;
			}
		}

		[XmlAttribute("property")]
		public string PropertyName
		{
			get
			{
				return this._propertyName;
			}
			set
			{
				this._propertyName = value;
				if (this._propertyName.IndexOf('.') < 0)
				{
					this._isComplexMemberName = false;
				}
				else
				{
					this._isComplexMemberName = true;
				}
			}
		}

		[XmlIgnore]
		public ISetAccessor SetAccessor
		{
			get
			{
				return this._setAccessor;
			}
		}

		[XmlIgnore]
		public virtual System.Type MemberType
		{
			get
			{
				System.Type result;
				if (this._setAccessor != null)
				{
					result = this._setAccessor.MemberType;
				}
				else
				{
					if (this._nestedResultMap == null)
					{
						throw new IBatisNetException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Could not resolve member type for result property '{0}'. Neither nested result map nor typed setter was provided.", new object[]
						{
							this._propertyName
						}));
					}
					result = this._nestedResultMap.Class;
				}
				return result;
			}
		}

		[XmlIgnore]
		public bool HasNullValue
		{
			get
			{
				return this._nullValue != null;
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
		public IResultMap NestedResultMap
		{
			get
			{
				return this._nestedResultMap;
			}
			set
			{
				this._nestedResultMap = value;
			}
		}

		public bool IsComplexMemberName
		{
			get
			{
				return this._isComplexMemberName;
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

		public void Initialize(ConfigurationScope configScope, System.Type resultClass)
		{
			if (this._propertyName.Length > 0 && this._propertyName != "value" && !typeof(System.Collections.IDictionary).IsAssignableFrom(resultClass))
			{
				if (!this._isComplexMemberName)
				{
					this._setAccessor = configScope.DataExchangeFactory.AccessorFactory.SetAccessorFactory.CreateSetAccessor(resultClass, this._propertyName);
				}
				else
				{
					System.Reflection.MemberInfo memberInfoForSetter = ObjectProbe.GetMemberInfoForSetter(resultClass, this._propertyName);
					string name = this._propertyName.Substring(this._propertyName.LastIndexOf('.') + 1);
					this._setAccessor = configScope.DataExchangeFactory.AccessorFactory.SetAccessorFactory.CreateSetAccessor(memberInfoForSetter.ReflectedType, name);
				}
				this._isGenericIList = TypeUtils.IsImplementGenericIListInterface(this.MemberType);
				this._isIList = typeof(System.Collections.IList).IsAssignableFrom(this.MemberType);
				if (this._isGenericIList)
				{
					if (this.MemberType.IsArray)
					{
						this._listFactory = ResultProperty._arrayListFactory;
					}
					else
					{
						System.Type[] genericArguments = this.MemberType.GetGenericArguments();
						if (genericArguments.Length == 0)
						{
							this._listFactory = configScope.DataExchangeFactory.ObjectFactory.CreateFactory(this.MemberType, System.Type.EmptyTypes);
						}
						else
						{
							System.Type typeFromHandle = typeof(System.Collections.Generic.IList<>);
							System.Type left = typeFromHandle.MakeGenericType(genericArguments);
							System.Type typeFromHandle2 = typeof(System.Collections.Generic.List<>);
							System.Type left2 = typeFromHandle2.MakeGenericType(genericArguments);
							if (left == this.MemberType || left2 == this.MemberType)
							{
								System.Type typeToCreate = typeFromHandle2.MakeGenericType(genericArguments);
								this._listFactory = configScope.DataExchangeFactory.ObjectFactory.CreateFactory(typeToCreate, System.Type.EmptyTypes);
							}
							else
							{
								this._listFactory = configScope.DataExchangeFactory.ObjectFactory.CreateFactory(this.MemberType, System.Type.EmptyTypes);
							}
						}
					}
				}
				else if (this._isIList)
				{
					if (this.MemberType.IsArray)
					{
						this._listFactory = ResultProperty._arrayListFactory;
					}
					else if (this.MemberType == typeof(System.Collections.IList))
					{
						this._listFactory = ResultProperty._arrayListFactory;
					}
					else
					{
						this._listFactory = configScope.DataExchangeFactory.ObjectFactory.CreateFactory(this.MemberType, System.Type.EmptyTypes);
					}
				}
			}
			if (this.CallBackName != null && this.CallBackName.Length > 0)
			{
				configScope.ErrorContext.MoreInfo = string.Concat(new string[]
				{
					"Result property '",
					this._propertyName,
					"' check the typeHandler attribute '",
					this.CallBackName,
					"' (must be a ITypeHandlerCallback implementation)."
				});
				try
				{
					System.Type type = configScope.SqlMapper.TypeHandlerFactory.GetType(this.CallBackName);
					ITypeHandlerCallback callback = (ITypeHandlerCallback)System.Activator.CreateInstance(type);
					this._typeHandler = new CustomTypeHandler(callback);
				}
				catch (System.Exception ex)
				{
					throw new ConfigurationException("Error occurred during custom type handler configuration.  Cause: " + ex.Message, ex);
				}
			}
			else
			{
				configScope.ErrorContext.MoreInfo = "Result property '" + this._propertyName + "' set the typeHandler attribute.";
				this._typeHandler = configScope.ResolveTypeHandler(resultClass, this._propertyName, this._clrType, this._dbType, true);
			}
			if (this.IsLazyLoad)
			{
				this._lazyFactory = new LazyFactoryBuilder().GetLazyFactory(this._setAccessor.MemberType);
			}
		}

		internal void Initialize(TypeHandlerFactory typeHandlerFactory, ISetAccessor setAccessor)
		{
			this._setAccessor = setAccessor;
			this._typeHandler = typeHandlerFactory.GetTypeHandler(setAccessor.MemberType);
		}

		public object GetDataBaseValue(IDataReader dataReader)
		{
			object obj;
			if (this._columnIndex == -999999)
			{
				obj = this.TypeHandler.GetValueByName(this, dataReader);
			}
			else
			{
				obj = this.TypeHandler.GetValueByIndex(this, dataReader);
			}
			bool flag = obj == System.DBNull.Value;
			if (flag)
			{
				if (this.HasNullValue)
				{
					if (this._setAccessor != null)
					{
						obj = this.TypeHandler.ValueOf(this._setAccessor.MemberType, this._nullValue);
					}
					else
					{
						obj = this.TypeHandler.ValueOf(null, this._nullValue);
					}
				}
				else
				{
					obj = this.TypeHandler.NullValue;
				}
			}
			return obj;
		}

		public object TranslateValue(object value)
		{
			object result;
			if (value == null)
			{
				result = this.TypeHandler.NullValue;
			}
			else
			{
				result = value;
			}
			return result;
		}

		public ResultProperty Clone()
		{
			return new ResultProperty
			{
				CLRType = this.CLRType,
				CallBackName = this.CallBackName,
				ColumnIndex = this.ColumnIndex,
				ColumnName = this.ColumnName,
				DbType = this.DbType,
				IsLazyLoad = this.IsLazyLoad,
				NestedResultMapName = this.NestedResultMapName,
				NullValue = this.NullValue,
				PropertyName = this.PropertyName,
				Select = this.Select
			};
		}
	}
}
