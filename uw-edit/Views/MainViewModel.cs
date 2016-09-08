using System;
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

		public MainViewModel()
		{
			Browser = new Browser();
			Browser.Dock = DockStyle.Fill;
			Browser.DocumentCompleted += Browser_DocumentCompleted;
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
					var sel = TextTools.GetSelection(Browser.WebBrowser);
					Console.Out.WriteLine(sel);
					break;

				case MainViewStrip.MainStripOption.Verse:
					break;
					
				case MainViewStrip.MainStripOption.Paragraph:
					ExitProgram?.Invoke(this, eventArgs);
					return;
			}
		}

		void Browser_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
		{
			var doc = Browser.WebBrowser.Document;
			var found = (GeckoHtmlElement)doc.GetElementById("usfm-content");
			//found.InnerHtml = "there";
		}

		#endregion
	}
}