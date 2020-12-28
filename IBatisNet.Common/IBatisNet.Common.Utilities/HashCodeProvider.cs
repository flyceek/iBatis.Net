using System;
using System.Reflection;

namespace IBatisNet.Common.Utilities
{
	/// <summary>
	/// Summary description for HashCodeProvider.
	/// </summary>
	public sealed class HashCodeProvider
	{
		private static MethodInfo getHashCodeMethodInfo;

		static HashCodeProvider()
		{
			getHashCodeMethodInfo = null;
			Type typeFromHandle = typeof(object);
			getHashCodeMethodInfo = typeFromHandle.GetMethod("GetHashCode");
		}

		/// <summary>
		/// Supplies a hash code for an object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>A hash code</returns>
		/// <remarks>
		/// Buggy in .NET V1.0
		/// .NET Fx v1.1 Update: 
		/// As of v1.1 of the framework, there is a method System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(object) that does this as well.
		/// I will not use to Keep compatiblity with .NET V1.0
		/// </remarks>
		public static int GetIdentityHashCode(object obj)
		{
			return (int)getHashCodeMethodInfo.Invoke(obj, null);
		}
	}
}
