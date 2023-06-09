using System;
using System.Reflection;
using System.Text;
using IBatisNet.Common.Logging;

namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// A wrapper arround an <see cref="T:IBatisNet.Common.Utilities.Objects.IFactory" /> implementation which logs argument type and value
	/// when CreateInstance is called.
	/// </summary>
	public class FactoryLogAdapter : IFactory
	{
		private IFactory _factory = null;

		private string _typeName = string.Empty;

		private string _parametersTypeName = string.Empty;

		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.FactoryLogAdapter" /> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="paramtersTypes">The paramters types.</param>
		/// <param name="factory">The factory.</param>
		public FactoryLogAdapter(Type type, Type[] paramtersTypes, IFactory factory)
		{
			_factory = factory;
			_typeName = type.FullName;
			_parametersTypeName = GenerateParametersName(paramtersTypes);
		}

		/// <summary>
		/// Create a new instance with the specified parameters
		/// </summary>
		/// <param name="parameters">An array of values that matches the number, order and type
		/// of the parameters for this constructor.</param>
		/// <returns>A new instance</returns>
		/// <remarks>
		/// If you call a constructor with no parameters, pass null.
		/// Anyway, what you pass will be ignore.
		/// </remarks>
		public object CreateInstance(object[] parameters)
		{
			object obj = null;
			try
			{
				return _factory.CreateInstance(parameters);
			}
			catch
			{
				_logger.Debug("Enabled to create instance for type '" + _typeName);
				_logger.Debug("  using parameters type : " + _parametersTypeName);
				_logger.Debug("  using parameters value : " + GenerateLogInfoForParameterValue(parameters));
				throw;
			}
		}

		/// <summary>
		/// Generates the a string containing all parameter type names.
		/// </summary>
		/// <param name="arguments">The types of the constructor arguments</param>
		/// <returns>The string.</returns>
		private string GenerateParametersName(object[] arguments)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (arguments != null && arguments.Length != 0)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					stringBuilder.Append("[").Append(arguments[i]).Append("] ");
				}
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Generates the a string containing all parameters value.
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <returns>The string.</returns>
		private string GenerateLogInfoForParameterValue(object[] arguments)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (arguments != null && arguments.Length != 0)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					if (arguments[i] != null)
					{
						stringBuilder.Append("[").Append(arguments[i].ToString()).Append("] ");
					}
					else
					{
						stringBuilder.Append("[null] ");
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
