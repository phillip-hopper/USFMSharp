using System;
using System.Windows.Forms;

namespace uw_edit.UserControls
{
    public class MainViewMenu : MenuStrip
    {
		public enum MainMenuOption
		{
			FileExit
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

            // build the main menu
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] {fileExitMenu});

            // finalize the main menu
            Items.AddRange(new ToolStripItem[] {fileMenu});

            ResumeLayout(true);
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