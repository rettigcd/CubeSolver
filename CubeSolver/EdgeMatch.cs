
namespace CubeSolver {

	public class EdgeMatch : CubeMatch {

		static public EdgeMatch Stationary( Edge edge ) => new EdgeMatch( edge, edge );

		public EdgeMatch(Edge fromColor, Edge toLocation) {
			this.FromColor = fromColor;
			this.ToLocation = toLocation;
		}

		/// <summary> 
		/// Since initially, all pieces are in their starting position,
		/// this is both the initial location and the 'color' of the piece we are going to move. 
		/// Note: .
		/// </summary>
		/// <remarks>
		/// We are using Side to designate color. 
		/// Instead of the top being 'White' color, the top is 'Top' color. </remarks>
		public Edge FromColor { get; private set; }

		/// <summary>
		/// Where we want the piece to be;
		/// </summary>
		public Edge ToLocation { get; private set; }

		public bool IsMatch( Cube cube ) {
			return cube[ ToLocation.Pos0 ] == FromColor.Side0
				&& cube[ ToLocation.Pos1 ] == FromColor.Side1;
		}
	}

}
