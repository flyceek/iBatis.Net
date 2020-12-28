namespace IBatisNet.Common.Utilities.Objects.Members
{
	/// <summary>
	/// Accessor factory
	/// </summary>
	public class AccessorFactory
	{
		private ISetAccessorFactory _setAccessorFactory = null;

		private IGetAccessorFactory _getAccessorFactory = null;

		/// <summary>
		/// The factory which build <see cref="T:IBatisNet.Common.Utilities.Objects.Members.ISetAccessor" />
		/// </summary>
		public ISetAccessorFactory SetAccessorFactory => _setAccessorFactory;

		/// <summary>
		/// The factory which build <see cref="T:IBatisNet.Common.Utilities.Objects.Members.IGetAccessor" />
		/// </summary>
		public IGetAccessorFactory GetAccessorFactory => _getAccessorFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.Members.AccessorFactory" /> class.
		/// </summary>
		/// <param name="setAccessorFactory">The set accessor factory.</param>
		/// <param name="getAccessorFactory">The get accessor factory.</param>
		public AccessorFactory(ISetAccessorFactory setAccessorFactory, IGetAccessorFactory getAccessorFactory)
		{
			_setAccessorFactory = setAccessorFactory;
			_getAccessorFactory = getAccessorFactory;
		}
	}
}
