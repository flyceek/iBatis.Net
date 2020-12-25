using IBatisNet.DataMapper.Configuration.ParameterMapping;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.MappedStatements;
using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;

namespace IBatisNet.DataMapper.Scope
{
	public class RequestScope : IScope
	{
		private IStatement _statement = null;

		private ErrorContext _errorContext = null;

		private ParameterMap _parameterMap = null;

		private PreparedStatement _preparedStatement = null;

		private IDbCommand _command = null;

		private System.Collections.Queue _selects = new System.Collections.Queue();

		private bool _rowDataFound = false;

		private static long _nextId = 0L;

		private long _id = 0L;

		private DataExchangeFactory _dataExchangeFactory = null;

		private ISqlMapSession _session = null;

		private IMappedStatement _mappedStatement = null;

		private int _currentResultMapIndex = -1;

		private System.Collections.IDictionary _uniqueKeys = null;

		public IMappedStatement MappedStatement
		{
			get
			{
				return this._mappedStatement;
			}
			set
			{
				this._mappedStatement = value;
			}
		}

		public IStatement Statement
		{
			get
			{
				return this._statement;
			}
		}

		public ISqlMapSession Session
		{
			get
			{
				return this._session;
			}
		}

		public IDbCommand IDbCommand
		{
			get
			{
				return this._command;
			}
			set
			{
				this._command = value;
			}
		}

		public bool IsRowDataFound
		{
			get
			{
				return this._rowDataFound;
			}
			set
			{
				this._rowDataFound = value;
			}
		}

		public System.Collections.Queue QueueSelect
		{
			get
			{
				return this._selects;
			}
			set
			{
				this._selects = value;
			}
		}

		public IResultMap CurrentResultMap
		{
			get
			{
				return this._statement.ResultsMap[this._currentResultMapIndex];
			}
		}

		public ParameterMap ParameterMap
		{
			get
			{
				return this._parameterMap;
			}
			set
			{
				this._parameterMap = value;
			}
		}

		public PreparedStatement PreparedStatement
		{
			get
			{
				return this._preparedStatement;
			}
			set
			{
				this._preparedStatement = value;
			}
		}

		public DataExchangeFactory DataExchangeFactory
		{
			get
			{
				return this._dataExchangeFactory;
			}
		}

		public ErrorContext ErrorContext
		{
			get
			{
				return this._errorContext;
			}
		}

		public System.Collections.IDictionary GetUniqueKeys(IResultMap map)
		{
			System.Collections.IDictionary result;
			if (this._uniqueKeys == null)
			{
				result = null;
			}
			else
			{
				result = (System.Collections.IDictionary)this._uniqueKeys[map];
			}
			return result;
		}

		public void SetUniqueKeys(IResultMap map, System.Collections.IDictionary keys)
		{
			if (this._uniqueKeys == null)
			{
				this._uniqueKeys = new System.Collections.Hashtable();
			}
			this._uniqueKeys.Add(map, keys);
		}

		public bool MoveNextResultMap()
		{
			bool result;
			if (this._currentResultMapIndex < this._statement.ResultsMap.Count - 1)
			{
				this._currentResultMapIndex++;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public RequestScope(DataExchangeFactory dataExchangeFactory, ISqlMapSession session, IStatement statement)
		{
			this._errorContext = new ErrorContext();
			this._statement = statement;
			this._parameterMap = statement.ParameterMap;
			this._session = session;
			this._dataExchangeFactory = dataExchangeFactory;
			this._id = RequestScope.GetNextId();
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (this == obj)
			{
				result = true;
			}
			else if (!(obj is RequestScope))
			{
				result = false;
			}
			else
			{
				RequestScope requestScope = (RequestScope)obj;
				result = (this._id == requestScope._id);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return (int)(this._id ^ this._id >> 32);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public static long GetNextId()
		{
			long expr_06 = RequestScope._nextId;
			RequestScope._nextId = expr_06 + 1L;
			return expr_06;
		}
	}
}
