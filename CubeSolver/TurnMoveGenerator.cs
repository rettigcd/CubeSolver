using System.Collections.Generic;
using AiSearch.OneSide;

namespace CubeSolver {

	/// <summary>
	/// Generates TurnMoves for use in AI Tree Search
	/// </summary>
	class TurnMoveGenerator : MoveGenerator<Cube> {

		static readonly TurnMove[] AllPossibleMoves;

		static TurnMoveGenerator() {

			// Build all possible single turns
			var allPossibleTurns = new List<TurnMove>();
			foreach(var side in CubeGeometry.AllSides) {
				allPossibleTurns.Add( new TurnMove( new Turn(side,Direction.Clockwise) ) );
				allPossibleTurns.Add( new TurnMove( new Turn(side,Direction.CounterClockwise) ) );
			}
			AllPossibleMoves = allPossibleTurns.ToArray();
		}

		public IEnumerable<Move<Cube>> GetMoves( Cube s ) => AllPossibleMoves;

	}

}
