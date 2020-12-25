using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Utilities;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.Configuration.Sql.Dynamic;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.Scope;
using IBatisNet.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Text;

namespace IBatisNet.DataMapper.Configuration.ParameterMapping
{
	internal class InlineParameterMapParser
	{
		private const string PARAMETER_TOKEN = "#";

		private const string PARAM_DELIM = ":";

		public SqlText ParseInlineParameterMap(IScope scope, IStatement statement, string sqlStatement)
		{
			System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
			System.Type parameterClassType = null;
			if (statement != null)
			{
				parameterClassType = statement.ParameterClass;
			}
			StringTokenizer stringTokenizer = new StringTokenizer(sqlStatement, "#", true);
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			string value = null;
			System.Collections.IEnumerator enumerator = stringTokenizer.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current;
				if ("#".Equals(value))
				{
					if ("#".Equals(text))
					{
						stringBuilder.Append("#");
						text = null;
					}
					else
					{
						ParameterProperty value2;
						if (text.IndexOf(":") > -1)
						{
							value2 = this.OldParseMapping(text, parameterClassType, scope);
						}
						else
						{
							value2 = this.NewParseMapping(text, parameterClassType, scope);
						}
						arrayList.Add(value2);
						stringBuilder.Append("? ");
						enumerator.MoveNext();
						text = (string)enumerator.Current;
						if (!"#".Equals(text))
						{
							throw new DataMapperException("Unterminated inline parameter in mapped statement (" + statement.Id + ").");
						}
						text = null;
					}
				}
				else if (!"#".Equals(text))
				{
					stringBuilder.Append(text);
				}
				value = text;
			}
			string text2 = stringBuilder.ToString();
			ParameterProperty[] parameters = (ParameterProperty[])arrayList.ToArray(typeof(ParameterProperty));
			return new SqlText
			{
				Text = text2,
				Parameters = parameters
			};
		}

