using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Draco18s.AoCLib;
using RestSharp.Validation;

namespace AdventofCode2025
{
	internal static class Day9
	{
		internal static long Part1(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			List<Vector2> redTiles = new List<Vector2>();
			foreach (string line in lines)
			{
				redTiles.Add(Vector2.Parse(line));
			}

			foreach (var r1 in redTiles)
			{
				foreach (var r2 in redTiles)
				{
					(long X, long Y, long W, long H) r = (Math.Min(r1.x, r2.x), Math.Min(r1.y, r2.y), 0, 0);
					//Rectangle r = new Rectangle((int)Math.Min(r1.x, r2.x), (int)Math.Min(r1.y, r2.y), 0, 0);
					r.W = Math.Max(r1.x, r2.x) - r.X + 1;
					r.H = Math.Max(r1.y, r2.y) - r.Y + 1;
					long area = r.W * r.H;
					result = Math.Max(result, area);
				}
			}
			return result;
		}

		internal static long Part2(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0;
			List<Vector2> redTiles = new List<Vector2>();
			
			foreach (string line in lines)
			{
				if (redTiles.Count > 0)
				{
					redTiles.Add(Vector2.Parse(line));

					
				}
				else
				{
					redTiles.Add(Vector2.Parse(line));
				}
			}

			List<int> allX = redTiles.Select(t => t.x).Distinct().OrderBy(x => x).ToList();
			List<int> allY = redTiles.Select(t => t.y).Distinct().OrderBy(x => x).ToList();

			foreach (int x in allX.ToArray())
			{
				if (!allX.Contains(x + 1) && allX.Contains(x + 2))
				{
					allX.Add(x+1);
				}
			}
			foreach (int y in allY.ToArray())
			{
				if (!allY.Contains(y + 1) && allY.Contains(y + 2))
				{
					allY.Add(y + 1);
				}
			}
			allX.Sort();
			allY.Sort();

			/*Dictionary<int, int> tileToMapX = redTiles.Select(t => t.x).Distinct().ToDictionary(t => t, t => allX.IndexOf(t));
			Dictionary<int, int> mapToTileX = tileToMapX.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

			Dictionary<int, int> tileToMapY = redTiles.Select(t => t.y).Distinct().ToDictionary(t => t, t => allY.IndexOf(t));
			Dictionary<int, int> mapToTileY = tileToMapY.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);*/

			Dictionary<Vector2, Vector2> tileToMap = redTiles.ToDictionary(t => t, t => new Vector2(allX.IndexOf(t.x), allY.IndexOf(t.y)));
			Dictionary<Vector2, Vector2> mapToTile = tileToMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

			Grid g = new Grid(allX.Count, allY.Count);
			g.FloodFill(Vector2.ZERO, '.', (value, fill) => true, false);
			//List<Vector2> greenTiles = new List<Vector2>();

			for (var i = 0; i < redTiles.Count; i++)
			{
				Vector2 p = redTiles[i];
				g[tileToMap[p]] = 'R';
				if(i == 0) continue;
				Vector2 g1 = redTiles[i-1];
				Vector2 g2 = redTiles[i];

				for (int x = Math.Min(g1.x, g2.x); x <= Math.Max(g1.x, g2.x); x++)
				{
					for (int y = Math.Min(g1.y, g2.y); y <= Math.Max(g1.y, g2.y); y++)
					{
						if (!allX.Contains(x) || !allY.Contains(y)) continue;
						Vector2 gp = new Vector2(allX.IndexOf(x), allY.IndexOf(y));
						if(g[gp] != 'R')
							g[gp] = 'G';
					}
				}
			}

			Vector2 g1_ = redTiles[0];
			Vector2 g2_ = redTiles[^1];
			for (int x = Math.Min(g1_.x, g2_.x); x <= Math.Max(g1_.x, g2_.x); x++)
			{
				for (int y = Math.Min(g1_.y, g2_.y); y <= Math.Max(g1_.y, g2_.y); y++)
				{
					if (!allX.Contains(x) || !allY.Contains(y)) continue;
					Vector2 gp = new Vector2(allX.IndexOf(x), allY.IndexOf(y));
					if (g[gp] != 'R')
						g[gp] = 'G';
				}
			}

			g.IncreaseGridToInclude(new Vector2(g.MinX - 1, g.MinY - 1), () => '.');
			g.IncreaseGridToInclude(new Vector2(g.MaxX + 1, g.MaxY + 1), () => '.');
			g.FloodFill(new Vector2(g.MinX, g.MinY), ' ', (value, fill) => value == '.', false);
			do
			{
				Vector2 p = g.FindFirst('.');
				if (p.x < g.MinX) break;
				g.FloodFill(p, 'G', (value, fill) => value == '.', false);
			} while (true);
			//Console.WriteLine(g);

			(Vector2 a, Vector2 b) best = (Vector2.ZERO, Vector2.ZERO);

			Vector2[] map = mapToTile.Keys.ToArray();
			for (int i = 0; i < map.Length; i++)
			{
				for (int j = map.Length - 1; j > i; j--)
				{
					Vector2 r1 = map[i];
					Vector2 r2 = map[j];

					int x = Math.Min(r1.x, r2.x);
					int y = Math.Min(r1.y, r2.y);
					int w = Math.Abs(r2.x - r1.x);
					int h = Math.Abs(r2.y - r1.y);

					Vector2 t1 = mapToTile[r1];
					Vector2 t2 = mapToTile[r2];

					long area = (Math.Abs(t1.x - t2.x) + 1) * (Math.Abs(t1.y - t2.y) + 1);

					if (area <= result) continue;
					(int, int, int, int) rect = (x, y, w, h);

					if (Validate(rect, g))
					{
						result = area;
						best = (t1, t2);
					}
				}
			}
			Console.WriteLine($"{best.a}-{best.b}");
			g[tileToMap[best.a]] = 'A';
			g[tileToMap[best.b]] = 'B';

			string path = Path.GetFullPath("./../../../inputs/day9_print.txt");
			File.WriteAllText(path, g.ToString());

			//  35  / 123008 :  4161542
			// 473  / 122967 : 24161542
			// 477  / 122971 : 19030003
			// 1820 / 122799 : 96700464
			// 1824 / 122798 : 96700464


			// 1737/122755 : 103975812
			// 1738/122755 : 136045940
			// 1739/122753 : 828833572
			// 1834/122578 : 828833572

			// too high     1556457424
			// too high?    1732118848
			return result;
		}

		private static bool Validate((int X, int Y, int W, int H) r, Grid g)
		{
			(int X, int Y, int W, int H) = r;
			for (int x = X; x <= X+W; x++)
			{
				if (g[x, Y] == ' ') return false;
				if (g[x, Y+H] == ' ') return false;
			}
			for (int y = Y; y <= Y + H; y++)
			{
				if (g[X, y] == ' ') return false;
				if (g[X+W, y] == ' ') return false;
			}
			return true;
		}
	}
}
