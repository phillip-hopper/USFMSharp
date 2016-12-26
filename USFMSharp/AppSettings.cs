using USFMSharp.Tools;

namespace USFMSharp
{
	public class AppSettings : XmlSettings
	{
		private const string AppSettingsSection = "appSettings";
		// private readonly string _boolTruePattern = ";yes;true;1;";

		public AppSettings()
			: base(SettingsType.User, "", "USFMSharp")
		{
			// constructor code
		}

		/// <summary>
		/// Default UI language is English
		/// </summary>
		public string UILanguage
		{
			get { return Get_String_Setting(AppSettingsSection, "uiLanguage", "en"); }
			set { Set_String_Setting(AppSettingsSection, "uiLanguage", value); }
		}
	}
}
