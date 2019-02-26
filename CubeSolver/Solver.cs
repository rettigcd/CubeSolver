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
				.FirstOrDefault( node => constraint.IsMatch(node.State) );

			if( winner == null ) throw new MoveNotFoundExcpetion();

			var path = winner.GetNodePath();
			return path.Skip( 1 ).Select( x => ((TurnMove)x.Move)._turn ).ToArray();
		}

		static internal Turn[] GetCrossSolution( Cube cube ) {

			// find 1st 2 cubes
			var frontDown = CubeGeometry.AllEdgePositions.First(edge => cube[ edge.Pos0 ] == Side.Front && cube[ edge.Pos1 ] == Side.Down );
			var rightDown = CubeGeometry.AllEdgePositions.First(edge => cube[ edge.Pos0 ] == Side.Right && cube[ edge.Pos1 ] == Side.Down );

			var move1Constraints = new CompoundConstraint();
			move1Constraints.Add( new EdgeConstraint(frontDown,new Edge(Side.Front,Side.Down)) );
			move1Constraints.Add( new EdgeConstraint(rightDown,new Edge(Side.Right,Side.Down)) );

			Turn[] move1Turns = GetStepsToAcheiveMatch(6, move1Constraints);

			Cube temp = cube;
			foreach(var turn in move1Turns)
				temp = temp.ApplyTurn(turn);

			// Find 2nd 2 cubes
			var backDown = CubeGeometry.AllEdgePositions.First(edge => temp[ edge.Pos0 ] == Side.Back && temp[ edge.Pos1 ] == Side.Down );
			var leftDown = CubeGeometry.AllEdgePositions.First(edge => temp[ edge.Pos0 ] == Side.Left && temp[ edge.Pos1 ] == Side.Down );

			var move2Constraints = new CompoundConstraint();
			move2Constraints.Add( new EdgeConstraint(backDown,new Edge(Side.Back,Side.Down)) );
			move2Constraints.Add( new EdgeConstraint(leftDown,new Edge(Side.Left,Side.Down)) );

			Turn[] move2Turns = GetStepsToAcheiveMatch(6, move2Constraints);

			// combine
			return move1Turns.Concat(move2Turns).ToArray();
		}

	}

}
