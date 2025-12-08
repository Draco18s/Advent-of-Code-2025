using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Draco18s.AoCLib;

namespace AdventofCode2025
{
	internal static class Day4
	{
		internal static long Part1(string input)
		{
			long result = 0l;
			Grid g = new Grid(input, true);
			for (int x = 0; x < g.Width; x++)
			{
				for (int y = 0; y < g.Height; y++)
				{
					if(g[x,y] != '@') continue;
					if (CountRolls(g, x, y) < 4)
					{
						result++;
					}
				}
			}
			return result;
		}

		private static int CountRolls(Grid g, int x, int y)
		{
			int count = 0;
			for (int i = -1; i <= 1; i+=2)
			{
				if (g[x + i, y    , false, () => '.'] == '@') count++;
				if (g[x    , y + i, false, () => '.'] == '@') count++;
				if (g[x + i, y + i, false, () => '.'] == '@') count++;
				if (g[x - i, y + i, false, () => '.'] == '@') count++;
			}

			return count;
		}

		internal static long Part2(string input)
		{
			long result = 0l;
			Grid g = new Grid(input, true);
			bool didRemove = false;
			do
			{
				didRemove = false;
				for (int x = 0; x < g.Width; x++)
				{
					for (int y = 0; y < g.Height; y++)
					{
						if (g[x, y] != '@') continue;
						if (CountRolls(g, x, y) < 4)
						{
							result++;
							g[x, y] = '.';
							didRemove = true;
						}
					}
				}
			} while (didRemove);

			return result;
		}
	}
}
