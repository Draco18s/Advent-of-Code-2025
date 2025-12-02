using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.VisualBasic;

namespace Draco18s.AoCLib {
	public class Grid {
		int[,] cells;
		private int width;
		private int height;
		private int offsetx;
		private int offsety;

		public delegate int EdgeHandler();
		public static EdgeHandler returnZero = () => 0;
		public static EdgeHandler returnInf = () => int.MaxValue;
		public static EdgeHandler returnNegInf = () => int.MinValue;

		public static readonly Vector2[] FACING = new[] { Vector2.RIGHT, Vector2.LEFT, Vector2.UP, Vector2.DOWN };

		public Grid(int w, int h, int offx=0, int offy=0) {
			width = w;
			height = h;
			cells = new int[w,h];
			offsetx = offx;
			offsety = offy;
		}

		public Grid(string input, bool asChar, char separator=(char)0, int offx=0, int offy=0) {
			string[] lines = input.Split('\n');
			width = lines.Max(x => x.Length);
			height = lines.Length;
			offsetx = offx;
			offsety = offy;

			cells = new int[width,height];
			int y = 0;
			foreach(string lin in lines) {
				int x = 0;
				string[] ca = null;
				
				if(separator == (char)0)
					ca = Regex.Split(lin, string.Empty);
				else
					ca = lin.Split(separator);

				foreach(string c in ca) {
					if(c == string.Empty) continue;
					cells[x,y] = (!asChar) ? int.Parse(c) : (int)c[0];
					x++;
				}
				y++;
			}
		}

		public Grid(Grid o)
		{
			width = o.Width;
			height = o.Height;
			cells = new int[width, height];
			offsetx = o.offsetx;
			offsety = o.offsety;
			for (int x = 0; x < o.Width; x++)
			{
				for (int y = 0; y < o.Height; y++)
				{
					cells[x, y] = o.cells[x, y];
				}
			}
		}

		public int Width => width;
		public int Height => height;
		public int MinX => offsetx;
		public int MinY => offsety;
		public int MaxX => width+offsetx;
		public int MaxY => height+offsety;

		public int this[int X, int Y, bool useOffset=true]
		{
			get => cells[X-(useOffset?offsetx:0),Y-(useOffset?offsety:0)];
			set => cells[X-(useOffset?offsetx:0),Y-(useOffset?offsety:0)] = value;
		}

		public int this[Vector2 p, bool useOffset=true]
		{
			get => this[p.x,p.y,useOffset];
			set => this[p.x,p.y,useOffset] = value;
		}

		public int this[Vector2 p, bool useOffset, EdgeHandler edges]
		{
			get => this[p.x, p.y, useOffset, edges];
			set => this[p.x, p.y, useOffset, edges] = value;
		}

		public int this[int X, int Y, bool useOffset, EdgeHandler edges]
		{
			get {
				int xx = X - (useOffset ? offsetx : 0);
				int yy = Y - (useOffset ? offsety : 0);
				if (xx < 0 || xx >= this.Width || yy < 0 || yy >= this.Height) return edges();

				return cells[xx, yy];
			}
			set {
				int xx = X - (useOffset ? offsetx : 0);
				int yy = Y - (useOffset ? offsety : 0);
				if (xx < 0 || xx >= this.Width) return;
				if (yy < 0 || yy >= this.Height) return;

				cells[xx, yy] = value;
			}
		}


		public int GetLength(int i) {
			return cells.GetLength(i);
		}

		internal bool IsInside(Vector2 p1)
		{
			if (p1.x < MinX || p1.x >= MaxX) return false;
			if (p1.y < MinY || p1.y >= MaxY) return false;
			return true;
		}

		internal void IncreaseGridToInclude(Vector2 p1, EdgeHandler edgeHandler)
		{
			int dx=0, dy=0;
			if(p1.x < MinX)
			{
				dx = -Math.Abs(MinX - p1.x)-1;
			}
			if (p1.x >= MaxX)
			{
				dx = Math.Abs(p1.x - MaxX)+1;
			}
			if (p1.y < MinY)
			{
				dy = -Math.Abs(MinY - p1.y)-1;
			}
			if (p1.y >= MaxY)
			{
				dy = Math.Abs(p1.y - MaxY)+1;
			}

			IncreaseGridBy(dx, dy, edgeHandler);
		}

