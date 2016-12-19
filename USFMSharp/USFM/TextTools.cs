using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using USFMSharp.Tools;


namespace USFMSharp.USFM
{
	public static class TextTools
	{
		private static string _previousText = string.Empty;

	    private const string FootnoteCallers = "+-?";
	    private static readonly Regex AllTagsRe = new Regex(@"(\\[\w]+)(\s|\*)(([0-9-+\?]*?)(\s)(\s)*)*", RegexOptions.Compiled);
		private static readonly Regex ChapterNumberRe = new Regex(@"^[1234567890]+$", RegexOptions.Compiled);
		private static readonly Regex VerseNumberRe = new Regex(@"^[1234567890\-,]+$", RegexOptions.Compiled);
		private static readonly Regex S5Re = new Regex(@"\\s5\s*", RegexOptions.Compiled);
		private static readonly Regex TrailingRe = new Regex(@"[\t\v\x85\p{Z}]+$", RegexOptions.Compiled | RegexOptions.Multiline);
		private static readonly Regex BlankLineRe = new Regex(@"^\s*\n", RegexOptions.Compiled | RegexOptions.Multiline);

		/// <summary>
		/// Sets the USFM loaded from a file
		/// </summary>
		/// <param name="richText">The RichTextBox control</param>
		/// <param name="fileName">The name of the file to load</param>
		public static void SetUsfmFromFile(RichTextBox richText, string fileName)
		{
			_previousText = string.Empty;
			string allText = File.ReadAllText(fileName, Encoding.UTF8);
			richText.Text = allText;
		}

