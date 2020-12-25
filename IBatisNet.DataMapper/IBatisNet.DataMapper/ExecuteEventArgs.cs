using System;

namespace IBatisNet.DataMapper
{
	public class ExecuteEventArgs : System.EventArgs
	{
		private string _statementName = string.Empty;

		public string StatementName
		{
			get
			{
				return this._statementName;
			}
			set
			{
				this._statementName = value;
			}
		}
	}
}
