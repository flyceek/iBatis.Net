using System;
using System.Collections;

namespace IBatisNet.DataMapper.Configuration.Cache.Lru
{
	public class LruCacheController : ICacheController
	{
		private int _cacheSize = 0;

		private System.Collections.Hashtable _cache = null;

		private System.Collections.IList _keyList = null;

		public object this[object key]
		{
			get
			{
				this._keyList.Remove(key);
				this._keyList.Add(key);
				return this._cache[key];
			}
			set
			{
				this._cache[key] = value;
				this._keyList.Add(key);
				if (this._keyList.Count > this._cacheSize)
				{
					object key2 = this._keyList[0];
					this._keyList.Remove(0);
					this._cache.Remove(key2);
				}
			}
		}

		public LruCacheController()
		{
			this._cacheSize = 100;
			this._cache = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
			this._keyList = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
		}

		public object Remove(object key)
		{
			object result = this[key];
			this._keyList.Remove(key);
			this._cache.Remove(key);
			return result;
		}

		public void Flush()
		{
			this._cache.Clear();
			this._keyList.Clear();
		}

		public void Configure(System.Collections.IDictionary properties)
		{
			string text = (string)properties["CacheSize"];
			if (text != null)
			{
				this._cacheSize = System.Convert.ToInt32(text);
			}
		}
	}
}
