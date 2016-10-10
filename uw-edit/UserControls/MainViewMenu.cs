using System;
using System.Windows.Forms;

namespace uw_edit.UserControls
{
    public class MainViewMenu : MenuStrip
    {
		public enum MainMenuOption
		{
			FileExit,
			WordWrapOn,
			WordWrapOff
		}

        public event EventHandler<MainMenuClickEventArgs> MenuItemClicked;

        public MainViewMenu()
        {
            InitializeMenu();
        }

        void InitializeMenu()
        {
            SuspendLayout();

            // file menu
            var fileMenu = new ToolStripMenuItem("&File");

            // file menu sub-items
            var fileExitMenu = new ToolStripMenuItem("E&xit");
			fileExitMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileExit));

			fileMenu.DropDownItems.AddRange(new ToolStripItem[] { fileExitMenu });

			// view menu
			var viewMenu = new ToolStripMenuItem("&View");

			// view menu sub-items
			var viewWrapMenu = new ToolStripMenuItem("&Word wrap") { CheckOnClick = true, Checked = true };
			viewWrapMenu.Click += ViewWrapMenu_Click;

			viewMenu.DropDownItems.AddRange(new ToolStripItem[] { viewWrapMenu });

            // finalize the main menu
            Items.AddRange(new ToolStripItem[] {fileMenu, viewMenu});

            ResumeLayout(true);
        }

		void ViewWrapMenu_Click(object sender, EventArgs e)
		{
			MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(((ToolStripMenuItem)sender).Checked ? MainMenuOption.WordWrapOn : MainMenuOption.WordWrapOff));
		}
	}

	public class MainMenuClickEventArgs : EventArgs
	{
		public MainViewMenu.MainMenuOption ItemClicked;

		public MainMenuClickEventArgs(MainViewMenu.MainMenuOption itemClicked)
		{
			ItemClicked = itemClicked;
		}
	}
}