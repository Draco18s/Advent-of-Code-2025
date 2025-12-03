using AdventofCode.StatsBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdventofCode2025 {
	static class Program {
		private const bool CUSTOM_LEADERBOARD = true;
		private const float MAX_EXPECTED = 3f;
		private const string year = "2025";
		private static readonly Uri baseAddress = new Uri("https://adventofcode.com");
		private const string leaderboardURI = "{0}/leaderboard/private/view/{1}.json";
		private static Dictionary<string,List<string>> conf;
		
		private static string puzzleNum = "3";

		static void Main(string[] args) {
			/*** DAY 1 IMPORTANT NOTE: DO THIS BEFORE STARTING ***/
			/*** HOW TO GET SESSION ID (because I keep forgetting)
			 * Log in
			 * Go to a puzzle
			 * Get puzzle input
			 * Open console on the input page (Network)
			 * Refresh, look at the input request header
			 * Copy the cookie value to "sessionID" part of config
			 */
			#region config
			string path = Path.GetFullPath("./../../../inputs/config.json");
			if (!File.Exists(path))
			{
				Console.WriteLine("Create config file");
				conf = new Dictionary<string, List<string>>();
				conf.Add("sessionID", new List<string>(new string[] { "leaderboard" }));
				string dat = System.Text.Json.JsonSerializer.Serialize<Dictionary<string, List<string>>>(conf);
				StreamWriter writer = File.CreateText(path);
				writer.Write(dat);
				writer.Flush();
				writer.Close();
				return;
			}
			string confj = File.ReadAllText(path);
			conf = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,List<string>>>(confj);
			#endregion
			#region puzzle input
			string p = Path.GetFullPath(string.Format("./../../../inputs/day{0}.txt", puzzleNum));
			if(!File.Exists(p)) {
				Task.Run(async () => {

					string puzzleInput = await GetInputFor(puzzleNum, conf.Keys.First());
					if (string.IsNullOrEmpty(puzzleInput))
					{
						Console.WriteLine($"Puzzle {puzzleNum} not yet available!");
						return;
					}
					File.WriteAllText(p, puzzleInput);
					Main(args);
				});
			}
			#endregion
			else
			{
				Type ty = Type.GetType($"AdventofCode{year}.Day{puzzleNum}");
				if (ty == null)
				{
					Console.WriteLine($"Failed to find {year} Day {puzzleNum} class!");
					return;
				}
				string input = File.ReadAllText(p);
				input = input.Replace("\r", "");
				if(input[^1] == '\n')
					input = input.Substring(0, input.Length - 1); //stupid trailing newline
				//string input = @"";
				DateTime s = DateTime.Now;
				
				object invokeResult = ty.InvokeMember("Part1", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, new object[] { input });
				if (invokeResult == null)
				{
					Console.WriteLine($"{year} Day {puzzleNum} Part 1 returned null!");
					return;
				}
				long result = (long)invokeResult;

				//long result = Day1.Part1(input);
				
				DateTime e = DateTime.Now;
				Console.WriteLine(result);
				Console.WriteLine("Time: " + (e - s).TotalMilliseconds);
				s = DateTime.Now;

				invokeResult = ty.InvokeMember("Part2", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, new object[] { input });
				if (invokeResult == null)
				{
					Console.WriteLine($"{year} Day {puzzleNum} Part 2 returned null!");
					return;
				}
				result = (long)invokeResult;

				e = DateTime.Now;
				Console.WriteLine(result);
				Console.WriteLine("Time: " + (e - s).TotalMilliseconds);
				BuildLeaderboard();
			}
			
			Console.Read();
		}

		static void BuildLeaderboard()
		{
			Task.Run(async () => {
				AoCLeaderboard obj;
				List<AoCUser> users = new List<AoCUser>();
				foreach(string k in conf.Keys) {
					List<string> boards = conf[k];
					foreach(string b in boards) {
						string input = await GetFromAsync(string.Format(leaderboardURI,year,b), k);
						//string input = Path.GetFullPath("./../../../inputs/lb.txt");
						try
						{
							obj = Newtonsoft.Json.JsonConvert.DeserializeObject<AoCLeaderboard>(input);
							//obj = JsonSerializer.Deserialize<AoCLeaderboard>(input);
							foreach (AoCUser u in obj.members.Values)
							{
								if (u.name == null)
								{
									u.name = "(anonymous user #" + u.id + ")";
								}
								if (u.id.Equals("1081403") || users.Contains(u)) continue;
								users.Add(u);
							}
						}
						catch(Exception e)
						{
							Console.WriteLine(e);
						}
					}
				}

				string mainTable = "<table> <tbody> <tr> <td class=\"typeheader\" colspan=\"3\">(25 items)<span class=\"fixedextenser\">4</span></td></tr><tr> <th title=\"System.String\">day</th> <th title=\"System.String,System.DateTime,System.Int32[]\">silver_order</th> <th title=\"System.String,System.DateTime,System.Int32[]\">gold_order</th> </tr>{0}</tbody></table>";
				StringBuilder builder = new StringBuilder();
				int m = int.Parse(puzzleNum);
				for(int d = 1; d <= m; d++) {
					if(Count(users, d.ToString(), "1") == 0) break;
					try
					{
						builder.Append(GetTableRow(users, d));
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
				builder.Append(GetTableRowScores(users));
				try
				{
					string htmlTemplate = File.ReadAllText(Path.GetFullPath("./../../../inputs/leaderboard_html.txt"));
					htmlTemplate = htmlTemplate.Replace("{", "{{").Replace("}", "}}").Replace("{{0}}", "{0}");
					string outputFile = Path.GetFullPath("./../../../leaderboard.html");
					if (File.Exists(outputFile))
					{
						File.Delete(outputFile);
					}

					File.WriteAllText(outputFile, string.Format(htmlTemplate, string.Format(mainTable, builder.ToString())));
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}

				Console.WriteLine("Finished writing leaderboard.");
			});
		}

		private static async Task<string> GetInputFor(string day, string sessionID) {
			//var url = new Uri(baseAddress + jsonurl);
			var jsonurl = string.Format("{0}{1}/day/{2}/input", baseAddress, year, day);
			Console.WriteLine(jsonurl);
			var cookieContainer = new CookieContainer();
			cookieContainer.Add(baseAddress, new Cookie("session", sessionID));

			HttpClient httpClient = new HttpClient(
				new HttpClientHandler {
					CookieContainer = cookieContainer,
					AutomaticDecompression = DecompressionMethods.Deflate|DecompressionMethods.GZip,
				}) {
				BaseAddress = baseAddress,
			};
			var response = await httpClient.GetAsync(jsonurl);
			if(response.IsSuccessStatusCode) {
				return await response.Content.ReadAsStringAsync();
			}
			return string.Empty;
		}

		private static async Task<string> GetFromAsync(string jsonurl, string sessionID) {
			//var url = new Uri(baseAddress + jsonurl);
			var cookieContainer = new CookieContainer();
			cookieContainer.Add(baseAddress, new Cookie("session", sessionID));

			HttpClient httpClient = new HttpClient(
				new HttpClientHandler {
					CookieContainer = cookieContainer,
					AutomaticDecompression = DecompressionMethods.Deflate|DecompressionMethods.GZip,
				}) {
				BaseAddress = baseAddress,
			};
			var response = await httpClient.GetAsync(jsonurl);
			if(response.IsSuccessStatusCode) {
				return await response.Content.ReadAsStringAsync();
			}
			return string.Empty;
		}

		private static string GetTableRow(List<AoCUser> users, int d) {
			string tablerowtable = "<tr> <td>{4}</td><td> <table> <tbody> <tr> <td class=\"typeheader\" colspan=\"3\">ø[{2}]&nbsp;<span class=\"fixedextenser\">4</span></td></tr><tr> <th title=\"System.String\">name</th> <th title=\"System.DateTime\">get_star_ts</th> <th title=\"System.Int32\">value</th> </tr>{0}</tbody> </table> </td><td> <table> <tbody> <tr> <td class=\"typeheader\" colspan=\"3\">ø[{3}]&nbsp;<span class=\"fixedextenser\">4</span></td></tr><tr> <th title=\"System.String\">name</th> <th title=\"System.DateTime\">get_star_ts</th> <th title=\"System.Int32\">value</th> </tr>{1}</tbody> </table> </td></tr>";
			string day = d.ToString();
			string[] parts = new string[2];
			parts[0] = "";
			parts[1] = "";

			DateTime start = new DateTime(int.Parse(year), (d>1?12:11), (d>1?d-1:30));
			start = start.AddHours(23);
			double avg = users.Where(x => x.completion_day_level.ContainsKey(day) && x.completion_day_level[day].ContainsKey("1")).Where(
				x =>
				{
					TimeSpan dur = x.completion_day_level[day]["1"].dateTime - start;
					return dur.TotalHours <= MAX_EXPECTED;
				}).Average(x => (x.completion_day_level[day]["1"].dateTime - start).TotalSeconds);
			TimeSpan averageTime = new TimeSpan(0, 0, (int)avg);

			for (int p = 1; p <= 2; p++) {
				string part = p.ToString();
				SortUsers(ref users, day, part);
				foreach(AoCUser user in users) {
					int pts = GetPointsForUser(ref users, user.id, day, part);
					if(d > 0)
						user.locPoints += pts;
					parts[p - 1] += GetUserLineScore(user, day, part, pts, averageTime);
				}
			}
			return string.Format(tablerowtable, parts[0], parts[1], Count(users, day, "1"), Count(users, day, "2"), day);
		}

		private static string GetTableRowScores(List<AoCUser> users) {
			users.Sort((x, y) => y.locPoints.CompareTo(x.locPoints));
			string tablerowtable = "<tr> <td>{2}</td><td> <table> <tbody> <tr> <td class=\"typeheader\" colspan=\"3\">ø[{1}]&nbsp;<span class=\"fixedextenser\">4</span></td></tr><tr> <th title=\"System.String\">name</th> <th title=\"System.Int32\">total</th> </tr>{0}</tbody> </table> </td> </table> </td></tr>";
			string row = "";
			foreach(AoCUser user in users) {
				int pts = user.locPoints;
				//user.locPoints += pts;
				row += GetUserTotalScore(user, pts);
			}
			return string.Format(tablerowtable, row, users.Count, "Total");
		}

		private static int Count(List<AoCUser> users, string day, string part) {
			return users.Count(x => {
				return x.completion_day_level.ContainsKey(day) && x.completion_day_level[day].ContainsKey(part);
			});
		}

		private static void SortUsers(ref List<AoCUser> users, string day, string part) {
			int d = int.Parse(day);
			DateTime start = new DateTime(int.Parse(year), (d > 1 ? 12 : 11), (d > 1 ? d - 1 : 30));
			start = start.AddHours(23);
			double avg = users.Where(x => x.completion_day_level.ContainsKey(day) && x.completion_day_level[day].ContainsKey("1")).Where(
				x =>
				{
					TimeSpan dur = x.completion_day_level[day]["1"].dateTime - start;
					return dur.TotalHours <= MAX_EXPECTED;
				}).Average(x => (x.completion_day_level[day]["1"].dateTime - start).TotalSeconds);
			TimeSpan averageTime = new TimeSpan(0, 0, (int)avg);


			users.Sort((x, y) => {
				bool xhas = true;
				bool yhas = true;
				if(!y.completion_day_level.ContainsKey(day) || !y.completion_day_level[day].ContainsKey(part)) {
					yhas = false;
				}
				if(!x.completion_day_level.ContainsKey(day) || !x.completion_day_level[day].ContainsKey(part)) {
					xhas = false;
				}
				if (xhas && yhas)
				{
					if(!CUSTOM_LEADERBOARD)
						return x.completion_day_level[day][part].dateTime.CompareTo(y.completion_day_level[day][part].dateTime);
					if (part == "1")
					{
						TimeSpan xDur, yDur;
						if((x.completion_day_level[day][part].dateTime - start).TotalHours > MAX_EXPECTED)
						{
							DateTime s = (x.completion_day_level[day][part].dateTime - averageTime);
							int extra = s.Minute % 5;
							s = s.AddMinutes(-extra);
							xDur = x.completion_day_level[day][part].dateTime - s;
						}
						else
						{
							xDur = (x.completion_day_level[day][part].dateTime - start);
						}
						if ((y.completion_day_level[day][part].dateTime - start).TotalHours > MAX_EXPECTED)
						{
							DateTime s = (y.completion_day_level[day][part].dateTime - averageTime);
							int extra = s.Minute % 5;
							s = s.AddMinutes(-extra);
							yDur = y.completion_day_level[day][part].dateTime - s;
						}
						else
						{
							yDur = (y.completion_day_level[day][part].dateTime - start);
						}
						return xDur.CompareTo(yDur);
						//return x.completion_day_level[day][part].dateTime.CompareTo(y.completion_day_level[day][part].dateTime);
					}
					var d1 = (x.completion_day_level[day]["2"].dateTime - x.completion_day_level[day]["1"].dateTime);
					var d2 = (y.completion_day_level[day]["2"].dateTime - y.completion_day_level[day]["1"].dateTime);
					return d1.CompareTo(d2);
				}
				else
					return yhas.CompareTo(xhas);
			});
		}

		private static int GetPointsForUser(ref List<AoCUser> users, string user, string day, string part) {
			//if (part == "1") return 0;
			int i = users.FindIndex(x => x.id == user);
			if(users[i].completion_day_level.ContainsKey(day) && users[i].completion_day_level[day].ContainsKey(part))
				return users.Count - i;
			return 0;
		}

		private static string GetUserLineScore(AoCUser user, string day, string part, int pts, TimeSpan averageTime)
		{
			int d = int.Parse(day);
			DateTime start = new DateTime(int.Parse(year), (d > 1 ? 12 : 11), (d > 1 ? d - 1 : 30));
			start = start.AddHours(23);
			string p = "<tr><td>{0}</td><td>{1}</td><td class=\"n\">{2}</td></tr>";
			if (user.completion_day_level.ContainsKey(day) && user.completion_day_level[day].ContainsKey(part))
			{
				if(!CUSTOM_LEADERBOARD)
					return string.Format(p, user.name, user.completion_day_level[day][part].dateTime.ToString("M/d/yyyy h:mm:ss tt"), pts);
				if (part == "1")
				{
					TimeSpan yDur;
					if ((user.completion_day_level[day][part].dateTime - start).TotalHours > MAX_EXPECTED)
					{
						DateTime s = (user.completion_day_level[day][part].dateTime - averageTime);
						int extra = s.Minute % 5;
						s = s.AddMinutes(-extra);
						yDur = user.completion_day_level[day][part].dateTime - s;
					}
					else
					{
						yDur = (user.completion_day_level[day][part].dateTime - start);
					}
					return string.Format(p, user.name, yDur.ToString(), pts);
				}
				TimeSpan dur = (user.completion_day_level[day]["2"].dateTime - user.completion_day_level[day]["1"].dateTime);
				return string.Format(p, user.name, dur.ToString(), pts);
			}
			long.TryParse(user.last_star_ts.ToString(), out long last);
			if(last == 0)
				return string.Empty;
			return string.Format(p, user.name, "----", "0");
		}

		private static string GetUserTotalScore(AoCUser user, int pts) {
			if(pts == 0) return string.Empty;
			string p = "<tr><td>{0}</td><td class=\"n\">{1}</td></tr>";
			return string.Format(p, user.name, pts.ToString());
		}
	}
}
