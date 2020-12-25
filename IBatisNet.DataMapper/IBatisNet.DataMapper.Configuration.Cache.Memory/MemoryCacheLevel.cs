using IBatisNet.DataMapper.Exceptions;
using System;
using System.Collections;

namespace IBatisNet.DataMapper.Configuration.Cache.Memory
{
	public class MemoryCacheLevel
	{
		private static System.Collections.Hashtable _cacheLevelMap;

		public static MemoryCacheLevel Weak;

		public static MemoryCacheLevel Strong;

		private string _referenceType;

		public string ReferenceType
		{
			get
			{
				return this._referenceType;
			}
		}

		static MemoryCacheLevel()
		{
			MemoryCacheLevel._cacheLevelMap = new System.Collections.Hashtable();
			MemoryCacheLevel.Weak = new MemoryCacheLevel("WEAK");
			MemoryCacheLevel.Strong = new MemoryCacheLevel("STRONG");
			MemoryCacheLevel._cacheLevelMap[MemoryCacheLevel.Weak.ReferenceType] = MemoryCacheLevel.Weak;
			MemoryCacheLevel._cacheLevelMap[MemoryCacheLevel.Strong.ReferenceType] = MemoryCacheLevel.Strong;
		}

		private MemoryCacheLevel(string type)
		{
			this._referenceType = type;
		}

		public static MemoryCacheLevel GetByRefenceType(string referenceType)
		{
			MemoryCacheLevel memoryCacheLevel = (MemoryCacheLevel)MemoryCacheLevel._cacheLevelMap[referenceType];
			if (memoryCacheLevel == null)
			{
				throw new DataMapperException("Error getting CacheLevel (reference type) for name: '" + referenceType + "'.");
			}
			return memoryCacheLevel;
		}
	}
}
