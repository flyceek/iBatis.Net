using IBatisNet.Common;
using IBatisNet.Common.Logging;
using IBatisNet.DataMapper.Exceptions;
using System;
using System.Data;
using System.Reflection;

namespace IBatisNet.DataMapper
{
	[System.Serializable]
	public class SqlMapSession : ISqlMapSession, IDalSession, System.IDisposable
	{
		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private ISqlMapper _sqlMapper = null;

		private IDataSource _dataSource = null;

		private bool _isTransactionOpen = false;

		private bool _consistent = false;

		private IDbConnection _connection = null;

		private IDbTransaction _transaction = null;

		public ISqlMapper SqlMapper
		{
			get
			{
				return this._sqlMapper;
			}
		}

		public IDataSource DataSource
		{
			get
			{
				return this._dataSource;
			}
		}

		public IDbConnection Connection
		{
			get
			{
				return this._connection;
			}
		}

		public IDbTransaction Transaction
		{
			get
			{
				return this._transaction;
			}
		}

		public bool IsTransactionStart
		{
			get
			{
				return this._isTransactionOpen;
			}
		}

		private bool Consistent
		{
			set
			{
				this._consistent = value;
			}
		}

		public SqlMapSession(ISqlMapper sqlMapper)
		{
			this._dataSource = sqlMapper.DataSource;
			this._sqlMapper = sqlMapper;
		}

		public void Complete()
		{
			this.Consistent = true;
		}

		public void OpenConnection()
		{
			this.OpenConnection(this._dataSource.ConnectionString);
		}

		public void CreateConnection()
		{
			this.CreateConnection(this._dataSource.ConnectionString);
		}

		public void CreateConnection(string connectionString)
		{
			this._connection = this._dataSource.DbProvider.CreateConnection();
			this._connection.ConnectionString = connectionString;
		}

		public void OpenConnection(string connectionString)
		{
			if (this._connection == null)
			{
				this.CreateConnection(connectionString);
				try
				{
					this._connection.Open();
					if (SqlMapSession._logger.IsDebugEnabled)
					{
						SqlMapSession._logger.Debug(string.Format("Open Connection \"{0}\" to \"{1}\".", this._connection.GetHashCode().ToString(), this._dataSource.DbProvider.Description));
					}
				}
				catch (System.Exception inner)
				{
					throw new DataMapperException(string.Format("Unable to open connection to \"{0}\".", this._dataSource.DbProvider.Description), inner);
				}
			}
			else if (this._connection.State != ConnectionState.Open)
			{
				try
				{
					this._connection.Open();
					if (SqlMapSession._logger.IsDebugEnabled)
					{
						SqlMapSession._logger.Debug(string.Format("Open Connection \"{0}\" to \"{1}\".", this._connection.GetHashCode().ToString(), this._dataSource.DbProvider.Description));
					}
				}
				catch (System.Exception inner)
				{
					throw new DataMapperException(string.Format("Unable to open connection to \"{0}\".", this._dataSource.DbProvider.Description), inner);
				}
			}
		}

		public void CloseConnection()
		{
			if (this._connection != null && this._connection.State != ConnectionState.Closed)
			{
				this._connection.Close();
				if (SqlMapSession._logger.IsDebugEnabled)
				{
					SqlMapSession._logger.Debug(string.Format("Close Connection \"{0}\" to \"{1}\".", this._connection.GetHashCode().ToString(), this._dataSource.DbProvider.Description));
				}
				this._connection.Dispose();
			}
			this._connection = null;
		}

		public void BeginTransaction()
		{
			this.BeginTransaction(this._dataSource.ConnectionString);
		}

		public void BeginTransaction(string connectionString)
		{
			if (this._connection == null || this._connection.State != ConnectionState.Open)
			{
				this.OpenConnection(connectionString);
			}
			this._transaction = this._connection.BeginTransaction();
			if (SqlMapSession._logger.IsDebugEnabled)
			{
				SqlMapSession._logger.Debug("Begin Transaction.");
			}
			this._isTransactionOpen = true;
		}

		public void BeginTransaction(bool openConnection)
		{
			if (openConnection)
			{
				this.BeginTransaction();
			}
			else
			{
				if (this._connection == null || this._connection.State != ConnectionState.Open)
				{
					this.OpenConnection();
				}
				this._transaction = this._connection.BeginTransaction();
				if (SqlMapSession._logger.IsDebugEnabled)
				{
					SqlMapSession._logger.Debug("Begin Transaction.");
				}
				this._isTransactionOpen = true;
			}
		}

		public void BeginTransaction(IsolationLevel isolationLevel)
		{
			this.BeginTransaction(this._dataSource.ConnectionString, isolationLevel);
		}

