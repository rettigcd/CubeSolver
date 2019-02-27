﻿using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;

namespace CubeSolver {

	/// <summary>
	/// Generates TurnMoves for use in AI Tree Search
	/// </summary>
	class TurnMoveGenerator : NodeMoveGenerator<Cube> {

		static public readonly TurnMove[] AllPossibleTurnMoves;

		static TurnMoveGenerator() {
			AllPossibleTurnMoves = Turn.BuildAllTurns()
				.Select( x => new TurnMove( x ) )
				.ToArray();
		}

		public IEnumerable<Move<Cube>> GetMoves( Node<Cube> s ) {
			if( s.Move == null )
				return AllPossibleTurnMoves;

			Side previousTurnSide = ((TurnMove)s.Move)._turn.Side;
			return AllPossibleTurnMoves
				.Where(turn => turn._turn.Side != previousTurnSide );
		}

	}

}
