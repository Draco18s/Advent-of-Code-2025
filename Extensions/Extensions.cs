using System.Collections.Generic;
using Draco18s.AoCLib;

public static class Extentions {
	public static IEnumerable<T> Enumerate<T>(this T[,] arr) {
		int l1 = arr.GetLength(0);
		int l2 = arr.GetLength(1);
		for(int y=0;y<l1;y++) {
			for(int x=0;x<l2;x++) {
				yield return arr[x, y];
			}
		}
	}

	private static System.Random rng = new System.Random();  

	public static void Shuffle<T>(this IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	//I'm fekking tired of converting lists to queues
	public static void Add<T>(this Queue<T> que, T obj)
	{
		que.Enqueue(obj);
	}

	public static T RemoveAt<T>(this Queue<T> que, int _)
	{
		return que.Dequeue();
	}

	public static Vector2 Offset(this Orientation dir)
	{
		switch (dir)
		{
			case Orientation.EAST:
				return new Vector2(1, 0);
			case Orientation.WEST:
				return new Vector2(-1, 0);
			case Orientation.SOUTH:
				return new Vector2(0, 1);
			case Orientation.NORTH:
				return new Vector2(0, 1);
		}

		return new Vector2(0, 0);
	}
}