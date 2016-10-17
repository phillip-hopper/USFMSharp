using System.Collections.Generic;
using uw_edit.Tools;

namespace uw_edit.USFM
{
	public class ParatextStylesheet
	{
		private int _lastUpdated;
		private List<ParatextStyle> _styles = new List<ParatextStyle>();

		public ParatextStylesheet()
		{
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
				string occursUnder = string.Empty;
				string textType = string.Empty;
				string styleType = string.Empty;

				for (var j = 1; j < parts.Length; j++)
				{
					if (parts[j].StartsWith("\\OccursUnder", System.StringComparison.Ordinal))
						occursUnder = parts[j].Substring(13).Trim();

					else if (parts[j].StartsWith("\\TextType", System.StringComparison.Ordinal))
						textType = parts[j].Substring(10).Trim();

					else if (parts[j].StartsWith("\\StyleType", System.StringComparison.Ordinal))
						styleType = parts[j].Substring(11).Trim();
				}

				_styles.Add(new ParatextStyle(marker, occursUnder, textType, styleType));
			}


			_lastUpdated = UnixTimestamp.GetTimestamp();
		}
	}

	public class ParatextStyle
	{
		public string Marker;
		public string OccursUnder;
		public string TextType;
		public string StyleType;

		public ParatextStyle(string marker, string occursUnder, string textType, string styleType)
		{
			Marker = marker;
			OccursUnder = occursUnder;
			TextType = textType;
			StyleType = styleType;
		}
	}
}
