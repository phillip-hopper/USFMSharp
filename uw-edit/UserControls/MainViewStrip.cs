using System;
using System.Windows.Forms;

namespace uw_edit
{
	public class MainViewStrip : ToolStrip
	{
		public MainViewStrip()
		{
			InitializeStrip();
		}

		private void InitializeStrip()
		{
			SuspendLayout();

			var pBtn = new ToolStripButton("\\p");
			pBtn.ToolTipText = "Paragraph";

			var vBtn = new ToolStripButton("\\v");
			vBtn.ToolTipText = "Verse";

			Items.AddRange(new ToolStripItem[] {pBtn, vBtn});

			ResumeLayout(true);
		}
	}
}

