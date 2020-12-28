namespace IBatisNet.Common
{
	/// <summary>
	/// IDataSource
	/// </summary>
	public interface IDataSource
	{
		/// <summary>
		/// DataSource Name.
		/// </summary>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Connection string used to create connections.
		/// </summary>
		string ConnectionString
		{
			get;
			set;
		}

		/// <summary>
		/// The data provider.
		/// </summary>
		IDbProvider DbProvider
		{
			get;
			set;
		}
	}
}
