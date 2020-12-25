using IBatisNet.DataMapper.Configuration.ResultMapping;
using System;

namespace IBatisNet.DataMapper.MappedStatements
{
	public sealed class PostBindind
	{
		public enum ExecuteMethod
		{
			ExecuteQueryForObject = 1,
			ExecuteQueryForIList,
			ExecuteQueryForGenericIList,
			ExecuteQueryForArrayList,
			ExecuteQueryForStrongTypedIList
		}

		private IMappedStatement _statement = null;

		private ResultProperty _property = null;

		private object _target = null;

		private object _keys = null;

		private PostBindind.ExecuteMethod _method = PostBindind.ExecuteMethod.ExecuteQueryForIList;

		public IMappedStatement Statement
		{
			get
			{
				return this._statement;
			}
			set
			{
				this._statement = value;
			}
		}

		public ResultProperty ResultProperty
		{
			get
			{
				return this._property;
			}
			set
			{
				this._property = value;
			}
		}

		public object Target
		{
			get
			{
				return this._target;
			}
			set
			{
				this._target = value;
			}
		}

		public object Keys
		{
			get
			{
				return this._keys;
			}
			set
			{
				this._keys = value;
			}
		}

		public PostBindind.ExecuteMethod Method
		{
			get
			{
				return this._method;
			}
			set
			{
				this._method = value;
			}
		}
	}
}
