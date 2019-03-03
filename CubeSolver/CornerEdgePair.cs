// create 2-turn, dis allow duplicate turning
// turn sequence
// remove repeat

namespace CubeSolver {

	/// <summary>
	/// For solving 1 edge and 1 corner at the same time.
	/// </summary>
	public class CornerEdgePair {

		public Corner Corner { get; private set; }
		public Edge Edge { get; private set; }

		public CornerEdgePair(Corner corner,Edge edge) {
			Corner = corner;
			Edge = edge;
		}

		public CubeConstraint Stationary => new CompoundConstraint( 
			EdgeConstraint.Stationary(Edge),
			CornerConstraint.Stationary(Corner)
		);

		public CornerEdgePair Examine( Cube cube ) => new CornerEdgePair( Corner.Examine(cube), Edge.Examine(cube ) );

		public CornerEdgePair Locate(Cube cube) => new CornerEdgePair( Corner.Locate( cube ), Edge.Locate( cube ) );

		public override string ToString() => $"{Corner} {Edge}";

	}


}
