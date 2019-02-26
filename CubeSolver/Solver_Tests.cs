using System;
using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;
using Xunit;

namespace CubeSolver {

	// There are approximately:
	// 1 Million - 6-move positions
	// 21 Million - 7-move
	// 234 Milion - 8-move positions

	public class Solver_Tests {

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

		[Fact(Skip ="test not finding timely solution")]
		public void CanPlaceF2L() {

			CanPlaceCornerEdgePair_FrontRightSlot( new Edge(Side.Up,Side.Back), new Corner(Side.Right,Side.Front,Side.Up) );

			// Use key-hole method to bring turn count down for all but last pair...

			// corner above the slot
			Corner[] startingCornerPositions = new [] {
				new Corner( Side.Up, Side.Right, Side.Front ),
				new Corner( Side.Right, Side.Front, Side.Up ),
				new Corner( Side.Front, Side.Up, Side.Right ),
			};
			// missing corner already in slot...

			Edge[] staringEdgePositions = new [] {
				// top
				new Edge( Side.Up, Side.Front ),
				new Edge( Side.Up, Side.Right ),
				new Edge( Side.Up, Side.Back ),
				new Edge( Side.Up, Side.Left ),
				new Edge( Side.Front, Side.Up ),
				new Edge( Side.Right, Side.Up ),
				new Edge( Side.Back, Side.Up ),
				new Edge( Side.Left, Side.Up ),
				// slot
				new Edge( Side.Front, Side.Right ),
				new Edge( Side.Right, Side.Front )

			};

			foreach(var corner in startingCornerPositions)
				foreach(var edge in staringEdgePositions)
					CanPlaceCornerEdgePair_FrontRightSlot( edge, corner );

		}

		void CanPlaceCornerEdgePair_FrontRightSlot( Edge edgeSource, Corner cornerSource ) {

			Edge edgeDestination = new Edge( Side.Front, Side.Right );
			Corner cornerDestination = new Corner( Side.Down, Side.Front, Side.Right );

			CubeConstraint[] otherPairConstriants = new CubeConstraint[] {
				CornerConstraint.Stationary(new Corner(Side.Down,Side.Right,Side.Back)),
				CornerConstraint.Stationary(new Corner(Side.Down,Side.Back,Side.Left)),
				CornerConstraint.Stationary(new Corner(Side.Down,Side.Left,Side.Front)),
				EdgeConstraint.Stationary(new Edge(Side.Right,Side.Back)),
				EdgeConstraint.Stationary(new Edge(Side.Back,Side.Left)),
				EdgeConstraint.Stationary(new Edge(Side.Left,Side.Front)),
			};

			var constraints = new CompoundConstraint();
			constraints.AddRange( CrossConstraints() );
			constraints.AddRange( otherPairConstriants );
			constraints.Add( new EdgeConstraint( edgeSource, edgeDestination ) );
			constraints.Add( new CornerConstraint( cornerSource, cornerDestination ) );

			var turns = Solver.GetStepsToAcheiveMatch( 8, constraints );
			string s = string.Join( "", (IEnumerable<Turn>)turns );
			int i = 0;

		}

		private static IEnumerable<EdgeConstraint> CrossConstraints() {
			return new[] {
				new Edge( Side.Down, Side.Right ),
				new Edge( Side.Down, Side.Left ),
				new Edge( Side.Down, Side.Back ),
				new Edge( Side.Down, Side.Front )
			}.Select( x => EdgeConstraint.Stationary( x ) );
		}

		Turn[] GetStepsToPlaceCrossEdge( Edge initialEdgePosition, Edge destination ) {

			var bottomEdges = new [] {
				new Edge( Side.Down, Side.Right ),
				new Edge( Side.Down, Side.Left ),
				new Edge( Side.Down, Side.Back )
			};

			var bottomEdgeStationaryConstraints = bottomEdges
				.Where( e=>!e.InSameSpace(initialEdgePosition) )
				.Select( x=>EdgeConstraint.Stationary(x) );

			var constraints = new CompoundConstraint();
			constraints.AddRange( bottomEdgeStationaryConstraints ); // unmoved
			constraints.Add( new EdgeConstraint( initialEdgePosition, destination ) );

			return Solver.GetStepsToAcheiveMatch( 6, constraints );
		}

		string GetStepsToPlaceCrossEdgeAsString( Edge initialEdgePosition, Edge destination ) =>
			string.Join("",(IEnumerable<Turn>)GetStepsToPlaceCrossEdge(initialEdgePosition,destination));

		void Assert_TurnIs(Side side, Direction dir, Turn turn ) {
			Assert.Equal(side,turn.Side);
			Assert.Equal(dir,turn.Direction);
		}

	}

}
