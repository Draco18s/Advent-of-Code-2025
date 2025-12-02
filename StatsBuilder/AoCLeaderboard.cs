using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AdventofCode.StatsBuilder {
	public class AoCLeaderboard {

		[JsonProperty("event")]
		public string _event { get; set; }
		public string owner_id { get; set; }
		public Dictionary<string, AoCUser> members { get; set; }
	}
}