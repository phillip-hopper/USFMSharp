using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using uw_edit.UserControls;
using uw_edit.USFM;

namespace uw_edit.Views
{
    public class MainViewModel
    {
		public event EventHandler ExitProgram;
		public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
		public event EventHandler<FileLoadedEventArgs> FileLoaded;

		public RichTextBox RichText { get; }
		public PictureBox RichTextImage { get; }
		public ListView ErrorList { get; }
		public MainViewStrip ToolStrip { get; }
		public MainViewMenu MenuStrip { get; }

        private string _fileToOpen;
		private string _currentFileName;
        private System.Timers.Timer _timer;
		private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
		private bool _loaded;
		private bool _needsSaved;
		private readonly UTF8Encoding _utf8NoBOM = new UTF8Encoding(false);

		public MainViewModel()
		{
			RichTextImage = new PictureBox { Dock = DockStyle.Fill, Visible = false };
			RichText = new RichTextBox { Dock = DockStyle.Fill };
		    RichText.TextChanged += HandleRichTextOnTextChanged;
			RichText.SelectionChanged += HandleRichTextOnSelectionChanged;

			ErrorList = new ListView
			{
				View = View.Details,
				Dock = DockStyle.Fill,
				FullRowSelect = true,
				HideSelection = false
			};
			ErrorList.Columns.Add("Line #", 80, HorizontalAlignment.Left);
			ErrorList.Columns.Add("Text", 120, HorizontalAlignment.Left);
			ErrorList.Columns.Add("Description", 200, HorizontalAlignment.Left);
			ErrorList.Resize += HandleErrorListResize;
			ErrorList.MouseDoubleClick += HandleErrorListMouseDoubleClick;

			ToolStrip = new MainViewStrip();
			ToolStrip.StripItemClicked += (sender, e) => StripItemClicked(e);

			MenuStrip = new MainViewMenu { Dock = DockStyle.Top };
			MenuStrip.MenuItemClicked += (sender, e) => MenuItemClicked(e);

			_backgroundWorker.DoWork += HandleBackgroundWorkerDoWork;
		}

        public string FileToOpen
        {
            get { return _fileToOpen; }
            set
            {
                // process a relative path
                if (value.StartsWith(".", StringComparison.Ordinal))
                    value = Path.GetFullPath(Path.Combine(Program.GetAppDirectory(), value));

                _fileToOpen = value;
            }
        }

        public void LoadUsfmFile()
        {
            // if no usfm file was selected when starting, open the default template
            if (string.IsNullOrEmpty(FileToOpen))
                FileToOpen = Path.Combine(Program.GetResourcesDirectory(), "usfm_template.usfm");

            if (string.IsNullOrEmpty(FileToOpen)) return;

            Application.UseWaitCursor = true;
            Application.DoEvents();

            try
            {
                if (_timer != null)
                    _timer.Enabled = false;
                _loaded = false;

                RichText.Visible = false;
                ErrorList.Items.Clear();
                ErrorList.Enabled = false;
                Application.DoEvents();
                TextTools.SetUsfmFromFile(RichText, FileToOpen);
                RichText.SelectionStart = 0;
                RichText.SelectionLength = RichText.Text.Length;
                RichText.SelectionFont = Program.GetTextFont();
                RichText.SelectionLength = 0;
                RichText.WordWrap = false;
                TextTools.MarkupUSFM(RichText);
                _backgroundWorker.RunWorkerAsync();
                RichText.Visible = true;

                // if this is a new USFM file, forget the name
                _currentFileName = FileToOpen.EndsWith("usfm_template.usfm", StringComparison.Ordinal) ? string.Empty : FileToOpen;

                FileLoaded?.Invoke(this, new FileLoadedEventArgs(_currentFileName));

                FileToOpen = string.Empty;
                ToolStrip.SaveButton.Enabled = false;
                EnableTimer();
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }

		private void EnableTimer()
		{
			if (_timer == null)
			{
				_timer = new System.Timers.Timer(2000) { AutoReset = false };
				_timer.Elapsed += HandleTimerOnElapsed;
				_timer.Enabled = false;
			}

			_loaded = true;
		}

		private void SelectFileToOpen()
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "USFM files (*.usfm)|*.usfm|All files (*.*)|*.*";
				ofd.Multiselect = false;

				if (ofd.ShowDialog() != DialogResult.OK)
					return;

				FileToOpen = ofd.FileName;
				LoadUsfmFile();
			}
		}

