using System;

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

		public SquarePos Pos0 => SquarePos.Get( Side0, Side1 | Side2 );
		public SquarePos Pos1 => SquarePos.Get( Side1, Side2 | Side0 );
		public SquarePos Pos2 => SquarePos.Get( Side2, Side0 | Side1 );

	}

	public class CornerMatch : CubeMatch {

		static public CornerMatch Stationary( Corner corner ) => new CornerMatch( corner, corner );

		Corner _requiredColor;
		Corner _targetLocation;

		public CornerMatch( Corner color, Corner target ) {
			_requiredColor = color;
			_targetLocation = target;
		}

		public bool IsMatch( Cube cube ) =>
			   cube[ _targetLocation.Pos0 ] == _requiredColor.Side0
			&& cube[ _targetLocation.Pos1 ] == _requiredColor.Side1
			&& cube[ _targetLocation.Pos2 ] == _requiredColor.Side2;

	}

}