		///<summary>
		/// Increases the grid in the positive direction if parameters are positive and in the negative
		/// direction if parameters are negative. The value accessed at this[0,0] remains the same.
		///</summary>
		public void IncreaseGridBy(int x, int y, EdgeHandler edgeHandler) {
			int nwidth = width + Math.Abs(x);
			int nheight = height + Math.Abs(y);
			int[,] ncells = new int[nwidth,nheight];

			for(int ny = 0; ny < nheight; ny++) {
				for(int nx = 0; nx < nwidth; nx++) {
					int ox = nx;
					int oy = ny;
					if(x < 0) ox += x;
					if(y < 0) oy += y;
					ncells[nx,ny] = ox >= 0 && ox < width && oy >= 0 && oy < height ? cells[ox,oy] : edgeHandler();
				}
			}
			width = nwidth;
			height = nheight;
			cells = ncells;
			if(x < 0) offsetx += x;
			if(y < 0) offsety += y;
		}
		public void IncreaseGridBy(Vector2 amt, EdgeHandler edgeHandler) {
			IncreaseGridBy(amt.x, amt.y, edgeHandler);
		}

		///<summary>
		/// Decreases the grid in the positive direction if parameters are positive and in the negative
		/// direction if parameters are negative. The value accessed at this[0,0] remains the same.
		///</summary>
		public void DecreaseGridBy(int x, int y) {
			int nwidth = width - Math.Abs(x);
			int nheight = height - Math.Abs(y);
			int[,] ncells = new int[nwidth,nheight];

			for(int ny = 0; ny < nheight; ny++) {
				for(int nx = 0; nx < nwidth; nx++) {
					int ox = nx;
					int oy = ny;
					if(x < 0) ox -= x;
					if(y < 0) oy -= y;
					ncells[nx,ny] = ox >= 0 && ox < width && oy >= 0 && oy < height ? cells[ox,oy] : 0;
				}
			}
			width = nwidth;
			height = nheight;
			cells = ncells;
			if(x < 0) offsetx -= x;
			if(y < 0) offsety -= y;
		}

		public void DecreaseGridBy(Vector2 amt) {
			DecreaseGridBy(amt.x, amt.y);
		}

		public void TrimGrid(EdgeHandler edgeHandler) {
			DecreaseGridBy(MinX - GetMinimumX(edgeHandler), MinY - GetMinimumY(edgeHandler));
			DecreaseGridBy(MaxX - GetMaximumX(edgeHandler)-1, MaxY - GetMaximumY(edgeHandler)-1);
		}

		public int GetMinimumX(EdgeHandler edgeHandler) {
			int edgeVal = edgeHandler();
			for(int x=MinX;x<MaxX;x++) {
				for(int y=MinY;y<MaxY;y++) {
					if(this[x,y] == edgeVal) continue;
					return x;
				}
			}
			return MinX;
		}

		public int GetMinimumY(EdgeHandler edgeHandler) {
			int edgeVal = edgeHandler();
			for(int y=MinY;y<MaxY;y++) {
				for(int x=MinX;x<MaxX;x++) {
					if(this[x,y] == edgeVal) continue;
					return y;
				}
			}
			return MinY;
		}

		public int GetMaximumX(EdgeHandler edgeHandler) {
			int edgeVal = edgeHandler();
			for(int x=MaxX-1;x>=MinX;x--) {
				for(int y=MinY;y<MaxY;y++) {
					if(this[x,y] == edgeVal) continue;
					return x;
				}
			}
			return MaxX;
		}

		public int GetMaximumY(EdgeHandler edgeHandler) {
			int edgeVal = edgeHandler();
			for(int y=MaxY-1;y>=MinY;y--) {
				for(int x=MinX;x<MaxX;x++) {
					if(this[x,y] == edgeVal) continue;
					return y;
				}
			}
			return MaxY;
		}

