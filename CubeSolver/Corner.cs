
namespace CubeSolver {

	public class Corner {

		// Sides should be listed in clockwise order going aound the corner
		public Side Side0 { get; private set; }
		public Side Side1 { get; private set; }
		public Side Side2 { get; private set; }

		public Corner(Side side0, Side side1, Side side2) {
			Side0 = side0;
			Side1 = side1;
			Side2 = side2;
		}

		public MovablePosition Pos0 => MovablePosition.Get( Side0, Side1 | Side2 );
		public MovablePosition Pos1 => MovablePosition.Get( Side1, Side2 | Side0 );
		public MovablePosition Pos2 => MovablePosition.Get( Side2, Side0 | Side1 );

		/// <summary>
		/// In same location but may have different orientation
		/// </summary>
		/// <remarks>for detecting Constraint collisions</remarks>
		public bool InSameSpace( Corner other ) {
			return (Side0 == other.Side0 && Side1 == other.Side1)
				|| (Side1 == other.Side0 && Side2 == other.Side1)
				|| (Side2 == other.Side0 && Side0 == other.Side1);
		}

	}

}
