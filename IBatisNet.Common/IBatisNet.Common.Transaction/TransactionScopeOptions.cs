namespace IBatisNet.Common.Transaction
{
	/// <summary>
	/// Describes how a transaction scope is associated with a transaction.
	/// </summary>
	public enum TransactionScopeOptions
	{
		/// <summary>
		/// The transaction scope must be associated with a transaction.
		/// If we are in a transaction scope join it. If we aren't, create a new one.
		/// </summary>
		Required,
		/// <summary>
		/// Always creates a new transaction scope.
		/// </summary>
		RequiresNew,
		/// <summary>
		/// Don't need a transaction scope, but if we are in a transaction scope then join it.
		/// </summary>
		Supported,
		/// <summary>
		/// Means that cannot cannot be associated with a transaction scope.
		/// </summary>
		NotSupported,
		/// <summary>
		/// The transaction scope must be associated with an existing transaction scope.
		/// </summary>
		Mandatory
	}
}
