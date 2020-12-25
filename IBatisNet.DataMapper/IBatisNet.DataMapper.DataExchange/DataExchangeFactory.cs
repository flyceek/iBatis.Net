using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;

namespace IBatisNet.DataMapper.DataExchange
{
	public class DataExchangeFactory
	{
		private TypeHandlerFactory _typeHandlerFactory = null;

		private IObjectFactory _objectFactory = null;

		private AccessorFactory _accessorFactory = null;

		private IDataExchange _primitiveDataExchange = null;

		private IDataExchange _complexDataExchange = null;

		private IDataExchange _listDataExchange = null;

		private IDataExchange _dictionaryDataExchange = null;

		public TypeHandlerFactory TypeHandlerFactory
		{
			get
			{
				return this._typeHandlerFactory;
			}
		}

		public IObjectFactory ObjectFactory
		{
			get
			{
				return this._objectFactory;
			}
		}

		public AccessorFactory AccessorFactory
		{
			get
			{
				return this._accessorFactory;
			}
		}

		public DataExchangeFactory(TypeHandlerFactory typeHandlerFactory, IObjectFactory objectFactory, AccessorFactory accessorFactory)
		{
			this._objectFactory = objectFactory;
			this._typeHandlerFactory = typeHandlerFactory;
			this._accessorFactory = accessorFactory;
			this._primitiveDataExchange = new PrimitiveDataExchange(this);
			this._complexDataExchange = new ComplexDataExchange(this);
			this._listDataExchange = new ListDataExchange(this);
			this._dictionaryDataExchange = new DictionaryDataExchange(this);
		}

		public IDataExchange GetDataExchangeForClass(System.Type clazz)
		{
			IDataExchange result;
			if (clazz == null)
			{
				result = this._complexDataExchange;
			}
			else if (typeof(System.Collections.IList).IsAssignableFrom(clazz))
			{
				result = this._listDataExchange;
			}
			else if (typeof(System.Collections.IDictionary).IsAssignableFrom(clazz))
			{
				result = this._dictionaryDataExchange;
			}
			else if (this._typeHandlerFactory.GetTypeHandler(clazz) != null)
			{
				result = this._primitiveDataExchange;
			}
			else
			{
				result = new DotNetObjectDataExchange(clazz, this);
			}
			return result;
		}
	}
}
