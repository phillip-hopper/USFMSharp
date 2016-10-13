using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using uw_edit.Tools;


namespace uw_edit.USFM
{
	public static class TextTools
	{
		private static Regex _knownTagsRe;
		private static string _previousText = string.Empty;

		private static string[] _knownTags = {
			@"^(\\id|\\ide|\\h)(\s+)",  // book id, encoding and heading
			@"^(\\sts|\\rem)(\s+)",     // status and remarks
			@"^(\\toc[0-9])(\s+)",      // table of contents
			@"^(\\mt[0-9]?)(\s+)",      // major titles
			@"^(\\p)(\s+)",             // paragraph
			@"^(\\v [0-9-]+)(\s+)",     // verse
			@"^(\\c [0-9]+)(\s+)"       // chapter
		};

		private static Regex _unknownTagsRe = new Regex(@"(\\\w+)(\s+)", RegexOptions.Compiled);

		/// <summary>
		/// Sets the USFM loaded from a file
		/// </summary>
		/// <param name="richText">The RichTextBox control</param>
		/// <param name="fileName">The name of the file to load</param>
		public static void SetUsfmFromFile(RichTextBox richText, string fileName)
		{
			string allText = File.ReadAllText(fileName, Encoding.UTF8);
			richText.Text = allText;
		}

		public static void MarkupUSFM(RichTextBox richText)
		{
			// compile the known-tags Regex, if it hasn't been done already
			if (_knownTagsRe == null)
			{
				var temp = new string[_knownTags.Length];
				for (var i = 0; i < _knownTags.Length; i++)
				{
					var tag = _knownTags[i];
					temp[i] = "(?:" + tag + ")";
				}

				_knownTagsRe = new Regex(string.Join("|", temp), RegexOptions.Multiline | RegexOptions.Compiled);
			}

			// do a diff
			var diff = Diff.DiffText(_previousText, richText.Text, false, false, false);
			if (diff.Length == 0)
				return;
			
			// minimize flickering
			richText.Visible = false;

			// remember the current cursor and scroll location
			var currentStart = richText.SelectionStart;
			var currentLength = richText.SelectionLength;
			var firstVisibleChar = richText.GetCharIndexFromPosition(new Point(1, 1));

			// get the start index
			var startLine = diff[0].StartB == 0 ? 0 : diff[0].StartB - 1;
			var startIndex = GetNthIndexOfNL(richText.Text, startLine, 0);
			if (startIndex == -1)
				startIndex = 0;
			richText.SelectionStart = startIndex == -1 ? 0 : startIndex;

			// get the end index
			var endLine = diff[0].insertedB + 1;
			var endIndex = GetNthIndexOfNL(richText.Text, endLine, startIndex);
			if (endIndex < startIndex)
				endIndex = richText.TextLength;
			else if (endIndex > richText.TextLength)
				endIndex = richText.TextLength;
			
			richText.SelectionLength = endIndex - startIndex;

			// clear highlighting
			richText.SelectionColor = Color.Black;

			// mark the unknown tags
			var textToMark = richText.SelectedText;
			var matches = _unknownTagsRe.Matches(textToMark);
			foreach (Match match in matches)
			{
				richText.SelectionStart = startIndex + match.Index;
				richText.SelectionLength = match.Value.TrimEnd().Length;
				richText.SelectionColor = Color.Red;
				//richText.SelectionFont = Program.GetTagFont();
			}
				
			// mark the known tags
			matches = _knownTagsRe.Matches(textToMark);
			foreach (Match match in matches)
			{
				richText.SelectionStart = startIndex + match.Index;
				richText.SelectionLength = match.Value.TrimEnd().Length;
				richText.SelectionColor = Color.Blue;
				//richText.SelectionFont = Program.GetTagFont();
			}

			// restore original scroll position
			richText.SelectionStart = firstVisibleChar;
			richText.SelectionLength = 0;

			// restore original selection
			richText.SelectionStart = currentStart;
			richText.SelectionLength = currentLength;
			richText.Visible = true;

			_previousText = richText.Text;
		}

		private static int GetNthIndexOfNL(string s, int nth, int startIndex)
		{
			if (nth == 0)
				return 0;

			var counter = 0;

			while ((counter <= nth) && (startIndex != -1))
			{
				// check if we found it
				if (counter == nth)
					return startIndex + 1;

				// find the next one
				startIndex = s.IndexOf('\n', startIndex > 0 ? startIndex + 1 : startIndex);
				counter++;
			}

			// if we are here, we didn't find it
			return -1;
		}
	}
}
