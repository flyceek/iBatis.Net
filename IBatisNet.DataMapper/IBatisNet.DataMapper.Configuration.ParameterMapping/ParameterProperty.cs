using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Scope;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Data;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.ParameterMapping
{
	[XmlRoot("parameter", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class ParameterProperty
	{
		[System.NonSerialized]
		private string _nullValue = null;

		[System.NonSerialized]
		private string _propertyName = string.Empty;

		[System.NonSerialized]
		private ParameterDirection _direction = ParameterDirection.Input;

		[System.NonSerialized]
		private string _directionAttribute = string.Empty;

		[System.NonSerialized]
		private string _dbType = null;

		[System.NonSerialized]
		private int _size = -1;

		[System.NonSerialized]
		private byte _scale = 0;

		[System.NonSerialized]
		private byte _precision = 0;

		[System.NonSerialized]
		private string _columnName = string.Empty;

		[System.NonSerialized]
		private ITypeHandler _typeHandler = null;

		[System.NonSerialized]
		private string _clrType = string.Empty;

		[System.NonSerialized]
		private string _callBackName = string.Empty;

		[System.NonSerialized]
		private IGetAccessor _getAccessor = null;

		[System.NonSerialized]
		private bool _isComplexMemberName = false;

		public bool IsComplexMemberName
		{
			get
			{
				return this._isComplexMemberName;
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

		[XmlIgnore]
		public ITypeHandler TypeHandler
		{
			get
			{
				return this._typeHandler;
			}
			set
			{
				this._typeHandler = value;
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

		[XmlAttribute("size")]
		public int Size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = value;
			}
		}

		[XmlAttribute("scale")]
		public byte Scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this._scale = value;
			}
		}

		[XmlAttribute("precision")]
		public byte Precision
		{
			get
			{
				return this._precision;
			}
			set
			{
				this._precision = value;
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

		[XmlAttribute("direction")]
		public string DirectionAttribute
		{
			get
			{
				return this._directionAttribute;
			}
			set
			{
				this._directionAttribute = value;
			}
		}

		[XmlIgnore]
		public ParameterDirection Direction
		{
			get
			{
				return this._direction;
			}
			set
			{
				this._direction = value;
				this._directionAttribute = this._direction.ToString();
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
				if (value == null || value.Length < 1)
				{
					throw new System.ArgumentNullException("The property attribute is mandatory in a paremeter property.");
				}
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
		public IGetAccessor GetAccessor
		{
			get
			{
				return this._getAccessor;
			}
		}

		public void Initialize(IScope scope, System.Type parameterClass)
		{
			if (this._directionAttribute.Length > 0)
			{
				this._direction = (ParameterDirection)System.Enum.Parse(typeof(ParameterDirection), this._directionAttribute, true);
			}
			if (!typeof(System.Collections.IDictionary).IsAssignableFrom(parameterClass) && parameterClass != null && !scope.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(parameterClass))
			{
				if (!this._isComplexMemberName)
				{
					IGetAccessorFactory getAccessorFactory = scope.DataExchangeFactory.AccessorFactory.GetAccessorFactory;
					this._getAccessor = getAccessorFactory.CreateGetAccessor(parameterClass, this._propertyName);
				}
				else
				{
					string name = this._propertyName.Substring(this._propertyName.LastIndexOf('.') + 1);
					string memberName = this._propertyName.Substring(0, this._propertyName.LastIndexOf('.'));
					System.Type memberTypeForGetter = ObjectProbe.GetMemberTypeForGetter(parameterClass, memberName);
					IGetAccessorFactory getAccessorFactory = scope.DataExchangeFactory.AccessorFactory.GetAccessorFactory;
					this._getAccessor = getAccessorFactory.CreateGetAccessor(memberTypeForGetter, name);
				}
			}
			scope.ErrorContext.MoreInfo = "Check the parameter mapping typeHandler attribute '" + this.CallBackName + "' (must be a ITypeHandlerCallback implementation).";
			if (this.CallBackName.Length > 0)
			{
				try
				{
					System.Type type = scope.DataExchangeFactory.TypeHandlerFactory.GetType(this.CallBackName);
					ITypeHandlerCallback callback = (ITypeHandlerCallback)System.Activator.CreateInstance(type);
					this._typeHandler = new CustomTypeHandler(callback);
				}
				catch (System.Exception ex)
				{
					throw new ConfigurationException("Error occurred during custom type handler configuration.  Cause: " + ex.Message, ex);
				}
			}
			else if (this.CLRType.Length == 0)
			{
				if (this._getAccessor != null && scope.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(this._getAccessor.MemberType))
				{
					this._typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(this._getAccessor.MemberType, this._dbType);
				}
				else
				{
					this._typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
				}
			}
			else
			{
				System.Type type = TypeUtils.ResolveType(this.CLRType);
				if (scope.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(type))
				{
					this._typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, this._dbType);
				}
				else
				{
					type = ObjectProbe.GetMemberTypeForGetter(type, this.PropertyName);
					this._typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, this._dbType);
				}
			}
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj == null || base.GetType() != obj.GetType())
			{
				result = false;
			}
			else
			{
				ParameterProperty parameterProperty = (ParameterProperty)obj;
				result = (this.PropertyName == parameterProperty.PropertyName);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this._propertyName.GetHashCode();
		}

		public ParameterProperty Clone()
		{
			return new ParameterProperty
			{
				CallBackName = this.CallBackName,
				CLRType = this.CLRType,
				ColumnName = this.ColumnName,
				DbType = this.DbType,
				DirectionAttribute = this.DirectionAttribute,
				NullValue = this.NullValue,
				PropertyName = this.PropertyName,
				Precision = this.Precision,
				Scale = this.Scale,
				Size = this.Size
			};
		}
	}
}
