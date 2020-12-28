using System;
using System.Collections;

namespace IBatisNet.Common.Pagination
{
	/// <summary>
	/// Summary description for PaginatedArrayList.
	/// </summary>
	public class PaginatedArrayList : IPaginatedList, IList, ICollection, IEnumerable, IEnumerator
	{
		private static ArrayList _emptyList = new ArrayList();

		private int _pageSize = 0;

		private int _pageIndex = 0;

		private IList _list = null;

		private IList _page = null;

		/// <summary>
		///
		/// </summary>
		public bool IsEmpty => _page.Count == 0;

		/// <summary>
		///
		/// </summary>
		public int PageSize => _pageSize;

		/// <summary>
		///
		/// </summary>
		public bool IsFirstPage => _pageIndex == 0;

		/// <summary>
		///
		/// </summary>
		public bool IsMiddlePage => !IsFirstPage && !IsLastPage;

		/// <summary>
		///
		/// </summary>
		public bool IsLastPage => _list.Count - (_pageIndex + 1) * _pageSize < 1;

		/// <summary>
		///
		/// </summary>
		public bool IsNextPageAvailable => !IsLastPage;

		/// <summary>
		///
		/// </summary>
		public bool IsPreviousPageAvailable => !IsFirstPage;

		/// <summary>
		///
		/// </summary>
		public int PageIndex => _pageIndex;

		/// <summary>
		///
		/// </summary>
		public bool IsReadOnly => _list.IsReadOnly;

		/// <summary>
		///
		/// </summary>
		public object this[int index]
		{
			get
			{
				return _page[index];
			}
			set
			{
				_list[index] = value;
				Repaginate();
			}
		}

		/// <summary>
		///
		/// </summary>
		public bool IsFixedSize => _list.IsFixedSize;

		/// <summary>
		///
		/// </summary>
		public bool IsSynchronized => _page.IsSynchronized;

		/// <summary>
		///
		/// </summary>
		public int Count => _page.Count;

		/// <summary>
		///
		/// </summary>
		public object SyncRoot => _page.SyncRoot;

		/// <summary>
		/// Gets the current element in the page.
		/// </summary>
		public object Current => _page.GetEnumerator().Current;

		/// <summary>
		///
		/// </summary>
		/// <param name="pageSize"></param>
		public PaginatedArrayList(int pageSize)
		{
			_pageSize = pageSize;
			_pageIndex = 0;
			_list = new ArrayList();
			Repaginate();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="initialCapacity"></param>
		/// <param name="pageSize"></param>
		public PaginatedArrayList(int initialCapacity, int pageSize)
		{
			_pageSize = pageSize;
			_pageIndex = 0;
			_list = new ArrayList(initialCapacity);
			Repaginate();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="c"></param>
		/// <param name="pageSize"></param>
		public PaginatedArrayList(ICollection c, int pageSize)
		{
			_pageSize = pageSize;
			_pageIndex = 0;
			_list = new ArrayList(c);
			Repaginate();
		}

		/// <summary>
		///
		/// </summary>
		private void Repaginate()
		{
			if (_list.Count == 0)
			{
				_page = _emptyList;
				return;
			}
			int num = _pageIndex * _pageSize;
			int num2 = num + _pageSize - 1;
			if (num2 >= _list.Count)
			{
				num2 = _list.Count - 1;
			}
			if (num >= _list.Count)
			{
				_pageIndex = 0;
				Repaginate();
			}
			else if (num < 0)
			{
				_pageIndex = _list.Count / _pageSize;
				if (_list.Count % _pageSize == 0)
				{
					_pageIndex--;
				}
				Repaginate();
			}
			else
			{
				_page = SubList(_list, num, num2 + 1);
			}
		}

		/// <summary>
		/// Provides a view of the IList pramaeter 
		/// from the specified position <paramref name="fromIndex" /> 
		/// to the specified position <paramref name="toIndex" />. 
		/// </summary>
		/// <param name="list">The IList elements.</param>
		/// <param name="fromIndex">Starting position for the view of elements. </param>
		/// <param name="toIndex">Ending position for the view of elements. </param>
		/// <returns> A view of list.
		/// </returns>
		/// <remarks>
		/// The list that is returned is just a view, it is still backed
		/// by the orignal list.  Any changes you make to it will be 
		/// reflected in the orignal list.
		/// </remarks>
		private IList SubList(IList list, int fromIndex, int toIndex)
		{
			return ((ArrayList)list).GetRange(fromIndex, toIndex - fromIndex);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool NextPage()
		{
			if (IsNextPageAvailable)
			{
				_pageIndex++;
				Repaginate();
				return true;
			}
			return false;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool PreviousPage()
		{
			if (IsPreviousPageAvailable)
			{
				_pageIndex--;
				Repaginate();
				return true;
			}
			return false;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="pageIndex"></param>
		public void GotoPage(int pageIndex)
		{
			_pageIndex = pageIndex;
			Repaginate();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
			Repaginate();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void Insert(int index, object value)
		{
			_list.Insert(index, value);
			Repaginate();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		public void Remove(object value)
		{
			_list.Remove(value);
			Repaginate();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(object value)
		{
			return _page.Contains(value);
		}

		/// <summary>
		///
		/// </summary>
		public void Clear()
		{
			_list.Clear();
			Repaginate();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(object value)
		{
			return _page.IndexOf(value);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(object value)
		{
			int result = _list.Add(value);
			Repaginate();
			return result;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			_page.CopyTo(array, index);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _page.GetEnumerator();
		}

		/// <summary>
		/// Sets the enumerator to its initial position, 
		/// which is before the first element in the collection.
		/// </summary>
		public void Reset()
		{
			_page.GetEnumerator().Reset();
		}

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; 
		/// false if the enumerator has passed the end of the collection.
		/// </returns>
		public bool MoveNext()
		{
			return _page.GetEnumerator().MoveNext();
		}
	}
}
