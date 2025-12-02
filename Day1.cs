using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdventofCode2025 {
	internal static class Day1 {
		internal static long Part1(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;

			long pointer = 50;
			foreach (string l in lines)
			{
				int mult = l.StartsWith('L') ? -1 : 1;
				int offset = int.Parse(l.Substring(1));

				pointer = (pointer + offset * mult)%100;

				if (pointer == 0)
				{
					result++;
				}
			}

			return result;
		}

		internal static long Part2(string input) {
			string[] lines = input.Split('\n');
			long result = 0l;

			long pointer = 50;
			foreach (string l in lines)
			{
				int mult = l.StartsWith('L') ? -1 : 1;
				int offset = int.Parse(l.Substring(1));

				for (; offset>0; offset--)
				{
					pointer += mult;

					if (pointer == 100)
						pointer = 0;

					if (pointer == 0)
					{
						result++;
					}

					if (pointer == -1)
						pointer = 99;
				}
			}

			return result;
		}
	}
}