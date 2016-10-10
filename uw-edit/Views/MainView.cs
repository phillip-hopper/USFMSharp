using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using uw_edit.UserControls;

namespace uw_edit.Views
{
    public class MainView : Form
    {
        private readonly MainViewModel _model;

        public MainView(MainViewModel model)
        {
			_model = model;
			_model.ExitProgram += _model_ExitProgram;

			Application.UseWaitCursor = true;

            InitializeForm();

			Application.UseWaitCursor = false;

            Load += HandleLoad;
            Closing += HandleClosing;

            _model.LoadTemplate();
        }

        private void InitializeForm()
        {
            SuspendLayout();

			Font = new Font(Font.FontFamily, 10);

            // layout the form
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 500);
            Name = "MainForm";
            Text = "tx-Edit";

			// editor image
			Controls.Add(_model.RichTextImage);

			// editor control
            _model.RichText.Font = new Font(Font.FontFamily, 12);
            Controls.Add(_model.RichText);

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

        private void HandleClosing(object sender, CancelEventArgs cancelEventArgs)
        {
//            throw new NotImplementedException();
        }

        private void HandleLoad(object sender, EventArgs eventArgs)
        {
//            throw new NotImplementedException();
        }

		#endregion

		#region Control Events

        private void _model_ExitProgram(object sender, EventArgs e)
		{
			Close();
		}

		#endregion
	}
}