using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdventofCode2025
{
	internal static class Day2
	{
		internal static long Part1(string input)
		{
			string[] lines = input.Split(',');
			long result = 0l;
			foreach (string line in lines)
			{
				string[] ranges = line.Split('-');
				long min = long.Parse(ranges[0]);
				long max = long.Parse(ranges[1]);
				for (long i = min; i <= max; i++)
				{
					string iStr = i.ToString();
					int l = iStr.Length;
					if (l % 2 == 1) continue;
					if (iStr.EndsWith(iStr.Substring(0, l / 2)))
					{
						result += i;
					}
				}
			}
			return result;
		}

		internal static long Part2(string input)
		{
			string[] lines = input.Split(',');
			long result = 0l;
			foreach (string line in lines)
			{
				string[] ranges = line.Split('-');
				long min = long.Parse(ranges[0]);
				long max = long.Parse(ranges[1]);
				for (long i = min; i <= max; i++)
				{
					string iStr = i.ToString();
					int l = iStr.Length;
					if (IsValid(iStr, l))
					{
						continue;
					}

					result += i;
				}
			}
			return result;
		}

		private static bool IsValid(string str, int len)
		{
			for (int i = 1; i <= len/2; i++)
			{
				if ((str.Length / (float)i)%1 > 0)
				{
					continue;
				}
				string[] subStr = Split(str,i).ToArray();
				if (subStr.All(x => x == subStr[0]))
					return false;
			}

			return true;
		}

		static IEnumerable<string> Split(string str, int chunkSize)
		{
			return Enumerable.Range(0, str.Length / chunkSize)
				.Select(i => str.Substring(i * chunkSize, chunkSize));
		}
	}
}
