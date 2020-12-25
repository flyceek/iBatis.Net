using IBatisNet.DataMapper.DataExchange;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
	public interface IResultMap
	{
		[XmlIgnore]
		ResultPropertyCollection Parameters
		{
			get;
		}

		[XmlIgnore]
		ResultPropertyCollection Properties
		{
			get;
		}

		[XmlIgnore]
		ResultPropertyCollection GroupByProperties
		{
			get;
		}

		[XmlAttribute("id")]
		string Id
		{
			get;
		}

		[XmlIgnore]
		StringCollection GroupByPropertyNames
		{
			get;
		}

		[XmlIgnore]
		System.Type Class
		{
			get;
		}

		[XmlIgnore]
		IDataExchange DataExchange
		{
			set;
		}

		[XmlIgnore]
		bool IsInitalized
		{
			get;
			set;
		}

		object CreateInstanceOfResult(object[] parameters);

		void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue);

		IResultMap ResolveSubMap(IDataReader dataReader);
	}
}
