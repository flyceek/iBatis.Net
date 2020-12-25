using System;
using System.Collections;

namespace IBatisNet.DataMapper.Configuration.Cache.Memory
{
	public class MemoryCacheControler : ICacheController
	{
		private class StrongReference
		{
			private object _target = null;

			public object Target
			{
				get
				{
					return this._target;
				}
			}

			public StrongReference(object obj)
			{
				this._target = obj;
			}
		}

		private MemoryCacheLevel _cacheLevel = MemoryCacheLevel.Weak;

		private System.Collections.Hashtable _cache = null;

		public object this[object key]
		{
			get
			{
				object result = null;
				object obj = this._cache[key];
				if (obj != null)
				{
					if (obj is MemoryCacheControler.StrongReference)
					{
						result = ((MemoryCacheControler.StrongReference)obj).Target;
					}
					else if (obj is System.WeakReference)
					{
						result = ((System.WeakReference)obj).Target;
					}
				}
				return result;
			}
			set
			{
				object value2 = null;
				if (this._cacheLevel.Equals(MemoryCacheLevel.Weak))
				{
					value2 = new System.WeakReference(value);
				}
				else if (this._cacheLevel.Equals(MemoryCacheLevel.Strong))
				{
					value2 = new MemoryCacheControler.StrongReference(value);
				}
				this._cache[key] = value2;
			}
		}

		public MemoryCacheControler()
		{
			this._cache = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
		}

		public object Remove(object key)
		{
			object result = null;
			object obj = this[key];
			this._cache.Remove(key);
			if (obj != null)
			{
				if (obj is MemoryCacheControler.StrongReference)
				{
					result = ((MemoryCacheControler.StrongReference)obj).Target;
				}
				else if (obj is System.WeakReference)
				{
					result = ((System.WeakReference)obj).Target;
				}
			}
			return result;
		}

		public void Flush()
		{
			lock (this)
			{
				this._cache.Clear();
			}
		}

		public void Configure(System.Collections.IDictionary properties)
		{
			string text = (string)properties["Type"];
			if (text != null)
			{
				this._cacheLevel = MemoryCacheLevel.GetByRefenceType(text.ToUpper());
			}
		}
	}
}
