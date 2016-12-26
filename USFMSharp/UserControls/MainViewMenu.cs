using System;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;

namespace USFMSharp.UserControls
{
    public class MainViewMenu : MenuStrip
    {
	    private ToolStripMenuItem _fileMenu;
	    private ToolStripMenuItem _fileExitMenu;
	    private ToolStripMenuItem _fileOpenMenu;
	    private ToolStripMenuItem _fileSaveMenu;
	    private ToolStripMenuItem _fileNewMenu;

	    private ToolStripMenuItem _viewMenu;
	    private ToolStripMenuItem _viewWrapMenu;
	    private ToolStripMenuItem _viewRefreshMenu;

	    private ToolStripMenuItem _toolsMenu;
	    private ToolStripMenuItem _toolsS5Menu;
	    private ToolStripMenuItem _toolsTrailingSpace;
	    private ToolStripMenuItem _toolsBlankLines;

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

	    private void InitializeMenu()
        {
            SuspendLayout();

			#region file menu
			_fileMenu = new ToolStripMenuItem();
	        _fileMenu.Text = LocalizationManager.GetString("MainMenu.File", "File", null, null, null, _fileMenu);

	        // file menu sub-items
	        _fileExitMenu = new ToolStripMenuItem();
	        _fileExitMenu.Text = LocalizationManager.GetString("MainMenu.FileQuit", "Quit", null, null, "Ctrl+Q", _fileExitMenu);
	        _fileExitMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileExit));

			_fileOpenMenu = new ToolStripMenuItem();
			_fileOpenMenu.Text = LocalizationManager.GetString("MainMenu.FileOpen", "Open", null, null, "Ctrl+O", _fileOpenMenu);
			_fileOpenMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileOpen));

			_fileSaveMenu = new ToolStripMenuItem();
			_fileSaveMenu.Text = LocalizationManager.GetString("MainMenu.FileSave", "Save", null, null, "Ctrl+S", _fileSaveMenu);
			_fileSaveMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileSave));

			_fileNewMenu = new ToolStripMenuItem();
			_fileNewMenu.Text = LocalizationManager.GetString("MainMenu.FileNew", "New", null, null, "Ctrl+N", _fileNewMenu);
			_fileNewMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.FileNew));

			_fileMenu.DropDownItems.AddRange(new ToolStripItem[] { _fileNewMenu, _fileOpenMenu, _fileSaveMenu, _fileExitMenu });
			#endregion

			#region view menu
	        _viewMenu = new ToolStripMenuItem();
	        _viewMenu.Text = LocalizationManager.GetString("MainMenu.View", "View", null, null, null, _viewMenu);

	        // view menu sub-items
			_viewWrapMenu = new ToolStripMenuItem() { CheckOnClick = true, Checked = false };
	        _viewWrapMenu.Text = LocalizationManager.GetString("MainMenu.ViewWrap", "Word wrap", null, null, "Ctrl+W", _viewWrapMenu);
	        _viewWrapMenu.Click += ViewWrapMenu_Click;

			_viewRefreshMenu = new ToolStripMenuItem();
	        _viewRefreshMenu.Text = LocalizationManager.GetString("MainMenu.ViewRefresh", "Refresh", null, null, "F5", _viewRefreshMenu);
	        _viewRefreshMenu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.ViewRefresh));

			_viewMenu.DropDownItems.AddRange(new ToolStripItem[] { _viewWrapMenu, _viewRefreshMenu });
			#endregion

			#region tools menu
			_toolsMenu = new ToolStripMenuItem();
	        _toolsMenu.Text = LocalizationManager.GetString("MainMenu.Tools", "Tools", null, null, null, _toolsMenu);

	        // remove \s5 tags
			_toolsS5Menu = new ToolStripMenuItem();
	        _toolsS5Menu.Text = LocalizationManager.GetString("MainMenu.ToolsRemoveS5", "Remove all \\s5 tags", null, null, null, _toolsS5Menu);
	        _toolsS5Menu.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.ToolsRemoveS5));

			// remove trailing spaces
			_toolsTrailingSpace = new ToolStripMenuItem();
	        _toolsTrailingSpace.Text = LocalizationManager.GetString("MainMenu.ToolsTrailingSpace", "Remove trailing spaces", null, null, null, _toolsTrailingSpace);
	        _toolsTrailingSpace.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.ToolsRemoveTrailingSpace));

			// remove blank lines
			_toolsBlankLines = new ToolStripMenuItem();
	        _toolsBlankLines.Text = LocalizationManager.GetString("MainMenu.ToolsRemoveBlankLines", "Remove blank lines", null, null, null, _toolsBlankLines);
	        _toolsBlankLines.Click += (sender, e) => MenuItemClicked?.Invoke(this, new MainMenuClickEventArgs(MainMenuOption.ToolsRemoveBlankLines));

			_toolsMenu.DropDownItems.AddRange(new ToolStripItem[] { _toolsS5Menu, _toolsTrailingSpace, _toolsBlankLines });
			#endregion

            // finalize the main menu
            Items.AddRange(new ToolStripItem[] {_fileMenu, _viewMenu, _toolsMenu});

            ResumeLayout(true);
        }

	    private void ViewWrapMenu_Click(object sender, EventArgs e)
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