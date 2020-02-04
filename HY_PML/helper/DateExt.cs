using System;

namespace HY_PML.helper
{
    public static class DateExt
	{
		public static string ToDateTimeString(this DateTime date, string dateSep = "/", bool h24 = true)
		{
			var hStr = h24 ? "HH" : "tt hh";
			return date.ToString($"yyyy{dateSep}MM{dateSep}dd {hStr}:mm");
		}
		public static string ToDateTimeString(this DateTime? date, string dateSep = "/", bool h24 = true)
		{
			if (date.HasValue)
				return ToDateTimeString(date.Value, dateSep, h24);
			else
				return null;
		}

		public static string ToDateString(this DateTime date, string dateSep = "/")
		{
			return date.ToString($"yyyy{dateSep}MM{dateSep}dd");
		}

		public static string ToDateString(this DateTime? date, string dateSep = "/")
		{
			if (date.HasValue)
				return ToDateString(date.Value, dateSep);
			else
				return null;
		}
	}
}
