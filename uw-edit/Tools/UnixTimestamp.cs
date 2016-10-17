using System;
namespace uw_edit.Tools
{
	public static class UnixTimestamp
	{
		public static int GetTimestamp()
		{
			TimeSpan t = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));
			return (int)t.TotalSeconds;
		}
	}
}
