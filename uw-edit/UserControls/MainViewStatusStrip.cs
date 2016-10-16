using System.Windows.Forms;

namespace uw_edit.UserControls
{
	public class MainViewStatusStrip : StatusStrip
	{
		public MainViewStatusStrip()
		{
			InitializeStrip();
		}

		void InitializeStrip()
		{
			Items.AddRange(new ToolStripItem[] {
				new ToolStripStatusLabel("Ln: 1"),
				new ToolStripStatusLabel("Col: 1")
			});
		}
	}
}
