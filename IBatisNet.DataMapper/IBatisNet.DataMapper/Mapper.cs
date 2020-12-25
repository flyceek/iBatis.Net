using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper.Configuration;
using System;

namespace IBatisNet.DataMapper
{
	public sealed class Mapper
	{
		private static volatile ISqlMapper _mapper = null;

		public static void Configure(object obj)
		{
			Mapper._mapper = null;
		}

		public static void InitMapper()
		{
			ConfigureHandler configureDelegate = new ConfigureHandler(Mapper.Configure);
			DomSqlMapBuilder domSqlMapBuilder = new DomSqlMapBuilder();
			Mapper._mapper = domSqlMapBuilder.ConfigureAndWatch(configureDelegate);
		}

		public static ISqlMapper Instance()
		{
			if (Mapper._mapper == null)
			{
				lock (typeof(SqlMapper))
				{
					if (Mapper._mapper == null)
					{
						Mapper.InitMapper();
					}
				}
			}
			return Mapper._mapper;
		}

		public static ISqlMapper Get()
		{
			return Mapper.Instance();
		}
	}
}