		public static void MarkupUSFM(RichTextBox richText)
		{
			// do a diff
			var diffs = Diff.DiffText(_previousText, richText.Text, false, false, false);
			if (diffs.Length == 0)
				return;
			
			// minimize flickering
			richText.Visible = false;

			// remember the current cursor and scroll location
			var currentStart = richText.SelectionStart;
			var currentLength = richText.SelectionLength;
			var firstVisibleChar = richText.GetCharIndexFromPosition(new Point(1, 1));

			// get the start index
			var startLine = int.MaxValue;
			foreach (var diff in diffs)
			{
				var testStartLine = diff.StartB == 0 ? 0 : diff.StartB - 1;
				if (testStartLine < startLine)
					startLine = testStartLine;
			}

			var startIndex = GetNthIndexOfNewLine(richText.Text, startLine, 0);
			if (startIndex == -1)
				startIndex = 0;
			richText.SelectionStart = startIndex == -1 ? 0 : startIndex;

			// get the end index
			var endLine = 0;
			foreach (var diff in diffs)
			{
				var testEndLine = diff.insertedB + 1;
				if (testEndLine > endLine)
					endLine = testEndLine;
			}

			var endIndex = GetNthIndexOfNewLine(richText.Text, endLine, startIndex);
			if (endIndex < startIndex)
				endIndex = richText.TextLength;
			else if (endIndex > richText.TextLength)
				endIndex = richText.TextLength;
			
			richText.SelectionLength = endIndex - startIndex;

			// clear highlighting
			richText.SelectionColor = Color.Black;

			// get all the tags in the selection
			var textToMark = richText.SelectedText;
			var matches = AllTagsRe.Matches(textToMark);
			foreach (Match match in matches)
			{
				richText.SelectionStart = startIndex + match.Index;
				richText.SelectionLength = match.Value.TrimEnd().Length;

				var foundTag = match.Groups[1].Value + match.Groups[2].Value.Trim();
				var foundExtra = match.Groups[4].Value;

				// \c and \v must be accompanied by chapter or verse numbers
				if (foundTag == "\\c" || foundTag == "\\v")
				{
					if (!string.IsNullOrEmpty(foundExtra) && IsValidChapterOrVerseNumber(foundTag, foundExtra))
						richText.SelectionColor = Color.Blue;
					else
						richText.SelectionColor = Color.Red;
				}
				// \f must be followed by " + ", " - " or " ? "
				else if (foundTag == "\\f")
				{
					if (!string.IsNullOrEmpty(foundExtra) && FootnoteCallers.Contains(foundExtra))
						richText.SelectionColor = Color.Blue;
					else
						richText.SelectionColor = Color.Red;
				}
				else 
				{
					if (Program.StyleSheet.Tags.Contains(foundTag))
						richText.SelectionColor = Color.Blue;
					else
						richText.SelectionColor = Color.Red;
				}
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

		private static int GetNthIndexOfNewLine(string s, int nth, int startIndex)
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

		private static bool IsValidChapterOrVerseNumber(string tag, string stringToCheck)
		{
			if (tag == "\\v")
				return VerseNumberRe.IsMatch(stringToCheck);

			return ChapterNumberRe.IsMatch(stringToCheck);
		}

		private static TagError ErrorFromMatch(RichTextBox richText, Match match, string description)
		{
			var lineNum = richText.GetLineFromCharIndex(match.Index) + 1;

			return new TagError(lineNum, match.Groups[0].Index, match.Groups[0].Value.Trim(), description);
		}

		public static TagErrors GetErrors(RichTextBox richText)
		{
			var errors = new TagErrors();

			// get all the tags in the selection
			var textToMark = richText.Text;
			var matches = AllTagsRe.Matches(textToMark);
			foreach (Match match in matches)
			{
				var foundTag = match.Groups[1].Value + match.Groups[2].Value.Trim();
				var foundExtra = match.Groups[4].Value;

				// \c must be accompanied by chapter number
				if (foundTag == "\\c")
				{
					if (string.IsNullOrEmpty(foundExtra) || !IsValidChapterOrVerseNumber(foundTag, foundExtra))
						errors.Add(ErrorFromMatch(richText, match, "\\c must be followed by a chapter number"));
				}
				// \v must be accompanied by verse numbers
				else if (foundTag == "\\v")
				{
					if (string.IsNullOrEmpty(foundExtra) || !IsValidChapterOrVerseNumber(foundTag, foundExtra))
						errors.Add(ErrorFromMatch(richText, match, "\\v must be followed by a verse number or verse bridge"));
				}
				// \f must be followed by " + ", " - " or " ? "
				else if (foundTag == "\\f")
				{
					if (string.IsNullOrEmpty(foundExtra) || !FootnoteCallers.Contains(foundExtra))
						errors.Add(ErrorFromMatch(richText, match, "\\f must be followed by \" + \", \" - \" or \" ? \""));
				}
				else
				{
					if (!Program.StyleSheet.Tags.Contains(foundTag))
						errors.Add(ErrorFromMatch(richText, match, "Unknown USFM tag"));
				}

				// check for extra space
				var extraSpace = !string.IsNullOrEmpty(match.Groups[6].Value) && !match.Groups[5].Value.Contains("\n");
				if (extraSpace)
				{
					errors.Add(ErrorFromMatch(richText, match, "Extra space following"));
				}
			}

			return errors;
		}

		public static void RemoveS5Tags(RichTextBox richText)
		{
			RegexReplace(richText, S5Re, string.Empty);
		}

		public static void RemoveBlankLines(RichTextBox richText)
		{
			RegexReplace(richText, BlankLineRe, string.Empty);
		}

		public static void RemoveTrailingWhitespace(RichTextBox richText)
		{
			RegexReplace(richText, TrailingRe, string.Empty);
		}

		private static void RegexReplace(RichTextBox richText, Regex regex, string replaceWith)
		{
			// minimize flickering
			richText.Visible = false;
			Application.UseWaitCursor = true;
			Application.DoEvents();

			try
			{
				// remember the current cursor and scroll location
				var currentStart = richText.SelectionStart;
				var currentLength = richText.SelectionLength;
				var firstVisibleChar = richText.GetCharIndexFromPosition(new Point(1, 1));

				var matches = regex.Matches(richText.Text);

				// we need to work backward because we are changing the text
				for (var i = matches.Count - 1; i > -1; i--)
				{
					var match = matches[i];
					richText.SelectionStart = match.Index;
					richText.SelectionLength = match.Value.Length;
					richText.SelectedText = replaceWith;
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
			finally
			{
				Application.UseWaitCursor = false;
			}
		}
	}
}
