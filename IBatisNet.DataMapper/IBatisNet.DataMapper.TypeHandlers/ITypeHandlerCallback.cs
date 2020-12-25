using System;

namespace IBatisNet.DataMapper.TypeHandlers
{
	public interface ITypeHandlerCallback
	{
		object NullValue
		{
			get;
		}

		void SetParameter(IParameterSetter setter, object parameter);

		object GetResult(IResultGetter getter);

		object ValueOf(string s);
	}
}
