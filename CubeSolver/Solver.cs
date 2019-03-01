using System.Collections;
using System.Linq;
using AiSearch.OneSide;

namespace CubeSolver {

	public class Solver {

		#region First 2 Layers (F2L,FTL)

		static public TurnSequence PlaceFtlPairs( Cube cube ) {
			Constraints.VerifyConstraint( cube, Constraints.CrossConstraint, "Cross not solved" );

			// while (unsolved) 
				// while(at least 1 pair in top), push into slot
				// if(unsolved) push pair into top

			throw new System.NotImplementedException();
		}

		static public TurnSequence SingleFtlPair( FtlPair pair, Cube cube ) {
			Constraints.VerifyConstraint( cube, Constraints.CrossConstraint, "Cross not solved" );

			// these are constraints to apply move to a solved cube, so constraints don't work on messed up cube
			return Solver.GetStepsToAcheiveMatch(6, new CompoundConstraint(
				Constraints.MovePairDirectlyToSlot( pair, cube ),
				Constraints.CrossConstraint,
				Constraints.DontMoveOtherSlotConstraint( pair )
			), new SlotTurnGenerator(pair)
			);

		}

		#endregion

		#region cross

		static public TurnSequence GetCrossSolution( Cube cube ) {

			// Try all 4 edge options to find which is shortest
			return Enumerable.Range(0,4)
				.Select( i=>GetCrossSolution_Inner( cube, i ) )
				.OrderBy(x=>x._turns.Length)
				.First();
		}

		static TurnSequence GetCrossSolution_Inner( Cube cube, int i ) {
			Edge[] e = Enumerable.Range(0,4)
				.Select(x=>Constraints.CrossEdges[(i+x)%4])
				.ToArray();

			TurnSequence move1Turns = Solve_First2CrossEdges( cube, e[0], e[1] );
			TurnSequence move2Turns = Solve_Second2CrossEdges( cube.Apply( move1Turns ), e[0], e[1], e[2], e[3] );

			return new TurnSequence( move1Turns._turns.Concat( move2Turns._turns ).ToArray() );
		}

		static TurnSequence Solve_First2CrossEdges( Cube cube, Edge bottomEdge0, Edge bottomEdge1 ) {
			return GetStepsToAcheiveMatch( 6, new CompoundConstraint(
				FindEdgeAndSolveIt( cube, bottomEdge0 ),
				FindEdgeAndSolveIt( cube, bottomEdge1 )
			), new TurnMoveGenerator() );
		}

		static TurnSequence Solve_Second2CrossEdges( Cube cube, Edge solvedEdge0, Edge solvedEdge1, Edge remainingEdge2, Edge remainingEdge3 ) {
			return GetStepsToAcheiveMatch( 6, new CompoundConstraint(
				EdgeConstraint.Stationary( solvedEdge0 ),
				EdgeConstraint.Stationary( solvedEdge1 ),
				FindEdgeAndSolveIt( cube, remainingEdge2 ),
				FindEdgeAndSolveIt( cube, remainingEdge3 )
			), new TurnMoveGenerator() );
		}

		#endregion

		/// <remarks>
		/// Starts with a solved cube and moves the desired cubies into the constraint. (essentially unsolving it but finding the correct moves)
		/// Could alternatively move the cubes backwards from the solved state into their desired starting position and then reverse the moves.
		/// </remarks>
		static internal TurnSequence GetStepsToAcheiveMatch( 
			int maxTurnCount, 
			CubeConstraint constraint, 
			NodeMoveGenerator<Cube> moveGenerator
		) {

			var moveIterator = new IterativeDeepeningIterator<Cube>( moveGenerator, maxTurnCount ) { DontRepeat = true };

			Node<Cube> winner = moveIterator.Iterate( new Cube() )
				.FirstOrDefault( node => constraint.IsMatch(node.State) );

			if( winner == null ) throw new MoveNotFoundExcpetion();

			var path = winner.GetNodePath();
			
			Turn[] ddd = path
				.Skip( 1 )
				.SelectMany( x => ((TurnSequenceMove)x.Move)._sequence._turns )
				.ToArray();

			return new TurnSequence( ddd );
		}

		#region Cube Finders

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

		#endregion

	}

}
