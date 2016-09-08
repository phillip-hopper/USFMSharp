using System;
using Gecko;

namespace uw_edit.USFM
{
	public static class TextTools
	{
		public static TextSelection GetSelection(GeckoWebBrowser webBrowser)
		{
			var returnVal = new TextSelection();
			var usfmDiv = (GeckoHtmlElement)webBrowser.Document.GetElementById("usfm-content");

			// get length of the selection
			var sel = webBrowser.Window.Selection;
			returnVal.Length = Math.Abs(sel.AnchorOffset - sel.FocusOffset);

			// get the starting position
			var range = webBrowser.Window.Selection.GetRangeAt(0);
			var preCursor = range.CloneRange();
			preCursor.SelectNodeContents(usfmDiv);
			preCursor.SetEnd(range.EndContainer, range.StartOffset);
			returnVal.Start = preCursor.ToString().Length;

			// get the selected text
			if (returnVal.Length > 0)
			{
				returnVal.Text = usfmDiv.InnerHtml.Substring(returnVal.Start, returnVal.Length);
			}

			return returnVal;
		}
	}

	public class TextSelection
	{
		public int Start;
		public int Length;
		public string Text;
	}
}

