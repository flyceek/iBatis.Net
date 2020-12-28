using System;

namespace IBatisNet.Common.Transaction
{
	/// <summary>
	/// Contains parameters that specify Transaction behaviors.
	/// </summary>
	public struct TransactionOptions
	{
		/// <summary>
		/// Length of time that the transaction waits before automatically 
		/// closing itself
		/// </summary>
		public TimeSpan TimeOut;

		/// <summary>
		/// The isolation level of the transaction.
		/// </summary>
		public IsolationLevel IsolationLevel;
	}
}
