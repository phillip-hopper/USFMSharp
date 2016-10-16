using System;
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

		public RichTextBox RichText { get; set; }
		public PictureBox RichTextImage { get; set; }

        private string _fileToOpen;
        private System.Timers.Timer _timer;

		public MainViewModel()
		{
			RichTextImage = new PictureBox { Dock = DockStyle.Fill, Visible = false };
			RichText = new RichTextBox { Dock = DockStyle.Fill };
		    RichText.TextChanged += RichTextOnTextChanged;
			RichText.SelectionChanged += RichTextOnSelectionChanged;
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

        public void LoadTemplate()
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
				FileToOpen = string.Empty;
				TextTools.MarkupUSFM(RichText);
				RichText.Visible = true;
				EnableTimer();
			}
        }

		#region Event Handlers

		public void HandleMenuItemClicked(object sender, MainMenuClickEventArgs eventArgs)
		{
			switch (eventArgs.ItemClicked)
			{
				case MainViewMenu.MainMenuOption.FileExit:
					ExitProgram?.Invoke(this, eventArgs);
					return;

				case MainViewMenu.MainMenuOption.WordWrapOn:
					RichText.WordWrap = true;
					return;

				case MainViewMenu.MainMenuOption.WordWrapOff:
					RichText.WordWrap = false;
					return;
			}
		}

		public void HandleStripItemClicked(object sender, MainStripClickEventArgs eventArgs)
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

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
			RichText.Invoke((MethodInvoker)delegate ()
			{
				RichTextImage.Invoke((MethodInvoker)delegate ()
				{
					try
					{
						RichText.TextChanged -= RichTextOnTextChanged;

						// do this to avoid flicker
						Rectangle sourceRect = RichText.ClientRectangle;
						Size targetSize = RichText.Size;
						using (Bitmap tmp = new Bitmap(sourceRect.Width, sourceRect.Height, PixelFormat.Format32bppArgb))
						{
							RichText.DrawToBitmap(tmp, sourceRect);
							RichTextImage.Image = tmp;
							RichTextImage.Visible = true;
						}

						TextTools.MarkupUSFM(RichText);
					}
					catch (Exception e)
					{
						MessageBox.Show(e.ToString());
					}
					finally
					{
						RichTextImage.Visible = false;
						RichText.TextChanged += RichTextOnTextChanged;
					}
				});
			});
        }

		private void EnableTimer()
		{
			if (_timer == null)
			{
				_timer = new System.Timers.Timer(2000) { AutoReset = false };
				_timer.Elapsed += TimerOnElapsed;
			}
		}

        private void RichTextOnTextChanged(object sender, EventArgs eventArgs)
        {
			if (_timer == null) return;

			if (_timer.Enabled)
            {
                _timer.Enabled = false;
            }

            _timer.Enabled = true;
        }

		void RichTextOnSelectionChanged(object sender, EventArgs eventArgs)
		{
			var lineNum = RichText.GetLineFromCharIndex(RichText.SelectionStart) + 1;
			var columnNum = RichText.SelectionStart - RichText.GetFirstCharIndexOfCurrentLine() + 1;

			SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(lineNum, columnNum));
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
}