using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdventofCode2025
{
	internal static class Day5
	{
		private class IntRange
		{
			public readonly long min;
			public readonly long max;

			public IntRange(long mn, long mx)
			{
				min = mn;
				max = mx;
			}

			public long GetSize()
			{
				return max - min + 1;
			}

			public long GetSize(long last)
			{
				if (last > max) return 0;
				return max - Math.Max(last,min) + 1;
			}
		}

		internal static long Part1(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			List<IntRange> ranges = new List<IntRange>();
			
			bool readingRanges = true;
			foreach (string line in lines)
			{
				if (string.IsNullOrEmpty(line))
				{
					readingRanges = false;
					ranges = ranges.OrderBy(r => r.min).ToList();
					continue;
				}

				if (readingRanges)
				{
					string[] parts = line.Split('-');
					IntRange r = new IntRange(long.Parse(parts[0]), long.Parse(parts[1]));
					ranges.Add(r);
				}
				else
				{
					long val = long.Parse(line);
					bool b = ranges.SkipWhile(r => val > r.min && val > r.max).Any(r => r.min <= val && val <= r.max);
					if (b) result++;
				}
			}
			return result;
		}

		internal static long Part2(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			List<IntRange> ranges = new List<IntRange>();

			bool readingRanges = true;
			foreach (string line in lines)
			{
				if (string.IsNullOrEmpty(line))
				{
					readingRanges = false;
					ranges = ranges.OrderBy(r => r.min).ToList();
					break;
				}

				string[] parts = line.Split('-');
				IntRange r = new IntRange(long.Parse(parts[0]), long.Parse(parts[1]));
				ranges.Add(r);
			}

			long lastId = -1;
			foreach (IntRange range in ranges)
			{
				result += range.GetSize(lastId);
				lastId = Math.Max(lastId, range.max + 1);
			}

			return result;
		}
	}
}
