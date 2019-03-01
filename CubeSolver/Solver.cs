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

		static public TurnSequence FtlSolution( FtlPair pair, Cube cube ) {
			var part1 = PrepareToPlaceFtlPairDirectly( pair, cube );
			var temp = cube.Apply(part1);
			var part2 = PlaceFtlPairDirectly( pair, temp );
			return new TurnSequence(part1._turns.Concat(part2._turns).ToArray());
		}

		static public TurnSequence PrepareToPlaceFtlPairDirectly( FtlPair pair, Cube cube) {

			var srcEdge = Solver.Find(cube,pair.Edge);
			var srcCorner = Solver.Find(cube,pair.Corner);

			// corner: Right,Front,Up  edge: front,up
			Corner targetCornerWhiteRight = new Corner( pair.Rightish, pair.Leftish, Side.Up ); 
			Edge joinedEdgeOnLeft = new Edge( pair.Leftish, Side.Up );
			CompoundConstraint prepSolve0 = new CompoundConstraint(
				new EdgeConstraint( srcEdge, joinedEdgeOnLeft ),
				new CornerConstraint( srcCorner, targetCornerWhiteRight)
			);

			// corner: front,up,right  edge: up,right
			Corner targetCornerWhiteLeft = new Corner( pair.Leftish, Side.Up, pair.Rightish ); 
			Edge joinedEdgeOnRight = new Edge( Side.Up, pair.Rightish );
			CompoundConstraint prepSolve1 = new CompoundConstraint(
				new EdgeConstraint( srcEdge, joinedEdgeOnRight ),
				new CornerConstraint( srcCorner, targetCornerWhiteLeft)
			);

			// corner: Right,Front,Up  edge: up,back
			Edge oppositeBackRight = new Edge( Side.Up, CubeGeometry.OppositeSideOf( pair.Leftish ) );
			CompoundConstraint prepSolve2 = new CompoundConstraint(
				new EdgeConstraint( srcEdge, oppositeBackRight ),
				new CornerConstraint( srcCorner, targetCornerWhiteRight)
			);

			// corner: front,up,right  edge: left,up
			Edge oppositeBackLeft = new Edge( CubeGeometry.OppositeSideOf( pair.Rightish ), Side.Up );
			CompoundConstraint prepSolve3 = new CompoundConstraint(
				new EdgeConstraint( srcEdge, oppositeBackLeft ),
				new CornerConstraint( srcCorner, targetCornerWhiteLeft)
			);

			var options = new OptionalConstraint(
				prepSolve0,
				prepSolve1,
				prepSolve2,
				prepSolve3
			);

			// these are constraints to apply move to a solved cube, so constraints don't work on messed up cube
			var result = Solver.GetStepsToAcheiveMatch(6, new CompoundConstraint(
				options,
				CrossConstraint,
				new FtlPair( Side.Right, Side.Back ).Stationary,
				new FtlPair( Side.Back, Side.Left ).Stationary,
				new FtlPair( Side.Left, Side.Front ).Stationary
			));

			// Test
			var preppedCube = cube.Apply(result);

			return result;
		}

		static public TurnSequence PlaceFtlPairDirectly( FtlPair pair, Cube cube ) {

			CompoundConstraint directSolve = new CompoundConstraint(
				Solver.FindEdgeAndSolveIt( cube, pair.Edge ),
				Solver.FindCornerAndSolveIt( cube, pair.Corner )
			);

			return Solver.GetStepsToAcheiveMatch(6, new CompoundConstraint(
				directSolve,
				CrossConstraint,
				new FtlPair( Side.Right, Side.Back ).Stationary,
				new FtlPair( Side.Back, Side.Left ).Stationary,
				new FtlPair( Side.Left, Side.Front ).Stationary
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
