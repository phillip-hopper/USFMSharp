using System;
using System.Windows.Forms;

namespace uw_edit
{
	public class MainViewStrip : ToolStrip
	{
		public enum MainStripOption
		{
			Chapter,
			Verse,
			Paragraph
		}

		public event EventHandler<MainStripClickEventArgs> StripItemClicked;

		public MainViewStrip()
		{
			InitializeStrip();
		}

		void InitializeStrip()
		{
			SuspendLayout();

			var pBtn = new ToolStripButton("\\p");
			pBtn.ToolTipText = "Paragraph";

			var vBtn = new ToolStripButton("\\v");
			vBtn.ToolTipText = "Verse";

			Items.AddRange(new ToolStripItem[] {
				CreateButton("\\c", "Chapter", MainStripOption.Chapter),
				CreateButton("\\v", "Verse", MainStripOption.Verse),
				CreateButton("\\p", "Pragraph", MainStripOption.Paragraph)
			});

			ResumeLayout(true);
		}

		ToolStripButton CreateButton(string text, string toolTip, MainStripOption option)
		{
			var btn = new ToolStripButton(text);
			btn.ToolTipText = toolTip;
			btn.Click += (sender, e) => StripItemClicked?.Invoke(this, new MainStripClickEventArgs(option));
			return btn;
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

