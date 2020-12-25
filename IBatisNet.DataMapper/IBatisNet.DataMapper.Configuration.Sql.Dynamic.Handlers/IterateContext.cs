using IBatisNet.DataMapper.Exceptions;
using System;
using System.Collections;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
	public sealed class IterateContext : System.Collections.IEnumerator
	{
		private System.Collections.ICollection _collection;

		private System.Collections.ArrayList _items = new System.Collections.ArrayList();

		private int _index = -1;

		public object Current
		{
			get
			{
				return this._items[this._index];
			}
		}

		public int Index
		{
			get
			{
				return this._index;
			}
		}

		public bool IsFirst
		{
			get
			{
				return this._index == 0;
			}
		}

		public bool IsLast
		{
			get
			{
				return this._index == this._items.Count - 1;
			}
		}

		public bool HasNext
		{
			get
			{
				return this._index >= -1 && this._index < this._items.Count - 1;
			}
		}

		public IterateContext(object collection)
		{
			if (collection is System.Collections.ICollection)
			{
				this._collection = (System.Collections.ICollection)collection;
			}
			else
			{
				if (!collection.GetType().IsArray)
				{
					throw new DataMapperException("ParameterObject or property was not a Collection, Array or Iterator.");
				}
				object[] array = (object[])collection;
				System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					arrayList.Add(array[i]);
				}
				this._collection = arrayList;
			}
			System.Collections.IEnumerable enumerable = (System.Collections.IEnumerable)collection;
			System.Collections.IEnumerator enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				this._items.Add(enumerator.Current);
			}
			System.IDisposable disposable = enumerator as System.IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			this._index = -1;
		}

		public void Reset()
		{
			this._index = -1;
		}

		public bool MoveNext()
		{
			this._index++;
			return this._index != this._items.Count;
		}

		public void Remove()
		{
			if (this._collection is System.Collections.IList)
			{
				((System.Collections.IList)this._collection).Remove(this.Current);
			}
			else if (this._collection is System.Collections.IDictionary)
			{
				((System.Collections.IDictionary)this._collection).Remove(this.Current);
			}
		}
	}
}
