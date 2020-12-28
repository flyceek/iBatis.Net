using System;
using System.Collections;
using System.Data;
using System.IO;
using IBatisNet.Common.Exceptions;

namespace IBatisNet.Common.Utilities
{
	/// <summary>
	/// Description r閟um閑 de ScriptRunner.
	/// </summary>
	public class ScriptRunner
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScriptRunner()
		{
		}

		/// <summary>
		/// Run an sql script
		/// </summary>
		/// <param name="dataSource">The dataSouce that will be used to run the script.</param>
		/// <param name="sqlScriptPath">a path to an sql script file.</param>
		public void RunScript(IDataSource dataSource, string sqlScriptPath)
		{
			RunScript(dataSource, sqlScriptPath, doParse: true);
		}

		/// <summary>
		/// Run an sql script
		/// </summary>
		/// <param name="dataSource">The dataSouce that will be used to run the script.</param>
		/// <param name="sqlScriptPath">a path to an sql script file.</param>
		/// <param name="doParse">parse out the statements in the sql script file.</param>
		public void RunScript(IDataSource dataSource, string sqlScriptPath, bool doParse)
		{
			FileInfo fileInfo = new FileInfo(sqlScriptPath);
			string text = fileInfo.OpenText().ReadToEnd();
			ArrayList arrayList = new ArrayList();
			if (doParse)
			{
				switch (dataSource.DbProvider.Name)
				{
				case "oracle9.2":
				case "oracle10.1":
				case "oracleClient1.0":
				case "ByteFx":
				case "MySql":
					arrayList = ParseScript(text);
					break;
				case "OleDb1.1":
					if (dataSource.ConnectionString.IndexOf("Microsoft.Jet.OLEDB") > 0)
					{
						arrayList = ParseScript(text);
					}
					else
					{
						arrayList.Add(text);
					}
					break;
				default:
					arrayList.Add(text);
					break;
				}
			}
			else
			{
				switch (dataSource.DbProvider.Name)
				{
				case "oracle9.2":
				case "oracle10.1":
				case "oracleClient1.0":
				case "ByteFx":
				case "MySql":
					text = text.Replace("\r\n", " ");
					text = text.Replace("\t", " ");
					arrayList.Add(text);
					break;
				case "OleDb1.1":
					if (dataSource.ConnectionString.IndexOf("Microsoft.Jet.OLEDB") > 0)
					{
						text = text.Replace("\r\n", " ");
						text = text.Replace("\t", " ");
						arrayList.Add(text);
					}
					else
					{
						arrayList.Add(text);
					}
					break;
				default:
					arrayList.Add(text);
					break;
				}
			}
			try
			{
				ExecuteStatements(dataSource, arrayList);
			}
			catch (Exception inner)
			{
				throw new IBatisNetException("Unable to execute the sql: " + fileInfo.Name, inner);
			}
			finally
			{
			}
		}

		/// <summary>
		/// Execute the given sql statements
		/// </summary>
		/// <param name="dataSource">The dataSouce that will be used.</param>
		/// <param name="sqlStatements">An ArrayList of sql statements to execute.</param>
		private void ExecuteStatements(IDataSource dataSource, ArrayList sqlStatements)
		{
			IDbConnection dbConnection = dataSource.DbProvider.CreateConnection();
			dbConnection.ConnectionString = dataSource.ConnectionString;
			dbConnection.Open();
			IDbTransaction dbTransaction = dbConnection.BeginTransaction();
			IDbCommand dbCommand = dbConnection.CreateCommand();
			dbCommand.Connection = dbConnection;
			dbCommand.Transaction = dbTransaction;
			try
			{
				IEnumerator enumerator = sqlStatements.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string text2 = (dbCommand.CommandText = (string)enumerator.Current);
						dbCommand.ExecuteNonQuery();
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				dbTransaction.Commit();
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				throw ex;
			}
			finally
			{
				dbConnection.Close();
			}
		}

		/// <summary>
		/// Parse and tokenize the sql script into multiple statements
		/// </summary>
		/// <param name="script">the script to parse</param>
		private ArrayList ParseScript(string script)
		{
			ArrayList arrayList = new ArrayList();
			StringTokenizer stringTokenizer = new StringTokenizer(script, ";");
			IEnumerator enumerator = stringTokenizer.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = ((string)enumerator.Current).Replace("\r\n", " ");
				text = text.Trim();
				if (text != string.Empty)
				{
					arrayList.Add(text);
				}
			}
			return arrayList;
		}
	}
}
