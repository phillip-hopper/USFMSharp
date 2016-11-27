using System;
using System.Drawing;
using System.Windows.Forms;

namespace uw_edit.UserControls
{
	public class MainViewStrip : ToolStrip
	{
		public ToolStripButton SaveButton { get; private set; }

		public enum MainStripOption
		{
			Save
		}

		public event EventHandler<MainStripClickEventArgs> StripItemClicked;

		public MainViewStrip()
		{
			InitializeStrip();
		}

		void InitializeStrip()
		{
			SuspendLayout();

			MinimumSize = new Size(18, 18);
			SaveButton = CreateButton("save.png", "Save", MainStripOption.Save);

			Items.AddRange(new ToolStripItem[] {
				SaveButton,
				new ToolStripSeparator()
			});

			ResumeLayout(true);
		}

		ToolStripButton CreateButton(string imageFileName, string toolTip, MainStripOption option)
		{
			var btn = new ToolStripButton();
			btn.Image = Program.GetImageResource(imageFileName);
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

