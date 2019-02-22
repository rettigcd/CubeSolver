using System;
using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;
using Xunit;

namespace CubeSolver {

	public class EdgeMatch {

		static public EdgeMatch Stationary( Edge edge ) => new EdgeMatch( edge, edge );

		public EdgeMatch(Edge fromColor, Edge toLocation) {
			this.FromColor = fromColor;
			this.ToLocation = toLocation;
		}

		/// <summary> 
		/// Since initially, all pieces are in their starting position,
		/// this is both the initial location and the 'color' of the piece we are going to move. 
		/// Note: .
		/// </summary>
		/// <remarks>
		/// We are using Side to designate color. 
		/// Instead of the top being 'White' color, the top is 'Top' color. </remarks>
		public Edge FromColor { get; private set; }

		/// <summary>
		/// Where we want the piece to be;
		/// </summary>
		public Edge ToLocation { get; private set; }

		public bool IsMatch( Cube cube ) {
			return cube[ ToLocation.Pos0 ] == FromColor.Side0
				&& cube[ ToLocation.Pos1 ] == FromColor.Side1;
		}
	}


	public class Solver {

		Edge frontBottomEdge = new Edge( Side.Front, Side.Bottom );

		[Fact]
		public void CanSeeEdgeIsSolved() {

			var turns = GetStepsToAcheiveMatch( EdgeMatch.Stationary( frontBottomEdge ) );
			Assert.Empty(turns);

		}

		[Fact]
		public void Requires_1Turn1() {
			var turns = GetStepsToAcheiveMatch( new EdgeMatch( new Edge( Side.Front, Side.Right ), frontBottomEdge ) );
			var turn = Assert.Single(turns);
			Assert_TurnIs(Side.Front,Direction.Clockwise,turn);
		}

		[Fact]
		public void Requires_1Turn2() {
			var turns = GetStepsToAcheiveMatch( new EdgeMatch( new Edge( Side.Front, Side.Left ), frontBottomEdge ) );
			var turn = Assert.Single(turns);
			Assert_TurnIs(Side.Front,Direction.CounterClockwise,turn);
		}

		[Fact]
		public void Requres_2Turns1() {
			var turns = GetStepsToAcheiveMatch( new EdgeMatch( new Edge( Side.Front, Side.Top ), frontBottomEdge ) );
			Assert.Equal(2, turns.Length);
			Assert_TurnIs(Side.Front,Direction.Clockwise,turns[0]);
			Assert_TurnIs(Side.Front,Direction.Clockwise,turns[1]);
		}

//		[Fact]
		public void CanFindAllMovesToPlaceCrossEdge() {	 // !!!
			Turn[] turns;

			// bottom row
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Bottom,Side.Right) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Bottom,Side.Left) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Bottom,Side.Back) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Bottom,Side.Front) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Right,Side.Bottom) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Left,Side.Bottom) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Back,Side.Bottom) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Front,Side.Bottom) ); Assert.NotNull( turns );

			// tops
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Top,Side.Front) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Front,Side.Top) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Top,Side.Left) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Left,Side.Top) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Top,Side.Right) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Right,Side.Top) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Top,Side.Back) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Back,Side.Top) ); Assert.NotNull( turns );

			// Middle row
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Front,Side.Right) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Right,Side.Front) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Front,Side.Left) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Left,Side.Front) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Right,Side.Back) ); Assert.NotNull( turns );
			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Left,Side.Back) ); Assert.NotNull( turns );
