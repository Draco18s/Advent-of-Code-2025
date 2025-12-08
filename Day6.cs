using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Draco18s.AoCLib;

namespace AdventofCode2025
{
	internal static class Day6
	{
		internal static long Part1(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			List<List<long>> array = new List<List<long>>();
			List<char> operators = new();
			foreach (string line in lines)
			{
				if (line.Contains('+'))
				{
					operators = line.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s[0]).ToList();
					break;
				}
				List<long> ff = line.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Where(s => !string.IsNullOrWhiteSpace(s)).Select(long.Parse).ToList();
				array.Add(ff);
			}

			for (int x = 0; x < operators.Count; x++)
			{
				List<long> numbers = new List<long>();
				for (int y = 0; y < array.Count; y++)
				{
					numbers.Add(array[y][x]);
				}

				switch (operators[x])
				{
					case '+':
						result += numbers.Sum();
						break;
					case '*':
						result += numbers.Aggregate((s, a) => s == 0 ? a : s * a);
						break;
					default:
						break;
				}
			}
			return result;
		}

		internal static long Part2(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			Grid g = new Grid(input, true);
			List<long> numbers = new List<long>();
			for (int x = g.MaxX - 1; x >= 0; x--)
			{
				long num = 0;
				for (int y = 0; y < g.MaxY; y++)
				{
					if (g[x, y] == '+')
					{
						break;
					}
					if (g[x, y] == '*')
					{
						break;
					}
					if (g[x, y] == ' ')
					{
						continue;
					}
					num *= 10;
					num += int.Parse($"{(char)g[x, y]}");
				}
				numbers.Add(num);
				switch (g[x, g.MaxY-1])
				{
					case '+':
						result += numbers.Sum();
						numbers.Clear();
						break;
					case '*':
						result += numbers.Aggregate((s, a) => s == 0 ? a : s * a);
						numbers.Clear();
						break;
					default:
						break;
				}
			}
			return result;
		}
	}
}
