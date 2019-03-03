
using System.Linq;

namespace CubeSolver {

	public class Edge {

		public Edge(Side side0, Side side1) { Side0=side0; Side1=side1; }

		public Side Side0;
		public Side Side1;

		public MovablePosition Pos0 => MovablePosition.Get(Side0,Side1);
		public MovablePosition Pos1 => MovablePosition.Get(Side1,Side0);

		/// <summary>
		/// In same location but may have different orientation
		/// </summary>
		public bool InSameSpace( Edge other ) {
			return (Side0 == other.Side0 && Side1 == other.Side1)
				|| (Side1 == other.Side0 && Side0 == other.Side1);
		}

		public Edge Examine( Cube cube ) {
			return new Edge( cube[Pos0], cube[Pos1] );
		}

		public Edge Locate( Cube cube ) {
			return CubeGeometry.AllEdgePositions
				.First( edge => cube[edge.Pos0] == Side0
							 && cube[edge.Pos1] == Side1
				);
		}


		public override string ToString() => Side0+":"+Side1;

	}

}