		private void SaveFile()
		{
			// has the current document previously been saved?
			if (!string.IsNullOrEmpty(_currentFileName))
			{
				File.WriteAllText(_currentFileName, RichText.Text.Replace("\n", Environment.NewLine), _utf8NoBOM);
				NeedsSaved = false;
				return;
			}

			using (var sfd = new SaveFileDialog())
			{
				sfd.Filter = "USFM files (*.usfm)|*.usfm|All files (*.*)|*.*";

				if (sfd.ShowDialog() != DialogResult.OK)
					return;

				File.WriteAllText(sfd.FileName, RichText.Text.Replace("\n", Environment.NewLine), _utf8NoBOM);
				NeedsSaved = false;
			}
		}

		private void DisplayErrors(TagErrors tagErrors)
		{

			ErrorList.Items.Clear();

			foreach (var error in tagErrors.Errors)
			{
				ErrorList.Items.Add(new ListViewItem(new[] { error.LineNumber.ToString(), error.HintText, error.Description }));
			}

			ErrorList.Enabled = true;
		}

		private bool NeedsSaved
		{
			get
			{
				return _needsSaved;
			}
			set
			{
				_needsSaved = value;
				ToolStrip.SaveButton.Enabled = value;
			}
		}

		public bool FreeToOpen()
		{
			if (!NeedsSaved)
				return true;

			string msg;

			// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
			if (!string.IsNullOrEmpty(_currentFileName))
			{
				msg = string.Format("Save changes to file {0} before closing?", Path.GetFileName(_currentFileName));
			}
			else
			{
				msg = "Save changes to new document before closing?";
			}

			using (var dlg = new SaveChangesDialog(msg))
			{
				var result = dlg.ShowDialog(Program.MainView);

				// ReSharper disable once SwitchStatementMissingSomeCases
				switch (result)
				{
					case DialogResult.OK:
						// save
						SaveFile();
						break;

					case DialogResult.Cancel:
						// cancel
						return false;

					case DialogResult.Ignore:
						// do not save
						break;

					default:
						throw new NotSupportedException(string.Format("The value {0} is not supported.", result));
				}
			}

			return true;
		}

		#region Event Handlers

		public void MenuItemClicked(MainMenuClickEventArgs eventArgs)
		{
			switch (eventArgs.ItemClicked)
			{
				case MainViewMenu.MainMenuOption.FileNew:
					if (!FreeToOpen()) return;
					FileToOpen = string.Empty;
					LoadUsfmFile();
					NeedsSaved = true;
					return;
					
				case MainViewMenu.MainMenuOption.FileOpen:
					if (FreeToOpen())
						SelectFileToOpen();
					return;
					
				case MainViewMenu.MainMenuOption.FileSave:
					SaveFile();
					return;
					
				case MainViewMenu.MainMenuOption.FileExit:
					if (!FreeToOpen()) return;
					ExitProgram?.Invoke(this, eventArgs);
					return;

				case MainViewMenu.MainMenuOption.WordWrapOn:
					RichText.WordWrap = true;
					return;

				case MainViewMenu.MainMenuOption.WordWrapOff:
					RichText.WordWrap = false;
					return;

				case MainViewMenu.MainMenuOption.ViewRefresh:
					if (_timer != null && _timer.Enabled)
						_timer.Enabled = false;

					DisplayErrors(TextTools.GetErrors(RichText));
					return;

				case MainViewMenu.MainMenuOption.ToolsRemoveS5:
					TextTools.RemoveS5Tags(RichText);
					return;

				case MainViewMenu.MainMenuOption.ToolsRemoveTrailingSpace:
					TextTools.RemoveTrailingWhitespace(RichText);
					return;

				case MainViewMenu.MainMenuOption.ToolsRemoveBlankLines:
					TextTools.RemoveBlankLines(RichText);
					return;
			}
		}

