using System;
using System.Xml.Serialization;

namespace IBatisNet.Common
{
	/// <summary>
	/// Information about a data source.
	/// </summary>
	[Serializable]
	[XmlRoot("dataSource", Namespace = "http://ibatis.apache.org/dataMapper")]
	public class DataSource : IDataSource
	{
		[NonSerialized]
		private string _connectionString = string.Empty;

		[NonSerialized]
		private IDbProvider _provider;

		[NonSerialized]
		private string _name = string.Empty;

		/// <summary>
		/// The connection string.
		/// </summary>
		[XmlAttribute("connectionString")]
		public virtual string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				CheckPropertyString("ConnectionString", value);
				_connectionString = value;
			}
		}

		/// <summary>
		/// DataSource Name
		/// </summary>
		[XmlAttribute("name")]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				CheckPropertyString("Name", value);
				_name = value;
			}
		}

		/// <summary>
		/// The provider to use for this data source.
		/// </summary>
		[XmlIgnore]
		public virtual IDbProvider DbProvider
		{
			get
			{
				return _provider;
			}
			set
			{
				_provider = value;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public DataSource()
		{
		}

		private void CheckPropertyString(string propertyName, string value)
		{
			if (value == null || value.Trim().Length == 0)
			{
				throw new ArgumentException("The " + propertyName + " property cannot be set to a null or empty string value.", propertyName);
			}
		}

		/// <summary>
		/// ToString implementation.
		/// </summary>
		/// <returns>A string that describes the data source</returns>
		public override string ToString()
		{
			return "Source: ConnectionString : " + ConnectionString;
		}
	}
}