		public IEnumerable<int> GetNeighbors(int x, int y, bool orthoOnly, bool includeSelf) {
			return GetNeighbors(x, y, orthoOnly, includeSelf, returnZero);
		}

		public IEnumerable<int> GetNeighbors(int x, int y, bool orthoOnly, bool includeSelf, EdgeHandler edgeHandler) {
			for(int oy=-1;oy<=1;oy++) {
				for(int ox=-1;ox<=1;ox++) {
					if(orthoOnly && ox != 0 && oy != 0) continue;
					if(!includeSelf && ox == 0 && oy == 0) continue;
					if(x+ox < MinX || x+ox>=MaxX || y+oy < MinY || y+oy>=MaxY) {
						yield return edgeHandler == null ? 0 : edgeHandler();
					}
					else
						yield return this[x+ox, y+oy];
				}
			}
		}

		public IEnumerable<int> GetNeighbors(Vector2 p, bool orthoOnly, bool includeSelf) {
			return GetNeighbors(p.x, p.y, orthoOnly, includeSelf, returnZero);
		}

		public IEnumerable<int> GetNeighbors(Vector2 p, bool orthoOnly, bool includeSelf, EdgeHandler edgeHandler) {
			return GetNeighbors(p.x, p.y, orthoOnly, includeSelf, edgeHandler);
		}

		public void Rotate(Orientation northBecomes) {
			int[,] ncells = null;
			switch(northBecomes) {
				case Orientation.NORTH:
					ncells = new int[width,height];
					for(int ny = 0; ny < height; ny++) {
						for(int nx = 0; nx < width; nx++) {
							ncells[nx,ny] = cells[nx,ny];
						}
					}
					break;
				case Orientation.SOUTH:
					ncells = new int[width,height];
					for(int ny = 0; ny < height; ny++) {
						for(int nx = 0; nx < width; nx++) {
							ncells[nx,ny] = cells[width-nx-1,height-ny-1];
						}
					}
					offsetx = width-offsetx;
					break;
				case Orientation.EAST:
					ncells = new int[height,width];
					for(int ny = 0; ny < width; ny++) {
						for(int nx = 0; nx < height; nx++) {
							ncells[nx,ny] = cells[ny,width-nx-1];
						}
					}
					int t = offsetx;
					offsetx = -offsety;
					offsety = t;
					break;
				case Orientation.WEST:
					ncells = new int[height,width];
					for(int ny = 0; ny < width; ny++) {
						for(int nx = 0; nx < height; nx++) {
							ncells[nx,ny] = cells[width-ny-1,nx];
						}
					}
					int tm = offsetx;
					offsetx = offsety;
					offsety = -tm;
					break;
			}
			cells = ncells;
			width = cells.GetLength(0);
			height = cells.GetLength(1);
		}

		public void Translate(int x, int y) {
			offsetx += x;
			offsety += y;
		}

		public void Translate(Vector2 amt) {
			Translate(amt.x, amt.y);
		}

		public override string ToString() {
			/*StringBuilder sb = new StringBuilder();
			int pad = 0;
			foreach(int y in cells) {
				pad = Math.Max(pad, y);
			}
			pad = (int)Math.Log(pad)+1;
			//(int)Math.Log(cells.Max())+1;
			for(int y = 0; y < height; y++) {
				for(int x = 0; x < width; x++) {
					sb.Append(this[x,y,false].ToString().PadLeft(5,' '));
				}
				sb.Append('\n');
			}
			sb.Remove(sb.Length-1,1);
			return sb.ToString();*/
			return ToString("char+0");
		}
		public string ToString(string format) {
			StringBuilder sb = new StringBuilder();
			int pad = 0;
			int max = int.MinValue;
			foreach(int y in cells) {
				max = Math.Max(max, y);
			}
			pad = (int)Math.Log(max)+1;
			string numFormat = "";
			//(int)Math.Log(cells.Max())+1;
			if(format.Substring(0,4) == "char") {
				if(format == "char") format += "+32";
				int.TryParse(format.Substring(4, format.Length-4), out int v);
				for(int y = 0; y < height; y++) {
					for(int x = 0; x < width; x++)
					{
						if (this[x, y, false] < 0)
							sb.Append("#");
						else if (this[x, y, false] == int.MaxValue)
							sb.Append(" ");
						else
							sb.Append(((char)((this[x, y, false]%(128-v)+v))).ToString());
					}
					sb.Append('\n');
				}
				sb.Remove(sb.Length-1,1);
				return sb.ToString();
			}
			else if(format[0] == 'P' && int.TryParse(format.Substring(1, format.Length-1), out int v)) {
				pad = v;
			}
			else {
				numFormat = format;
				pad = max.ToString(numFormat).Length+1;
			}
			for(int y = 0; y < height; y++) {
				for(int x = 0; x < width; x++) {
					sb.Append(this[x,y,false].ToString(numFormat).PadLeft(pad,' '));
				}
				sb.Append('\n');
			}
			sb.Remove(sb.Length-1,1);
			return sb.ToString();
		}

