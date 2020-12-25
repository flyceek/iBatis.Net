using System;

namespace IBatisNet.DataMapper.Commands
{
	internal sealed class PreparedCommandFactory
	{
		public static IPreparedCommand GetPreparedCommand(bool isEmbedStatementParams)
		{
			return new DefaultPreparedCommand();
		}
	}
}
