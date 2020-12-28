using System;
using System.Reflection;
using System.Reflection.Emit;
using IBatisNet.Common.Exceptions;

namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// A <see cref="T:IBatisNet.Common.Utilities.Objects.IFactory" /> implementation that builds object via DynamicMethod.
	/// </summary>
	public sealed class DelegateFactory : IFactory
	{
		private delegate object Create(object[] parameters);

		private const BindingFlags VISIBILITY = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private Create _create = null;

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
		public object CreateInstance(object[] parameters)
		{
			return _create(parameters);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:IBatisNet.Common.Utilities.Objects.DelegateFactory" /> class.
		/// </summary>
		/// <param name="typeToCreate">The instance type to create.</param>
		/// <param name="argumentTypes">The types argument.</param>
		public DelegateFactory(Type typeToCreate, Type[] argumentTypes)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("CreateImplementation", typeof(object), new Type[1]
			{
				typeof(object[])
			}, GetType().Module, skipVisibility: false);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			ConstructorInfo constructor = typeToCreate.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
			if (constructor == null || !constructor.IsPublic)
			{
				throw new ProbeException($"Unable to optimize create instance. Cause : Could not find public constructor matching specified arguments for type \"{typeToCreate.Name}\".");
			}
			EmitArgsIL(iLGenerator, argumentTypes);
			iLGenerator.Emit(OpCodes.Newobj, constructor);
			iLGenerator.Emit(OpCodes.Ret);
			_create = (Create)dynamicMethod.CreateDelegate(typeof(Create));
		}

		/// <summary>   
		/// Emit parameter IL for a method call.   
		/// </summary>   
		/// <param name="il">IL generator.</param>   
		/// <param name="argumentTypes">Arguments type defined for a the constructor.</param>   
		private void EmitArgsIL(ILGenerator il, Type[] argumentTypes)
		{
			for (int i = 0; i < argumentTypes.Length; i++)
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldelem_Ref);
				Type type = argumentTypes[i];
				if (type.IsValueType)
				{
					if (type.IsPrimitive || type.IsEnum)
					{
						il.Emit(OpCodes.Unbox, type);
						il.Emit(BoxingOpCodes.GetOpCode(type));
					}
					else if (type.IsValueType)
					{
						il.Emit(OpCodes.Unbox, type);
						il.Emit(OpCodes.Ldobj, type);
					}
				}
			}
		}
	}
}
