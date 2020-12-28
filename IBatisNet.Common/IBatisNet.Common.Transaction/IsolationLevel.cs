namespace IBatisNet.Common.Transaction
{
	/// <summary>
	/// Specifies the isolation level of a transaction.
	/// </summary>
	public enum IsolationLevel
	{
		/// <summary>
		/// Volatile data can be read but not modified, 
		/// and no new data can be added during the transaction.
		/// </summary>
		Serializable,
		/// <summary>
		/// Volatile data can be read but not modified during the transaction. 
		/// New data may be added during the transaction.
		/// </summary>
		RepeatableRead,
		/// <summary>
		/// Volatile data cannot be read during the transaction, but can be modified.
		/// </summary>
		ReadCommitted,
		/// <summary>
		/// Volatile data can be read and modified during the transaction.
		/// </summary>
		ReadUncommitted,
		/// <summary>
		/// Volatile data can be read but not modified, 
		/// and no new data can be added during the transaction.
		/// </summary>
		Unspecified
	}
}
