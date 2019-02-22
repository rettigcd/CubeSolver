using System;
using System.Linq;
using AiSearch.OneSide;
using Xunit;

namespace CubeSolver {

	public class Edge {
		public Edge(Side side0, Side side1) { Side0=side0; Side1=side1; }
		public Side Side0;
		public Side Side1;

		public SquarePos Pos0 => SquarePos.Get(Side0,Side1);
		public SquarePos Pos1 => SquarePos.Get(Side1,Side0);

	}

}
