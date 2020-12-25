using IBatisNet.Common;
using System;
using System.Data;

namespace IBatisNet.DataMapper.Commands
{
	public sealed class DataReaderTransformer
	{
		public static IDataReader Transform(IDataReader reader, IDbProvider dbProvider)
		{
			IDataReader result;
			if (!dbProvider.AllowMARS && !(reader is InMemoryDataReader))
			{
				result = new InMemoryDataReader(reader);
			}
			else
			{
				result = reader;
			}
			return result;
		}
	}
}
