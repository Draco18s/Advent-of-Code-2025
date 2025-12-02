using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AdventofCode.StatsBuilder {
	public class AoCUser {
		public int global_score { get; set; }
		public int local_score { get; set; }
		public long last_star_ts { get; set; }
		public string id { get; set; }
		public Dictionary<string, Dictionary<string, StarTime>> completion_day_level { get; set; }
		public string name { get; set; }
		public int stars { get; set; }

		public int locPoints = 0;

		public override bool Equals(object obj) {
			if(obj is AoCUser user)
				return id.Equals(user.id);
			return false;
		}

		public override int GetHashCode() {
			return id.GetHashCode();
		}
	}
}