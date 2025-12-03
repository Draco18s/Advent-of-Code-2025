using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdventofCode2025
{
	internal static class Day3
	{
		internal static long Part1(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			foreach (string line in lines)
			{
				long max = GetMaxJolts1(line);
				result += max;
			}
			return result;
		}

		private static long GetMaxJolts1(string line)
		{
			int best = 0;
			for (int i1 = 0; i1 < line.Length - 1; i1++)
			{
				for (int i2 = i1+1; i2 < line.Length; i2++)
				{
					int test = int.Parse($"{line[i1]}{line[i2]}");
					if (test > best)
					{
						best = test;
					}
				}
			}

			return best;
		}

		internal static long Part2(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			foreach (string line in lines)
			{
				long max = GetMaxJolts2(line, 12);
				result += max;
			}
			return result;
		}

		private static long GetMaxJolts2(string line, int digits)
		{
			if(digits == 2)
				return GetMaxJolts1(line);

			string bestStr = GetMaxJolts2(line, digits - 1).ToString();

			string[] parts = new string[bestStr.Length+1];
			int j = 0;

			long bestResult = 0;

			for (int i = 0; i < parts.Length-1; i++)
			{
				int p = line.IndexOf(bestStr[i], j);
				parts[i] = line.Substring(j, p-j); // I forgot that line[j..p] syntax existed.
				j = p+1;
			}
			parts[^1] = line.Substring(j);
			for (int i = 0; i < parts.Length; i++)
			{
				if (int.TryParse($"{GetMaxChar(parts[i])}", out int v))
				{
					string result = $"{string.Join("", bestStr.Take(i))}{v}{string.Join("", bestStr.Skip(i))}";
					bestResult = Math.Max(bestResult, long.Parse(result));
				}
			}

			return bestResult;
		}

		private static char GetMaxChar(string line)
		{
			char best = '-';
			foreach (var test in line)
			{
				if (test > best)
				{
					best = test;
				}
			}

			if (best == '-')
				return ' ';
			return best;
		}
	}
}
