using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;

namespace USFMSharp.UserControls
{
	public class MainViewStrip : ToolStrip
	{
		public ToolStripButton SaveButton { get; private set; }
		public ToolStripDropDownButton LanguageButton { get; private set; }

		public enum MainStripOption
		{
			Save,
			UiLanguage
		}

		public event EventHandler<MainStripClickEventArgs> StripItemClicked;

		public MainViewStrip()
		{
			InitializeStrip();
			SetupUiLanguageMenu();
			LocalizeItemDlg.StringsLocalized += HandleStringsLocalized;

			HandleStringsLocalized();
		}

		private void InitializeStrip()
		{
			SuspendLayout();

			MinimumSize = new Size(22, 22);

			SaveButton = new ToolStripButton { Image = Program.GetImageResource("save.png") };
			SaveButton.Click += (sender, e) => StripItemClicked?.Invoke(this, new MainStripClickEventArgs(MainStripOption.Save));

			LanguageButton = new ToolStripDropDownButton("English");

			Items.AddRange(new ToolStripItem[]
			{
				SaveButton,
				new ToolStripSeparator(),
				LanguageButton
			});

			ResumeLayout(true);
		}

		private void HandleStringsLocalized()
		{
			SaveButton.ToolTipText = LocalizationManager.GetString("MainButtonStrip.SaveButton_ToolTip_", "Save");
			LanguageButton.ToolTipText = LocalizationManager.GetString("MainButtonStrip.UILanguage_ToolTip_", "User-interface language");
		}

		private void SetupUiLanguageMenu()
		{
			LanguageButton.DropDownItems.Clear();
			foreach (var lang in LocalizationManager.GetUILanguages(true))
			{
				var item = LanguageButton.DropDownItems.Add(lang.NativeName);
				item.Tag = lang;
				var languageId = ((CultureInfo)item.Tag).IetfLanguageTag;
				item.Click += ((a, b) =>
				{
					LocalizationManager.SetUILanguage(languageId, true);
					Program.Settings.UILanguage = languageId;
					item.Select();
					LanguageButton.Text = ((CultureInfo)item.Tag).NativeName;
				});

				if (languageId == Program.Settings.UILanguage)
					LanguageButton.Text = ((CultureInfo)item.Tag).NativeName;
			}

			LanguageButton.DropDownItems.Add(new ToolStripSeparator());
			var menu = LanguageButton.DropDownItems.Add(LocalizationManager.GetString("MainButtonStrip.MoreMenuItem",
				"More...", "Last item in menu of UI languages"));
			menu.Click += ((a, b) =>
			{
				Program.LocalizationManager.ShowLocalizationDialogBox(false);
				SetupUiLanguageMenu();
			});
		}
	}

	public class MainStripClickEventArgs : EventArgs
	{
		public MainViewStrip.MainStripOption ItemClicked;

		public MainStripClickEventArgs(MainViewStrip.MainStripOption itemClicked)
		{
			ItemClicked = itemClicked;
		}
	}
}
