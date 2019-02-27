using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;

namespace CubeSolver {

	/// <summary>
	/// Generates TurnMoves for use in AI Tree Search
	/// </summary>
	class TurnMoveGenerator : MoveGenerator<Cube> {

		static public readonly TurnMove[] AllPossibleMoves;

		static TurnMoveGenerator() {
			AllPossibleMoves = Turn.AllPossibleMoves
				.Select( x => new TurnMove( x ) )
				.ToArray();
		}

		public IEnumerable<Move<Cube>> GetMoves( Cube s ) => AllPossibleMoves;

	}

}
