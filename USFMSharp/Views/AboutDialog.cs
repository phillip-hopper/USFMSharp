using System.Drawing;
using System.Windows.Forms;

namespace USFMSharp.Views
{
	public class AboutDialog : Form
	{
		public AboutDialog()
		{
			InitializeForm();
		}

		private void InitializeForm()
		{
			SuspendLayout();

			Font = new Font(Font.FontFamily, 10);

			// layout the form
			Text = "About USFMSharp";
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			AutoSize = true;
			MinimizeBox = false;
			MaximizeBox = false;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			StartPosition = FormStartPosition.CenterParent;
		}
	}
}