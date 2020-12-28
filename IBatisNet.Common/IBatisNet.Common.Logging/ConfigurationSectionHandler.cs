using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Logging.Impl;

namespace IBatisNet.Common.Logging
{
	/// <summary>
	/// Used in an application's configuration file (App.Config or Web.Config) to configure the logging subsystem.
	/// </summary>
	/// <remarks>
	/// <example>
	/// An example configuration section that writes IBatisNet messages to the Console using the built-in Console Logger.
	/// <code lang="XML" escaped="true">
	/// <configuration>
	/// 	<configSections>
	/// 		<sectionGroup name="iBATIS">
	/// 			<section name="logging" type="IBatisNet.Common.Logging.ConfigurationSectionHandler, IBatisNet.Common" />
	/// 		</sectionGroup>	
	/// 	</configSections>
	/// 	<iBATIS>
	/// 		<logging>
	/// 			<logFactoryAdapter type="IBatisNet.Common.Logging.Impl.ConsoleOutLoggerFA, IBatisNet.Common">
	/// 				<arg key="showLogName" value="true" />
	/// 				<arg key="showDataTime" value="true" />
	/// 				<arg key="level" value="ALL" />
	/// 				<arg key="dateTimeFormat" value="yyyy/MM/dd HH:mm:ss:SSS" />
	/// 			</logFactoryAdapter>
	/// 		</logging>
	/// 	</iBATIS>
	/// </configuration>
	/// </code> 
	/// </example>
	/// <para>
	/// The following aliases are recognized for the type attribute of logFactoryAdapter: 
	/// </para>
	/// <list type="table">
	/// <item><term>CONSOLE</term><description>Alias for IBatisNet.Common.Logging.Impl.ConsoleOutLoggerFA, IBatisNet.Common</description></item>
	/// <item><term>TRACE</term><description>Alias for IBatisNet.Common.Logging.Impl.TraceLoggerFA, IBatisNet.Common</description></item>
	/// <item><term>NOOP</term><description>Alias IBatisNet.Common.Logging.Impl.NoOpLoggerFA, IBatisNet.Common</description></item>
	/// </list>
	/// </remarks>
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		private static readonly string LOGFACTORYADAPTER_ELEMENT = "logFactoryAdapter";

		private static readonly string LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB = "type";

		private static readonly string ARGUMENT_ELEMENT = "arg";

		private static readonly string ARGUMENT_ELEMENT_KEY_ATTRIB = "key";

		private static readonly string ARGUMENT_ELEMENT_VALUE_ATTRIB = "value";

		/// <summary>
		/// Constructor
		/// </summary>
		public ConfigurationSectionHandler()
		{
		}

		/// <summary>
		/// Retrieves the <see cref="T:System.Type" /> of the logger the use by looking at the logFactoryAdapter element
		/// of the logging configuration element.
		/// </summary>
		/// <param name="section"></param>
		/// <returns>
		/// A <see cref="T:IBatisNet.Common.Logging.LogSetting" /> object containing the specified type that implements 
		/// <see cref="T:IBatisNet.Common.Logging.ILoggerFactoryAdapter" /> along with zero or more properties that will be 
		/// passed to the logger factory adapter's constructor as an <see cref="T:System.Collections.IDictionary" />.
		/// </returns>
		private LogSetting ReadConfiguration(XmlNode section)
		{
			XmlNode xmlNode = section.SelectSingleNode(LOGFACTORYADAPTER_ELEMENT);
			string text = string.Empty;
			if (xmlNode.Attributes[LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB] != null)
			{
				text = xmlNode.Attributes[LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB].Value;
			}
			if (text == string.Empty)
			{
				throw new IBatisNet.Common.Exceptions.ConfigurationException("Required Attribute '" + LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB + "' not found in element '" + LOGFACTORYADAPTER_ELEMENT + "'");
			}
			Type type = null;
			try
			{
				type = ((string.Compare(text, "CONSOLE", ignoreCase: true) == 0) ? typeof(ConsoleOutLoggerFA) : ((string.Compare(text, "TRACE", ignoreCase: true) == 0) ? typeof(TraceLoggerFA) : ((string.Compare(text, "NOOP", ignoreCase: true) != 0) ? Type.GetType(text, throwOnError: true, ignoreCase: false) : typeof(NoOpLoggerFA))));
			}
			catch (Exception inner)
			{
				throw new IBatisNet.Common.Exceptions.ConfigurationException("Unable to create type '" + text + "'", inner);
			}
			XmlNodeList xmlNodeList = xmlNode.SelectNodes(ARGUMENT_ELEMENT);
			NameValueCollection nameValueCollection = null;
			nameValueCollection = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
			foreach (XmlNode item in xmlNodeList)
			{
				string empty = string.Empty;
				string value = string.Empty;
				XmlAttribute xmlAttribute = item.Attributes[ARGUMENT_ELEMENT_KEY_ATTRIB];
				XmlAttribute xmlAttribute2 = item.Attributes[ARGUMENT_ELEMENT_VALUE_ATTRIB];
				if (xmlAttribute == null)
				{
					throw new IBatisNet.Common.Exceptions.ConfigurationException("Required Attribute '" + ARGUMENT_ELEMENT_KEY_ATTRIB + "' not found in element '" + ARGUMENT_ELEMENT + "'");
				}
				empty = xmlAttribute.Value;
				if (xmlAttribute2 != null)
				{
					value = xmlAttribute2.Value;
				}
				nameValueCollection.Add(empty, value);
			}
			return new LogSetting(type, nameValueCollection);
		}

		/// <summary>
		/// Verifies that the logFactoryAdapter element appears once in the configuration section.
		/// </summary>
		/// <param name="parent">The parent of the current item.</param>
		/// <param name="configContext">Additional information about the configuration process.</param>
		/// <param name="section">The configuration section to apply an XPath query too.</param>
		/// <returns>
		/// A <see cref="T:IBatisNet.Common.Logging.LogSetting" /> object containing the specified logFactoryAdapter type
		/// along with user supplied configuration properties.
		/// </returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			int count = section.SelectNodes(LOGFACTORYADAPTER_ELEMENT).Count;
			if (count > 1)
			{
				throw new IBatisNet.Common.Exceptions.ConfigurationException("Only one <logFactoryAdapter> element allowed");
			}
			if (count == 1)
			{
				return ReadConfiguration(section);
			}
			return null;
		}
	}
}
