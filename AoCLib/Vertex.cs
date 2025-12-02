using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draco18s.AoCLib
{
	public class Vertex<T> where T : class
	{
		// For pathfinding
		public bool Visited;
		public int Score;

		public T Value => _value;
		public List<(Vertex<T> node, int weight)> Edges => _edges;

		private readonly List<(Vertex<T> node, int weight)> _edges;
		private readonly T _value;

		public Vertex(T value)
		{
			_value = value;
			_edges = new List<(Vertex<T> neighbor, int weight)>();
		}

		// Edges are only added 1-way, in order to create a 2-way edge you must call AddEdge from the neighbor separately
		// This is to allow functionality of 1-way paths
		public void AddEdge(Vertex<T> neighbor, int weight)
		{
			if (neighbor == this) return;
			if (_edges.Any(x => x.node == neighbor))
			{
				//Console.Error.WriteLine("Attempting to add duplicate neighbor! Check this.");
				return; // Will not add duplicate edges
			}
			_edges.Add((neighbor, weight));
		}

		public bool HasNeighbor(T neighbor)
		{
			return _edges.Find(x => x.node.Value == neighbor) != default;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vertex<T> other)
			{
				return other.Value.Equals(Value);
			}
			return false;
		}

		public override string ToString()
		{
			return $"Vertex<{_value.ToString()}>";
		}
	}
}
