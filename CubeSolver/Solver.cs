using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;

namespace CubeSolver {

	public class Solver {

		static public TurnSequence GetCrossSolution( Cube cube ) {

			// Try all 4 edge options to find which is shortest
			return Enumerable.Range(0,4)
				.Select( i=>GetCrossSolution_Inner( cube, i ) )
				.OrderBy(x=>x._turns.Length)
				.First();
		}

		// FYI - FTL => F2L => First 2 Layers

		static public TurnSequence PlaceFtlPair( CubeConstraint ftlConstraint ) {
			return Solver.GetStepsToAcheiveMatch(6, new CompoundConstraint(
				ftlConstraint,
				CrossConstraint
			));
		}

		static public TurnSequence PlaceFtlPair( Cube cube ) {
			VerifyConstraint( cube, CrossConstraint, "Cross not solved" );

			// while (unsolved) 
				// while(at least 1 pair in top), push into slot
				// if(unsolved) push pair into top

			throw new System.NotImplementedException();
		}

		static void VerifyConstraint( Cube cube, CubeConstraint constraint, string msg ) {
			if( !constraint.IsMatch( cube ) ) throw new System.InvalidOperationException( msg );
		}

		static public CompoundConstraint CrossConstraint =>
			new CompoundConstraint(
				EdgeConstraint.Stationary( CrossEdges[0] ),
				EdgeConstraint.Stationary( CrossEdges[1] ),
				EdgeConstraint.Stationary( CrossEdges[2] ),
				EdgeConstraint.Stationary( CrossEdges[3] )
			);

		static Edge[] CrossEdges = new[] {
			new Edge( Side.Front, Side.Down ),
			new Edge( Side.Right, Side.Down ),
			new Edge( Side.Back, Side.Down ),
			new Edge( Side.Left, Side.Down ),
		};

		static TurnSequence GetCrossSolution_Inner( Cube cube, int i ) {
			var e0 = CrossEdges[i];
			var e1 = CrossEdges[(i+1)%4];
			var e2 = CrossEdges[(i+2)%4];
			var e3 = CrossEdges[(i+3)%4];
			TurnSequence move1Turns = Solve_First2CrossEdges( cube, e0, e1 );
			TurnSequence move2Turns = Solve_Second2CrossEdges( cube.Apply( move1Turns ), e0, e1, e2, e3 );

			return new TurnSequence( move1Turns._turns.Concat( move2Turns._turns ).ToArray() );
		}

		static TurnSequence Solve_First2CrossEdges( Cube cube, Edge bottomEdge0, Edge bottomEdge1 ) {
			return GetStepsToAcheiveMatch( 6, new CompoundConstraint(
				FindEdgeAndSolveIt( cube, bottomEdge0 ),
				FindEdgeAndSolveIt( cube, bottomEdge1 )
			) );
		}

		static TurnSequence Solve_Second2CrossEdges( Cube cube, Edge solvedEdge0, Edge solvedEdge1, Edge remainingEdge2, Edge remainingEdge3 ) {
			return GetStepsToAcheiveMatch( 6, new CompoundConstraint(
				EdgeConstraint.Stationary( solvedEdge0 ),
				EdgeConstraint.Stationary( solvedEdge1 ),
				FindEdgeAndSolveIt( cube, remainingEdge2 ),
				FindEdgeAndSolveIt( cube, remainingEdge3 )
			) );
		}

		static readonly TurnMoveGenerator _moveGenerator = new TurnMoveGenerator();

		/// <remarks>
		/// Starts with a solved cube and moves the desired cubies into the constraint. (essentially unsolving it but finding the correct moves)
		/// Could alternatively move the cubes backwards from the solved state into their desired starting position and then reverse the moves.
		/// </remarks>
		static internal TurnSequence GetStepsToAcheiveMatch( int maxTurnCount, CubeConstraint constraint ) {

			// Tried BreadthFirst but it runs out of memory around depth=6.
			var moveIterator = new IterativeDeepeningIterator<Cube>( _moveGenerator, maxTurnCount ) { DontRepeat = true };

			Node<Cube> winner = moveIterator.Iterate( new Cube() )
				.FirstOrDefault( node => constraint.IsMatch(node.State) );

			if( winner == null ) throw new MoveNotFoundExcpetion();

			var path = winner.GetNodePath();
			return new TurnSequence( path.Skip( 1 ).Select( x => ((TurnMove)x.Move)._turn ).ToArray() );
		}

		static public EdgeConstraint FindEdgeAndSolveIt( Cube cube, Edge edge ) {
			return new EdgeConstraint( Find( cube, edge ), edge );
		}

		static public CornerConstraint FindCornerAndSolveIt( Cube cube, Corner corner ) {
			return new CornerConstraint( Find( cube, corner ), corner );
		}

		static Edge Find( Cube cube, Edge needle ) {
			return CubeGeometry.AllEdgePositions
				.First( edge => cube[edge.Pos0] == needle.Side0
							 && cube[edge.Pos1] == needle.Side1
				);
		}

		static Corner Find( Cube cube, Corner needle ) {
			return CubeGeometry.AllCornerPositions
				.First( corner => cube[corner.Pos0] == needle.Side0
							   && cube[corner.Pos1] == needle.Side1
							   && cube[corner.Pos2] == needle.Side2
				);
		}

	}

}
