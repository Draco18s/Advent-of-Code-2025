using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Draco18s.AoCLib;

namespace AdventofCode2025
{
	internal static class Day8
	{
		private class Network
		{
			private readonly List<Edge> edges = new List<Edge>();

			public bool Contains(Vector3 p)
			{
				return edges.Any(e => e.A == p || e.B == p);
			}

			public void Add(Edge e)
			{
				edges.Add(e);
			}

			public void Add(Network n)
			{
				edges.AddRange(n.edges);
			}

			public int Size()
			{
				IEnumerable<Vector3> pts = edges.Select(e => e.A).Concat(edges.Select(e => e.B)).Distinct();
				return pts.Count();
			}

			public Vector3 NearestTo(Vector3 p1)
			{
				return edges.Select(e => e.A).Concat(edges.Select(e => e.B)).Distinct().OrderBy(p2 => Vector3.Distance(p1, p2)).First();
			}
		}

		private class Edge
		{
			public readonly Vector3 A;
			public readonly Vector3 B;

			public Edge(Vector3 p1, Vector3 p2)
			{
				A = p1;
				B = p2;
			}

			public double Distance()
			{
				return Vector3.Distance(A, B);
			}

			public override bool Equals(object obj)
			{
				if (obj is Edge e)
					return (e.A == A && e.B == B) || (e.A == B && e.B == A);
				return false;
			}

			public override int GetHashCode()
			{
				return Math.Min(A.GetHashCode(), B.GetHashCode()) * 17 + Math.Max(A.GetHashCode(), B.GetHashCode());
			}

			public override string ToString()
			{
				return $"{A} - {B}";
			}
		}
		internal static long Part1(string input)
		{
			string[] lines = input.Split('\n');

			List<Vector3> boxes = new List<Vector3>();
			long result = 0l;
			foreach (string line in lines)
			{
				boxes.Add(Vector3.Parse(line));
			}

			return ConnectClosest(boxes, lines.Length > 20 ? 1000 : 10);
		}

		private static long ConnectClosest(List<Vector3> boxes, int i)
		{
			Queue<Vector3> sorted = new Queue<Vector3>(boxes);
			List<Edge> shortestEdges = new List<Edge>();
			do
			{
				Vector3 p1 = sorted.Dequeue();
				var ord = boxes.OrderBy(p2 => Vector3.Distance(p1, p2));
				Vector3[] nextChoice = ord.Skip(1).ToArray();
				Vector3 p2 = nextChoice[0];
				shortestEdges.Add(new Edge(p1, p2));

				//this is stupid but it works

				p2 = nextChoice[1];
				shortestEdges.Add(new Edge(p1, p2));
				p2 = nextChoice[2];
				shortestEdges.Add(new Edge(p1, p2));
				p2 = nextChoice[3];
				shortestEdges.Add(new Edge(p1, p2));
			} while (sorted.TryPeek(out _));

			Edge[] dist = shortestEdges.Distinct().ToArray();
			Edge[] best = dist.OrderBy(e => e.Distance()).Take(i).ToArray();
			IEnumerable<Network> largest = GetThreeLargest(best);
			long result = 1;
			foreach (Network net in largest)
			{
				int s = net.Size();
				result *= s;
			}

			return result;
		}

		private static IEnumerable<Network> GetThreeLargest(IEnumerable<Edge> best)
		{
			List<Network> networks = new List<Network>();
			foreach (Edge edge in best)
			{
				Network[] nets = networks.Where(n => n.Contains(edge.A) || n.Contains(edge.B)).ToArray();
				if (nets.Any())
				{
					if (nets.Length == 1)
					{
						nets[0].Add(edge);
					}
					else
					{
						int q = nets.Length;
						nets[0].Add(edge);
						for (int i = 1; i < nets.Length; i++)
						{
							nets[0].Add(nets[i]);
							networks.Remove(nets[i]);
						}
					}
				}
				else
				{
					Network newNet = new Network();
					newNet.Add(edge);
					networks.Add(newNet);
				}
			}
			return networks.OrderByDescending(n => n.Size()).Take(3);
		}

		internal static long Part2(string input)
		{
			string[] lines = input.Split('\n');

			List<Vector3> boxes = new List<Vector3>();
			foreach (string line in lines)
			{
				boxes.Add(Vector3.Parse(line));
			}

			return ConnectMST(boxes, lines.Length > 20 ? 1000 : 10);
		}

		private static long ConnectMST(List<Vector3> boxes, int i)
		{
			Queue<Vector3> sorted = new Queue<Vector3>(boxes);
			List<Edge> shortestEdges = new List<Edge>();
			do
			{
				Vector3 p1 = sorted.Dequeue();
				Vector3[] ord = boxes.OrderBy(p2 => Vector3.Distance(p1, p2)).Skip(1).ToArray();
				shortestEdges.Add(new Edge(p1, ord[0]));
			} while (sorted.TryPeek(out _));

			Edge[] dist = shortestEdges.Distinct().OrderBy(e => e.Distance()).ToArray();
			Network mstNet = new Network();
			mstNet.Add(dist[0]);
			boxes.Remove(dist[0].A);
			boxes.Remove(dist[0].B);
			List<Vector3> addOrder = new List<Vector3>();
			do
			{
				AddNextNearest(mstNet, boxes, addOrder);
			} while (boxes.Any());

			Console.WriteLine($"{addOrder[^1].x} * {addOrder[^2].x}");
			long result = addOrder[^1].x * addOrder[^2].x;
			return result;
		}

		private static void AddNextNearest(Network mstNet, List<Vector3> boxes, List<Vector3> added)
		{
			Edge newEdge = boxes.Select(b => new Edge(b, mstNet.NearestTo(b))).OrderBy(e => e.Distance()).First();
			added.Add(newEdge.A);
			added.Add(newEdge.B);
			boxes.Remove(newEdge.A);
			mstNet.Add(newEdge);
		}
	}
}
