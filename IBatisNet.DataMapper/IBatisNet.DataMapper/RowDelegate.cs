using System;
using System.Collections;
using System.Collections.Generic;

namespace IBatisNet.DataMapper
{
	public delegate void RowDelegate(object obj, object parameterObject, System.Collections.IList list);
	public delegate void RowDelegate<T>(object obj, object parameterObject, System.Collections.Generic.IList<T> list);
}
