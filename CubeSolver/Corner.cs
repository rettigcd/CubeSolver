
using System.Linq;

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

		public Corner Examine( Cube cube ) {
			return new Corner( cube[Pos0], cube[Pos1], cube[Pos2]);
		}

		public Corner Locate( Cube cube ) {
			return CubeGeometry.AllCornerPositions
				.First( corner => cube[corner.Pos0] == Side0
							   && cube[corner.Pos1] == Side1
							   && cube[corner.Pos2] == Side2
				);
		}

		public override string ToString() {
			return $"{Side0}:{Side1}:{Side2}";
		}

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
