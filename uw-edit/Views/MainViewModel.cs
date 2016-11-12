using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

		public RichTextBox RichText { get; private set; }
		public PictureBox RichTextImage { get; private set; }
		public ListView ErrorList { get; private set; }

        private string _fileToOpen;
        private System.Timers.Timer _timer;
		private BackgroundWorker _backgroundWorker = new BackgroundWorker();

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
				FullRowSelect = true
			};
			ErrorList.Columns.Add("Line #", 80, HorizontalAlignment.Left);
			ErrorList.Columns.Add("Text of the error", 200, HorizontalAlignment.Left);
			ErrorList.Resize += HandleErrorListResize;
			ErrorList.MouseDoubleClick += HandleErrorListMouseDoubleClick;

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

			if (!string.IsNullOrEmpty(FileToOpen))
			{
				RichText.Visible = false;
				TextTools.SetUsfmFromFile(RichText, FileToOpen);
				RichText.SelectionStart = 0;
				RichText.SelectionLength = RichText.Text.Length;
				RichText.SelectionFont = Program.GetTextFont();
				RichText.SelectionLength = 0;
				RichText.WordWrap = false;
				TextTools.MarkupUSFM(RichText);
				RichText.Visible = true;
				FileLoaded?.Invoke(this, new FileLoadedEventArgs(FileToOpen));
				FileToOpen = string.Empty;
				EnableTimer();
			}
        }

		private void EnableTimer()
		{
			if (_timer == null)
			{
				_timer = new System.Timers.Timer(2000) { AutoReset = false };
				_timer.Elapsed += HandleTimerOnElapsed;
			}
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

		private void DisplayErrors(TagErrors tagErrors)
		{

			ErrorList.Items.Clear();

			foreach (var error in tagErrors.Errors)
			{
				ErrorList.Items.Add(new ListViewItem(new[] { error.LineNumber.ToString(), error.HintText }));
			}
		}

		#region Event Handlers

		public void MenuItemClicked(MainMenuClickEventArgs eventArgs)
		{
			switch (eventArgs.ItemClicked)
			{
				case MainViewMenu.MainMenuOption.FileNew:
					return;
					
				case MainViewMenu.MainMenuOption.FileOpen:
					SelectFileToOpen();
					return;
					
				case MainViewMenu.MainMenuOption.FileSave:
					return;
					
				case MainViewMenu.MainMenuOption.FileExit:
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
			}
		}

		public void StripItemClicked(MainStripClickEventArgs eventArgs)
		{
			switch (eventArgs.ItemClicked)
			{
				case MainViewStrip.MainStripOption.Chapter:
					//Browser.InsertTag("\\c ", " ");
					break;

				case MainViewStrip.MainStripOption.Verse:
					//Browser.InsertText("\\v ");
					break;
					
				case MainViewStrip.MainStripOption.Paragraph:
					//ExitProgram?.Invoke(this, eventArgs);
					//nsIDOMWindowUtils utils = Xpcom.QueryInterface<nsIDOMWindowUtils>(Browser.WebBrowser.Window.DomWindow);
					//nsIDOMWindowUtils utils = Xpcom.QueryInterface<nsIDOMWindowUtils>(Browser.WebBrowser.Window.DomWindow);
					//Browser.WebBrowser.Window.WindowUtils.SendKeyEvent("keypress", 0, 102, 0, false);
					return;
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
			if (_timer == null) return;

			if (_timer.Enabled)
            {
                _timer.Enabled = false;
            }

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

			var line = RichText.Lines[lineNumber];
			var pos = RichText.Find(line);
			RichText.SelectionStart = pos;
			RichText.ScrollToCaret();
			RichText.Focus();
		}

		void HandleErrorListResize (object sender, EventArgs e)
		{
			if (ErrorList.Width > 200)
			{
				ErrorList.Columns[1].Width = ErrorList.Width - ErrorList.Columns[0].Width - 22;
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
		public string FileName { get { return Path.GetFileName(FullPath); } }

		public FileLoadedEventArgs(string fullPath)
		{
			FullPath = fullPath;
		}
	}
}
