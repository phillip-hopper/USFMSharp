using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using uw_edit.UserControls;

namespace uw_edit.Views
{
    public class MainView : Form
    {
        private MainViewModel _model;

        public MainView(MainViewModel model)
        {
            InitializeForm();

            Load += HandleLoad;
            Closing += HandleClosing;

            _model = model;
        }

        private void InitializeForm()
        {
            SuspendLayout();

            // layout the form
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 500);
            Name = "MainForm";
            Text = "tx-Edit";

            // main menu
            var menuStrip = new MainViewMenu();
            menuStrip.MenuExitClicked += HandleMenuExitClicked;
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

        private void HandleMenuExitClicked(object sender, EventArgs eventArgs)
        {
            Close();
        }

        #endregion
    }
}