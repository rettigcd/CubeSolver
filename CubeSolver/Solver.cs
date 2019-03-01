using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;

namespace CubeSolver {

	public class Solver {

		#region First 2 Layers (F2L,FTL)

		static public TurnSequence PlaceFtlPairs( Cube cube ) {
			Constraints.VerifyConstraint( cube, Constraints.CrossConstraint, "Cross not solved" );

			var cur = cube;

			var turns = new List<Turn>();

			FtlSlot easiestSlotToSolve = FindEasiestSlotToSolve( cur );
			int num = HoldingSlotCount(easiestSlotToSolve,cur);
			while(easiestSlotToSolve != null) {
				var move = PlaceSingleFtlPairFromTop( easiestSlotToSolve, cube );
				turns.AddRange( move._turns );
				cur = cur.Apply( move );
				easiestSlotToSolve = FindEasiestSlotToSolve( cur );
				num = HoldingSlotCount(easiestSlotToSolve,cur);
			}

			return new TurnSequence(turns.ToArray());

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

		}

		static FtlSlot FindEasiestSlotToSolve( Cube cube ) {
			return Constraints.AllFtlSlots
				.Where( slot => !slot.Home.Stationary.IsMatch( cube ) ) // not solved
				.OrderBy( slot => HoldingSlotCount( slot, cube ) )
				.FirstOrDefault();
		}

		static int HoldingSlotCount(FtlSlot slot,Cube cube) => HoldingSlots( Find( cube, slot ) ).Length;

		static FtlSlot[] HoldingSlots(CornerEdgePair src) {
			var holdingSlots = new List<FtlSlot>();
			foreach(var slot in Constraints.AllFtlSlots) {
				if(slot.Home.Edge.InSameSpace(src.Edge)
					|| slot.Home.Corner.InSameSpace(src.Corner)
				) holdingSlots.Add(slot);
			}
			return holdingSlots.ToArray();
		}

		static bool IsInTop(Edge e) => e.Side0 == Side.Up || e.Side1 == Side.Up;
		static bool IsInTop(Corner c) => c.Side0 == Side.Up || c.Side1 == Side.Up || c.Side2 == Side.Up;

		static public TurnSequence PlaceSingleFtlPairFromTop( FtlSlot slot, Cube cube ) {
			Constraints.VerifyConstraint( cube, Constraints.CrossConstraint, "Cross not solved" );

			var pair = Find( cube, slot );
			var holdingSlots = HoldingSlots( pair );
			var moveGenerator = new SlotTurnGenerator(slot);
			if( holdingSlots.Length == 1 )
				moveGenerator.SetFirstSlot(holdingSlots[0]);

			// these are constraints to apply move to a solved cube, so constraints don't work on messed up cube
			return Solver.GetStepsToAcheiveMatch(6, new CompoundConstraint(
				FindFtlPairAndSolveIt( slot, cube ),
				Constraints.CrossConstraint,
				Constraints.OtherSlotsConstraint( slot )
			), moveGenerator
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
			CornerEdgePair src = Find( cube, slot );
			return new CompoundConstraint(
				new CornerConstraint( src.Corner, slot.Home.Corner ),
				new EdgeConstraint( src.Edge, slot.Home.Edge )
			);
		}

		static CornerEdgePair Find(Cube cube, FtlSlot slot) {
			return new CornerEdgePair(
				Find( cube, slot.Home.Corner ),
				Find( cube, slot.Home.Edge )
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
