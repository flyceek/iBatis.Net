using System;
using System.Collections;
using System.Reflection;
using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Utilities.Objects.Members;

namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// Description r閟um閑 de ObjectProbe.
	/// </summary>
	public sealed class ObjectProbe
	{
		private static ArrayList _simpleTypeMap;

		static ObjectProbe()
		{
			_simpleTypeMap = new ArrayList();
			_simpleTypeMap.Add(typeof(string));
			_simpleTypeMap.Add(typeof(byte));
			_simpleTypeMap.Add(typeof(short));
			_simpleTypeMap.Add(typeof(char));
			_simpleTypeMap.Add(typeof(int));
			_simpleTypeMap.Add(typeof(long));
			_simpleTypeMap.Add(typeof(float));
			_simpleTypeMap.Add(typeof(double));
			_simpleTypeMap.Add(typeof(bool));
			_simpleTypeMap.Add(typeof(DateTime));
			_simpleTypeMap.Add(typeof(decimal));
			_simpleTypeMap.Add(typeof(sbyte));
			_simpleTypeMap.Add(typeof(ushort));
			_simpleTypeMap.Add(typeof(uint));
			_simpleTypeMap.Add(typeof(ulong));
			_simpleTypeMap.Add(typeof(IEnumerator));
		}

		/// <summary>
		/// Returns an array of the readable properties names exposed by an object
		/// </summary>
		/// <param name="obj">The object</param>
		/// <returns>The properties name</returns>
		public static string[] GetReadablePropertyNames(object obj)
		{
			return ReflectionInfo.GetInstance(obj.GetType()).GetReadableMemberNames();
		}

		/// <summary>
		/// Returns an array of the writeable members name exposed by a object
		/// </summary>
		/// <param name="obj">The object</param>
		/// <returns>The members name</returns>
		public static string[] GetWriteableMemberNames(object obj)
		{
			return ReflectionInfo.GetInstance(obj.GetType()).GetWriteableMemberNames();
		}

		/// <summary>
		///  Returns the type that the set expects to receive as a parameter when
		///  setting a member value.
		/// </summary>
		/// <param name="obj">The object to check</param>
		/// <param name="memberName">The name of the member</param>
		/// <returns>The type of the member</returns>
		public static Type GetMemberTypeForSetter(object obj, string memberName)
		{
			Type type = obj.GetType();
			if (obj is IDictionary)
			{
				IDictionary dictionary = (IDictionary)obj;
				object obj2 = dictionary[memberName];
				type = ((obj2 != null) ? obj2.GetType() : typeof(object));
			}
			else if (memberName.IndexOf('.') > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(memberName, ".");
				IEnumerator enumerator = stringTokenizer.GetEnumerator();
				while (enumerator.MoveNext())
				{
					memberName = (string)enumerator.Current;
					type = ReflectionInfo.GetInstance(type).GetSetterType(memberName);
				}
			}
			else
			{
				type = ReflectionInfo.GetInstance(type).GetSetterType(memberName);
			}
			return type;
		}

		/// <summary>
		///  Returns the type that the set expects to receive as a parameter when
		///  setting a member value.
		/// </summary>
		/// <param name="type">The class type to check</param>
		/// <param name="memberName">The name of the member</param>
		/// <returns>The type of the member</returns>
		public static Type GetMemberTypeForSetter(Type type, string memberName)
		{
			Type type2 = type;
			if (memberName.IndexOf('.') > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(memberName, ".");
				IEnumerator enumerator = stringTokenizer.GetEnumerator();
				while (enumerator.MoveNext())
				{
					memberName = (string)enumerator.Current;
					type2 = ReflectionInfo.GetInstance(type2).GetSetterType(memberName);
				}
			}
			else
			{
				type2 = ReflectionInfo.GetInstance(type).GetSetterType(memberName);
			}
			return type2;
		}

		/// <summary>
		///  Returns the type that the get expects to receive as a parameter when
		///  setting a member value.
		/// </summary>
		/// <param name="obj">The object to check</param>
		/// <param name="memberName">The name of the member</param>
		/// <returns>The type of the member</returns>
		public static Type GetMemberTypeForGetter(object obj, string memberName)
		{
			Type type = obj.GetType();
			if (obj is IDictionary)
			{
				IDictionary dictionary = (IDictionary)obj;
				object obj2 = dictionary[memberName];
				type = ((obj2 != null) ? obj2.GetType() : typeof(object));
			}
			else if (memberName.IndexOf('.') > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(memberName, ".");
				IEnumerator enumerator = stringTokenizer.GetEnumerator();
				while (enumerator.MoveNext())
				{
					memberName = (string)enumerator.Current;
					type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
				}
			}
			else
			{
				type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
			}
			return type;
		}

		/// <summary>
		///  Returns the type that the get expects to receive as a parameter when
		///  setting a member value.
		/// </summary>
		/// <param name="type">The type to check</param>
		/// <param name="memberName">The name of the member</param>
		/// <returns>The type of the member</returns>
		public static Type GetMemberTypeForGetter(Type type, string memberName)
		{
			if (memberName.IndexOf('.') > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(memberName, ".");
				IEnumerator enumerator = stringTokenizer.GetEnumerator();
				while (enumerator.MoveNext())
				{
					memberName = (string)enumerator.Current;
					type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
				}
			}
			else
			{
				type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
			}
			return type;
		}

		/// <summary>
		///  Returns the MemberInfo of the set member on the specified type.
		/// </summary>
		/// <param name="type">The type to check</param>
		/// <param name="memberName">The name of the member</param>
		/// <returns>The type of the member</returns>
		public static MemberInfo GetMemberInfoForSetter(Type type, string memberName)
		{
			MemberInfo memberInfo = null;
			if (memberName.IndexOf('.') > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(memberName, ".");
				IEnumerator enumerator = stringTokenizer.GetEnumerator();
				Type type2 = null;
				while (enumerator.MoveNext())
				{
					memberName = (string)enumerator.Current;
					type2 = type;
					type = ReflectionInfo.GetInstance(type).GetSetterType(memberName);
				}
				return ReflectionInfo.GetInstance(type2).GetSetter(memberName);
			}
			return ReflectionInfo.GetInstance(type).GetSetter(memberName);
		}

		/// <summary>
		/// Gets the value of an array member on the specified object.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="indexedName">The array index.</param>
		/// <param name="accessorFactory">The accessor factory.</param>
		/// <returns>The member value.</returns>
		private static object GetArrayMember(object obj, string indexedName, AccessorFactory accessorFactory)
		{
			object obj2 = null;
			try
			{
				int num = indexedName.IndexOf("[");
				int num2 = indexedName.IndexOf("]");
				string text = indexedName.Substring(0, num);
				string value = indexedName.Substring(num + 1, num2 - (num + 1));
				int index = Convert.ToInt32(value);
				obj2 = ((text.Length <= 0) ? obj : GetMember(obj, text, accessorFactory));
				if (obj2 is IList)
				{
					return ((IList)obj2)[index];
				}
				throw new ProbeException("The '" + text + "' member of the " + obj.GetType().Name + " class is not a List or Array.");
			}
			catch (ProbeException ex)
			{
				throw ex;
			}
			catch (Exception ex2)
			{
				throw new ProbeException("Error getting ordinal value from .net object. Cause" + ex2.Message, ex2);
			}
		}

		/// <summary>
		/// Sets the array member.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="indexedName">Name of the indexed.</param>
		/// <param name="value">The value.</param>
		/// <param name="accessorFactory">The accessor factory.</param>
		private static void SetArrayMember(object obj, string indexedName, object value, AccessorFactory accessorFactory)
		{
			try
			{
				int num = indexedName.IndexOf("[");
				int num2 = indexedName.IndexOf("]");
				string text = indexedName.Substring(0, num);
				string value2 = indexedName.Substring(num + 1, num2 - (num + 1));
				int index = Convert.ToInt32(value2);
				object obj2 = null;
				obj2 = ((text.Length <= 0) ? obj : GetMember(obj, text, accessorFactory));
				if (obj2 is IList)
				{
					((IList)obj2)[index] = value;
					return;
				}
				throw new ProbeException("The '" + text + "' member of the " + obj.GetType().Name + " class is not a List or Array.");
			}
			catch (ProbeException ex)
			{
				throw ex;
			}
			catch (Exception ex2)
			{
				throw new ProbeException("Error getting ordinal value from .net object. Cause" + ex2.Message, ex2);
			}
		}

		/// <summary>
		/// Return the specified member on an object. 
		/// </summary>
		/// <param name="obj">The Object on which to invoke the specified property.</param>
		/// <param name="memberName">Name of the member.</param>
		/// <param name="accessorFactory">The accessor factory.</param>
		/// <returns>An Object representing the return value of the invoked property.</returns>
		public static object GetMemberValue(object obj, string memberName, AccessorFactory accessorFactory)
		{
			if (memberName.IndexOf('.') > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(memberName, ".");
				IEnumerator enumerator = stringTokenizer.GetEnumerator();
				object obj2 = obj;
				string text = null;
				while (enumerator.MoveNext())
				{
					text = (string)enumerator.Current;
					obj2 = GetMember(obj2, text, accessorFactory);
					if (obj2 == null)
					{
						break;
					}
				}
				return obj2;
			}
			return GetMember(obj, memberName, accessorFactory);
		}

		/// <summary>
		/// Gets the member's value on the specified object.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="memberName">Name of the member.</param>
		/// <param name="accessorFactory">The accessor factory.</param>
		/// <returns>The member's value</returns>
		public static object GetMember(object obj, string memberName, AccessorFactory accessorFactory)
		{
			try
			{
				object obj2 = null;
				if (memberName.IndexOf("[") > -1)
				{
					return GetArrayMember(obj, memberName, accessorFactory);
				}
				if (obj is IDictionary)
				{
					return ((IDictionary)obj)[memberName];
				}
				Type type = obj.GetType();
				IGetAccessorFactory getAccessorFactory = accessorFactory.GetAccessorFactory;
				IGetAccessor getAccessor = getAccessorFactory.CreateGetAccessor(type, memberName);
				if (getAccessor == null)
				{
					throw new ProbeException("No Get method for member " + memberName + " on instance of " + obj.GetType().Name);
				}
				try
				{
					return getAccessor.Get(obj);
				}
				catch (Exception ex)
				{
					throw new ProbeException(ex);
				}
			}
			catch (ProbeException ex2)
			{
				throw ex2;
			}
			catch (Exception ex3)
			{
				throw new ProbeException("Could not Set property '" + memberName + "' for " + obj.GetType().Name + ".  Cause: " + ex3.Message, ex3);
			}
		}

		/// <summary>
		/// Sets the member value.
		/// </summary>
		/// <param name="obj">he Object on which to invoke the specified mmber.</param>
		/// <param name="memberName">Name of the member.</param>
		/// <param name="memberValue">The member value.</param>
		/// <param name="objectFactory">The object factory.</param>
		/// <param name="accessorFactory">The accessor factory.</param>
		public static void SetMemberValue(object obj, string memberName, object memberValue, IObjectFactory objectFactory, AccessorFactory accessorFactory)
		{
			if (memberName.IndexOf('.') > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(memberName, ".");
				IEnumerator enumerator = stringTokenizer.GetEnumerator();
				enumerator.MoveNext();
				string text = (string)enumerator.Current;
				object obj2 = obj;
				while (enumerator.MoveNext())
				{
					Type memberTypeForSetter = GetMemberTypeForSetter(obj2, text);
					object obj3 = obj2;
					obj2 = GetMember(obj3, text, accessorFactory);
					if (obj2 == null)
					{
						try
						{
							IFactory factory = objectFactory.CreateFactory(memberTypeForSetter, Type.EmptyTypes);
							obj2 = factory.CreateInstance(Type.EmptyTypes);
							SetMemberValue(obj3, text, obj2, objectFactory, accessorFactory);
						}
						catch (Exception ex)
						{
							throw new ProbeException("Cannot set value of property '" + memberName + "' because '" + text + "' is null and cannot be instantiated on instance of " + memberTypeForSetter.Name + ". Cause:" + ex.Message, ex);
						}
					}
					text = (string)enumerator.Current;
				}
				SetMember(obj2, text, memberValue, accessorFactory);
			}
			else
			{
				SetMember(obj, memberName, memberValue, accessorFactory);
			}
		}

		/// <summary>
		/// Sets the member.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="memberName">Name of the member.</param>
		/// <param name="memberValue">The member value.</param>
		/// <param name="accessorFactory">The accessor factory.</param>
		public static void SetMember(object obj, string memberName, object memberValue, AccessorFactory accessorFactory)
		{
			try
			{
				if (memberName.IndexOf("[") > -1)
				{
					SetArrayMember(obj, memberName, memberValue, accessorFactory);
					return;
				}
				if (obj is IDictionary)
				{
					((IDictionary)obj)[memberName] = memberValue;
					return;
				}
				Type type = obj.GetType();
				ISetAccessorFactory setAccessorFactory = accessorFactory.SetAccessorFactory;
				ISetAccessor setAccessor = setAccessorFactory.CreateSetAccessor(type, memberName);
				if (setAccessor == null)
				{
					throw new ProbeException("No Set method for member " + memberName + " on instance of " + obj.GetType().Name);
				}
				try
				{
					setAccessorFactory.CreateSetAccessor(type, memberName).Set(obj, memberValue);
				}
				catch (Exception ex)
				{
					throw new ProbeException(ex);
				}
			}
			catch (ProbeException ex2)
			{
				throw ex2;
			}
			catch (Exception ex3)
			{
				throw new ProbeException("Could not Get property '" + memberName + "' for " + obj.GetType().Name + ".  Cause: " + ex3.Message, ex3);
			}
		}

		/// <summary>
		/// Checks to see if a Object has a writable property/field be a given name
		/// </summary>
		/// <param name="obj"> The object to check</param>
		/// <param name="propertyName">The property to check for</param>
		/// <returns>True if the property exists and is writable</returns>
		public static bool HasWritableProperty(object obj, string propertyName)
		{
			bool result = false;
			if (obj is IDictionary)
			{
				result = ((IDictionary)obj).Contains(propertyName);
			}
			else if (propertyName.IndexOf('.') > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(propertyName, ".");
				IEnumerator enumerator = stringTokenizer.GetEnumerator();
				Type type = obj.GetType();
				while (enumerator.MoveNext())
				{
					propertyName = (string)enumerator.Current;
					type = ReflectionInfo.GetInstance(type).GetGetterType(propertyName);
					result = ReflectionInfo.GetInstance(type).HasWritableMember(propertyName);
				}
			}
			else
			{
				result = ReflectionInfo.GetInstance(obj.GetType()).HasWritableMember(propertyName);
			}
			return result;
		}

		/// <summary>
		/// Checks to see if the Object have a property/field be a given name.
		/// </summary>
		/// <param name="obj">The Object on which to invoke the specified property.</param>
		/// <param name="propertyName">The name of the property to check for.</param>
		/// <returns>
		/// True or false if the property exists and is readable.
		/// </returns>
		public static bool HasReadableProperty(object obj, string propertyName)
		{
			bool result = false;
			if (obj is IDictionary)
			{
				result = ((IDictionary)obj).Contains(propertyName);
			}
			else if (propertyName.IndexOf('.') > -1)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(propertyName, ".");
				IEnumerator enumerator = stringTokenizer.GetEnumerator();
				Type type = obj.GetType();
				while (enumerator.MoveNext())
				{
					propertyName = (string)enumerator.Current;
					type = ReflectionInfo.GetInstance(type).GetGetterType(propertyName);
					result = ReflectionInfo.GetInstance(type).HasReadableMember(propertyName);
				}
			}
			else
			{
				result = ReflectionInfo.GetInstance(obj.GetType()).HasReadableMember(propertyName);
			}
			return result;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsSimpleType(Type type)
		{
			if (_simpleTypeMap.Contains(type))
			{
				return true;
			}
			if (typeof(ICollection).IsAssignableFrom(type))
			{
				return true;
			}
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				return true;
			}
			if (typeof(IList).IsAssignableFrom(type))
			{
				return true;
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				return true;
			}
			return false;
		}
	}
}
