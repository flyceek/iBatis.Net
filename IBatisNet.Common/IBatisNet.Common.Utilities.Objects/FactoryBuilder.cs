using System;
using System.Reflection;
using System.Reflection.Emit;
using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Logging;

namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// Build IFactory object via IL 
	/// </summary>
	public class FactoryBuilder
	{
		private const BindingFlags VISIBILITY = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private const MethodAttributes CREATE_METHOD_ATTRIBUTES = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask;

		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private ModuleBuilder _moduleBuilder = null;

		/// <summary>
		/// constructor
		/// </summary>
		public FactoryBuilder()
		{
			AssemblyName assemblyName = new AssemblyName
			{
				Name = "iBATIS.EmitFactory" + HashCodeProvider.GetIdentityHashCode(this)
			};
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			_moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name + ".dll");
		}

		/// <summary>
		/// Create a factory which build class of type typeToCreate
		/// </summary>
		/// <param name="typeToCreate">The type instance to build</param>
		/// <param name="types">The types of the constructor arguments</param>
		/// <returns>Returns a new <see cref="T:IBatisNet.Common.Utilities.Objects.IFactory" /> instance.</returns>
		public IFactory CreateFactory(Type typeToCreate, Type[] types)
		{
			if (typeToCreate.IsAbstract)
			{
				if (_logger.IsInfoEnabled)
				{
					_logger.Info("Create a stub IFactory for abstract type " + typeToCreate.Name);
				}
				return new AbstractFactory(typeToCreate);
			}
			Type type = CreateFactoryType(typeToCreate, types);
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			return (IFactory)constructor.Invoke(new object[0]);
		}

		/// <summary>
		/// Creates a <see cref="T:IBatisNet.Common.Utilities.Objects.IFactory" />.
		/// </summary>
		/// <param name="typeToCreate">The type instance to create.</param>
		/// <param name="types">The types.</param>
		/// <returns>The <see cref="T:IBatisNet.Common.Utilities.Objects.IFactory" /></returns>
		private Type CreateFactoryType(Type typeToCreate, Type[] types)
		{
			string text = string.Empty;
			for (int i = 0; i < types.Length; i++)
			{
				text = text + types[i].Name.Replace("[]", string.Empty) + i;
			}
			TypeBuilder typeBuilder = _moduleBuilder.DefineType("EmitFactoryFor" + typeToCreate.FullName + text, TypeAttributes.Public);
			typeBuilder.AddInterfaceImplementation(typeof(IFactory));
			ImplementCreateInstance(typeBuilder, typeToCreate, types);
			return typeBuilder.CreateType();
		}

		/// <summary>
		/// Implements the create instance.
		/// </summary>
		/// <param name="typeBuilder">The type builder.</param>
		/// <param name="typeToCreate">The type to create.</param>
		/// <param name="argumentTypes">The argument types.</param>
		private void ImplementCreateInstance(TypeBuilder typeBuilder, Type typeToCreate, Type[] argumentTypes)
		{
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("CreateInstance", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, typeof(object), new Type[1]
			{
				typeof(object[])
			});
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			ConstructorInfo constructor = typeToCreate.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
			if (constructor == null || !constructor.IsPublic)
			{
				throw new ProbeException($"Unable to optimize create instance. Cause : Could not find public constructor matching specified arguments for type \"{typeToCreate.Name}\".");
			}
			EmitArgsIL(iLGenerator, argumentTypes);
			iLGenerator.Emit(OpCodes.Newobj, constructor);
			iLGenerator.Emit(OpCodes.Ret);
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
				il.Emit(OpCodes.Ldarg_1);
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
