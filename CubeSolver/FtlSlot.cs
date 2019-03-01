// create 2-turn, dis allow duplicate turning
// turn sequence
// remove repeat

namespace CubeSolver {

	public class FtlSlot {

		/// <summary>
		/// Constructs an F2L pait in their 'Home' slot.
		/// </summary>
		/// <param name="leftOf">the side to the left of the slot</param>
		/// <param name="rightOf">the side to the right of the slot</param>
		public FtlSlot(Side leftOf, Side rightOf ) {
			LeftOf = leftOf;
			RightOf = rightOf;

			Home = new CornerEdgePair( 
				new Corner(Side.Down,LeftOf,RightOf), 
				new Edge(LeftOf,RightOf)
			);
		}

		/// <summary> The side to the left of slot. </summary>
		public Side LeftOf{ get; private set; }
		/// <summary> The side to the right of slot. </summary>
		public Side RightOf{ get; private set; }
		/// <summary> Cubes in the slot. </summary>
		public CornerEdgePair Home{ get; private set; }

	}


}
