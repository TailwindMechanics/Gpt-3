using System.Globalization;
using System;


namespace Modules.Utilities.External
{
	public enum Season
	{
		Sprint = 1,
		Summer = 2,
		Autumn = 3,
		Winter = 4
	}

	public static class DateUtils
	{
		public static string CurrentTime ()
			=> DateTime.Now.ToString("HH:mm");
		public static string CurrentYear()
			=> DateTime.Now.Year.ToString();
		public static string CurrentMonth()
			=> DateTime.Now.Month.ToString();
		public static string CurrentDay()
			=> DateTime.Now.Day.ToString();
		public static string CurrentSeason ()
		{
			var persianMonth = new PersianCalendar().GetMonth( DateTime.Now );
			var season = (Season) Math.Ceiling( persianMonth / 3.0 );
			return season.ToString();
		}
	}
}