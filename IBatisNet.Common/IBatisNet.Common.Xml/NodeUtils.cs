using System.Collections.Specialized;
using System.Xml;

namespace IBatisNet.Common.Xml
{
	/// <summary>
	/// Summary description for NodeUtils.
	/// </summary>
	public sealed class NodeUtils
	{
		/// <summary>
		/// Searches for the attribute with the specified name in this attributes list.
		/// </summary>
		/// <param name="attributes"></param>
		/// <param name="name">The key</param>
		/// <returns></returns>
		public static string GetStringAttribute(NameValueCollection attributes, string name)
		{
			string text = attributes[name];
			if (text == null)
			{
				return string.Empty;
			}
			return text;
		}

		/// <summary>
		/// Searches for the attribute with the specified name in this attributes list.
		/// </summary>
		/// <param name="attributes"></param>
		/// <param name="name">The key</param>
		/// <param name="def">The default value to be returned if the attribute is not found.</param>
		/// <returns></returns>
		public static string GetStringAttribute(NameValueCollection attributes, string name, string def)
		{
			string text = attributes[name];
			if (text == null)
			{
				return def;
			}
			return text;
		}

		/// <summary>
		/// Searches for the attribute with the specified name in this attributes list.
		/// </summary>
		/// <param name="attributes"></param>
		/// <param name="name">The key</param>
		/// <param name="def">The default value to be returned if the attribute is not found.</param>
		/// <returns></returns>
		public static byte GetByteAttribute(NameValueCollection attributes, string name, byte def)
		{
			string text = attributes[name];
			if (text == null)
			{
				return def;
			}
			return XmlConvert.ToByte(text);
		}

		/// <summary>
		/// Searches for the attribute with the specified name in this attributes list.
		/// </summary>
		/// <param name="attributes"></param>
		/// <param name="name">The key</param>
		/// <param name="def">The default value to be returned if the attribute is not found.</param>
		/// <returns></returns>
		public static int GetIntAttribute(NameValueCollection attributes, string name, int def)
		{
			string text = attributes[name];
			if (text == null)
			{
				return def;
			}
			return XmlConvert.ToInt32(text);
		}

		/// <summary>
		/// Searches for the attribute with the specified name in this attributes list.
		/// </summary>
		/// <param name="attributes"></param>
		/// <param name="name">The key</param>
		/// <param name="def">The default value to be returned if the attribute is not found.</param>
		/// <returns></returns>
		public static bool GetBooleanAttribute(NameValueCollection attributes, string name, bool def)
		{
			string text = attributes[name];
			if (text == null)
			{
				return def;
			}
			return XmlConvert.ToBoolean(text);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static NameValueCollection ParseAttributes(XmlNode node)
		{
			return ParseAttributes(node, null);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="node"></param>
		/// <param name="variables"></param>
		/// <returns></returns>
		public static NameValueCollection ParseAttributes(XmlNode node, NameValueCollection variables)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			int count = node.Attributes.Count;
			for (int i = 0; i < count; i++)
			{
				XmlAttribute xmlAttribute = node.Attributes[i];
				string value = ParsePropertyTokens(xmlAttribute.Value, variables);
				nameValueCollection.Add(xmlAttribute.Name, value);
			}
			return nameValueCollection;
		}

		/// <summary>
		/// Replace properties by their values in the given string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		public static string ParsePropertyTokens(string str, NameValueCollection properties)
		{
			string text = "${";
			string text2 = "}";
			string text3 = str;
			if (text3 != null && properties != null)
			{
				int num = text3.IndexOf(text);
				int num2 = text3.IndexOf(text2);
				while (num > -1 && num2 > num)
				{
					string str2 = text3.Substring(0, num);
					string str3 = text3.Substring(num2 + text2.Length);
					int num3 = num + text.Length;
					string text4 = text3.Substring(num3, num2 - num3);
					string text5 = properties.Get(text4);
					text3 = ((text5 != null) ? (str2 + text5 + str3) : (str2 + text4 + str3));
					num = text3.IndexOf(text);
					num2 = text3.IndexOf(text2);
				}
			}
			return text3;
		}
	}
}
