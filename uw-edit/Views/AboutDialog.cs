using System.Drawing;
using System.Windows.Forms;

namespace uw_edit.Views
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
			Text = "About uw-edit";
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			AutoSize = true;
			MinimizeBox = false;
			MaximizeBox = false;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			StartPosition = FormStartPosition.CenterParent;
		}
	}
}