// 			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Back,Side.Left) ); Assert.NotNull( turns ); // Out of memory
//			turns = GetStepsToPlaceCrossEdge( new Edge(Side.Back,Side.Right) ); Assert.NotNull( turns ); // Not enough memory

		}

		[Fact]
		public void Requres_3Turns() {
			Edge initialEdgePosition = new Edge( Side.Top, Side.Right );
			Turn[] turns = GetStepsToPlaceCrossEdge( initialEdgePosition );

			Assert.Equal( 3, turns.Length );
			Assert_TurnIs( Side.Right, Direction.CounterClockwise, turns[0] );
			Assert_TurnIs( Side.Front, Direction.Clockwise, turns[1] );
			Assert_TurnIs( Side.Right, Direction.Clockwise, turns[2] );
		}

		Turn[] GetStepsToPlaceCrossEdge( Edge initialEdgePosition ) {

			var bottomEdges = new [] {
				new Edge( Side.Bottom, Side.Right ),
				new Edge( Side.Bottom, Side.Left ),
				new Edge( Side.Bottom, Side.Back )
			};

			var bottomEdgeStationaryConstraints = bottomEdges
				.Where( e=>!e.Equals(initialEdgePosition) )
				.Select( x=>EdgeMatch.Stationary(x) );

			var constraints = new List<EdgeMatch>();
			constraints.AddRange( bottomEdgeStationaryConstraints );
			constraints.Add( new EdgeMatch( initialEdgePosition, frontBottomEdge ) );

			var turns = GetStepsToAcheiveMatch( constraints.ToArray() );

			string msg = string.Join(" => ",(IEnumerable<Turn>)turns);
			// Console.WriteLine( msg );
			System.Diagnostics.Debug.WriteLine( msg );
			return turns;
		}

		void Assert_TurnIs(Side side, Direction dir, Turn turn ) {
			Assert.Equal(side,turn.Side);
			Assert.Equal(dir,turn.Direction);
		}

		CubeMoveGenerator moveGenerator = new CubeMoveGenerator();

		Turn[] GetStepsToAcheiveMatch( params EdgeMatch[] matches ) {
			var cube = new Cube();

			var winner = new BreadthFirst<Cube>( new Node<Cube>( new Cube(), null ), moveGenerator, 8 ).IterateFilteringOutAncestors()
				.FirstOrDefault( node => matches.All(match=>match.IsMatch( node.State )) );

			if( winner == null ) throw new Exception( "no solution found" );

			var path = winner.GetNodePath();
			return path.Skip( 1 ).Select( x => ((CubeMove)x.Move)._turn ).ToArray();
		}

		public void CanFindMoveToMakeCrossPiece() {

			// -- Solved --
			// Front / Bottom => Front / Bottom

			// -- Middle row --
			// Front/Right => Front/Bottom, Keep: Bottom/Right Bottom/Back Bottom/Left
			// Right/Front => Front/Bottom, Keep: Bottom/Right Bottom/Back Bottom/Left

			// Front/Left => Front/Bottom, Keep: Bottom/Right Bottom/Back Bottom/Left
			// Left/Front => Front/Bottom, Keep: Bottom/Right Bottom/Back Bottom/Left

			// Back/Right => Front/Bottom, Keep: Bottom/Right Bottom/Back Bottom/Left
			// Right/Back => Front/Bottom, Keep: Bottom/Right Bottom/Back Bottom/Left

			// Back/Left => Front/Bottom, Keep: Bottom/Right Bottom/Back Bottom/Left
			// Left/Back => Front/Bottom, Keep: Bottom/Right Bottom/Back Bottom/Left

			// -- Top Row --
			// Front/Top => Front / Bottom
			// Top/Front => Front / Bottom

			// Left/Top => Front / Bottom
			// Top/Left => Front / Bottom

			// Back/Top => Front / Bottom
			// Top/Back => Front / Bottom

			// Right/Top => Front / Bottom
			// Top/Right => Front / Bottom

			// -- Bottom Row --
			// Right/Bottom => Front / Bottom  (allow other slot to move)
			// Bottom/Right => Front / Bottom  (allow other slot to move)

			// Back/Bottom => Front / Bottom   (allow other slot to move)
			// Bottom/Back => Front / Bottom   (allow other slot to move)

			// Left/Bottom => Front / Bottom   (allow other slot to move)
			// Bottom/Left => Front / Bottom   (allow other slot to move)

			// -- Solved --
			// Front / Bottom => Front / Bottom

		}


		// make the cross without disturbing other cross
			// 4 slots on side * 2 orientations
			// 4 slots on bottom * 2 orientations
			// 7 on top
			// 1 solved

		// fill the slots without distrubing other slots

		// last layer part 1
		// last layer part 2



	}

}
