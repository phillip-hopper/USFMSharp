using System;
using System.Windows.Forms;

namespace uw_edit.UserControls
{
    public class MainViewMenu : MenuStrip
    {
        public event EventHandler MenuExitClicked;

        public MainViewMenu()
        {
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            SuspendLayout();

            // file menu
            var fileMenu = new ToolStripMenuItem("&File");

            // file menu sub-items
            var fileExitMenu = new ToolStripMenuItem("E&xit");
            fileExitMenu.Click += FileExitMenuOnClick;

            // build the main menu
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] {fileExitMenu});

            // finalize the main menu
            Items.AddRange(new ToolStripItem[] {fileMenu});

            ResumeLayout(true);
        }

        private void FileExitMenuOnClick(object sender, EventArgs eventArgs)
        {
            MenuExitClicked?.Invoke(this, eventArgs);
        }
    }
}