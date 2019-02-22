using System.Collections.Generic;
using AiSearch.OneSide;

namespace CubeSolver {

	class CubeMoveGenerator : MoveGenerator<Cube> {

		static readonly CubeMove[] AllPossibleMoves;

		static CubeMoveGenerator() {
			// Build all possible single turns
			var allPossibleTurns = new List<CubeMove>();
			foreach(var side in Cube.AllSides)
				foreach(var dir in new Direction[] { Direction.Clockwise, Direction.CounterClockwise })
					allPossibleTurns.Add( new CubeMove( new Turn(side,dir) ) );
			AllPossibleMoves = allPossibleTurns.ToArray();
		}

		public IEnumerable<Move<Cube>> GetMoves( Cube s ) => AllPossibleMoves;

	}

	class CubeMove : Move<Cube> {
		public Turn _turn;
		public CubeMove( Turn turn ) { _turn = turn; }
		public string HumanReadable => _turn.ToString();
		public Cube GenerateChild( Cube state ) => state.ApplyTurn( _turn );

	}

}
