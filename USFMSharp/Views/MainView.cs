using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using L10NSharp;
using L10NSharp.UI;
using USFMSharp.UserControls;

namespace USFMSharp.Views
{
	public class MainView : Form
	{
		private readonly MainViewModel _model;
		private MainViewStatusStrip _statusStrip;
		private readonly bool _loaded;
		private const string FormDisplayName = "USFMSharp";
		private SplitContainer _splitter;
		private string _newFile;

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
			LocalizeItemDlg.StringsLocalized += HandleStringsLocalized;

			_model.LoadUsfmFile();

			_loaded = true;

			HandleStringsLocalized();
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
			Text = FormDisplayName;

			// editor image
			Controls.Add(_model.RichTextImage);

			// splitter
			_splitter = new SplitContainer {Dock = DockStyle.Fill, Orientation = Orientation.Horizontal};
			Controls.Add(_splitter);

			// editor control
			_model.RichText.Font = new Font(Font.FontFamily, 12);
			_splitter.Panel1.Controls.Add(_model.RichText);

			// error list
			_splitter.Panel2.Controls.Add(_model.ErrorList);
			_splitter.SplitterDistance = 100;

			// tool strip
			_model.ToolStrip.Font = new Font(Font.FontFamily, 10);
			Controls.Add(_model.ToolStrip);

			// main menu
			_model.MenuStrip.Font = Font;
			Controls.Add(_model.MenuStrip);
			MainMenuStrip = _model.MenuStrip;

			// status bar
			_statusStrip = new MainViewStatusStrip
			{
				Font = new Font(Font.FontFamily, 10),
				Dock = DockStyle.Bottom
			};
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

		private void _model_FileLoaded(object sender, FileLoadedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.FileName))
				Text = $"{FormDisplayName}  –  {_newFile}";
			else
				Text = $"{FormDisplayName}  –  {e.FileName}";
		}

		private void HandleStringsLocalized()
		{
			_newFile = LocalizationManager.GetString("MainView.NewFileHeading", "New File",
				"Displayed when a new file has been created and not yet saved.");
		}

		#endregion

		#region Control Events

		private void _model_ExitProgram(object sender, EventArgs e)
		{
			Close();
		}

		private void _model_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_loaded) return;
			_statusStrip.SetCurrentPosition(e.LineNumber, e.ColumnNumber);
		}

		#endregion
	}
}