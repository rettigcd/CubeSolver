
namespace CubeSolver {

	public class CornerConstraint : CubeConstraint {

		static public CornerConstraint Stationary( Corner corner ) => new CornerConstraint( corner, corner );

		public CornerConstraint( Corner color, Corner target ) {
			_requiredColor = color;
			_targetLocation = target;
		}

		public bool IsMatch( Cube cube ) =>
			   cube[ _targetLocation.Pos0 ] == _requiredColor.Side0
			&& cube[ _targetLocation.Pos1 ] == _requiredColor.Side1
			&& cube[ _targetLocation.Pos2 ] == _requiredColor.Side2;

		Corner _requiredColor;
		Corner _targetLocation;

	}

}
