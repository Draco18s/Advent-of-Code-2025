using System;

namespace Draco18s.AoCLib
{
	public readonly struct Vector3
	{
		public readonly int x;
		public readonly int y;
		public readonly int z;
		public Vector3(int _x, int _y, int _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}

		public static Vector3 Parse(string val)
		{
			return Parse(val, ',');
		}

		public static Vector3 Parse(string val, char split)
		{
			string[] vals = val.Split(split);
			return new Vector3(int.Parse(vals[0]), int.Parse(vals[1]), int.Parse(vals[2]));
		}

		public static Vector3 operator *(Vector3 a, int b)
		{
			return new Vector3(a.x * b, a.y * b, a.z * b);
		}

		public static Vector3 operator /(Vector3 a, int b)
		{
			return new Vector3(a.x / b, a.y / b, a.z / b);
		}

		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static bool operator ==(Vector3 a, Vector3 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Vector3 a, Vector3 b)
		{
			return !a.Equals(b);
		}

		public static double Distance(Vector3 p1, Vector3 p2)
		{
			double dx = p1.x - p2.x;
			double dy = p1.y - p2.y;
			double dz = p1.z - p2.z;
			return Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector3 o)
			{
				return o.x == x && o.y == y && o.z == z;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)((1100011100011L * x + 10011101L * y + z) % int.MaxValue);
		}

		public override string ToString()
		{
			return $"({x},{y},{z})";
		}
	}
}