using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace USFMSharp.Tools
{
	public class XmlSettings : IDisposable
	{
		private bool _disposed;
		private readonly string _fileName;
		private bool _changed;
		private readonly XmlDocument _xmlDoc;

		public enum SettingsType { User, Machine }

		public XmlSettings(SettingsType pType, string pCompanyName, string pApplicationName)
		{
			// name of settings file
			_fileName = get_File_Name(pType, pCompanyName, pApplicationName);

			// create if it does not exist yet
			if (!File.Exists(_fileName)) create_New_File();

			// now load the file
			_xmlDoc = new XmlDocument();
			_xmlDoc.Load(_fileName);

		}

		private static string get_File_Name(SettingsType settingType, string companyName, string applicationName)
		{
			var dirName = Environment.GetFolderPath(settingType == SettingsType.User ? Environment.SpecialFolder.LocalApplicationData : Environment.SpecialFolder.CommonApplicationData);

			if (!string.IsNullOrEmpty(companyName))
				dirName = Path.Combine(dirName, companyName);

			if (!string.IsNullOrEmpty(applicationName))
				dirName = Path.Combine(dirName, applicationName);

			return Path.Combine(dirName, settingType == SettingsType.User ? "UserSettings.config" : "AppSettings.config");
		}

		private void create_New_File()
		{
			var doc = new XmlDocument();

			// root element
			doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
			            "<configuration>" +
			            "</configuration>");

			// sections
			doc.DocumentElement?.AppendChild(doc.CreateNode(XmlNodeType.Element, "AppSettings", ""));
			doc.DocumentElement?.AppendChild(doc.CreateNode(XmlNodeType.Element, "connectionStrings", ""));

			// check for directory
			var fi = new FileInfo(_fileName);
			if (!Directory.Exists(fi.DirectoryName))
			{
				Debug.Assert(fi.DirectoryName != null, "fi.DirectoryName != null");
				Directory.CreateDirectory(fi.DirectoryName);
			}

			// write to disk
			doc.Save(_fileName);
		}

		public XmlNode Get_Section(string pSectionName)
		{
			return _xmlDoc.DocumentElement?.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => string.Compare(node.Name, pSectionName, StringComparison.Ordinal) == 0);
		}

		public string Get_String_Setting(string pSectionName, string pSettingName)
		{

			var section = Get_Section(pSectionName);

			// check for missing section
			if (section == null) return null;

			foreach (XmlNode node in section.ChildNodes)
			{
				if (string.Compare(node.Name, pSettingName, StringComparison.Ordinal) == 0)
				{
					Debug.Assert(node.Attributes != null, "node.Attributes != null");
					return node.Attributes.GetNamedItem("value").Value;
				}
			}

			// if you are here, it was not found
			return null;

		}

		public string Get_String_Setting(string pSectionName, string pSettingName, string pDefault)
		{

			var s = Get_String_Setting(pSectionName, pSettingName);

			if (s == null) return pDefault;
			return s;
		}

		public decimal Get_Numeric_Setting(string pSectionName, string pSettingName)
		{
			var tempVal = Get_String_Setting(pSectionName, pSettingName);

			return tempVal == null ? 0 : decimal.Parse(tempVal);
		}

		public void Set_String_Setting(string pSectionName, string pSettingName, string pValue)
		{
			var section = Get_Section(pSectionName);
			var exists = false;

			// check for missing section
			if (section == null)
			{
				Debug.Assert(_xmlDoc.DocumentElement != null, "_xmlDoc.DocumentElement != null");
				section = _xmlDoc.DocumentElement.AppendChild(_xmlDoc.CreateNode(XmlNodeType.Element, pSectionName, ""));
			}

			// set the value, if it exists
			foreach (XmlNode node in section.ChildNodes)
			{
				if (string.Compare(node.Name, pSettingName, StringComparison.Ordinal) == 0)
				{
					exists = true;
					_changed = true;

					var nodeExists = false;
					Debug.Assert(node.Attributes != null, "node.Attributes != null");
					foreach (XmlNode attr in node.Attributes)
					{
						if (string.Compare(attr.Name, "value", StringComparison.Ordinal) == 0)
						{
							nodeExists = true;
							attr.Value = pValue;
						}
					}

					if (!nodeExists)
					{
						var attr = _xmlDoc.CreateNode(XmlNodeType.Attribute, "value", "");
						attr.Value = pValue;
						node.Attributes.SetNamedItem(attr);
					}
				}
			}

			// add the setting if not found
			if (!exists)
			{
				var node = section.AppendChild(_xmlDoc.CreateNode(XmlNodeType.Element, pSettingName, ""));
				var attr = _xmlDoc.CreateNode(XmlNodeType.Attribute, "value", "");
				attr.Value = pValue;
				Debug.Assert(node.Attributes != null, "node.Attributes != null");
				node.Attributes.SetNamedItem(attr);

				_changed = true;
			}

		}

		public void Set_Numeric_Setting(string pSectionName, string pSettingName, decimal pValue)
		{
			Set_String_Setting(pSectionName, pSettingName, pValue.ToString(CultureInfo.InvariantCulture));
		}

		public void Save()
		{
			if (_changed)
			{
				_xmlDoc.Save(_fileName);

				_changed = false;
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool pDisposing)
		{
			if (!_disposed)
			{
				if (pDisposing)
				{
					// code for managed objects
				}

				Save();
			}
			_disposed = true;
		}
	}
}
