using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Xml;
using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Logging;
using IBatisNet.Common.Xml;

namespace IBatisNet.Common.Utilities
{
	/// <summary>
	/// A class to simplify access to resources.
	///
	/// The file can be loaded from the application root directory 
	/// (use the resource attribute) 
	/// or from any valid URL (use the url attribute). 
	/// For example,to load a fixed path file, use:
	/// &lt;properties url=攆ile:///c:/config/my.properties?/&gt;
	/// </summary>
	public class Resources
	{
		/// <summary>
		/// Holds data about a <see cref="T:System.Type" /> and it's
		/// attendant <see cref="T:System.Reflection.Assembly" />.
		/// </summary>
		internal class FileAssemblyInfo
		{
			/// <summary>
			/// The string that separates file name
			/// from their attendant <see cref="T:System.Reflection.Assembly" />
			/// names in an assembly qualified type name.
			/// </summary>
			public const string FileAssemblySeparator = ",";

			private string _unresolvedAssemblyName = string.Empty;

			private string _unresolvedFileName = string.Empty;

			private string _originalFileName = string.Empty;

			/// <summary>
			/// The resource file name .
			/// </summary>
			public string ResourceFileName => AssemblyName + "." + FileName;

			/// <summary>
			/// The original name.
			/// </summary>
			public string OriginalFileName => _originalFileName;

			/// <summary>
			/// The file name portion.
			/// </summary>
			public string FileName => _unresolvedFileName;

			/// <summary>
			/// The (unresolved, possibly partial) name of the attandant assembly.
			/// </summary>
			public string AssemblyName => _unresolvedAssemblyName;

			/// <summary>
			/// Is the type name being resolved assembly qualified?
			/// </summary>
			public bool IsAssemblyQualified
			{
				get
				{
					if (AssemblyName == null || AssemblyName.Trim().Length == 0)
					{
						return false;
					}
					return true;
				}
			}

			/// <summary>
			/// Creates a new instance of the FileAssemblyInfo class.
			/// </summary>
			/// <param name="unresolvedFileName">
			/// The unresolved name of a <see cref="T:System.Type" />.
			/// </param>
			public FileAssemblyInfo(string unresolvedFileName)
			{
				SplitFileAndAssemblyNames(unresolvedFileName);
			}

			/// <summary>
			///
			/// </summary>
			/// <param name="originalFileName"></param>
			private void SplitFileAndAssemblyNames(string originalFileName)
			{
				_originalFileName = originalFileName;
				int num = originalFileName.IndexOf(",");
				if (num < 0)
				{
					_unresolvedFileName = originalFileName.Trim();
					_unresolvedAssemblyName = null;
				}
				else
				{
					_unresolvedFileName = originalFileName.Substring(0, num).Trim();
					_unresolvedAssemblyName = originalFileName.Substring(num + 1).Trim();
				}
			}
		}

		/// <summary>
		/// Protocole separator
		/// </summary>
		public const string PROTOCOL_SEPARATOR = "://";

		private static string _applicationBase;

		private static string _baseDirectory;

		private static readonly ILog _logger;

		/// <summary>
		/// The name of the directory containing the application
		/// </summary>
		public static string ApplicationBase => _applicationBase;

		/// <summary>
		/// The name of the directory used to probe the assemblies.
		/// </summary>
		public static string BaseDirectory => _baseDirectory;

		static Resources()
		{
			_applicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			_baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			_logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		/// <summary>
		/// Strips protocol name from the resource name
		/// </summary>
		/// <param name="filePath">Name of the resource</param>
		/// <returns>Name of the resource without protocol name</returns>
		public static string GetFileSystemResourceWithoutProtocol(string filePath)
		{
			int num = filePath.IndexOf("://");
			if (num == -1)
			{
				return filePath;
			}
			if (filePath.Length > num + "://".Length)
			{
				while (filePath[++num] == '/')
				{
				}
			}
			return filePath.Substring(num);
		}

		/// <summary>
		/// Get config file
		/// </summary>
		/// <param name="resourcePath">
		/// A config resource path.
		/// </param>
		/// <returns>An XmlDocument representation of the config file</returns>
		public static XmlDocument GetConfigAsXmlDocument(string resourcePath)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlTextReader xmlTextReader = null;
			resourcePath = GetFileSystemResourceWithoutProtocol(resourcePath);
			if (!FileExists(resourcePath))
			{
				resourcePath = Path.Combine(_baseDirectory, resourcePath);
			}
			try
			{
				xmlTextReader = new XmlTextReader(resourcePath);
				xmlDocument.Load(xmlTextReader);
			}
			catch (Exception ex)
			{
				throw new ConfigurationException($"Unable to load config file \"{resourcePath}\". Cause : {ex.Message}", ex);
			}
			finally
			{
				xmlTextReader?.Close();
			}
			return xmlDocument;
		}

