namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// Factory to create object for a given type
	/// </summary>
	public interface IFactory
	{
		/// <summary>
		/// Create a new instance with the specified parameters
		/// </summary>
		/// <param name="parameters">
		/// An array of values that matches the number, order and type 
		/// of the parameters for this constructor. 
		/// </param>
		/// <remarks>
		/// If you call a constructor with no parameters, pass null. 
		/// Anyway, what you pass will be ignore.
		/// </remarks>
		/// <returns>A new instance</returns>
		object CreateInstance(object[] parameters);
	}
}
