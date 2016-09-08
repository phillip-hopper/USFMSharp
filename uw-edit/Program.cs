using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Gecko;
using SIL.PlatformUtilities;
using uw_edit.UserControls;
using uw_edit.Views;

namespace uw_edit
{
	public static class Program
	{
		static string _appDirectory;

		/// <summary>The main entry point for the application.</summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Browser.SetUpXulRunner();

			Application.ApplicationExit += (sender, e) =>
			{
				Xpcom.Shutdown();
			};

			Application.Run(new MainView(new MainViewModel()));
		}

		public static string GetAppDirectory()
		{
			if (string.IsNullOrEmpty(_appDirectory))
			{
				var asm = Assembly.GetExecutingAssembly();
				var file = asm.CodeBase.Replace("file://", string.Empty);
				if (Platform.IsWindows)
					file = file.TrimStart('/');
				_appDirectory = Path.GetDirectoryName(file);
			}

			return _appDirectory;
		}

		public static string GetResourcesDirectory()
		{
			return Path.Combine(GetAppDirectory(), "Resources");
		}
	}
}
