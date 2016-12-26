using System.Collections.Generic;
using USFMSharp.Tools;

namespace USFMSharp.USFM
{
	public class ParatextStylesheet
	{
		private int _lastUpdated;
		private List<ParatextStyle> _styles = new List<ParatextStyle>();

	    public string Tags { get; private set; }

		public ParatextStylesheet()
		{
		    Tags = " ";
			LoadParatextStylesheet();
		}

		private void LoadParatextStylesheet()
		{
			var text = Program.GetTextResource("usfm_sb.sty").Replace("\r", "");
			var sections = text.Split(new [] { "\\Marker " }, System.StringSplitOptions.None);

			for (var i = 1; i < sections.Length; i++)
			{
				var parts = sections[i].Split(new char[] { '\n' });
				var marker = parts[0];
			    var endMarker = string.Empty;
			    var occursUnder = string.Empty;
			    var textType = string.Empty;
			    var styleType = string.Empty;

			    Tags += "\\" + marker + " ";

				for (var j = 1; j < parts.Length; j++)
				{
					if (parts[j].StartsWith("\\OccursUnder", System.StringComparison.Ordinal))
						occursUnder = parts[j].Substring(13).Trim();

					else if (parts[j].StartsWith("\\TextType", System.StringComparison.Ordinal))
						textType = parts[j].Substring(10).Trim();

					else if (parts[j].StartsWith("\\StyleType", System.StringComparison.Ordinal))
						styleType = parts[j].Substring(11).Trim();

				    else if (parts[j].StartsWith("\\Endmarker", System.StringComparison.Ordinal))
					{
					    endMarker = parts[j].Substring(11).Trim();
					    Tags += "\\" + endMarker + " ";
					}
				}

				_styles.Add(new ParatextStyle(marker, endMarker, occursUnder, textType, styleType));
			}

			_lastUpdated = UnixTimestamp.GetTimestamp();
		}
	}

	public class ParatextStyle
	{
		public string Marker;
	    public string Endmarker;
	    public string OccursUnder;
		public string TextType;
		public string StyleType;

		public ParatextStyle(string marker, string endMarker, string occursUnder, string textType, string styleType)
		{
			Marker = marker;
		    Endmarker = endMarker;
			OccursUnder = occursUnder;
			TextType = textType;
			StyleType = styleType;
		}
	}
}
