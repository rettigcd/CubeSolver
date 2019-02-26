
namespace CubeSolver {

	public class Edge {

		public Edge(Side side0, Side side1) { Side0=side0; Side1=side1; }

		public Side Side0;
		public Side Side1;

		public MovablePosition Pos0 => MovablePosition.Get(Side0,Side1);
		public MovablePosition Pos1 => MovablePosition.Get(Side1,Side0);

		/// <summary>
		/// In same location mut may have different orientation
		/// </summary>
		public bool InSameSpace( Edge other ) {
			return (Side0 == other.Side0 && Side1 == other.Side1)
				|| (Side1 == other.Side0 && Side0 == other.Side1);
		}

	}

}
