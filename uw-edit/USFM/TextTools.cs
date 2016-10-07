using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace uw_edit.USFM
{
	public static class TextTools
	{
		private static Regex _knownTagsRe;

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
			richText.LoadFile(fileName, RichTextBoxStreamType.PlainText);
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

			// minimize flickering
			richText.Visible = false;

			// remember the current cursor and scroll location
			var currentStart = richText.SelectionStart;
			var currentLength = richText.SelectionLength;
			var firstVisibleChar = richText.GetCharIndexFromPosition(new Point(1, 1));

			// clear highlighting
			richText.SelectionStart = 0;
			richText.SelectionLength = richText.TextLength;
			richText.SelectionColor = Color.Black;

			// mark the unknown tags
			var matches = _unknownTagsRe.Matches(richText.Text);
			foreach (Match match in matches)
			{
				richText.SelectionStart = match.Index;
				richText.SelectionLength = match.Value.TrimEnd().Length;
				richText.SelectionColor = Color.Red;
			}
				
			// mark the known tags
			matches = _knownTagsRe.Matches(richText.Text);
			foreach (Match match in matches)
			{
				richText.SelectionStart = match.Index;
				richText.SelectionLength = match.Value.TrimEnd().Length;
				richText.SelectionColor = Color.Blue;
			}

			// restore original scroll position
			richText.SelectionStart = firstVisibleChar;
			richText.SelectionLength = 0;

			// restore original selection
			richText.SelectionStart = currentStart;
			richText.SelectionLength = currentLength;
			richText.Visible = true;

		}
	}
}
