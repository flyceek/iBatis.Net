using IBatisNet.DataMapper.Configuration.ParameterMapping;
using System;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	public sealed class SqlGenerator
	{
		public static string BuildQuery(IStatement statement)
		{
			string result = string.Empty;
			if (statement is Select)
			{
				result = SqlGenerator.BuildSelectQuery(statement);
			}
			else if (statement is Insert)
			{
				result = SqlGenerator.BuildInsertQuery(statement);
			}
			else if (statement is Update)
			{
				result = SqlGenerator.BuildUpdateQuery(statement);
			}
			else if (statement is Delete)
			{
				result = SqlGenerator.BuildDeleteQuery(statement);
			}
			return result;
		}

		private static string BuildSelectQuery(IStatement statement)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			Select select = (Select)statement;
			int count = statement.ParameterMap.PropertiesList.Count;
			stringBuilder.Append("SELECT ");
			for (int i = 0; i < count; i++)
			{
				ParameterProperty parameterProperty = statement.ParameterMap.PropertiesList[i];
				if (i < count - 1)
				{
					stringBuilder.Append(string.Concat(new string[]
					{
						"\t",
						parameterProperty.ColumnName,
						" as ",
						parameterProperty.PropertyName,
						","
					}));
				}
				else
				{
					stringBuilder.Append("\t" + parameterProperty.ColumnName + " as " + parameterProperty.PropertyName);
				}
			}
			stringBuilder.Append(" FROM ");
			stringBuilder.Append("\t" + select.Generate.Table);
			string[] array = select.Generate.By.Split(new char[]
			{
				','
			});
			if (array.Length > 0 && select.Generate.By.Length > 0)
			{
				stringBuilder.Append(" WHERE ");
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					string str = array[i];
					if (i > 0)
					{
						stringBuilder.Append("\tAND " + str + " = ?");
					}
					else
					{
						stringBuilder.Append("\t" + str + " = ?");
					}
				}
			}
			if (statement.ParameterClass == null)
			{
				statement.ParameterMap = null;
			}
			return stringBuilder.ToString();
		}

		private static string BuildInsertQuery(IStatement statement)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			Insert insert = (Insert)statement;
			int count = statement.ParameterMap.PropertiesList.Count;
			stringBuilder.Append("INSERT INTO " + insert.Generate.Table + " (");
			for (int i = 0; i < count; i++)
			{
				ParameterProperty parameterProperty = statement.ParameterMap.PropertiesList[i];
				if (i < count - 1)
				{
					stringBuilder.Append("\t" + parameterProperty.ColumnName + ",");
				}
				else
				{
					stringBuilder.Append("\t" + parameterProperty.ColumnName);
				}
			}
			stringBuilder.Append(") VALUES (");
			for (int i = 0; i < count; i++)
			{
				ParameterProperty parameterProperty = statement.ParameterMap.PropertiesList[i];
				if (i < count - 1)
				{
					stringBuilder.Append("\t?,");
				}
				else
				{
					stringBuilder.Append("\t?");
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private static string BuildUpdateQuery(IStatement statement)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			Update update = (Update)statement;
			int count = statement.ParameterMap.PropertiesList.Count;
			string[] array = update.Generate.By.Split(new char[]
			{
				','
			});
			stringBuilder.Append("UPDATE ");
			stringBuilder.Append("\t" + update.Generate.Table + " ");
			stringBuilder.Append("SET ");
			for (int i = 0; i < count; i++)
			{
				ParameterProperty parameterProperty = statement.ParameterMap.PropertiesList[i];
				if (update.Generate.By.IndexOf(parameterProperty.ColumnName) < 0)
				{
					if (i < count - array.Length - 1)
					{
						stringBuilder.Append("\t" + parameterProperty.ColumnName + " = ?,");
					}
					else
					{
						stringBuilder.Append("\t" + parameterProperty.ColumnName + " = ? ");
					}
				}
			}
			stringBuilder.Append(" WHERE ");
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				string str = array[i];
				if (i > 0)
				{
					stringBuilder.Append("\tAND " + str + " = ?");
				}
				else
				{
					stringBuilder.Append("\t " + str + " = ?");
				}
			}
			return stringBuilder.ToString();
		}

		private static string BuildDeleteQuery(IStatement statement)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			Delete delete = (Delete)statement;
			string[] array = delete.Generate.By.Split(new char[]
			{
				','
			});
			stringBuilder.Append("DELETE FROM");
			stringBuilder.Append("\t" + delete.Generate.Table);
			stringBuilder.Append(" WHERE ");
			int num = array.Length;
			for (int i = 0; i < array.Length; i++)
			{
				string str = array[i].Trim();
				if (i > 0)
				{
					stringBuilder.Append("\tAND " + str + " = ?");
				}
				else
				{
					stringBuilder.Append("\t " + str + " = ?");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
