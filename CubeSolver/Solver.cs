using System;
using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;
using Xunit;

namespace CubeSolver {

	public class Solver {

		void Assert_CanMoveEdgeToBottomCross(Side side0, Side side1, int expectedNumberOfTurns) {
			var startingPosition = new Edge(side0,side1);
			var destination = new Edge( Side.Front, Side.Down );
			var turns = GetStepsToPlaceCrossEdge( startingPosition, destination );
			Assert.Equal( expectedNumberOfTurns, turns.Length );
		}

		[Fact]
		public void CanFindAllMovesToPlaceCrossEdge() {

			// bottom row
			Assert_CanMoveEdgeToBottomCross(Side.Front,Side.Down,0);
			Assert_CanMoveEdgeToBottomCross(Side.Right,Side.Down,4);
			Assert_CanMoveEdgeToBottomCross(Side.Left,Side.Down,4);
			Assert_CanMoveEdgeToBottomCross(Side.Back,Side.Down,6);
			Assert_CanMoveEdgeToBottomCross(Side.Down,Side.Right,2);
			Assert_CanMoveEdgeToBottomCross(Side.Down,Side.Left,2);
			Assert_CanMoveEdgeToBottomCross(Side.Down,Side.Back,4);
			Assert_CanMoveEdgeToBottomCross(Side.Down,Side.Front,4);

			// Middle row
			Assert_CanMoveEdgeToBottomCross(Side.Front,Side.Right,1);
			Assert_CanMoveEdgeToBottomCross(Side.Right,Side.Front,3);
			Assert_CanMoveEdgeToBottomCross(Side.Front,Side.Left,1);
			Assert_CanMoveEdgeToBottomCross(Side.Left,Side.Front,3);
			Assert_CanMoveEdgeToBottomCross(Side.Right,Side.Back,3);
			Assert_CanMoveEdgeToBottomCross(Side.Left,Side.Back,3);
 			Assert_CanMoveEdgeToBottomCross(Side.Back,Side.Left,5);
			Assert_CanMoveEdgeToBottomCross(Side.Back,Side.Right,5);

			// top row
			Assert_CanMoveEdgeToBottomCross(Side.Front,Side.Up,2);
			Assert_CanMoveEdgeToBottomCross(Side.Up,Side.Front,4);
			Assert_CanMoveEdgeToBottomCross(Side.Left,Side.Up,3);
			Assert_CanMoveEdgeToBottomCross(Side.Up,Side.Left,3);
			Assert_CanMoveEdgeToBottomCross(Side.Right,Side.Up,3);
			Assert_CanMoveEdgeToBottomCross(Side.Up,Side.Right,3);
			Assert_CanMoveEdgeToBottomCross(Side.Back,Side.Up,4);
			Assert_CanMoveEdgeToBottomCross(Side.Up,Side.Back,4);

		}

		[Fact]
		public void CanPlaceCornerEdgePair() {
/*
			// Use key-hole method to bring turn count down for all but last pair...

			Corner[] startingCornerPositions = new [] {
				new Corner( Side.Up, Side.Right, Side.Front ),
				new Corner( Side.Right, Side.Front, Side.Up ),
				// This is not all the corner locations, there are 3*5=15 total
			};

			Edge[] staringEdgePositions = new [] {
				new Edge( Side.Up, Side.Front ),
				new Edge( Side.Up, Side.Right ),
				new Edge( Side.Up, Side.Back ),
				new Edge( Side.Up, Side.Left ),
				new Edge( Side.Front, Side.Up ),
				new Edge( Side.Right, Side.Up ),
				new Edge( Side.Back, Side.Up ),
				new Edge( Side.Left, Side.Up )
				// not all the corner locations, there are 5*2 total (assuming we can't get from other slot)

			};

			foreach(var corner in startingCornerPositions)
				foreach(var edge in staringEdgePositions)
					CanPlaceCornerEdgePair_Inner( edge, corner );
*/
		}

		void CanPlaceCornerEdgePair_Inner( Edge edgeSource, Corner cornerSource ) {

			Edge edgeDestination = new Edge( Side.Front, Side.Right );
			Corner cornerDestination = new Corner(Side.Down,Side.Front,Side.Right);

			var crossConstraints = new [] {
				new Edge( Side.Down, Side.Right ),
				new Edge( Side.Down, Side.Left ),
				new Edge( Side.Down, Side.Back )
			}.Select( x=>EdgeMatch.Stationary(x) );

			CubeMatch[] otherPairConstriants = new CubeMatch[] {
				CornerMatch.Stationary(new Corner(Side.Down,Side.Right,Side.Back)),
				CornerMatch.Stationary(new Corner(Side.Down,Side.Back,Side.Left)),
				CornerMatch.Stationary(new Corner(Side.Down,Side.Left,Side.Front)),
				EdgeMatch.Stationary(new Edge(Side.Right,Side.Back)),
				EdgeMatch.Stationary(new Edge(Side.Back,Side.Left)),
				EdgeMatch.Stationary(new Edge(Side.Left,Side.Front)),
			};

			var constraints = new List<CubeMatch>();
			constraints.AddRange( crossConstraints );
			constraints.AddRange( otherPairConstriants );
			constraints.Add( new EdgeMatch( edgeSource, edgeDestination ) );
			constraints.Add( new CornerMatch( cornerSource, cornerDestination ));

			var turns = GetStepsToAcheiveMatch( 8, constraints.ToArray() );
			string s = string.Join("",(IEnumerable<Turn>)turns);
			int i = 0;

		}

		Turn[] GetStepsToPlaceCrossEdge( Edge initialEdgePosition, Edge destination ) {

			var bottomEdges = new [] {
				new Edge( Side.Down, Side.Right ),
				new Edge( Side.Down, Side.Left ),
				new Edge( Side.Down, Side.Back )
			};

			var bottomEdgeStationaryConstraints = bottomEdges
				.Where( e=>!e.InSameSpace(initialEdgePosition) )
				.Select( x=>EdgeMatch.Stationary(x) );

			var constraints = new List<EdgeMatch>();
			constraints.AddRange( bottomEdgeStationaryConstraints ); // unmoved
			constraints.Add( new EdgeMatch( initialEdgePosition, destination ) );

			return GetStepsToAcheiveMatch( 6, constraints.ToArray() );
		}

		string GetStepsToPlaceCrossEdgeAsString( Edge initialEdgePosition, Edge destination ) =>
			string.Join("",(IEnumerable<Turn>)GetStepsToPlaceCrossEdge(initialEdgePosition,destination));

		void Assert_TurnIs(Side side, Direction dir, Turn turn ) {
			Assert.Equal(side,turn.Side);
			Assert.Equal(dir,turn.Direction);
		}

		CubeMoveGenerator moveGenerator = new CubeMoveGenerator();

		Turn[] GetStepsToAcheiveMatch( int maxTurnCount, params CubeMatch[] matches ) {
			var cube = new Cube();

			var moveIterator = new IterativeDeepeningIterator<Cube>( moveGenerator, maxTurnCount ) { DontRepeat = true }; // BreadthFirst runs out of memory...

			Node<Cube> winner = moveIterator.Iterate( new Cube() )
				.FirstOrDefault( node => matches.All(match=>match.IsMatch( node.State )) );

			if( winner == null ) throw new Exception( "no solution found" );

			var path = winner.GetNodePath();
			return path.Skip( 1 ).Select( x => ((CubeMove)x.Move)._turn ).ToArray();
		}

	}

}
