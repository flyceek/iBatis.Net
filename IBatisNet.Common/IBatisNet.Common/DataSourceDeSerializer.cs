using System.Collections.Specialized;
using System.Xml;
using IBatisNet.Common.Xml;

namespace IBatisNet.Common
{
	/// <summary>
	/// Summary description for DataSourceDeSerializer.
	/// </summary>
	public sealed class DataSourceDeSerializer
	{
		/// <summary>
		/// Deserialize a DataSource object
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static DataSource Deserialize(XmlNode node)
		{
			DataSource dataSource = new DataSource();
			NameValueCollection nameValueCollection = NodeUtils.ParseAttributes(node);
			dataSource.ConnectionString = nameValueCollection["connectionString"];
			dataSource.Name = nameValueCollection["name"];
			return dataSource;
		}
	}
}
