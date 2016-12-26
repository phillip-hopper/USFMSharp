using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;

namespace USFMSharp.UserControls
{
	public class MainViewStatusStrip : StatusStrip
	{
		private string _lineShort;
		private string _columnShort;
		private int _lineNumber = 1;
		private int _columnNumber = 1;

		public MainViewStatusStrip()
		{
			InitializeStrip();
		}

		private void InitializeStrip()
		{
			LocalizeItemDlg.StringsLocalized += HandleStringsLocalized;
			Items.AddRange(new ToolStripItem[] {
				new ToolStripStatusLabel(string.Empty),
				new ToolStripStatusLabel(string.Empty)
			});

			HandleStringsLocalized();
		}

		private void HandleStringsLocalized()
		{
			_lineShort = LocalizationManager.GetString("StatusStrip.LineShort", "Ln: {0}",
				"A short form of the word Line, used for displaying the line number.");
			_columnShort = LocalizationManager.GetString("StatusStrip.ColumnShort", "Col: {0}",
				"A short form of the word Column, used for displaying the column number.");

			DisplayCurrentPosition();
		}

		public void SetCurrentPosition(int lineNumber, int columnNumber)
		{
			_lineNumber = lineNumber;
			_columnNumber = columnNumber;
			DisplayCurrentPosition();
		}

		private void DisplayCurrentPosition()
		{
			Items[0].Text = string.Format(_lineShort, _lineNumber);
			Items[1].Text = string.Format(_columnShort, _columnNumber);
		}
	}
}
