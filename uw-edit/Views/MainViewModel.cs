﻿using System;
using System.IO;
using System.Windows.Forms;
using Gecko;
using uw_edit.UserControls;
using uw_edit.USFM;

namespace uw_edit.Views
{
    public class MainViewModel
    {
		public event EventHandler ExitProgram;

		public Browser Browser { get; set; }

        private string _fileToOpen;

		public MainViewModel()
		{
		    Browser = new Browser {Dock = DockStyle.Fill};
		    Browser.DocumentCompleted += Browser_DocumentCompleted;
		}

        public string FileToOpen
        {
            get { return _fileToOpen; }
            set
            {
                // process a relative path
                if (value.StartsWith(".", StringComparison.Ordinal))
                    value = Path.GetFullPath(Path.Combine(Program.GetAppDirectory(), value));

                _fileToOpen = value;
            }
        }

        public void LoadTemplate()
        {
            var minifier = new Microsoft.Ajax.Utilities.Minifier();

			// add our usfm stylesheet
			var css = File.ReadAllText(Path.Combine(Program.GetResourcesDirectory(), "USFMTemplate.css"));
			//css = minifier.MinifyStyleSheet(css);
			var s = "<style>\r\n" + css + "</style>\r\n";

			// add jquery
			s += "<script type=\"text/javascript\">\r\n" + File.ReadAllText(Path.Combine(Program.GetResourcesDirectory(), "jquery.slim.min.js")) + "</script>\r\n";

			// add our javascript
			var js = File.ReadAllText(Path.Combine(Program.GetResourcesDirectory(), "USFMTemplate.js"));
			//js = minifier.MinifyJavaScript(js);
			s += "<script type=\"text/javascript\">\r\n" + js + "</script>\r\n";

			// put it all together
			var html = File.ReadAllText(Path.Combine(Program.GetResourcesDirectory(), "USFMTemplate.html"));
            html = html.Replace("</head>", s + "</head>");

			// if no usfm file was selected when starting, open the default template
            if (string.IsNullOrEmpty(FileToOpen))
			    FileToOpen = Path.Combine(Program.GetResourcesDirectory(), "USFMTemplate.usfm");

            Browser.WebBrowser.LoadHtml(html);
        }

		#region Event Handlers

		public void HandleMenuItemClicked(object sender, MainMenuClickEventArgs eventArgs)
		{
			switch (eventArgs.ItemClicked)
			{
				case MainViewMenu.MainMenuOption.FileExit:
					ExitProgram?.Invoke(this, eventArgs);
					return;
			}
		}

		public void HandleStripItemClicked(object sender, MainStripClickEventArgs eventArgs)
		{
			switch (eventArgs.ItemClicked)
			{
				case MainViewStrip.MainStripOption.Chapter:
					Browser.InsertTag("\\c ", " ");
					break;

				case MainViewStrip.MainStripOption.Verse:
					Browser.InsertText("\\v ");
					break;
					
				case MainViewStrip.MainStripOption.Paragraph:
					//ExitProgram?.Invoke(this, eventArgs);
					//nsIDOMWindowUtils utils = Xpcom.QueryInterface<nsIDOMWindowUtils>(Browser.WebBrowser.Window.DomWindow);
					nsIDOMWindowUtils utils = Xpcom.QueryInterface<nsIDOMWindowUtils>(Browser.WebBrowser.Window.DomWindow);
					Browser.WebBrowser.Window.WindowUtils.SendKeyEvent("keypress", 0, 102, 0, false);
					return;
			}
		}

        private void Browser_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
		{
			// if a file name was passed as a command-line parameter, load it now
			if (!string.IsNullOrEmpty(FileToOpen))
			{
				TextTools.SetUsfmFromFile(Browser.WebBrowser, FileToOpen);
			    Browser.RunJavascript("usfmContent.focus(); markUsfmTags();");
				FileToOpen = string.Empty;
			}
		}

		#endregion
	}
}