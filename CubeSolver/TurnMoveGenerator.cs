using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;

namespace CubeSolver {

	/// <summary>
	/// Generates TurnMoves for use in AI Tree Search
	/// </summary>
	class TurnMoveGenerator : NodeMoveGenerator<Cube> {

		static public readonly TurnSequenceMove[] AllPossibleTurnMoves;

		static TurnMoveGenerator() {
			AllPossibleTurnMoves = Turn.BuildAllTurns()
				.Select( x => new TurnSequenceMove( new TurnSequence( x ) ) )
				.ToArray();
		}

		public IEnumerable<Move<Cube>> GetMoves( Node<Cube> s ) {
			if( s.Move == null )
				return AllPossibleTurnMoves;

			Side previousTurnSide = ((TurnSequenceMove)s.Move)._sequence._turns[0].Side;
			return AllPossibleTurnMoves
				.Where(turn => turn._sequence._turns[0].Side != previousTurnSide );
		}

	}

}
