using System.Collections;

namespace IBatisNet.Common.Utilities
{
	/// <summary>
	/// A StringTokenizer java like object 
	/// </summary>
	public class StringTokenizer : IEnumerable
	{
		private class StringTokenizerEnumerator : IEnumerator
		{
			private StringTokenizer _stokenizer;

			private int _cursor = 0;

			private string _next = null;

			public object Current => _next;

			public StringTokenizerEnumerator(StringTokenizer stok)
			{
				_stokenizer = stok;
			}

			public bool MoveNext()
			{
				_next = GetNext();
				return _next != null;
			}

			public void Reset()
			{
				_cursor = 0;
			}

			private string GetNext()
			{
				if (_cursor >= _stokenizer._origin.Length)
				{
					return null;
				}
				char value = _stokenizer._origin[_cursor];
				if (_stokenizer._delimiters.IndexOf(value) != -1)
				{
					_cursor++;
					if (_stokenizer._returnDelimiters)
					{
						return value.ToString();
					}
					return GetNext();
				}
				int num = _stokenizer._origin.IndexOfAny(_stokenizer._delimiters.ToCharArray(), _cursor);
				if (num == -1)
				{
					num = _stokenizer._origin.Length;
				}
				string result = _stokenizer._origin.Substring(_cursor, num - _cursor);
				_cursor = num;
				return result;
			}
		}

		private static readonly string _defaultDelim = " \t\n\r\f";

		private string _origin = string.Empty;

		private string _delimiters = string.Empty;

		private bool _returnDelimiters = false;

		/// <summary>
		/// Returns the number of tokens in the String using
		/// the current deliminter set.  This is the number of times
		/// nextToken() can return before it will generate an exception.
		/// Use of this routine to count the number of tokens is faster
		/// than repeatedly calling nextToken() because the substrings
		/// are not constructed and returned for each token.
		/// </summary>
		public int TokenNumber
		{
			get
			{
				int num = 0;
				int i = 0;
				int length = _origin.Length;
				while (i < length)
				{
					while (!_returnDelimiters && i < length && _delimiters.IndexOf(_origin[i]) >= 0)
					{
						i++;
					}
					if (i >= length)
					{
						break;
					}
					int num2 = i;
					for (; i < length && _delimiters.IndexOf(_origin[i]) < 0; i++)
					{
					}
					if (_returnDelimiters && num2 == i && _delimiters.IndexOf(_origin[i]) >= 0)
					{
						i++;
					}
					num++;
				}
				return num;
			}
		}

		/// <summary>
		/// Constructs a StringTokenizer on the specified String, using the
		/// default delimiter set (which is " \t\n\r\f").
		/// </summary>
		/// <param name="str">The input String</param>
		public StringTokenizer(string str)
		{
			_origin = str;
			_delimiters = _defaultDelim;
			_returnDelimiters = false;
		}

		/// <summary>
		/// Constructs a StringTokenizer on the specified String, 
		/// using the specified delimiter set.
		/// </summary>
		/// <param name="str">The input String</param>
		/// <param name="delimiters">The delimiter String</param>
		public StringTokenizer(string str, string delimiters)
		{
			_origin = str;
			_delimiters = delimiters;
			_returnDelimiters = false;
		}

		/// <summary>
		/// Constructs a StringTokenizer on the specified String, 
		/// using the specified delimiter set.
		/// </summary>
		/// <param name="str">The input String</param>
		/// <param name="delimiters">The delimiter String</param>
		/// <param name="returnDelimiters">Returns delimiters as tokens or skip them</param>
		public StringTokenizer(string str, string delimiters, bool returnDelimiters)
		{
			_origin = str;
			_delimiters = delimiters;
			_returnDelimiters = returnDelimiters;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return new StringTokenizerEnumerator(this);
		}
	}
}
