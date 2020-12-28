using System;
using System.EnterpriseServices;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using IBatisNet.Common.Logging;

namespace IBatisNet.Common.Transaction
{
	/// <summary>
	/// Simple interface to COM+ transactions through Enterprise Service. 
	/// Makes a code block transactional ?la Indigo (evolution will be easier, it's the same API)
	/// It's important to make sure that each instance 
	/// of this class gets Close()'d. 
	/// Easiest way to do that is with the using statement in C#.
	/// </summary>
	/// <remarks>
	/// Don't support nested transaction scope with different transaction options.
	///
	/// System.EnterpriseServices.ServiceDomain is available only on 
	/// - XP SP2 (or higher) 
	/// - Windows Server 2003 
	/// - XP SP1 + Hotfix 828741 
	/// and only in .Net 1.1.
	/// It CAN'T be used on Windows 2000.
	///
	/// http://support.microsoft.com/default.aspx/kb/319177/EN-US/
	/// </remarks>
	/// <example>
	/// using (TransactionScope tx = new TransactionScope())
	///             	{
	///
	///             		// Open connection to database 1	
	///             		// Transaction will be automatically enlist into it
	///             		// Execute update in database 1
	///             		// Open connection to database 2
	///             		// Transaction will be automatically enlist into it
	///             		// Execute update in database 2
	///
	///             		// the following code will be executed only if no exception
	///             		// occured in the above code; since we got here ok, let's vote for commit;
	///             		tx.Completed(); 
	///             	}
	/// when 搖sing?call Dispose on the transaction scope at the end
	/// of the 搖sing?code block, the "ambient" transaction will be commited only and only if
	/// the Completed method have been called.
	/// </example>
	public class TransactionScope : IDisposable
	{
		private const string TX_SCOPE_COUNT = "_TX_SCOPE_COUNT_";

		private bool _consistent = false;

		private bool _closed = false;

		private TransactionScopeOptions _txScopeOptions;

		private TransactionOptions _txOptions;

		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Changes the vote to commit (true) or to abort (false).
		/// If all the TransactionScope instances involved in a 
		/// transaction have voted to commit, then the entire thing is committed.
		/// If any TransactionScope instances involved in a 
		/// transaction vote to abort, then the entire thing is aborted.
		/// </summary>
		private bool Consistent
		{
			set
			{
				_consistent = value;
			}
		}

		/// <summary>
		/// Count of the TransactionScope that have been open.
		/// </summary>
		public int TransactionScopeCount
		{
			get
			{
				object data = CallContext.GetData("_TX_SCOPE_COUNT_");
				return (data != null) ? ((int)data) : 0;
			}
			set
			{
				CallContext.SetData("_TX_SCOPE_COUNT_", value);
			}
		}

		/// <summary>
		/// Returns whether or not the current thread is in a transaction context.
		/// </summary>
		public static bool IsInTransaction => ContextUtil.IsInTransaction;

		/// <summary>
		/// Gets the current value of the vote.
		/// </summary>
		public bool IsVoteCommit => ContextUtil.MyTransactionVote == TransactionVote.Commit;

		/// <summary>
		/// Creates a new instance with a TransactionScopeOptions.Required 
		/// and TransactionOptions.IsolationLevel.ReadCommitted.
		/// </summary>
		public TransactionScope()
		{
			_txOptions = default(TransactionOptions);
			_txOptions.IsolationLevel = IsolationLevel.ReadCommitted;
			_txOptions.TimeOut = new TimeSpan(0, 0, 0, 15);
			_txScopeOptions = TransactionScopeOptions.Required;
			EnterTransactionContext();
		}

