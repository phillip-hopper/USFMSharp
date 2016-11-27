using System.Drawing;
using System.Windows.Forms;

namespace uw_edit.Views
{
	public class SaveChangesDialog : Form
	{
		public SaveChangesDialog(string promptText)
		{
			InitializeForm(promptText);
		}

		private void InitializeForm(string promptText)
		{
			SuspendLayout();

			Font = new Font(Font.FontFamily, 10);

			// layout the form
			Text = "Document has changed";
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
			var warningLabel = new Label
			{
				Text = "If you don't save, changes made since the last save will be permanently lost.",
				Dock = DockStyle.Top,
				TextAlign = ContentAlignment.TopCenter,
				Margin = new Padding(6)
			};
			tbl.Controls.Add(warningLabel, 0, 1);
			tbl.SetColumnSpan(warningLabel, 3);

			// buttons
			var saveButton = new Button
			{
				Text = "Save",
				AutoSize = true,
				Width = 180
			};
			saveButton.Click += (object sender, System.EventArgs e) => { DialogResult = DialogResult.OK; };
			tbl.Controls.Add(saveButton, 2, 2);

			var cancelButton = new Button
			{
				Text = "Cancel",
				AutoSize = true,
				Width = 180
			};
			cancelButton.Click += (object sender, System.EventArgs e) => { DialogResult = DialogResult.Cancel; };
			tbl.Controls.Add(cancelButton, 1, 2);

			var noSaveButton = new Button
			{
				Text = "Close without Saving",
				AutoSize = true,
				Width = 180
			};
			noSaveButton.Click += (object sender, System.EventArgs e) => { DialogResult = DialogResult.Ignore; };
			tbl.Controls.Add(noSaveButton, 0, 2);

			Controls.Add(tbl);

			// finished adding controls
			ResumeLayout(true);
		}
	}
}