		public delegate bool ShouldFill(int neighborValue, int thisValueBeforeFill);
		public static ShouldFill equalVal = (n,t) => n==t;

		public long FloodFill(Vector2 pos, int fillValue, ShouldFill shouldFill, bool allowDiagonals=false) {
			return FloodFill(pos.x, pos.y, fillValue, shouldFill, returnNegInf,allowDiagonals);
		}

		public long FloodFill(Vector2 pos, int fillValue, ShouldFill shouldFill, EdgeHandler edgeHandler, bool allowDiagonals=false) {
			return FloodFill(pos.x, pos.y, fillValue, shouldFill, edgeHandler,allowDiagonals);
		}

		public long FloodFill(int _x, int _y, int fillValue, ShouldFill shouldFill, bool allowDiagonals=false) {
			return FloodFill(_x, _y, fillValue, shouldFill, returnNegInf,allowDiagonals);
		}


		public long FloodFill(Vector2 pos, Func<Vector2, Vector2, int> fillValue, Func<Vector2, Vector2, int, int, bool> shouldFill, EdgeHandler edgeHandler, bool allowDiagonals = false)
		{
			long size = 1;

			List<Vector2> open = new List<Vector2>();
			open.Add((pos));
			this[pos] = fillValue(pos, pos);

			while (open.Count > 0)
			{
				Vector2 p = open[0];
				open.RemoveAt(0);

				int L = this[pos];
				int v;

				if (p.x >= this.MaxX || p.y >= this.MaxY || p.x < this.MinX || p.y < this.MinY) continue;

				int N = (p.y == this.MinY) ? edgeHandler() : this[p.x, p.y - 1];
				int W = (p.x == this.MinX) ? edgeHandler() : this[p.x - 1, p.y];

				int S = (p.y == this.MaxY - 1) ? edgeHandler() : this[p.x, p.y + 1];
				int E = (p.x == this.MaxX - 1) ? edgeHandler() : this[p.x + 1, p.y];
				
				if (shouldFill(new Vector2(p.x, p.y-1), new Vector2(p.x, p.y), L, N))
				{
					if (p.y - 1 < this.MinY) continue;
					v = fillValue(new Vector2(p.x, p.y), new Vector2(p.x, p.y - 1));
					if (v != this[p.x, p.y - 1])
					{
						open.Add(new Vector2(p.x, p.y - 1));
						size++;
					}

					this[p.x, p.y - 1] = v;
				}
				if (shouldFill(new Vector2(p.x, p.y + 1), new Vector2(p.x, p.y), L, S))
				{
					if (p.y + 1 >= this.MaxY) continue;
					v = fillValue(new Vector2(p.x, p.y), new Vector2(p.x, p.y + 1));
					if (v != this[p.x, p.y + 1])
					{
						open.Add(new Vector2(p.x, p.y + 1));
						size++;
					}

					this[p.x, p.y + 1] = v;
				}
				if (shouldFill(new Vector2(p.x - 1, p.y), new Vector2(p.x, p.y), L, W))
				{
					if (p.x - 1 < this.MinX) continue;
					v = fillValue(new Vector2(p.x, p.y), new Vector2(p.x - 1, p.y));
					if (v != this[p.x - 1, p.y])
					{
						open.Add(new Vector2(p.x - 1, p.y));
						size++;
					}

					this[p.x - 1, p.y] = v;
				}
				if (shouldFill(new Vector2(p.x + 1, p.y), new Vector2(p.x, p.y), L, E))
				{
					if (p.x + 1 >= this.MaxX) continue;
					v = fillValue(new Vector2(p.x, p.y), new Vector2(p.x + 1, p.y));
					if (v != this[p.x + 1, p.y])
					{
						open.Add(new Vector2(p.x + 1, p.y));
						size++;
					}

					this[p.x + 1, p.y] = v;
				}
				//Console.WriteLine(this.ToString("char+32"));
			}

			return size;
		}

