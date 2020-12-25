using System;
using System.Collections;
using System.Collections.Generic;

namespace IBatisNet.DataMapper
{
	public delegate void DictionaryRowDelegate<K, V>(K key, V value, object parameterObject, System.Collections.Generic.IDictionary<K, V> dictionary);
	public delegate void DictionaryRowDelegate(object key, object value, object parameterObject, System.Collections.IDictionary dictionary);
}