		public void StripItemClicked(MainStripClickEventArgs eventArgs)
		{
			switch (eventArgs.ItemClicked)
			{
				case MainViewStrip.MainStripOption.Save:
					SaveFile();
					break;
			}
		}

		private void HandleTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
			RichText.Invoke((MethodInvoker)delegate
			{
				RichTextImage.Invoke((MethodInvoker)delegate
				{
					try
					{
						RichText.TextChanged -= HandleRichTextOnTextChanged;

						// do this to avoid flicker
						var sourceRect = RichText.ClientRectangle;

						using (var tmp = new Bitmap(sourceRect.Width, sourceRect.Height, PixelFormat.Format32bppArgb))
						{
							RichText.DrawToBitmap(tmp, sourceRect);
							RichTextImage.Image = tmp;
							RichTextImage.Visible = true;
						}

						// display errors
						TextTools.MarkupUSFM(RichText);
						_backgroundWorker.RunWorkerAsync();
					}
					catch (Exception e)
					{
						MessageBox.Show(e.ToString());
					}
					finally
					{
						RichTextImage.Visible = false;
						RichText.TextChanged += HandleRichTextOnTextChanged;
					}
				});
			});
        }

		private void HandleRichTextOnTextChanged(object sender, EventArgs eventArgs)
        {
			if (!_loaded) return;

			if (_timer == null) return;

			if (_timer.Enabled)
            {
                _timer.Enabled = false;
            }

			NeedsSaved = true;

            _timer.Enabled = true;
        }

		private void HandleRichTextOnSelectionChanged(object sender, EventArgs eventArgs)
		{
			var lineNum = RichText.GetLineFromCharIndex(RichText.SelectionStart) + 1;
			var columnNum = RichText.SelectionStart - RichText.GetFirstCharIndexOfCurrentLine() + 1;

			SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(lineNum, columnNum));
		}

		void HandleBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			if (RichText.InvokeRequired)
			{
				RichText.BeginInvoke((MethodInvoker)(() =>
				{
					DisplayErrors(TextTools.GetErrors(RichText));
				}));
			}

		}

		void HandleErrorListMouseDoubleClick(object sender, MouseEventArgs e)
		{
			ListViewHitTestInfo info = ErrorList.HitTest(e.X, e.Y);
			ListViewItem item = info.Item;

			if (item == null)
				return;

			var lineNumber = int.Parse(ErrorList.SelectedItems[0].Text) - 1;

			if (lineNumber > RichText.Lines.Length)
				return;

			// get the position of the selected item
			var lineStart = RichText.GetFirstCharIndexFromLine(lineNumber);
			var line = RichText.Lines[lineNumber];
			var pos = line.IndexOf(ErrorList.SelectedItems[0].SubItems[1].Text, StringComparison.Ordinal);
			RichText.SelectionStart = lineStart + pos;
			RichText.SelectionLength = ErrorList.SelectedItems[0].SubItems[1].Text.Length;

			RichText.Focus();
		}

		void HandleErrorListResize (object sender, EventArgs e)
		{
			if (ErrorList.Width > 200)
			{
				ErrorList.Columns[2].Width = ErrorList.Width - ErrorList.Columns[0].Width - ErrorList.Columns[1].Width - 22;
			}
		}

        #endregion

	}

	public class SelectionChangedEventArgs : EventArgs
	{
		public int LineNumber;
		public int ColumnNumber;

		public SelectionChangedEventArgs(int lineNumber, int columnNumber)
		{
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
		}
	}

	public class FileLoadedEventArgs : EventArgs
	{
		public string FullPath;
		public string FileName => Path.GetFileName(FullPath);

	    public FileLoadedEventArgs(string fullPath)
		{
			FullPath = fullPath;
		}
	}
}