		public long FloodFill(int _x, int _y, int fillValue, ShouldFill shouldFill, EdgeHandler edgeHandler, bool allowDiagonals = false)
		{
			long size = 1;
			;
			int L = this[_x, _y];
			if (L == fillValue) return 0;

			List<(int X, int Y)> open = new List<(int,int)>();
			open.Add((_x,_y));

			while (open.Count > 0)
			{
				(int x, int y) p = open[0];
				open.RemoveAt(0);
				if (p.x >= MaxX || p.y >= MaxY || p.x < MinX || p.y < MinY) continue;
				if(this[p.x, p.y] == fillValue) continue;
				this[p.x, p.y] = fillValue;
				size++;

				int N = (p.y == MinY) ? edgeHandler() : this[p.x, p.y - 1];
				int W = (p.x == MinX) ? edgeHandler() : this[p.x - 1, p.y];

				int S = (p.y == MaxY - 1) ? edgeHandler() : this[p.x, p.y + 1];
				int E = (p.x == MaxX - 1) ? edgeHandler() : this[p.x + 1, p.y];

				if (shouldFill(N, L))
				{
					open.Add((p.x, p.y - 1));
				}
				if (shouldFill(S, L))
				{
					open.Add((p.x, p.y + 1));
				}
				if (shouldFill(W, L))
				{
					open.Add((p.x - 1, p.y));
				}
				if (shouldFill(E, L))
				{
					open.Add((p.x + 1, p.y));
				}
			}

			return size;
			
		}

		public long FloodFillRecursive(int x, int y, int fillValue, ShouldFill shouldFill, EdgeHandler edgeHandler, bool allowDiagonals=false) {
			long size = 1;
			if(x >= MaxX || y >= MaxY || x < MinX || y < MinY) return 0;
			int L = cells[x,y];
			if(L == fillValue) return 0;

			int N = (y == MinY)?edgeHandler():cells[x,y-1];
			int W = (x == MinX)?edgeHandler():cells[x-1,y];

			int S = (y == MaxY-1)?edgeHandler():cells[x,y+1];
			int E = (x == MaxX-1)?edgeHandler():cells[x+1,y];

			cells[x,y] = fillValue;
			if(shouldFill(N,L)) {
				size += FloodFill(x, y-1, fillValue, shouldFill, edgeHandler, allowDiagonals);
			}
			if(shouldFill(S,L)) {
				size += FloodFill(x, y+1, fillValue, shouldFill, edgeHandler, allowDiagonals);
			}
			if(shouldFill(W,L)) {
				size += FloodFill(x-1, y, fillValue, shouldFill, edgeHandler, allowDiagonals);
			}
			if(shouldFill(E,L)) {
				size += FloodFill(x+1, y, fillValue, shouldFill, edgeHandler, allowDiagonals);
			}

			if(allowDiagonals) {
				int NE = (x == MaxX-1 && y == MinY)?edgeHandler():cells[x+1,y-1];
				int NW = (x == MinX && y == MinY)?edgeHandler():cells[x-1,y-1];

				int SW = (x == MinX && y == MaxY-1)?edgeHandler():cells[x-1,y+1];
				int SE = (x == MaxX-1 && y == MaxY-1)?edgeHandler():cells[x+1,y+1];

				if(shouldFill(NE,L)) {
					size += FloodFill(x+1, y-1, fillValue, shouldFill, edgeHandler, allowDiagonals);
				}
				if (shouldFill(NW,L)) {
					size += FloodFill(x-1, y-1, fillValue, shouldFill, edgeHandler, allowDiagonals);
				}
				if(shouldFill(SW,L)) {
					size += FloodFill(x-1, y+1, fillValue, shouldFill, edgeHandler, allowDiagonals);
				}
				if(shouldFill(SE,L)) {
					size += FloodFill(x+1, y+1, fillValue, shouldFill, edgeHandler, allowDiagonals);
				}
			}
			return size;
		}