		/// <summary>
		/// Determines whether the specified file exists.
		/// </summary>
		/// <param name="filePath">The file to check.</param>
		/// <returns>
		/// true if the caller has the required permissions and path contains the name of an existing file
		/// false if the caller has the required permissions and path doesn't contain the name of an existing file
		/// else exception
		/// </returns>
		public static bool FileExists(string filePath)
		{
			if (File.Exists(filePath))
			{
				return true;
			}
			FileIOPermission fileIOPermission = null;
			try
			{
				fileIOPermission = new FileIOPermission(FileIOPermissionAccess.Read, filePath);
			}
			catch
			{
				return false;
			}
			try
			{
				fileIOPermission.Demand();
			}
			catch (Exception ex)
			{
				throw new ConfigurationException($"iBATIS doesn't have the right to read the config file \"{filePath}\". Cause : {ex.Message}", ex);
			}
			return false;
		}

		/// <summary>
		/// Load an XML resource from a location specify by the node.
		/// </summary>
		/// <param name="node">An location node</param>
		/// <param name="properties">the global properties</param>
		/// <returns>Return the Xml document load.</returns>
		public static XmlDocument GetAsXmlDocument(XmlNode node, NameValueCollection properties)
		{
			XmlDocument result = null;
			if (node.Attributes["resource"] != null)
			{
				string resource = NodeUtils.ParsePropertyTokens(node.Attributes["resource"].Value, properties);
				result = GetResourceAsXmlDocument(resource);
			}
			else if (node.Attributes["url"] != null)
			{
				string url = NodeUtils.ParsePropertyTokens(node.Attributes["url"].Value, properties);
				result = GetUrlAsXmlDocument(url);
			}
			else if (node.Attributes["embedded"] != null)
			{
				string resource2 = NodeUtils.ParsePropertyTokens(node.Attributes["embedded"].Value, properties);
				result = GetEmbeddedResourceAsXmlDocument(resource2);
			}
			return result;
		}

		/// <summary>
		/// Get the path resource of an url or resource location.
		/// </summary>
		/// <param name="node">The specification from where to load.</param>
		/// <param name="properties">the global properties</param>
		/// <returns></returns>
		public static string GetValueOfNodeResourceUrl(XmlNode node, NameValueCollection properties)
		{
			string result = null;
			if (node.Attributes["resource"] != null)
			{
				string path = NodeUtils.ParsePropertyTokens(node.Attributes["resource"].Value, properties);
				result = Path.Combine(_applicationBase, path);
			}
			else if (node.Attributes["url"] != null)
			{
				string text = NodeUtils.ParsePropertyTokens(node.Attributes["url"].Value, properties);
				result = text;
			}
			return result;
		}

