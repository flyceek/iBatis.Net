using System;
using System.Collections.Specialized;
using System.Data;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	public class PreparedStatement
	{
		private string _preparedSsql = string.Empty;

		private StringCollection _dbParametersName = new StringCollection();

		private IDbDataParameter[] _dbParameters = null;

		public StringCollection DbParametersName
		{
			get
			{
				return this._dbParametersName;
			}
		}

		public IDbDataParameter[] DbParameters
		{
			get
			{
				return this._dbParameters;
			}
			set
			{
				this._dbParameters = value;
			}
		}

		public string PreparedSql
		{
			get
			{
				return this._preparedSsql;
			}
			set
			{
				this._preparedSsql = value;
			}
		}
	}
}
