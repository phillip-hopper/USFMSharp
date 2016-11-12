using System;
using System.Windows.Forms;

namespace uw_edit.UserControls
{
    public class MainViewMenu : MenuStrip
    {
		public enum MainMenuOption
		{
			FileExit,
			FileOpen,
			FileSave,
			FileNew,
			WordWrapOn,
			WordWrapOff,
			ViewRefresh
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
			var fileMenu = new ToolStripMenuItem("File");

			// file menu sub-items
			var fileExitMenu = new ToolStripMenuItem("Exit") { ShortcutKeys = Keys.Control | Keys.X };
			fileExitMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileExit));

			var fileOpenMenu = new ToolStripMenuItem("Open") { ShortcutKeys = Keys.Control | Keys.O };
			fileOpenMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileOpen));

			var fileSaveMenu = new ToolStripMenuItem("Save") { ShortcutKeys = Keys.Control | Keys.S };
			fileSaveMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileSave));

			var fileNewMenu = new ToolStripMenuItem("New") { ShortcutKeys = Keys.Control | Keys.N };
			fileNewMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileNew));

			fileMenu.DropDownItems.AddRange(new ToolStripItem[] { fileNewMenu, fileOpenMenu, fileSaveMenu, fileExitMenu });

			// view menu
			var viewMenu = new ToolStripMenuItem("View");

			// view menu sub-items
			var viewWrapMenu = new ToolStripMenuItem("Word wrap") { ShortcutKeys = Keys.Control | Keys.W, CheckOnClick = true, Checked = false };
			viewWrapMenu.Click += ViewWrapMenu_Click;

			var viewRefreshMenu = new ToolStripMenuItem("Refresh") { ShortcutKeys = Keys.F5 };
			viewRefreshMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.ViewRefresh));

			viewMenu.DropDownItems.AddRange(new ToolStripItem[] { viewWrapMenu, viewRefreshMenu });

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