		/// <summary>
		/// Get XmlDocument from a stream resource
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public static XmlDocument GetStreamAsXmlDocument(Stream resource)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(resource);
			}
			catch (Exception ex)
			{
				throw new ConfigurationException($"Unable to load XmlDocument via stream. Cause : {ex.Message}", ex);
			}
			return xmlDocument;
		}

		/// <summary>
		/// Get XmlDocument from a FileInfo resource
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public static XmlDocument GetFileInfoAsXmlDocument(FileInfo resource)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(resource.FullName);
			}
			catch (Exception ex)
			{
				throw new ConfigurationException($"Unable to load XmlDocument via FileInfo. Cause : {ex.Message}", ex);
			}
			return xmlDocument;
		}

		/// <summary>
		/// Get XmlDocument from a Uri resource
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public static XmlDocument GetUriAsXmlDocument(Uri resource)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(resource.AbsoluteUri);
			}
			catch (Exception ex)
			{
				throw new ConfigurationException($"Unable to load XmlDocument via Uri. Cause : {ex.Message}", ex);
			}
			return xmlDocument;
		}

		/// <summary>
		/// Get XmlDocument from relative (from root directory of the application) path resource
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public static XmlDocument GetResourceAsXmlDocument(string resource)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(Path.Combine(_applicationBase, resource));
			}
			catch (Exception ex)
			{
				throw new ConfigurationException($"Unable to load file via resource \"{resource}\" as resource. Cause : {ex.Message}", ex);
			}
			return xmlDocument;
		}

		/// <summary>
		/// Get XmlDocument from absolute path resource
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static XmlDocument GetUrlAsXmlDocument(string url)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(url);
			}
			catch (Exception ex)
			{
				throw new ConfigurationException($"Unable to load file via url \"{url}\" as url. Cause : {ex.Message}", ex);
			}
			return xmlDocument;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public static XmlDocument GetEmbeddedResourceAsXmlDocument(string resource)
		{
			XmlDocument xmlDocument = new XmlDocument();
			bool flag = false;
			FileAssemblyInfo fileAssemblyInfo = new FileAssemblyInfo(resource);
			if (fileAssemblyInfo.IsAssemblyQualified)
			{
				Assembly assembly = null;
				assembly = Assembly.Load(fileAssemblyInfo.AssemblyName);
				if (!assembly.IsDynamic)
				{
					Stream manifestResourceStream = assembly.GetManifestResourceStream(fileAssemblyInfo.ResourceFileName);
					if (manifestResourceStream == null)
					{
						manifestResourceStream = assembly.GetManifestResourceStream(fileAssemblyInfo.FileName);
					}
					if (manifestResourceStream != null)
					{
						try
						{
							xmlDocument.Load(manifestResourceStream);
							flag = true;
						}
						catch (Exception ex)
						{
							throw new ConfigurationException($"Unable to load file \"{resource}\" in embedded resource. Cause : {ex.Message}", ex);
						}
					}
				}
			}
			else
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				Assembly[] array = assemblies;
				foreach (Assembly assembly in array)
				{
					if (assembly.IsDynamic)
					{
						continue;
					}
					Stream manifestResourceStream = assembly.GetManifestResourceStream(fileAssemblyInfo.FileName);
					if (manifestResourceStream != null)
					{
						try
						{
							xmlDocument.Load(manifestResourceStream);
							flag = true;
						}
						catch (Exception ex)
						{
							throw new ConfigurationException(string.Format("Unable to load file \"{0}\" in embedded resource. Cause : ", resource, ex.Message), ex);
						}
						break;
					}
				}
			}
			if (!flag)
			{
				_logger.Error("Could not load embedded resource from assembly");
				throw new ConfigurationException($"Unable to load embedded resource from assembly \"{fileAssemblyInfo.OriginalFileName}\".");
			}
			return xmlDocument;
		}

		/// <summary>
		/// Load a file from a given resource path
		/// </summary>
		/// <param name="resourcePath">
		/// The resource path
		/// </param>
		/// <returns>return a FileInfo</returns>
		public static FileInfo GetFileInfo(string resourcePath)
		{
			FileInfo fileInfo = null;
			resourcePath = GetFileSystemResourceWithoutProtocol(resourcePath);
			if (!FileExists(resourcePath))
			{
				resourcePath = Path.Combine(_applicationBase, resourcePath);
			}
			try
			{
				return new FileInfo(resourcePath);
			}
			catch (Exception ex)
			{
				throw new ConfigurationException($"Unable to load file \"{resourcePath}\". Cause : \"{ex.Message}\"", ex);
			}
		}

		/// <summary>
		/// Resolves the supplied type name into a <see cref="T:System.Type" /> instance.
		/// </summary>
		/// <param name="typeName">
		/// The (possibly partially assembly qualified) name of a <see cref="T:System.Type" />.
		/// </param>
		/// <returns>
		/// A resolved <see cref="T:System.Type" /> instance.
		/// </returns>
		/// <exception cref="T:System.TypeLoadException">
		/// If the type cannot be resolved.
		/// </exception>
		[Obsolete("Use IBatisNet.Common.Utilities.TypeUtils")]
		public static Type TypeForName(string typeName)
		{
			return TypeUtils.ResolveType(typeName);
		}
	}
}