		public delegate bool FeatureSpec(int[,] values);

		/// <summary>Example usage: Finds the following shape
		/// <code>
		/// M.M
		/// .A.
		/// S.S
		/// 
		/// List&lt;Vector2&gt; result = grid.LocateFeature(v =&gt;
		/// {
		/// 	if (grid[x + 1, y + 1] != 'A') return false;
		///		if (grid[x + 2, y + 2] != 'M' &amp;&amp; grid[x + 2, y + 2] != 'S') return false;
		///		if (grid[x + 2, y + 0] != 'M' &amp;&amp; grid[x + 2, y - 0] != 'S') return false;
		///		if (grid[x + 0, y + 2] != 'M' &amp;&amp; grid[x + 0, y + 2] != 'S') return false;
		///		if (grid[x + 0, y + 0] != 'M' &amp;&amp; grid[x + 0, y + 0] != 'S') return false;
		///		if (grid[x + 0, y + 0] == 'M' &amp;&amp; grid[x + 2, y + 2] == 'M') return false;
		///		if (grid[x + 0, y + 0] == 'S' &amp;&amp; grid[x + 2, y + 2] == 'S') return false;
		///		if (grid[x + 2, y + 0] == 'M' &amp;&amp; grid[x + 0, y + 2] == 'M') return false;
		///		if (grid[x + 2, y + 0] == 'S' &amp;&amp; grid[x + 0, y + 2] == 'S') return false;
		/// }, new List&lt;Vector2&gt;()
		/// {
		/// 	new Vector2( 0,  0),
		/// 	new Vector2(-1, -1),
		/// 	new Vector2(-1,  1),
		/// 	new Vector2( 1, -1),
		/// 	new Vector2( 1,  1),
		/// }, Grid.returnZero);</code>
		/// </summary>
		/// <param name="featureIdentifier"></param>
		/// <param name="offsets"></param>
		/// <param name="edgeHandler"></param>
		/// <returns></returns>

		public List<Vector2> LocateFeature(FeatureSpec featureIdentifier, List<Vector2> offsets, EdgeHandler edgeHandler) {
			List<Vector2> ret = new List<Vector2>();
			int Mx = offsets.Max(p => p.x);
			int mx = offsets.Min(p => p.x);
			int My = offsets.Max(p => p.y);
			int my = offsets.Min(p => p.y);
			int w = Mx - mx + 1; 
			int h = My - my + 1;
			
			for(int y = MinY; y < MaxY; y++) {
				for(int x = MinX; x < MaxX; x++) {
					int[,] vals = new int[w,h];
					foreach(Vector2 off in offsets) {
						if(x + off.x < MinX || x+off.x >= MaxX || y + off.y < MinY || y+off.y >= MaxY)
						{
							int yy = off.y - my;
							int xx = off.x - mx;
							vals[xx, yy] = edgeHandler();
						}
						else
						{
							int yy = off.y - my;
							int xx = off.x - mx;
							vals[xx, yy] = cells[x + off.x, y + off.y];
						}
					}
					if(featureIdentifier(vals)) {
						ret.Add(new Vector2(x,y));
					}
				}
			}
			return ret;
		}

		private void FloodFill(int _x, int _y, EdgeHandler condition, EdgeHandler fillValue)
		{
			if (_x < MinX || _x >= MaxX) return;
			if (_y < MinY || _y >= MaxY) return;
			if (this[_x, _y, true] == condition())
			{
				this[_x, _y, true] = fillValue();
				FloodFill(_x + 1, _y, condition, fillValue);
				FloodFill(_x - 1, _y, condition, fillValue);
				FloodFill(_x, _y + 1, condition, fillValue);
				FloodFill(_x, _y - 1, condition, fillValue);
			}
		}
		public Vector2 FindFirst(char c)
		{
			for (int y = MinY; y < MaxY; y++)
			{
				for (int x = MinX; x < MaxX; x++)
				{
					if (cells[x, y] == c)
						return new Vector2(x, y);
				}
			}
			return new Vector2(int.MinValue, int.MinValue);
		}

