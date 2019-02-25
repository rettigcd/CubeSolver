using System;

namespace CubeSolver {

	public class Turn {

		public Side Side { get; private set; }
		public Direction Direction { get; private set; }

		public Turn(Side side, Direction direction) {
			this.Side = side;
			this.Direction = direction;
		}

		public override string ToString() => Direction == Direction.Clockwise ? SideSymbol : SideSymbol + "'";

		string SideSymbol => GetSideSymbol( Side );

		static string GetSideSymbol(Side side) {
			switch(side) {
				case Side.Up:    return "U";
				case Side.Down:  return "D";
				case Side.Left:  return "L";
				case Side.Right: return "R";
				case Side.Front: return "F";
				case Side.Back:  return "B";
				default: throw new ArgumentException($"Invalid Side {side}.");
			}
		}

	}

}
