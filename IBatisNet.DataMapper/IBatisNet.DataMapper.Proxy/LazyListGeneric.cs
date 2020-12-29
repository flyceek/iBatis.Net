using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.MappedStatements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace IBatisNet.DataMapper.Proxy
{
	[System.Serializable]
	public class LazyListGeneric<T> : System.Collections.Generic.IList<T>, System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable where T : class
	{
		private object _param = null;

		private object _target = null;

		private ISetAccessor _setAccessor = null;

		private ISqlMapper _sqlMap = null;

		private string _statementId = string.Empty;

		private bool _loaded = false;

		private object _loadLock = new object();

		private System.Collections.Generic.IList<T> _list = null;

		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public T this[int index]
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

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				this.Load("this");
				return this[index];
			}
			set
			{
				this.Load("this");
				((System.Collections.IList)this._list)[index] = value;
			}
		}

		int System.Collections.ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		public LazyListGeneric(IMappedStatement mappedSatement, object param, object target, ISetAccessor setAccessor)
		{
			this._param = param;
			this._statementId = mappedSatement.Id;
			this._sqlMap = mappedSatement.SqlMap;
			this._target = target;
			this._setAccessor = setAccessor;
			this._list = new System.Collections.Generic.List<T>();
		}

		private void Load(string methodName)
		{
			if (LazyListGeneric<T>._logger.IsDebugEnabled)
			{
				LazyListGeneric<T>._logger.Debug("Proxyfying call to " + methodName);
			}
			lock (this._loadLock)
			{
				if (!this._loaded)
				{
					if (LazyListGeneric<T>._logger.IsDebugEnabled)
					{
						LazyListGeneric<T>._logger.Debug("Proxyfying call, query statement " + this._statementId);
					}				
					this._list = this._sqlMap.QueryForList<T>(this._statementId, this._param);
					this._loaded = true;
					this._setAccessor.Set(this._target, this._list);
				}
			}
			if (LazyListGeneric<T>._logger.IsDebugEnabled)
			{
				LazyListGeneric<T>._logger.Debug("End of proxyfied call to " + methodName);
			}
		}

		public int IndexOf(T item)
		{
			this.Load("IndexOf");
			return this._list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.Load("Insert");
			this._list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			this.Load("RemoveAt");
			this._list.RemoveAt(index);
		}

		public void Add(T item)
		{
			this.Load("Add");
			this._list.Add(item);
		}

		public void Clear()
		{
			this.Load("Clear");
			this._list.Clear();
		}

		public bool Contains(T item)
		{
			this.Load("Contains");
			return this._list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.Load("CopyTo");
			this._list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			this.Load("Remove");
			return this._list.Remove(item);
		}

		public System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			this.Load("GetEnumerator<T>");
			return this._list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			this.Load("GetEnumerator");
			return this._list.GetEnumerator();
		}

		int System.Collections.IList.Add(object value)
		{
			this.Load("Add");
			return ((System.Collections.IList)this._list).Add(value);
		}

		void System.Collections.IList.Clear()
		{
			this.Clear();
		}

		bool System.Collections.IList.Contains(object value)
		{
			this.Load("Contains");
			return ((System.Collections.IList)this._list).Contains(value);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			this.Load("IndexOf");
			return ((System.Collections.IList)this._list).IndexOf(value);
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			this.Load("IndexOf");
			((System.Collections.IList)this._list).Insert(index, value);
		}

		void System.Collections.IList.Remove(object value)
		{
			this.Load("Remove");
			((System.Collections.IList)this._list).Remove(value);
		}

		void System.Collections.IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		void System.Collections.ICollection.CopyTo(System.Array array, int index)
		{
			this.Load("CopyTo");
			((System.Collections.IList)this._list).CopyTo(array, index);
		}
	}
}