		public void BeginTransaction(string connectionString, IsolationLevel isolationLevel)
		{
			if (this._connection == null || this._connection.State != ConnectionState.Open)
			{
				this.OpenConnection(connectionString);
			}
			this._transaction = this._connection.BeginTransaction(isolationLevel);
			if (SqlMapSession._logger.IsDebugEnabled)
			{
				SqlMapSession._logger.Debug("Begin Transaction.");
			}
			this._isTransactionOpen = true;
		}

		public void BeginTransaction(bool openConnection, IsolationLevel isolationLevel)
		{
			this.BeginTransaction(this._dataSource.ConnectionString, openConnection, isolationLevel);
		}

		public void BeginTransaction(string connectionString, bool openConnection, IsolationLevel isolationLevel)
		{
			if (openConnection)
			{
				this.BeginTransaction(connectionString, isolationLevel);
			}
			else
			{
				if (this._connection == null || this._connection.State != ConnectionState.Open)
				{
					throw new DataMapperException("SqlMapSession could not invoke StartTransaction(). A Connection must be started. Call OpenConnection() first.");
				}
				this._transaction = this._connection.BeginTransaction(isolationLevel);
				if (SqlMapSession._logger.IsDebugEnabled)
				{
					SqlMapSession._logger.Debug("Begin Transaction.");
				}
				this._isTransactionOpen = true;
			}
		}

		public void CommitTransaction()
		{
			if (SqlMapSession._logger.IsDebugEnabled)
			{
				SqlMapSession._logger.Debug("Commit Transaction.");
			}
			this._transaction.Commit();
			this._transaction.Dispose();
			this._transaction = null;
			this._isTransactionOpen = false;
			if (this._connection.State != ConnectionState.Closed)
			{
				this.CloseConnection();
			}
		}

		public void CommitTransaction(bool closeConnection)
		{
			if (closeConnection)
			{
				this.CommitTransaction();
			}
			else
			{
				if (SqlMapSession._logger.IsDebugEnabled)
				{
					SqlMapSession._logger.Debug("Commit Transaction.");
				}
				this._transaction.Commit();
				this._transaction.Dispose();
				this._transaction = null;
				this._isTransactionOpen = false;
			}
		}

		public void RollBackTransaction()
		{
			if (SqlMapSession._logger.IsDebugEnabled)
			{
				SqlMapSession._logger.Debug("RollBack Transaction.");
			}
			this._transaction.Rollback();
			this._transaction.Dispose();
			this._transaction = null;
			this._isTransactionOpen = false;
			if (this._connection.State != ConnectionState.Closed)
			{
				this.CloseConnection();
			}
		}

		public void RollBackTransaction(bool closeConnection)
		{
			if (closeConnection)
			{
				this.RollBackTransaction();
			}
			else
			{
				if (SqlMapSession._logger.IsDebugEnabled)
				{
					SqlMapSession._logger.Debug("RollBack Transaction.");
				}
				this._transaction.Rollback();
				this._transaction.Dispose();
				this._transaction = null;
				this._isTransactionOpen = false;
			}
		}

		public IDbCommand CreateCommand(CommandType commandType)
		{
			IDbCommand dbCommand = this._dataSource.DbProvider.CreateCommand();
			dbCommand.CommandType = commandType;
			dbCommand.Connection = this._connection;
			if (this._transaction != null)
			{
				try
				{
					dbCommand.Transaction = this._transaction;
				}
				catch
				{
				}
			}
			if (this._connection != null)
			{
				try
				{
					dbCommand.CommandTimeout = this._connection.ConnectionTimeout;
				}
				catch (System.NotSupportedException ex)
				{
					if (SqlMapSession._logger.IsInfoEnabled)
					{
						SqlMapSession._logger.Info(ex.Message);
					}
				}
			}
			return dbCommand;
		}

		public IDbDataParameter CreateDataParameter()
		{
			return this._dataSource.DbProvider.CreateDataParameter();
		}

		public IDbDataAdapter CreateDataAdapter()
		{
			return this._dataSource.DbProvider.CreateDataAdapter();
		}

		public IDbDataAdapter CreateDataAdapter(IDbCommand command)
		{
			IDbDataAdapter dbDataAdapter = this._dataSource.DbProvider.CreateDataAdapter();
			dbDataAdapter.SelectCommand = command;
			return dbDataAdapter;
		}

		public void Dispose()
		{
			if (SqlMapSession._logger.IsDebugEnabled)
			{
				SqlMapSession._logger.Debug("Dispose SqlMapSession");
			}
			if (!this._isTransactionOpen)
			{
				if (this._connection.State != ConnectionState.Closed)
				{
					this._sqlMapper.CloseConnection();
				}
			}
			else if (this._consistent)
			{
				this._sqlMapper.CommitTransaction();
				this._isTransactionOpen = false;
			}
			else if (this._connection.State != ConnectionState.Closed)
			{
				this._sqlMapper.RollBackTransaction();
				this._isTransactionOpen = false;
			}
		}
	}
}
