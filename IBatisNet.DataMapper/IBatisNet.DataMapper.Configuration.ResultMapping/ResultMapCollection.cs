using System;

namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
	public class ResultMapCollection
	{
		private const int DEFAULT_CAPACITY = 2;

		private const int CAPACITY_MULTIPLIER = 2;

		private int _count = 0;

		private IResultMap[] _innerList = null;

		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public int Length
		{
			get
			{
				return this._innerList.Length;
			}
		}

		public IResultMap this[int index]
		{
			get
			{
				if (index < 0 || index >= this._count)
				{
					throw new System.ArgumentOutOfRangeException("index");
				}
				return this._innerList[index];
			}
			set
			{
				if (index < 0 || index >= this._count)
				{
					throw new System.ArgumentOutOfRangeException("index");
				}
				this._innerList[index] = value;
			}
		}

		public ResultMapCollection()
		{
			this.Clear();
		}

		public void Clear()
		{
			this._innerList = new IResultMap[2];
			this._count = 0;
		}

		public ResultMapCollection(int capacity)
		{
			if (capacity < 0)
			{
				throw new System.ArgumentOutOfRangeException("Capacity", "The size of the list must be >0.");
			}
			this._innerList = new IResultMap[capacity];
		}

		public int Add(IResultMap value)
		{
			this.Resize(this._count + 1);
			int num = this._count++;
			this._innerList[num] = value;
			return num;
		}

		public void AddRange(IResultMap[] value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				this.Add(value[i]);
			}
		}

		public void AddRange(ResultMapCollection value)
		{
			for (int i = 0; i < value.Count; i++)
			{
				this.Add(value[i]);
			}
		}

		public bool Contains(IResultMap value)
		{
			bool result;
			for (int i = 0; i < this._count; i++)
			{
				if (this._innerList[i].Id == value.Id)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public void Insert(int index, IResultMap value)
		{
			if (index < 0 || index > this._count)
			{
				throw new System.ArgumentOutOfRangeException("index");
			}
			this.Resize(this._count + 1);
			System.Array.Copy(this._innerList, index, this._innerList, index + 1, this._count - index);
			this._innerList[index] = value;
			this._count++;
		}

		public void Remove(IResultMap value)
		{
			for (int i = 0; i < this._count; i++)
			{
				if (this._innerList[i].Id == value.Id)
				{
					this.RemoveAt(i);
					break;
				}
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this._count)
			{
				throw new System.ArgumentOutOfRangeException("index");
			}
			int num = this._count - index - 1;
			if (num > 0)
			{
				System.Array.Copy(this._innerList, index + 1, this._innerList, index, num);
			}
			this._count--;
			this._innerList[this._count] = null;
		}

		private void Resize(int minSize)
		{
			int num = this._innerList.Length;
			if (minSize > num)
			{
				IResultMap[] innerList = this._innerList;
				int num2 = innerList.Length * 2;
				if (num2 < minSize)
				{
					num2 = minSize;
				}
				this._innerList = new IResultMap[num2];
				System.Array.Copy(innerList, 0, this._innerList, 0, this._count);
			}
		}
	}
}
