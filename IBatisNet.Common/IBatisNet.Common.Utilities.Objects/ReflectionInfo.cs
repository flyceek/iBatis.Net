using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using IBatisNet.Common.Exceptions;

namespace IBatisNet.Common.Utilities.Objects
{
	/// <summary>
	/// This class represents a cached set of class definition information that
	/// allows for easy mapping between property names and get/set methods.
	/// </summary>
	public sealed class ReflectionInfo
	{
		/// <summary>
		///
		/// </summary>
		public static BindingFlags BINDING_FLAGS_PROPERTY;

		/// <summary>
		///
		/// </summary>
		public static BindingFlags BINDING_FLAGS_FIELD;

		private static readonly string[] _emptyStringArray;

		private static ArrayList _simleTypeMap;

		private static Hashtable _reflectionInfoMap;

		private string _className = string.Empty;

		private string[] _readableMemberNames = _emptyStringArray;

		private string[] _writeableMemberNames = _emptyStringArray;

		private Hashtable _setMembers = new Hashtable();

		private Hashtable _getMembers = new Hashtable();

		private Hashtable _setTypes = new Hashtable();

		private Hashtable _getTypes = new Hashtable();

		/// <summary>
		///
		/// </summary>
		public string ClassName => _className;

		/// <summary>
		///
		/// </summary>
		static ReflectionInfo()
		{
			BINDING_FLAGS_PROPERTY = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			BINDING_FLAGS_FIELD = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			_emptyStringArray = new string[0];
			_simleTypeMap = new ArrayList();
			_reflectionInfoMap = Hashtable.Synchronized(new Hashtable());
			_simleTypeMap.Add(typeof(string));
			_simleTypeMap.Add(typeof(byte));
			_simleTypeMap.Add(typeof(char));
			_simleTypeMap.Add(typeof(bool));
			_simleTypeMap.Add(typeof(Guid));
			_simleTypeMap.Add(typeof(short));
			_simleTypeMap.Add(typeof(int));
			_simleTypeMap.Add(typeof(long));
			_simleTypeMap.Add(typeof(float));
			_simleTypeMap.Add(typeof(double));
			_simleTypeMap.Add(typeof(decimal));
			_simleTypeMap.Add(typeof(DateTime));
			_simleTypeMap.Add(typeof(TimeSpan));
			_simleTypeMap.Add(typeof(Hashtable));
			_simleTypeMap.Add(typeof(SortedList));
			_simleTypeMap.Add(typeof(ListDictionary));
			_simleTypeMap.Add(typeof(HybridDictionary));
			_simleTypeMap.Add(typeof(ArrayList));
			_simleTypeMap.Add(typeof(IEnumerator));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		private ReflectionInfo(Type type)
		{
			_className = type.Name;
			AddMembers(type);
			string[] array = new string[_getMembers.Count];
			_getMembers.Keys.CopyTo(array, 0);
			_readableMemberNames = array;
			string[] array2 = new string[_setMembers.Count];
			_setMembers.Keys.CopyTo(array2, 0);
			_writeableMemberNames = array2;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		private void AddMembers(Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BINDING_FLAGS_PROPERTY);
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].CanWrite)
				{
					string name = properties[i].Name;
					_setMembers[name] = properties[i];
					_setTypes[name] = properties[i].PropertyType;
				}
				if (properties[i].CanRead)
				{
					string name = properties[i].Name;
					_getMembers[name] = properties[i];
					_getTypes[name] = properties[i].PropertyType;
				}
			}
			FieldInfo[] fields = type.GetFields(BINDING_FLAGS_FIELD);
			for (int i = 0; i < fields.Length; i++)
			{
				string name = fields[i].Name;
				_setMembers[name] = fields[i];
				_setTypes[name] = fields[i].FieldType;
				_getMembers[name] = fields[i];
				_getTypes[name] = fields[i].FieldType;
			}
			if (type.IsInterface)
			{
				Type[] interfaces = type.GetInterfaces();
				foreach (Type type2 in interfaces)
				{
					AddMembers(type2);
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="memberName"></param>
		/// <returns></returns>
		public MemberInfo GetSetter(string memberName)
		{
			MemberInfo memberInfo = (MemberInfo)_setMembers[memberName];
			if (memberInfo == null)
			{
				throw new ProbeException("There is no Set member named '" + memberName + "' in class '" + _className + "'");
			}
			return memberInfo;
		}

		/// <summary>
		/// Gets the <see cref="T:System.Reflection.MemberInfo" />.
		/// </summary>
		/// <param name="memberName">Member's name.</param>
		/// <returns>The <see cref="T:System.Reflection.MemberInfo" /></returns>
		public MemberInfo GetGetter(string memberName)
		{
			MemberInfo memberInfo = _getMembers[memberName] as MemberInfo;
			if (memberInfo == null)
			{
				throw new ProbeException("There is no Get member named '" + memberName + "' in class '" + _className + "'");
			}
			return memberInfo;
		}

		/// <summary>
		/// Gets the type of the member.
		/// </summary>
		/// <param name="memberName">Member's name.</param>
		/// <returns></returns>
		public Type GetSetterType(string memberName)
		{
			Type type = (Type)_setTypes[memberName];
			if (type == null)
			{
				throw new ProbeException("There is no Set member named '" + memberName + "' in class '" + _className + "'");
			}
			return type;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="memberName"></param>
		/// <returns></returns>
		public Type GetGetterType(string memberName)
		{
			Type type = (Type)_getTypes[memberName];
			if (type == null)
			{
				throw new ProbeException("There is no Get member named '" + memberName + "' in class '" + _className + "'");
			}
			return type;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public string[] GetReadableMemberNames()
		{
			return _readableMemberNames;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public string[] GetWriteableMemberNames()
		{
			return _writeableMemberNames;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="memberName"></param>
		/// <returns></returns>
		public bool HasWritableMember(string memberName)
		{
			return _setMembers.ContainsKey(memberName);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="memberName"></param>
		/// <returns></returns>
		public bool HasReadableMember(string memberName)
		{
			return _getMembers.ContainsKey(memberName);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsKnownType(Type type)
		{
			if (_simleTypeMap.Contains(type))
			{
				return true;
			}
			if (typeof(IList).IsAssignableFrom(type))
			{
				return true;
			}
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				return true;
			}
			if (typeof(IEnumerator).IsAssignableFrom(type))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets an instance of ReflectionInfo for the specified type.
		/// </summary>summary&gt;
		/// <param name="type">The type for which to lookup the method cache.</param>
		/// <returns>The properties cache for the type</returns>
		public static ReflectionInfo GetInstance(Type type)
		{
			lock (type)
			{
				ReflectionInfo reflectionInfo = (ReflectionInfo)_reflectionInfoMap[type];
				if (reflectionInfo == null)
				{
					reflectionInfo = new ReflectionInfo(type);
					_reflectionInfoMap.Add(type, reflectionInfo);
				}
				return reflectionInfo;
			}
		}
	}
}
