using IBatisNet.Common.Pagination;
using IBatisNet.DataMapper.Exceptions;
using System;
using System.Collections;

namespace IBatisNet.DataMapper.MappedStatements
{
	public class PaginatedList : IPaginatedList, System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IEnumerator
	{
		private int _pageSize = 0;

		private int _index = 0;

		private System.Collections.IList _prevPageList = null;

		private System.Collections.IList _currentPageList = null;

		private System.Collections.IList _nextPageList = null;

		private IMappedStatement _mappedStatement = null;

		private object _parameterObject = null;

		public bool IsEmpty
		{
			get
			{
				return this._currentPageList.Count == 0;
			}
		}

		public int PageIndex
		{
			get
			{
				return this._index;
			}
		}

		public bool IsPreviousPageAvailable
		{
			get
			{
				return this._prevPageList.Count > 0;
			}
		}

		public bool IsFirstPage
		{
			get
			{
				return this._index == 0;
			}
		}

		public int PageSize
		{
			get
			{
				return this._pageSize;
			}
		}

		public bool IsMiddlePage
		{
			get
			{
				return !this.IsFirstPage && !this.IsLastPage;
			}
		}

		public bool IsNextPageAvailable
		{
			get
			{
				return this._nextPageList.Count > 0;
			}
		}

		public bool IsLastPage
		{
			get
			{
				return this._nextPageList.Count < 1;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this._currentPageList.IsReadOnly;
			}
		}

		public object this[int index]
		{
			get
			{
				return this._currentPageList[index];
			}
			set
			{
				this._currentPageList[index] = value;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return this._currentPageList.IsFixedSize;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this._currentPageList.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this._currentPageList.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._currentPageList.SyncRoot;
			}
		}

		public object Current
		{
			get
			{
				return this._currentPageList.GetEnumerator().Current;
			}
		}

		public PaginatedList(IMappedStatement mappedStatement, object parameterObject, int pageSize)
		{
			this._mappedStatement = mappedStatement;
			this._parameterObject = parameterObject;
			this._pageSize = pageSize;
			this._index = 0;
			this.PageTo(0);
		}

		private void PageForward()
		{
			try
			{
				this._prevPageList = this._currentPageList;
				this._currentPageList = this._nextPageList;
				this._nextPageList = this.GetList(this._index + 1, this._pageSize);
			}
			catch (DataMapperException ex)
			{
				throw new DataMapperException("Unexpected error while repaginating paged list.  Cause: " + ex.Message, ex);
			}
		}

		private void PageBack()
		{
			try
			{
				this._nextPageList = this._currentPageList;
				this._currentPageList = this._prevPageList;
				if (this._index > 0)
				{
					this._prevPageList = this.GetList(this._index - 1, this._pageSize);
				}
				else
				{
					this._prevPageList = new System.Collections.ArrayList();
				}
			}
			catch (DataMapperException ex)
			{
				throw new DataMapperException("Unexpected error while repaginating paged list.  Cause: " + ex.Message, ex);
			}
		}

		private void SafePageTo(int index)
		{
			try
			{
				this.PageTo(index);
			}
			catch (DataMapperException ex)
			{
				throw new DataMapperException("Unexpected error while repaginating paged list.  Cause: " + ex.Message, ex);
			}
		}

		public void PageTo(int index)
		{
			this._index = index;
			System.Collections.IList list;
			if (index < 1)
			{
				list = this.GetList(this._index, this._pageSize * 2);
			}
			else
			{
				list = this.GetList(index - 1, this._pageSize * 3);
			}
			if (list.Count < 1)
			{
				this._prevPageList = new System.Collections.ArrayList();
				this._currentPageList = new System.Collections.ArrayList();
				this._nextPageList = new System.Collections.ArrayList();
			}
			else if (index < 1)
			{
				this._prevPageList = new System.Collections.ArrayList();
				if (list.Count <= this._pageSize)
				{
					this._currentPageList = this.SubList(list, 0, list.Count);
					this._nextPageList = new System.Collections.ArrayList();
				}
				else
				{
					this._currentPageList = this.SubList(list, 0, this._pageSize);
					this._nextPageList = this.SubList(list, this._pageSize, list.Count);
				}
			}
			else if (list.Count <= this._pageSize)
			{
				this._prevPageList = this.SubList(list, 0, list.Count);
				this._currentPageList = new System.Collections.ArrayList();
				this._nextPageList = new System.Collections.ArrayList();
			}
			else if (list.Count <= this._pageSize * 2)
			{
				this._prevPageList = this.SubList(list, 0, this._pageSize);
				this._currentPageList = this.SubList(list, this._pageSize, list.Count);
				this._nextPageList = new System.Collections.ArrayList();
			}
			else
			{
				this._prevPageList = this.SubList(list, 0, this._pageSize);
				this._currentPageList = this.SubList(list, this._pageSize, this._pageSize * 2);
				this._nextPageList = this.SubList(list, this._pageSize * 2, list.Count);
			}
		}

		private System.Collections.IList GetList(int index, int localPageSize)
		{
			bool flag = false;
			ISqlMapSession sqlMapSession = this._mappedStatement.SqlMap.LocalSession;
			if (sqlMapSession == null)
			{
				sqlMapSession = new SqlMapSession(this._mappedStatement.SqlMap);
				sqlMapSession.OpenConnection();
				flag = true;
			}
			System.Collections.IList result = null;
			try
			{
				result = this._mappedStatement.ExecuteQueryForList(sqlMapSession, this._parameterObject, index * this._pageSize, localPageSize);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (flag)
				{
					sqlMapSession.CloseConnection();
				}
			}
			return result;
		}

		private System.Collections.IList SubList(System.Collections.IList list, int fromIndex, int toIndex)
		{
			return ((System.Collections.ArrayList)list).GetRange(fromIndex, toIndex - fromIndex);
		}

		public void GotoPage(int pageIndex)
		{
			this.SafePageTo(pageIndex);
		}

		public bool NextPage()
		{
			bool result;
			if (this.IsNextPageAvailable)
			{
				this._index++;
				this.PageForward();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool PreviousPage()
		{
			bool result;
			if (this.IsPreviousPageAvailable)
			{
				this._index--;
				this.PageBack();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void RemoveAt(int index)
		{
			this._currentPageList.RemoveAt(index);
		}

		public void Insert(int index, object value)
		{
			this._currentPageList.Insert(index, value);
		}

		public void Remove(object value)
		{
			this._currentPageList.Remove(value);
		}

		public bool Contains(object value)
		{
			return this._currentPageList.Contains(value);
		}

		public void Clear()
		{
			this._currentPageList.Clear();
		}

		public int IndexOf(object value)
		{
			return this._currentPageList.IndexOf(value);
		}

		public int Add(object value)
		{
			return this._currentPageList.Add(value);
		}

		public void CopyTo(System.Array array, int index)
		{
			this._currentPageList.CopyTo(array, index);
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return this._currentPageList.GetEnumerator();
		}

		public void Reset()
		{
			this._currentPageList.GetEnumerator().Reset();
		}

		public bool MoveNext()
		{
			return this._currentPageList.GetEnumerator().MoveNext();
		}
	}
}