		public class PathNode
		{
			public readonly Vector2 pos;
			public readonly Vector2 dir;
			public readonly int cost;
			public readonly PathNode parent;

			public PathNode(Vector2 p, int c, PathNode parent = null)
			{
				pos = p;
				dir = Vector2.ZERO;
				cost = c;
				this.parent = parent;
			}

			public PathNode(Vector2 p, Vector2 d, int c, PathNode parent = null)
			{
				pos = p;
				dir = d;
				cost = c;
				this.parent = parent;
			}

			public override int GetHashCode()
			{
				return pos.GetHashCode();
			}
		}

		/// <summary>
		/// Find a path from S to E
		/// </summary>
		/// <param name="start">Start Pos</param>
		/// <param name="end">End Pos</param>
		/// <param name="isWalkable">(pos) => return if pos is walkable / open space / not a wall</param>
		/// <returns>List of all minimal distance paths</returns>
		public IEnumerable<PathNode> FindShortestPath(Vector2 start, Vector2 end, Func<Vector2, bool> isWalkable)
		{
			return FindShortestPath(start, end, Vector2.ZERO, (_,p2) => isWalkable(p2), (_, _, _, _) => 1);
		}

		/// <summary>
		/// Find a path from S to E
		/// </summary>
		/// <param name="start">Start Pos</param>
		/// <param name="end">End Pos</param>
		/// <param name="canMove">(currentPosition, nextPosition) => return if move is allowed</param>
		/// <param name="getCost">(currentPosition, nextPosition, currentFacing, nextFacing) => return move cost</param>
		/// <returns>List of all minimal distance paths</returns>
		public IEnumerable<PathNode> FindShortestPath(Vector2 start, Vector2 end, Vector2 fa, Func<Vector2, Vector2, bool> canMove, Func<Vector2, Vector2, Vector2, Vector2, int> getCost)
		{
			//always return a non-empty list. a no-path result is simply a node at the start with maximum cost
			List<PathNode> paths = new List<PathNode>()
			{
				new PathNode(
					start,
					fa,
					int.MaxValue
				)
			};

			List<PathNode> open = new List<PathNode>();
			Dictionary<Vector2, PathNode> closed = new Dictionary<Vector2, PathNode>();
			open.Add(new PathNode(
				start,
				0
			));
			while (open.Count > 0)
			{
				PathNode p = open[^1];
				open.Remove(p);
				if (!closed.ContainsKey(p.pos) || closed[p.pos].cost > p.cost)
				{
					closed[p.pos] = p;
				}
				if(p.pos == end)
					paths.Add(p);
				foreach (Vector2 d in FACING)
				{
					if (!IsInside(p.pos + d)) continue;
					if(!canMove(p.pos, p.pos + d)) continue;

					int q = closed.ContainsKey(p.pos + d) ? closed[p.pos + d].cost : int.MaxValue;
					int v = open.Where(o => o.pos == p.pos + d).DefaultIfEmpty(new PathNode(
						Vector2.ZERO, 
						int.MaxValue
					)).Min(o => o.cost);
					q = Math.Min(q, v);
					int cost = getCost(p.pos, p.pos + d, p.dir, d);
					if (q > p.cost + cost)
					{
						open.Add(new PathNode(
							p.pos + d,
							d,
							p.cost + cost,
							p
						));
					}
				}
				open.Sort((b,a) => a.cost.CompareTo(b.cost));
			}

			IOrderedEnumerable<PathNode> or = paths.OrderBy(p => p.cost);
			int m = or.First().cost;
			return or.TakeWhile(p => p.cost == m);
			//return paths.GroupBy(o => o.cost).OrderBy(g => g.Key).Select(g => g.First());
		}
	}
}