using System;
namespace uw_edit
{
	public class ParatextStylesheet
	{
		public ParatextStylesheet()
		{
			LoadStylesheet();
		}

		private void LoadStylesheet()
		{
			var text = Program.GetTextResource("usfm_sb.sty");

		}
	}
}
