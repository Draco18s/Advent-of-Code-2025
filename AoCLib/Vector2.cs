using System;

namespace Draco18s.AoCLib {
	public struct Vector2
	{
		public static readonly Vector2 ZERO = new Vector2(0, 0);
		public static readonly Vector2 ONE = new Vector2(1, 1);
		public static readonly Vector2 LEFT = new Vector2(-1, 0);
		public static readonly Vector2 RIGHT = new Vector2(1, 0);
		public static readonly Vector2 UP = new Vector2(0, -1);
		public static readonly Vector2 DOWN = new Vector2(0, 1);

		public readonly int x;
		public readonly int y;
		public double magnitude => Math.Sqrt(x * x + y * y);
		public Vector2(int _x, int _y) {
			x = _x;
			y = _y;
		}

		public static Vector2 Parse(string val)
		{
			return Parse(val, ',');
		}

		public static Vector2 Parse(string val, char split)
		{
			string[] vals = val.Split(split);
			return new Vector2(int.Parse(vals[0]), int.Parse(vals[1]));
		}

		public static Vector2 operator *(Vector2 a, int b)
		{
			return new Vector2(a.x * b, a.y * b);
		}
		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x - b.x, a.y - b.y);
		}
		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x + b.x, a.y + b.y);
		}
		public static bool operator ==(Vector2 a, Vector2 b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Vector2 a, Vector2 b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public override string ToString() {
			return $"({x},{y})";
		}

		public override bool Equals(object other)
		{
			if (other is Vector2 v)
				return Equals(v);
			return false;
		}

		public bool Equals(Vector2 other)
		{
			return x == other.x && y == other.y;
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ (y.GetHashCode() << 2);
		}
	}
}