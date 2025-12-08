using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Draco18s.AoCLib;

namespace AdventofCode2025
{
	internal static class Day7
	{
		internal static long Part1(string input)
		{
			Grid g = new Grid(input, true);
			HashSet<Vector2> openSet = new HashSet<Vector2>();
			for (int x = 0; x < g.Width; x++)
			{
				if (g[x, 0] == 'S')
				{
					openSet.Add(new Vector2(x,0));
				}
			}

			return CountBeamSplit(g, openSet);
		}

		private static long CountBeamSplit(Grid grid, HashSet<Vector2> openSet)
		{
			long result = 0;
			HashSet<Vector2> endSet = new HashSet<Vector2>();
			while (openSet.Any())
			{
				Vector2 p = openSet.First();
				openSet.Remove(p);
				for (int y = p.y; y < grid.Height; y++)
				{
					if (grid[p.x, y] == '^')
					{
						if (endSet.Add(new Vector2(p.x, y)))
						{
							openSet.Add(new Vector2(p.x - 1, y));
							openSet.Add(new Vector2(p.x + 1, y));
						}
						break;
					}

				}
			}
			return endSet.Count;
		}

		internal static long Part2(string input)
		{
			Grid g = new Grid(input, true);
			Dictionary<Vector2, long> openSet = new Dictionary<Vector2, long>();
			for (int x = 0; x < g.Width; x++)
			{
				if (g[x, 0] == 'S')
				{
					openSet.Add(new Vector2(x, 0), 1);
				}
			}

			return CountBeamChronoSplit(g, openSet);
		}

		private static long CountBeamChronoSplit(Grid grid, Dictionary<Vector2, long> openSet)
		{
			long result = 0;
			while (openSet.Any(kvp => kvp.Value > 0))
			{
				Vector2 p = openSet.OrderBy(kvp => kvp.Key.y).First(kvp => kvp.Value > 0).Key;
				long count = openSet[p];
				openSet[p] = 0;
				for (int y = p.y; y < grid.Height; y++)
				{
					if (grid[p.x, y] == '^')
					{
						if (!openSet.ContainsKey(new Vector2(p.x - 1, y))) openSet.Add(new Vector2(p.x - 1, y), 0);
						if (!openSet.ContainsKey(new Vector2(p.x + 1, y))) openSet.Add(new Vector2(p.x + 1, y), 0);

						openSet[new Vector2(p.x - 1, y)] += count;
						openSet[new Vector2(p.x + 1, y)] += count;
						break;
					}

					if (y != grid.Height - 1) continue;
					result += count;
					break;
				}
			}

			return result;
		}
	}
}