		private ParameterProperty NewParseMapping(string token, System.Type parameterClassType, IScope scope)
		{
			ParameterProperty parameterProperty = new ParameterProperty();
			StringTokenizer stringTokenizer = new StringTokenizer(token, "=,", false);
			System.Collections.IEnumerator enumerator = stringTokenizer.GetEnumerator();
			enumerator.MoveNext();
			parameterProperty.PropertyName = ((string)enumerator.Current).Trim();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current;
				if (!enumerator.MoveNext())
				{
					throw new DataMapperException("Incorrect inline parameter map format (missmatched name=value pairs): " + token);
				}
				string text2 = (string)enumerator.Current;
				if ("type".Equals(text))
				{
					parameterProperty.CLRType = text2;
				}
				else if ("dbType".Equals(text))
				{
					parameterProperty.DbType = text2;
				}
				else if ("direction".Equals(text))
				{
					parameterProperty.DirectionAttribute = text2;
				}
				else if ("nullValue".Equals(text))
				{
					parameterProperty.NullValue = text2;
				}
				else
				{
					if (!"handler".Equals(text))
					{
						throw new DataMapperException("Unrecognized parameter mapping field: '" + text + "' in " + token);
					}
					parameterProperty.CallBackName = text2;
				}
			}
			if (parameterProperty.CallBackName.Length > 0)
			{
				parameterProperty.Initialize(scope, parameterClassType);
			}
			else
			{
				ITypeHandler typeHandler;
				if (parameterClassType == null)
				{
					typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
				}
				else
				{
					typeHandler = this.ResolveTypeHandler(scope.DataExchangeFactory.TypeHandlerFactory, parameterClassType, parameterProperty.PropertyName, parameterProperty.CLRType, parameterProperty.DbType);
				}
				parameterProperty.TypeHandler = typeHandler;
				parameterProperty.Initialize(scope, parameterClassType);
			}
			return parameterProperty;
		}

		private ParameterProperty OldParseMapping(string token, System.Type parameterClassType, IScope scope)
		{
			ParameterProperty parameterProperty = new ParameterProperty();
			if (token.IndexOf(":") > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(token, ":", true);
				System.Collections.IEnumerator enumerator = stringTokenizer.GetEnumerator();
				int tokenNumber = stringTokenizer.TokenNumber;
				if (tokenNumber == 3)
				{
					enumerator.MoveNext();
					string propertyName = ((string)enumerator.Current).Trim();
					parameterProperty.PropertyName = propertyName;
					enumerator.MoveNext();
					enumerator.MoveNext();
					string dbType = ((string)enumerator.Current).Trim();
					parameterProperty.DbType = dbType;
					ITypeHandler typeHandler;
					if (parameterClassType == null)
					{
						typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
					}
					else
					{
						typeHandler = this.ResolveTypeHandler(scope.DataExchangeFactory.TypeHandlerFactory, parameterClassType, propertyName, null, dbType);
					}
					parameterProperty.TypeHandler = typeHandler;
					parameterProperty.Initialize(scope, parameterClassType);
				}
				else
				{
					if (tokenNumber < 5)
					{
						throw new ConfigurationException("Incorrect inline parameter map format: " + token);
					}
					enumerator.MoveNext();
					string propertyName = ((string)enumerator.Current).Trim();
					enumerator.MoveNext();
					enumerator.MoveNext();
					string dbType = ((string)enumerator.Current).Trim();
					enumerator.MoveNext();
					enumerator.MoveNext();
					string text = ((string)enumerator.Current).Trim();
					while (enumerator.MoveNext())
					{
						text += ((string)enumerator.Current).Trim();
					}
					parameterProperty.PropertyName = propertyName;
					parameterProperty.DbType = dbType;
					parameterProperty.NullValue = text;
					ITypeHandler typeHandler;
					if (parameterClassType == null)
					{
						typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
					}
					else
					{
						typeHandler = this.ResolveTypeHandler(scope.DataExchangeFactory.TypeHandlerFactory, parameterClassType, propertyName, null, dbType);
					}
					parameterProperty.TypeHandler = typeHandler;
					parameterProperty.Initialize(scope, parameterClassType);
				}
			}
			else
			{
				parameterProperty.PropertyName = token;
				ITypeHandler typeHandler;
				if (parameterClassType == null)
				{
					typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
				}
				else
				{
					typeHandler = this.ResolveTypeHandler(scope.DataExchangeFactory.TypeHandlerFactory, parameterClassType, token, null, null);
				}
				parameterProperty.TypeHandler = typeHandler;
				parameterProperty.Initialize(scope, parameterClassType);
			}
			return parameterProperty;
		}

		private ITypeHandler ResolveTypeHandler(TypeHandlerFactory typeHandlerFactory, System.Type parameterClassType, string propertyName, string propertyType, string dbType)
		{
			ITypeHandler result = null;
			if (parameterClassType == null)
			{
				result = typeHandlerFactory.GetUnkownTypeHandler();
			}
			else if (typeof(System.Collections.IDictionary).IsAssignableFrom(parameterClassType))
			{
				if (propertyType == null || propertyType.Length == 0)
				{
					result = typeHandlerFactory.GetUnkownTypeHandler();
				}
				else
				{
					try
					{
						System.Type type = TypeUtils.ResolveType(propertyType);
						result = typeHandlerFactory.GetTypeHandler(type, dbType);
					}
					catch (System.Exception ex)
					{
						throw new ConfigurationException("Error. Could not set TypeHandler.  Cause: " + ex.Message, ex);
					}
				}
			}
			else if (typeHandlerFactory.GetTypeHandler(parameterClassType, dbType) != null)
			{
				result = typeHandlerFactory.GetTypeHandler(parameterClassType, dbType);
			}
			else
			{
				System.Type type = ObjectProbe.GetMemberTypeForGetter(parameterClassType, propertyName);
				result = typeHandlerFactory.GetTypeHandler(type, dbType);
			}
			return result;
		}
	}
}
