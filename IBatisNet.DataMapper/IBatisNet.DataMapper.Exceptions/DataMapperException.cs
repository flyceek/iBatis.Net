using IBatisNet.Common.Exceptions;
using System;
using System.Runtime.Serialization;

namespace IBatisNet.DataMapper.Exceptions
{
	[System.Serializable]
	public class DataMapperException : IBatisNetException
	{
		public DataMapperException() : base("iBATIS.NET DataMapper component caused an exception.")
		{
		}

		public DataMapperException(System.Exception ex) : base("iBATIS.NET DataMapper component caused an exception.", ex)
		{
		}

		public DataMapperException(string message) : base(message)
		{
		}

		public DataMapperException(string message, System.Exception inner) : base(message, inner)
		{
		}

		protected DataMapperException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
	}
}
