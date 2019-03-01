// create 2-turn, dis allow duplicate turning
// turn sequence
// remove repeat

namespace CubeSolver {
	public class FtlPair {

		public FtlPair(Side leftishSide, Side rightishSide ) {
			Leftish = leftishSide;
			Rightish = rightishSide;
		}
		public Side Leftish{ get; set; }
		public Side Rightish{ get; set; }
		public Edge Edge => new Edge(Leftish,Rightish);
		public Corner Corner => new Corner(Side.Down,Leftish,Rightish);

		public CubeConstraint Stationary => new CompoundConstraint( 
			EdgeConstraint.Stationary(Edge),
			CornerConstraint.Stationary(Corner)
		);

	}


}
