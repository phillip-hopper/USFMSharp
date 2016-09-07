using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using uw_edit.UserControls;

namespace uw_edit.Views
{
    public class MainView : Form
    {
        MainViewModel _model;

        public MainView(MainViewModel model)
        {
			_model = model;
			_model.ExitProgram += _model_ExitProgram;

            InitializeForm();

            Load += HandleLoad;
            Closing += HandleClosing;
        }

        void InitializeForm()
        {
            SuspendLayout();

			Font = new Font(Font.FontFamily, 10);

            // layout the form
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 500);
            Name = "MainForm";
            Text = "tx-Edit";

			// tool strip
			var toolStrip = new MainViewStrip();
			toolStrip.StripItemClicked += _model.HandleStripItemClicked;
			toolStrip.Font = new Font(Font.FontFamily, 12);

			Controls.Add(toolStrip);

            // main menu
            var menuStrip = new MainViewMenu();
            menuStrip.MenuItemClicked += _model.HandleMenuItemClicked;
			menuStrip.Font = Font;
			menuStrip.Dock = DockStyle.Top;
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;

            // finished adding controls
            ResumeLayout(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // clean up objects that need it here
            }
            base.Dispose(disposing);
        }

        #region Form Events

        void HandleClosing(object sender, CancelEventArgs cancelEventArgs)
        {
//            throw new NotImplementedException();
        }

        void HandleLoad(object sender, EventArgs eventArgs)
        {
//            throw new NotImplementedException();
        }

		#endregion

		#region Control Events

		void _model_ExitProgram(object sender, EventArgs e)
		{
			Close();
		}

		#endregion
	}
}