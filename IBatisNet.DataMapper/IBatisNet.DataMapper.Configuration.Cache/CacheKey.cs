using IBatisNet.Common.Utilities;
using System;
using System.Collections;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Cache
{
	public class CacheKey
	{
		private const int DEFAULT_MULTIPLYER = 37;

		private const int DEFAULT_HASHCODE = 17;

		private int _multiplier = 37;

		private int _hashCode = 17;

		private long _checksum = -9223372036854775808L;

		private int _count = 0;

		private System.Collections.IList _paramList = new System.Collections.ArrayList();

		public CacheKey()
		{
			this._hashCode = 17;
			this._multiplier = 37;
			this._count = 0;
		}

		public CacheKey(int initialNonZeroOddNumber)
		{
			this._hashCode = initialNonZeroOddNumber;
			this._multiplier = 37;
			this._count = 0;
		}

		public CacheKey(int initialNonZeroOddNumber, int multiplierNonZeroOddNumber)
		{
			this._hashCode = initialNonZeroOddNumber;
			this._multiplier = multiplierNonZeroOddNumber;
			this._count = 0;
		}

		public CacheKey Update(object obj)
		{
			int num = HashCodeProvider.GetIdentityHashCode(obj);
			this._count++;
			this._checksum += (long)num;
			num *= this._count;
			this._hashCode = this._multiplier * this._hashCode + num;
			this._paramList.Add(obj);
			return this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (this == obj)
			{
				result = true;
			}
			else if (!(obj is CacheKey))
			{
				result = false;
			}
			else
			{
				CacheKey cacheKey = (CacheKey)obj;
				if (this._hashCode != cacheKey._hashCode)
				{
					result = false;
				}
				else if (this._checksum != cacheKey._checksum)
				{
					result = false;
				}
				else if (this._count != cacheKey._count)
				{
					result = false;
				}
				else
				{
					int count = this._paramList.Count;
					for (int i = 0; i < count; i++)
					{
						object obj2 = this._paramList[i];
						object obj3 = cacheKey._paramList[i];
						if (obj2 == null)
						{
							if (obj3 != null)
							{
								result = false;
								return result;
							}
						}
						else if (!obj2.Equals(obj3))
						{
							result = false;
							return result;
						}
					}
					result = true;
				}
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this._hashCode;
		}

		public override string ToString()
		{
			return new System.Text.StringBuilder().Append(this._hashCode).Append('|').Append(this._checksum).ToString();
		}
	}
}
