using System;
using System.Linq;
using AiSearch.OneSide;

namespace CubeSolver {

	public class Solver {

		static TurnMoveGenerator moveGenerator = new TurnMoveGenerator();

		/// <remarks>
		/// Starts with a solved cube and moves the desired cubies into the constraint. (essentially unsolving it but finding the correct moves)
		/// Could alternatively move the cubes backwards from the solved state into their desired starting position and then reverse the moves.
		/// </remarks>
		static public Turn[] GetStepsToAcheiveMatch( int maxTurnCount, CubeConstraint constraint ) {

			// Tried BreadthFirst but it runs out of memory around depth=6.
			var moveIterator = new IterativeDeepeningIterator<Cube>( moveGenerator, maxTurnCount ) { DontRepeat = true };

			Node<Cube> winner = moveIterator.Iterate( new Cube() )
				.FirstOrDefault( node => constraint.IsMatch(node.State ) );

			if( winner == null ) throw new MoveNotFoundExcpetion();

			var path = winner.GetNodePath();
			return path.Skip( 1 ).Select( x => ((TurnMove)x.Move)._turn ).ToArray();
		}

	}

}
