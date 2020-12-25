using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities.Objects;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.Configuration.ResultMapping;
using IBatisNet.DataMapper.DataExchange;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace IBatisNet.DataMapper.MappedStatements
{
	public sealed class ReaderAutoMapper
	{
		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public static ResultPropertyCollection Build(DataExchangeFactory dataExchangeFactory, IDataReader reader, ref object resultObject)
		{
			System.Type type = resultObject.GetType();
			ResultPropertyCollection resultPropertyCollection = new ResultPropertyCollection();
			try
			{
				ReflectionInfo instance = ReflectionInfo.GetInstance(type);
				string[] writeableMemberNames = instance.GetWriteableMemberNames();
				System.Collections.Hashtable hashtable = new System.Collections.Hashtable();
				int num = writeableMemberNames.Length;
				for (int i = 0; i < num; i++)
				{
					ISetAccessorFactory setAccessorFactory = dataExchangeFactory.AccessorFactory.SetAccessorFactory;
					ISetAccessor value = setAccessorFactory.CreateSetAccessor(type, writeableMemberNames[i]);
					hashtable.Add(writeableMemberNames[i], value);
				}
				DataTable schemaTable = reader.GetSchemaTable();
				int count = schemaTable.Rows.Count;
				for (int i = 0; i < count; i++)
				{
					string text = schemaTable.Rows[i][0].ToString();
					ISetAccessor setAccessor = hashtable[text] as ISetAccessor;
					ResultProperty resultProperty = new ResultProperty();
					resultProperty.ColumnName = text;
					resultProperty.ColumnIndex = i;
					if (resultObject is System.Collections.Hashtable)
					{
						resultProperty.PropertyName = text;
						resultPropertyCollection.Add(resultProperty);
					}
					System.Type type2 = null;
					if (setAccessor == null)
					{
						try
						{
							type2 = ObjectProbe.GetMemberTypeForSetter(resultObject, text);
						}
						catch
						{
							ReaderAutoMapper._logger.Error(string.Concat(new string[]
							{
								"The column [",
								text,
								"] could not be auto mapped to a property on [",
								resultObject.ToString(),
								"]"
							}));
						}
					}
					else
					{
						type2 = setAccessor.MemberType;
					}
					if (type2 != null || setAccessor != null)
					{
						resultProperty.PropertyName = ((setAccessor != null) ? setAccessor.Name : text);
						if (setAccessor != null)
						{
							resultProperty.Initialize(dataExchangeFactory.TypeHandlerFactory, setAccessor);
						}
						else
						{
							resultProperty.TypeHandler = dataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type2);
						}
						resultProperty.PropertyStrategy = PropertyStrategyFactory.Get(resultProperty);
						resultPropertyCollection.Add(resultProperty);
					}
				}
			}
			catch (System.Exception ex)
			{
				throw new DataMapperException("Error automapping columns. Cause: " + ex.Message, ex);
			}
			return resultPropertyCollection;
		}
	}
}
