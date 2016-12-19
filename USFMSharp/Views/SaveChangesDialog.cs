using System.Drawing;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;

namespace USFMSharp.Views
{
	public class SaveChangesDialog : Form
	{
		private Label _warningLabel;
		private Button _saveButton;
		private Button _cancelButton;
		private Button _noSaveButton;

		public SaveChangesDialog(string promptText)
		{
			InitializeForm(promptText);
			LocalizeItemDlg.StringsLocalized += HandleStringsLocalized;

			HandleStringsLocalized();
		}

		private void InitializeForm(string promptText)
		{
			SuspendLayout();

			Font = new Font(Font.FontFamily, 10);

			// layout the form
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			AutoSize = true;
			MinimizeBox = false;
			MaximizeBox = false;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			StartPosition = FormStartPosition.CenterParent;

			// table layout
			var tbl = new TableLayoutPanel
			{
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				AutoSize = true,
				Margin = new Padding(6),
				RowCount = 3,
				ColumnCount = 3,
				Top = 6,
				Left = 6
			};

			// prompt
			var promptLabel = new Label
			{
				Text = promptText,
				Dock = DockStyle.Top,
				TextAlign = ContentAlignment.TopCenter,
				Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
				Margin = new Padding(6)
			};
			tbl.Controls.Add(promptLabel, 0, 0);
			tbl.SetColumnSpan(promptLabel, 3);

			// warning
			_warningLabel = new Label
			{
				Dock = DockStyle.Top,
				TextAlign = ContentAlignment.TopCenter,
				Margin = new Padding(6)
			};
			tbl.Controls.Add(_warningLabel, 0, 1);
			tbl.SetColumnSpan(_warningLabel, 3);

			// buttons
			_saveButton = new Button
			{
				AutoSize = true,
				Width = 180
			};
			_saveButton.Click += (sender, e) => { DialogResult = DialogResult.OK; };
			tbl.Controls.Add(_saveButton, 2, 2);

			_cancelButton = new Button
			{
				AutoSize = true,
				Width = 180
			};
			_cancelButton.Click += (sender, e) => { DialogResult = DialogResult.Cancel; };
			tbl.Controls.Add(_cancelButton, 1, 2);

			_noSaveButton = new Button
			{
				AutoSize = true,
				Width = 180
			};
			_noSaveButton.Click += (sender, e) => { DialogResult = DialogResult.Ignore; };
			tbl.Controls.Add(_noSaveButton, 0, 2);

			Controls.Add(tbl);

			// finished adding controls
			ResumeLayout(true);
		}

		private void HandleStringsLocalized()
		{
			Text = LocalizationManager.GetString("SaveChangesDialog.Title", "Document has changed",
				"Text for the title bar of the dialog.");

			_warningLabel.Text = LocalizationManager.GetString("SaveChangesDialog.Warning",
				"If you don't save, changes made since the last save will be permanently lost.",
				"Text letting the user know changes may be lost.");

			_saveButton.Text = LocalizationManager.GetString("SaveChangesDialog.SaveButton", "Save",
				"Text of the Save button.");

			_cancelButton.Text = LocalizationManager.GetString("SaveChangesDialog.CancelButton", "Cancel",
				"Text of the Cancel button.");

			_noSaveButton.Text = LocalizationManager.GetString("SaveChangesDialog.NoSaveButton", "Close without Saving",
				"Text of the Close without Saving button.");
		}
	}
}