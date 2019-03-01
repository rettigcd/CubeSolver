using System.Collections;
using System.Linq;
using AiSearch.OneSide;

namespace CubeSolver {

	public class Solver {

		#region First 2 Layers (F2L,FTL)

		static public TurnSequence PlaceFtlPairs( Cube cube ) {
			Constraints.VerifyConstraint( cube, Constraints.CrossConstraint, "Cross not solved" );

			var cur = cube;

			// if parts of cube are in different slots
				// there are 2 options as to which cube you pop-up first
			
			// to move 1 slot up where there are no pairs in upper
				// there are only 6 option (3 leftish + 3 rightish)

			// to move 1 slot up where one of the pairs is already in upper
				// there are 4 Upper turns * (3 leftish + 3 rightish) = 24 options

			// Also 24 happens to be the Least Common Multiple of 8 edge positions & 12 corner positions

			// If we evaluate all of them, 2*6*24 ==> 288 positions we have to solve
			// but there are essenially only 12 possibilities (white-up:4 + white-to-theside:8)
			// knowledge:
				// white-on-the-side is better than white-up -> reduces possibility from 12 to 8
				// case 1 or 2 (joined or side-op) are better than case 3 or 4
				// joined incorrectly is the worse.
			

			//foreach(var slot in Constraints.AllFtlSlots) {
			//	Corner corner = Find(cube,slot.Corner);
			//	Edge edge = Find(cube,slot.Edge);
			//}

			// while (unsolved) 
				// while(at least 1 pair in top), push into slot
				// if(unsolved) push pair into top

			throw new System.NotImplementedException();
		}

		static public TurnSequence PlaceSingleFtlPairFromTop( FtlSlot pair, Cube cube ) {
			Constraints.VerifyConstraint( cube, Constraints.CrossConstraint, "Cross not solved" );

			// these are constraints to apply move to a solved cube, so constraints don't work on messed up cube
			return Solver.GetStepsToAcheiveMatch(6, new CompoundConstraint(
				FindFtlPairAndSolveIt( pair, cube ),
				Constraints.CrossConstraint,
				Constraints.OtherSlotsConstraint( pair )
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

		static public CompoundConstraint FindFtlPairAndSolveIt(FtlSlot slot, Cube cube) {
			CornerEdgePair src = FindFtlPair( slot, cube );
			return new CompoundConstraint(
				new CornerConstraint( src.Corner, slot.Home.Corner ),
				new EdgeConstraint( src.Edge, slot.Home.Edge )
			);
		}

		static CornerEdgePair FindFtlPair(FtlSlot destinationSlot, Cube cube) {
			return new CornerEdgePair(
				Find( cube, destinationSlot.Home.Corner ),
				Find( cube, destinationSlot.Home.Edge )
			);
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

		#endregion

	}

}
