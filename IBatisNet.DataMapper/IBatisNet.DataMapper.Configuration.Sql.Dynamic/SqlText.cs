using IBatisNet.DataMapper.Configuration.ParameterMapping;
using System;

namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic
{
	public sealed class SqlText : ISqlChild
	{
		private string _text = string.Empty;

		private bool _isWhiteSpace = false;

		private ParameterProperty[] _parameters = null;

		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
				this._isWhiteSpace = (this._text.Trim().Length == 0);
			}
		}

		public bool IsWhiteSpace
		{
			get
			{
				return this._isWhiteSpace;
			}
		}

		public ParameterProperty[] Parameters
		{
			get
			{
				return this._parameters;
			}
			set
			{
				this._parameters = value;
			}
		}
	}
}
