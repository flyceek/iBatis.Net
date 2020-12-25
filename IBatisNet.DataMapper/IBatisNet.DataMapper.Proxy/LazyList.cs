using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.MappedStatements;
using System;
using System.Collections;
using System.Reflection;

namespace IBatisNet.DataMapper.Proxy
{
	[System.Serializable]
	public class LazyList : System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable
	{
		private object _param = null;

		private object _target = null;

		private ISetAccessor _setAccessor = null;

		private ISqlMapper _sqlMap = null;

		private string _statementId = string.Empty;

		private bool _loaded = false;

		private object _loadLock = new object();

		private System.Collections.IList _list = null;

		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public object this[int index]
		{
			get
			{
				this.Load("this");
				return this._list[index];
			}
			set
			{
				this.Load("this");
				this._list[index] = value;
			}
		}

		public int Count
		{
			get
			{
				this.Load("Count");
				return this._list.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		private void Load(string methodName)
		{
			if (LazyList._logger.IsDebugEnabled)
			{
				LazyList._logger.Debug("Proxyfying call to " + methodName);
			}
			lock (this._loadLock)
			{
				if (!this._loaded)
				{
					if (LazyList._logger.IsDebugEnabled)
					{
						LazyList._logger.Debug("Proxyfying call, query statement " + this._statementId);
					}
					this._list = this._sqlMap.QueryForList(this._statementId, this._param);
					this._loaded = true;
					this._setAccessor.Set(this._target, this._list);
				}
			}
			if (LazyList._logger.IsDebugEnabled)
			{
				LazyList._logger.Debug("End of proxyfied call to " + methodName);
			}
		}

		public LazyList(IMappedStatement mappedSatement, object param, object target, ISetAccessor setAccessor)
		{
			this._list = new System.Collections.ArrayList();
			this._param = param;
			this._statementId = mappedSatement.Id;
			this._sqlMap = mappedSatement.SqlMap;
			this._target = target;
			this._setAccessor = setAccessor;
		}

		public int Add(object value)
		{
			this.Load("Add");
			return this._list.Add(value);
		}

		public void Clear()
		{
			this.Load("Clear");
			this._list.Clear();
		}

		public bool Contains(object value)
		{
			this.Load("Contains");
			return this._list.Contains(value);
		}

		public int IndexOf(object value)
		{
			this.Load("IndexOf");
			return this._list.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			this.Load("Insert");
			this._list.Insert(index, value);
		}

		public void Remove(object value)
		{
			this.Load("Remove");
			this._list.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.Load("RemoveAt");
			this._list.RemoveAt(index);
		}

		public void CopyTo(System.Array array, int index)
		{
			this.Load("CopyTo");
			this._list.CopyTo(array, index);
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			this.Load("Add");
			return this._list.GetEnumerator();
		}
	}
}
