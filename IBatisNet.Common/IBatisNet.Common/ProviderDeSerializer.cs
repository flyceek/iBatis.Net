using System.Collections.Specialized;
using System.Xml;
using IBatisNet.Common.Xml;

namespace IBatisNet.Common
{
	/// <summary>
	/// Summary description for ProviderDeSerializer.
	/// </summary>
	public sealed class ProviderDeSerializer
	{
		/// <summary>
		/// Deserializes the specified node in a <see cref="T:IBatisNet.Common.IDbProvider" />.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns>The <see cref="T:IBatisNet.Common.IDbProvider" /></returns>
		public static IDbProvider Deserialize(XmlNode node)
		{
			IDbProvider dbProvider = new DbProvider();
			NameValueCollection nameValueCollection = NodeUtils.ParseAttributes(node);
			dbProvider.AssemblyName = nameValueCollection["assemblyName"];
			dbProvider.CommandBuilderClass = nameValueCollection["commandBuilderClass"];
			dbProvider.DbCommandClass = nameValueCollection["commandClass"];
			dbProvider.DbConnectionClass = nameValueCollection["connectionClass"];
			dbProvider.DataAdapterClass = nameValueCollection["dataAdapterClass"];
			dbProvider.Description = nameValueCollection["description"];
			dbProvider.IsDefault = NodeUtils.GetBooleanAttribute(nameValueCollection, "default", def: false);
			dbProvider.IsEnabled = NodeUtils.GetBooleanAttribute(nameValueCollection, "enabled", def: true);
			dbProvider.Name = nameValueCollection["name"];
			dbProvider.ParameterDbTypeClass = nameValueCollection["parameterDbTypeClass"];
			dbProvider.ParameterDbTypeProperty = nameValueCollection["parameterDbTypeProperty"];
			dbProvider.ParameterPrefix = nameValueCollection["parameterPrefix"];
			dbProvider.SetDbParameterPrecision = NodeUtils.GetBooleanAttribute(nameValueCollection, "setDbParameterPrecision", def: true);
			dbProvider.SetDbParameterScale = NodeUtils.GetBooleanAttribute(nameValueCollection, "setDbParameterScale", def: true);
			dbProvider.SetDbParameterSize = NodeUtils.GetBooleanAttribute(nameValueCollection, "setDbParameterSize", def: true);
			dbProvider.UseDeriveParameters = NodeUtils.GetBooleanAttribute(nameValueCollection, "useDeriveParameters", def: true);
			dbProvider.UseParameterPrefixInParameter = NodeUtils.GetBooleanAttribute(nameValueCollection, "useParameterPrefixInParameter", def: true);
			dbProvider.UseParameterPrefixInSql = NodeUtils.GetBooleanAttribute(nameValueCollection, "useParameterPrefixInSql", def: true);
			dbProvider.UsePositionalParameters = NodeUtils.GetBooleanAttribute(nameValueCollection, "usePositionalParameters", def: false);
			dbProvider.AllowMARS = NodeUtils.GetBooleanAttribute(nameValueCollection, "allowMARS", def: false);
			return dbProvider;
		}
	}
}
