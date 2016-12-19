using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SIL.PlatformUtilities;
using USFMSharp.USFM;
using USFMSharp.Views;
using L10NSharp;

namespace USFMSharp
{
	public static class Program
	{
	    private static string _appDirectory;
		private static Font _textFont;
		public static ParatextStylesheet StyleSheet { get; private set; }
		public static MainView MainView { get; private set; }
		public static LocalizationManager LocalizationManager { get; private set; }
		public static AppSettings Settings { get; private set; }

		/// <summary>The main entry point for the application.</summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(true);

			Settings = new AppSettings();

			SetUpLocalization();

		    var viewModel = new MainViewModel();
			StyleSheet = new ParatextStylesheet();

		    var args = Environment.GetCommandLineArgs();
		    if (args.Length > 1)
		        viewModel.FileToOpen = args[1];

			MainView = new MainView(viewModel);
			LocalizationManager.ReapplyLocalizationsToAllObjects("USFMSharp");
			try
			{
				Application.Run(MainView);
			}
			finally
			{
				FinalCleanup();
			}
		}

		/// <summary>
		/// This function runs when the application is shutting down.
		/// </summary>
		private static void FinalCleanup()
		{
			LocalizationManager = null;

			if (Settings != null)
			{
				Settings.Save();
				Settings = null;
			}
		}

		private static void SetUpLocalization()
		{
			LocalizationManager = LocalizationManager.Create(Settings.UILanguage, "USFMSharp", "USFMSharp",
			                                                 Application.ProductVersion, 
			                                                 Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			                                                 "USFMSharp",
			                                                 Icon.ExtractAssociatedIcon(Application.ExecutablePath),
			                                                 "",
			                                                 "USFMSharp");
		}

		public static string GetAppDirectory()
		{
			if (!string.IsNullOrEmpty(_appDirectory)) return _appDirectory;

			var asm = Assembly.GetExecutingAssembly();
			var file = asm.CodeBase.Replace("file://", string.Empty);
			if (Platform.IsWindows)
				file = file.TrimStart('/');
			_appDirectory = Path.GetDirectoryName(file);

			return _appDirectory;
		}

		public static string GetResourcesDirectory()
		{
			return Path.Combine(GetAppDirectory(), "Resources");
		}

		public static string GetTextResource(string resourceFileName)
		{
			return File.ReadAllText(Path.Combine(GetResourcesDirectory(), resourceFileName), Encoding.UTF8);
		}

		public static Image GetImageResource(string imageFileName)
		{
			return Image.FromFile(Path.Combine(GetResourcesDirectory(), imageFileName));
		}

		public static Font GetTextFont()
		{
			if (_textFont != null)
				return _textFont;
			
			// Search for fonts in this order. These fonts appear to cover most languages.
			//   "Nirmala UI" is a Windows font.
			//   "FreeSerif" is a Linux font.
			var preferredFonts = new[] { "Nirmala UI", "FreeSerif" };

			foreach (var fontName in preferredFonts)
			{
				foreach (var family in FontFamily.Families)
				{
					if (family.Name != fontName) continue;
					_textFont = new Font(family, 11, FontStyle.Regular);
					return _textFont;
				}
			}

			// fall through
			_textFont = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Regular);
			return _textFont;
		}
	}
}
