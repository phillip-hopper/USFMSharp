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
		private MainViewStatusStrip _statusStrip;
		private readonly bool _loaded;
		private readonly string _formDisplayName = "uw-Edit";
		private SplitContainer _splitter;

        public MainView(MainViewModel model)
        {
			_model = model;
			_model.ExitProgram += _model_ExitProgram;
			_model.SelectionChanged += _model_SelectionChanged;
			_model.FileLoaded += _model_FileLoaded;

			Application.UseWaitCursor = true;

            InitializeForm();

			Application.UseWaitCursor = false;

            Load += HandleLoad;
            Closing += HandleClosing;

            _model.LoadUsfmFile();

			_loaded = true;
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
            Text = _formDisplayName;

			// editor image
			Controls.Add(_model.RichTextImage);

			// splitter
			_splitter = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
			Controls.Add(_splitter);

			// editor control
            _model.RichText.Font = new Font(Font.FontFamily, 12);
			_splitter.Panel1.Controls.Add(_model.RichText);

			// error list
			_splitter.Panel2.Controls.Add(_model.ErrorList);
			_splitter.SplitterDistance = 100;

			// tool strip
			_model.ToolStrip.Font = new Font(Font.FontFamily, 12);
			Controls.Add(_model.ToolStrip);

            // main menu
			_model.MenuStrip.Font = Font;
            Controls.Add(_model.MenuStrip);
            MainMenuStrip = _model.MenuStrip;

			// status bar
			_statusStrip = new MainViewStatusStrip();
			_statusStrip.Font = new Font(Font.FontFamily, 10);
			_statusStrip.Dock = DockStyle.Bottom;
			Controls.Add(_statusStrip);

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
			cancelEventArgs.Cancel |= !_model.FreeToOpen();
        }

        private void HandleLoad(object sender, EventArgs eventArgs)
        {
//            throw new NotImplementedException();
        }

		void _model_FileLoaded(object sender, FileLoadedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.FileName))
			{
				Text = string.Format("{0}  –  {1}", _formDisplayName, "New File");
			}
			else 
			{
				Text = string.Format("{0}  –  {1}", _formDisplayName, e.FileName);
			}
		}

		#endregion

		#region Control Events

		private void _model_ExitProgram(object sender, EventArgs e)
		{
			Close();
		}

		void _model_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_loaded) return;

			_statusStrip.Items[0].Text = string.Format("Ln: {0}", e.LineNumber);
			_statusStrip.Items[1].Text = string.Format("Col: {0}", e.ColumnNumber);
		}

		#endregion

	}
}