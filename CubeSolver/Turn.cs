using System;

namespace CubeSolver {

	public class Turn {

		public Side Side { get; private set; }
		public Direction Direction { get; private set; }

		public Turn(Side side, Direction direction) {
			this.Side = side;
			this.Direction = direction;
		}

		public override string ToString() {
			return this.Side+":"+this.Direction;
		}

	}

}
