using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SIL.PlatformUtilities;
using uw_edit.Views;

namespace uw_edit
{
	public static class Program
	{
	    private static string _appDirectory;

		/// <summary>The main entry point for the application.</summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

		    var viewModel = new MainViewModel();

		    var args = Environment.GetCommandLineArgs();
		    if (args.Length > 1)
		        viewModel.FileToOpen = args[1];

			Application.Run(new MainView(viewModel));
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

		public static string GetTextResource(string resourceFileName)
		{
			return File.ReadAllText(Path.Combine(Program.GetResourcesDirectory(), resourceFileName), Encoding.UTF8);
		}
	}
}
