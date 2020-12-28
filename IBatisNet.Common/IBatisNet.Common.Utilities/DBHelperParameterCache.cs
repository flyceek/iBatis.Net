using System;
using System.Collections;
using System.Data;
using System.Reflection;
using IBatisNet.Common.Exceptions;

namespace IBatisNet.Common.Utilities
{
	/// <summary>
	/// DBHelperParameterCache provides functions to leverage a 
	/// static cache of procedure parameters, and the
	/// ability to discover parameters for stored procedures at run-time.
	/// </summary>
	public sealed class DBHelperParameterCache
	{
		private Hashtable _paramCache = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.DBHelperParameterCache" /> class.
		/// </summary>
		public DBHelperParameterCache()
		{
		}

		/// <summary>
		/// Resolve at run time the appropriate set of Parameters for a stored procedure
		/// </summary>
		/// <param name="session">An IDalSession object</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">whether or not to include their return value parameter</param>
		/// <returns></returns>
		private IDataParameter[] DiscoverSpParameterSet(IDalSession session, string spName, bool includeReturnValueParameter)
		{
			return InternalDiscoverSpParameterSet(session, spName, includeReturnValueParameter);
		}

		/// <summary>
		/// Discover at run time the appropriate set of Parameters for a stored procedure
		/// </summary>
		/// <param name="session">An IDalSession object</param>
		/// <param name="spName">Name of the stored procedure.</param>
		/// <param name="includeReturnValueParameter">if set to <c>true</c> [include return value parameter].</param>
		/// <returns>The stored procedure parameters.</returns>
		private IDataParameter[] InternalDiscoverSpParameterSet(IDalSession session, string spName, bool includeReturnValueParameter)
		{
			using IDbCommand dbCommand = session.CreateCommand(CommandType.StoredProcedure);
			dbCommand.CommandText = spName;
			session.OpenConnection();
			DeriveParameters(session.DataSource.DbProvider, dbCommand);
			if (dbCommand.Parameters.Count > 0)
			{
				IDataParameter dataParameter = (IDataParameter)dbCommand.Parameters[0];
				if (dataParameter.Direction == ParameterDirection.ReturnValue && !includeReturnValueParameter)
				{
					dbCommand.Parameters.RemoveAt(0);
				}
			}
			IDataParameter[] array = new IDataParameter[dbCommand.Parameters.Count];
			dbCommand.Parameters.CopyTo(array, 0);
			return array;
		}

		private void DeriveParameters(IDbProvider provider, IDbCommand command)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (provider.CommandBuilderClass == null || provider.CommandBuilderClass.Length < 1)
			{
				throw new Exception($"CommandBuilderClass not defined for provider \"{provider.Name}\".");
			}
			Type commandBuilderType = provider.CommandBuilderType;
			try
			{
				commandBuilderType.InvokeMember("DeriveParameters", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[1]
				{
					command
				});
			}
			catch (Exception inner)
			{
				throw new IBatisNetException("Could not retrieve parameters for the store procedure named " + command.CommandText, inner);
			}
		}

		/// <summary>
		/// Deep copy of cached IDataParameter array.
		/// </summary>
		/// <param name="originalParameters"></param>
		/// <returns></returns>
		private IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
		{
			IDataParameter[] array = new IDataParameter[originalParameters.Length];
			int num = originalParameters.Length;
			int i = 0;
			for (int num2 = num; i < num2; i++)
			{
				array[i] = (IDataParameter)((ICloneable)originalParameters[i]).Clone();
			}
			return array;
		}

		/// <summary>
		/// Add parameter array to the cache
		/// </summary>
		/// <param name="connectionString">a valid connection string for an IDbConnection</param>
		/// <param name="commandText">the stored procedure name or SQL command</param>
		/// <param name="commandParameters">an array of IDataParameters to be cached</param>
		public void CacheParameterSet(string connectionString, string commandText, params IDataParameter[] commandParameters)
		{
			string key = connectionString + ":" + commandText;
			_paramCache[key] = commandParameters;
		}

		/// <summary>
		/// Clear the parameter cache.
		/// </summary>
		public void Clear()
		{
			_paramCache.Clear();
		}

		/// <summary>
		/// retrieve a parameter array from the cache
		/// </summary>
		/// <param name="connectionString">a valid connection string for an IDbConnection</param>
		/// <param name="commandText">the stored procedure name or SQL command</param>
		/// <returns>an array of IDataParameters</returns>
		public IDataParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			string key = connectionString + ":" + commandText;
			IDataParameter[] array = (IDataParameter[])_paramCache[key];
			if (array == null)
			{
				return null;
			}
			return CloneParameters(array);
		}

		/// <summary>
		/// Retrieves the set of IDataParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="session">a valid session</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <returns>an array of IDataParameters</returns>
		public IDataParameter[] GetSpParameterSet(IDalSession session, string spName)
		{
			return GetSpParameterSet(session, spName, includeReturnValueParameter: false);
		}

		/// <summary>
		/// Retrieves the set of IDataParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="session">a valid session</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">a bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>an array of IDataParameters</returns>
		public IDataParameter[] GetSpParameterSet(IDalSession session, string spName, bool includeReturnValueParameter)
		{
			string key = session.DataSource.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
			IDataParameter[] array = (IDataParameter[])_paramCache[key];
			if (array == null)
			{
				_paramCache[key] = DiscoverSpParameterSet(session, spName, includeReturnValueParameter);
				array = (IDataParameter[])_paramCache[key];
			}
			return CloneParameters(array);
		}
	}
}
