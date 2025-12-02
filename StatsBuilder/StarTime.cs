using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AdventofCode.StatsBuilder {
	public class StarTime {
		public DateTime dateTime {
			get {
				return UnixTimeStampToDateTime(get_star_ts);
			}
		}

		public int star_index { get; set; }
		public long get_star_ts { get; set; }

		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
			return dtDateTime.ToLocalTime();
		}
	}
}