		/// <summary>
		/// Creates a new instance with the specified TransactionScopeOptions
		///  and TransactionOptions.IsolationLevel.ReadCommitted.
		/// </summary>
		/// <param name="txScopeOptions">The specified TransactionScopeOptions</param>
		public TransactionScope(TransactionScopeOptions txScopeOptions)
		{
			_txOptions = default(TransactionOptions);
			_txOptions.IsolationLevel = IsolationLevel.ReadCommitted;
			_txOptions.TimeOut = new TimeSpan(0, 0, 0, 15);
			_txScopeOptions = txScopeOptions;
			EnterTransactionContext();
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="txScopeOptions">The specified TransactionScopeOptions.</param>
		/// <param name="options">The specified TransactionOptions.</param>
		public TransactionScope(TransactionScopeOptions txScopeOptions, TransactionOptions options)
		{
			_txOptions = options;
			_txScopeOptions = txScopeOptions;
			EnterTransactionContext();
		}

		/// <summary>
		///
		/// </summary>
		private void EnterTransactionContext()
		{
			if (++TransactionScopeCount == 1)
			{
				if (_logger.IsDebugEnabled)
				{
					_logger.Debug("Create a new ServiceConfig in ServiceDomain.");
				}
				ServiceConfig serviceConfig = new ServiceConfig();
				serviceConfig.TrackingEnabled = true;
				serviceConfig.TrackingAppName = "iBATIS.NET";
				serviceConfig.TrackingComponentName = "TransactionScope";
				serviceConfig.TransactionDescription = "iBATIS.NET Distributed Transaction";
				serviceConfig.Transaction = TransactionScopeOptions2TransactionOption(_txScopeOptions);
				serviceConfig.TransactionTimeout = _txOptions.TimeOut.Seconds;
				serviceConfig.IsolationLevel = IsolationLevel2TransactionIsolationLevel(_txOptions.IsolationLevel);
				ServiceDomain.Enter(serviceConfig);
			}
			_closed = false;
			if (_logger.IsDebugEnabled)
			{
				_logger.Debug("Open TransactionScope :" + ContextUtil.ContextId);
			}
		}

		/// <summary>
		/// Give the correpondance of a TransactionScopeOptions (?la Indigo) object in a TransactionOption (COM+) object
		/// </summary>
		/// <param name="transactionScopeOptions">The TransactionScopeOptions to macth.</param>
		/// <returns>The TransactionOption correspondance</returns>
		private TransactionOption TransactionScopeOptions2TransactionOption(TransactionScopeOptions transactionScopeOptions)
		{
			return transactionScopeOptions switch
			{
				TransactionScopeOptions.Mandatory => throw new NotImplementedException("Will be used in Indigo."), 
				TransactionScopeOptions.NotSupported => TransactionOption.NotSupported, 
				TransactionScopeOptions.Required => TransactionOption.Required, 
				TransactionScopeOptions.RequiresNew => TransactionOption.RequiresNew, 
				TransactionScopeOptions.Supported => TransactionOption.Supported, 
				_ => TransactionOption.Required, 
			};
		}

		/// <summary>
		/// Give the correpondance of a TransactionIsolationLevel (?la Indigo) object in a IsolationLevel (COM+) object
		/// </summary>
		/// <param name="isolation">The IsolationLevel to macth.</param>
		/// <returns>The TransactionIsolationLevel correspondance</returns>
		private TransactionIsolationLevel IsolationLevel2TransactionIsolationLevel(IsolationLevel isolation)
		{
			return isolation switch
			{
				IsolationLevel.ReadCommitted => TransactionIsolationLevel.ReadCommitted, 
				IsolationLevel.ReadUncommitted => TransactionIsolationLevel.ReadUncommitted, 
				IsolationLevel.RepeatableRead => TransactionIsolationLevel.RepeatableRead, 
				IsolationLevel.Serializable => TransactionIsolationLevel.Serializable, 
				IsolationLevel.Unspecified => throw new NotImplementedException("Will be used in Indigo."), 
				_ => TransactionIsolationLevel.ReadCommitted, 
			};
		}

		/// <summary>
		/// Close the TransactionScope
		/// </summary>
		public void Close()
		{
			if (_closed)
			{
				return;
			}
			if (_logger.IsDebugEnabled)
			{
				_logger.Debug("Close TransactionScope");
			}
			if (ContextUtil.IsInTransaction)
			{
				if (_consistent && IsVoteCommit)
				{
					ContextUtil.EnableCommit();
				}
				else
				{
					ContextUtil.DisableCommit();
				}
			}
			if (0 == --TransactionScopeCount)
			{
				if (_logger.IsDebugEnabled)
				{
					_logger.Debug("Leave in ServiceDomain ");
				}
				ServiceDomain.Leave();
			}
			_closed = true;
		}

		/// <summary>
		/// Complete (commit) a transsaction
		/// </summary>
		public void Complete()
		{
			Consistent = true;
		}

		/// <summary>
		/// Implementation of IDisposable so that this class 
		/// can be used with C#'s using statement.
		/// </summary>
		public void Dispose()
		{
			Close();
		}
	}
}
