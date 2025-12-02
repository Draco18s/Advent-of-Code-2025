using System;

namespace Draco18s.AoCLib
{
	public struct Vector3L
	{
		public readonly long x;
		public readonly long y;
		public readonly long z;
		public Vector3L(long _x, long _y, long _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}

		public static Vector3L Parse(string val)
		{
			return Parse(val, ',');
		}

		public static Vector3L Parse(string val, char split)
		{
			string[] vals = val.Split(split);
			return new Vector3L(long.Parse(vals[0]), long.Parse(vals[1]), long.Parse(vals[2]));
		}

		public static Vector3L operator *(Vector3L a, long b)
		{
			return new Vector3L(a.x * b, a.y * b, a.z * b);
		}

		public static Vector3L operator /(Vector3L a, long b)
		{
			if (b == 0) throw new ArgumentException("Div by zero");
			return new Vector3L(a.x / b, a.y / b, a.z / b);
		}

		public static Vector3L operator -(Vector3L a, Vector3L b)
		{
			return new Vector3L(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3L operator +(Vector3L a, Vector3L b)
		{
			return new Vector3L(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static bool operator ==(Vector3L a, Vector3L b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Vector3L a, Vector3L b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return (int)((1100011100011L * x + 10011101L * y + z) % int.MaxValue);
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector3L o)
			{
				return o.x == x && o.y == y && o.z == z;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("({0},{1},{2})", x, y, z);
		}
	}
}