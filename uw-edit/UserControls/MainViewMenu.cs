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
			ViewRefresh,
			ToolsRemoveS5,
			ToolsRemoveTrailingSpace,
			ToolsRemoveBlankLines
		}

        public event EventHandler<MainMenuClickEventArgs> MenuItemClicked;

        public MainViewMenu()
        {
            InitializeMenu();
        }

        void InitializeMenu()
        {
            SuspendLayout();

			#region file menu
			var fileMenu = new ToolStripMenuItem("File");

			// file menu sub-items
			var fileExitMenu = new ToolStripMenuItem("Quit") { ShortcutKeys = Keys.Control | Keys.Q };
			fileExitMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileExit));

			var fileOpenMenu = new ToolStripMenuItem("Open") { ShortcutKeys = Keys.Control | Keys.O };
			fileOpenMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileOpen));

			var fileSaveMenu = new ToolStripMenuItem("Save") { ShortcutKeys = Keys.Control | Keys.S };
			fileSaveMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileSave));

			var fileNewMenu = new ToolStripMenuItem("New") { ShortcutKeys = Keys.Control | Keys.N };
			fileNewMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileNew));

			fileMenu.DropDownItems.AddRange(new ToolStripItem[] { fileNewMenu, fileOpenMenu, fileSaveMenu, fileExitMenu });
			#endregion

			#region view menu
			var viewMenu = new ToolStripMenuItem("View");

			// view menu sub-items
			var viewWrapMenu = new ToolStripMenuItem("Word wrap") { ShortcutKeys = Keys.Control | Keys.W, CheckOnClick = true, Checked = false };
			viewWrapMenu.Click += ViewWrapMenu_Click;

			var viewRefreshMenu = new ToolStripMenuItem("Refresh") { ShortcutKeys = Keys.F5 };
			viewRefreshMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.ViewRefresh));

			viewMenu.DropDownItems.AddRange(new ToolStripItem[] { viewWrapMenu, viewRefreshMenu });
			#endregion

			#region tools menu
			var toolsMenu = new ToolStripMenuItem("Tools");

			// remove \s5 tags
			var toolsS5Menu = new ToolStripMenuItem("Remove all \\s5 tags");
			toolsS5Menu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.ToolsRemoveS5));

			// remove trailing spaces
			var toolsTrailingSpace = new ToolStripMenuItem("Remove trailing spaces");
			toolsTrailingSpace.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.ToolsRemoveTrailingSpace));

			// remove trailing spaces
			var toolsBlankLines = new ToolStripMenuItem("Remove blank lines");
			toolsBlankLines.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.ToolsRemoveBlankLines));

			toolsMenu.DropDownItems.AddRange(new ToolStripItem[] { toolsS5Menu, toolsTrailingSpace, toolsBlankLines });
			#endregion

            // finalize the main menu
            Items.AddRange(new ToolStripItem[] {fileMenu, viewMenu, toolsMenu});

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