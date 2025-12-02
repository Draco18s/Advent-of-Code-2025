using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draco18s.AoCLib
{
	public static class AoCMath
	{
		public static long GDC(long a, long b) => b == 1 ? 1 : (b == 0 ? a : GDC(b, a % b));
		public static long LCM(long b, long a) => b == 1 ? 1 : (b == 0 ? a : a / GDC(a, b) * b);

		public static long GetBinCoeff(long N, long K)
		{
			// This function gets the total number of unique combinations based upon N and K.
			// N is the total number of items.
			// K is the size of the group.
			// Total number of unique combinations = N! / ( K! (N - K)! ).
			// This function is less efficient, but is more likely to not overflow when N and K are large.
			// Taken from:  http://blog.plover.com/math/choose.html
			//
			long r = 1;
			long d;
			if (K > N) return 0;
			for (d = 1; d <= K; d++)
			{
				r *= N--;
				r /= d;
			}
			return r;
		}
	}
}
