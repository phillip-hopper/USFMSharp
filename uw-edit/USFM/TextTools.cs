using System.IO;
using Gecko;


namespace uw_edit.USFM
{
	public static class TextTools
	{
	    /// <summary>
	    /// Sets the USFM loaded from a file
	    /// </summary>
	    /// <returns>The selection.</returns>
	    /// <param name="webBrowser"></param>
	    /// <param name="fileName"></param>
	    public static void SetUsfmFromFile(GeckoWebBrowser webBrowser, string fileName)
		{
            var usfmDiv = (GeckoHtmlElement)webBrowser.Document.GetElementById("usfm-content");
			usfmDiv.InnerHtml = File.ReadAllText(fileName);
		}
	}
}

