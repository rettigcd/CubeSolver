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

		public static IEnumerable<object[]> EdgePositions {
			get {
				return CubeGeometry.GetAllEdgePositions()
					.Select( e => new object[] { e } )
					.ToArray();
			}
		}

		public static IEnumerable<object[]> EdgePositionPairs {
			get {
				var allEdgePositions = CubeGeometry.GetAllEdgePositions();
				foreach(var pos1 in allEdgePositions)
					foreach(var pos2 in allEdgePositions)
						if( !pos1.InSameSpace(pos2) )
							yield return new object[] { pos1, pos2};
			}
		}

		public static IEnumerable<object[]> EdgeCorners {
			get {

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
						yield return new object[] { edge, corner };
			}
		}

		[Theory(Skip = "These are data-generating methods, not really tests.")]
		[MemberData(nameof(EdgePositions))]
		public void CanMoveEdgeToBottomCross( Edge startingPosition ) {
			var destination = new Edge( Side.Front, Side.Down );

			var turns = Solver.GetStepsToAcheiveMatch( 6, new EdgeConstraint( startingPosition, destination ) );

			string moveStr = string.Join("",(IEnumerable<Turn>)turns);
			System.IO.File.AppendAllText("C:\\bob.txt", $"{turns.Length}\t{startingPosition}\t{moveStr}\r\n" );
		}

		[Theory(Skip = "These are data-generating methods, not really tests.")]
		[MemberData(nameof(EdgePositions))]
		public void CanMoveEdgeToBottomCross_WithoutDistubingCross( Edge startingPosition ) {
			var destination = new Edge( Side.Front, Side.Down );

			var otherBottomEdges = new [] {
				new Edge( Side.Down, Side.Right ),
				new Edge( Side.Down, Side.Left ),
				new Edge( Side.Down, Side.Back )
			};

			var bottomEdgeStationaryConstraints = otherBottomEdges
				.Where( e=>!e.InSameSpace(startingPosition) )
				.Select( x=>EdgeConstraint.Stationary(x) );

			var constraints = new CompoundConstraint();
			constraints.AddRange( bottomEdgeStationaryConstraints );
			constraints.Add( new EdgeConstraint( startingPosition, destination ) );

			var turns = Solver.GetStepsToAcheiveMatch( 6, constraints );
			string moveStr = string.Join("",(IEnumerable<Turn>)turns);
			System.IO.File.AppendAllText("C:\\bob.txt", $"{turns.Length}\t{startingPosition}\t{moveStr}\r\n" );

			Assert.True( turns.Length <= 6 );
		}

		[Theory(Skip = "These are data-generating methods, not really tests.")]
		[MemberData(nameof(EdgePositionPairs))]
		public void CanMove_First2Edges_ToBottomCross( Edge startingPosition1, Edge startingPosition2 ) {
			var constraints = new CompoundConstraint();
			constraints.Add( new EdgeConstraint( startingPosition1, new Edge( Side.Front, Side.Down ) ) );
			constraints.Add( new EdgeConstraint( startingPosition2, new Edge( Side.Back, Side.Down ) ) );

			var turns = Solver.GetStepsToAcheiveMatch( 6, constraints );
			string moveStr = string.Join("",(IEnumerable<Turn>)turns);
			System.IO.File.AppendAllText("C:\\bob.txt", $"{turns.Length}\t{startingPosition1}\t{startingPosition2}\t{moveStr}\r\n" );
		}

		[Theory(Skip = "These are data-generating methods, not really tests.")]
		[MemberData(nameof(EdgePositionPairs))]
		public void CanMove_Last2Edges_ToBottomCross( Edge startingPosition1, Edge startingPosition2 ) {

			const string filename = "C:\\bob.txt";

			Edge fixed1 = new Edge( Side.Left, Side.Down );
			Edge fixed2 = new Edge( Side.Back, Side.Down );

			if(    startingPosition1.InSameSpace(fixed1)
				|| startingPosition2.InSameSpace(fixed1)
				|| startingPosition1.InSameSpace(fixed2)
				|| startingPosition2.InSameSpace(fixed2)
			) {
				System.IO.File.AppendAllText(filename, "-----\r\n" );
				return;
			}

			var constraints = new CompoundConstraint();
			constraints.Add( new EdgeConstraint( startingPosition1, new Edge( Side.Front, Side.Down ) ) );
			constraints.Add( new EdgeConstraint( startingPosition2, new Edge( Side.Right, Side.Down ) ) );

			constraints.Add( EdgeConstraint.Stationary( fixed1 ) );
			constraints.Add( EdgeConstraint.Stationary( fixed2 ) );

			try {
				var turns = Solver.GetStepsToAcheiveMatch( 6, constraints );
				string moveStr = string.Join("",(IEnumerable<Turn>)turns);
				System.IO.File.AppendAllText(filename, $"{turns.Length}\t{startingPosition1}\t{startingPosition2}\t{moveStr}\r\n" );
			} catch(MoveNotFoundExcpetion) {
				System.IO.File.AppendAllText(filename, "-- No move found. -- \r\n" );
			}
		}

		[Theory(Skip = "These are data-generating methods, not really tests.")]
		[MemberData(nameof(EdgeCorners))]
		//	CanPlaceCornerEdgePair_FrontRightSlot( new Edge(Side.Up,Side.Back), new Corner(Side.Right,Side.Front,Side.Up) ); // RUR'
		//	CanPlaceCornerEdgePair_FrontRightSlot( new Edge(Side.Right,Side.Up), new Corner(Side.Back,Side.Right,Side.Up) ); // F'UF
		public void CanPlaceCornerEdgePair_FrontRightSlot( Edge edgeSource, Corner cornerSource ) {

			var constraints = new CompoundConstraint();

			Edge edgeDestination = new Edge( Side.Front, Side.Right );
				constraints.Add( new EdgeConstraint( edgeSource, edgeDestination ) );
			Corner cornerDestination = new Corner( Side.Down, Side.Front, Side.Right );
				constraints.Add( new CornerConstraint( cornerSource, cornerDestination ) );

			constraints.AddRange( CrossConstraints() );

			constraints.AddRange( new CubeConstraint[] {
				CornerConstraint.Stationary(new Corner(Side.Down,Side.Right,Side.Back)),
				CornerConstraint.Stationary(new Corner(Side.Down,Side.Back,Side.Left)),
				CornerConstraint.Stationary(new Corner(Side.Down,Side.Left,Side.Front)),
				EdgeConstraint.Stationary(new Edge(Side.Right,Side.Back)),
				EdgeConstraint.Stationary(new Edge(Side.Back,Side.Left)),
				EdgeConstraint.Stationary(new Edge(Side.Left,Side.Front)),
			} );

			var turns = Solver.GetStepsToAcheiveMatch( 8, constraints );
			string s = string.Join( "", (IEnumerable<Turn>)turns );
			int i = 0;

		}

		#region Long Running 3-edge cubes

		//public static IEnumerable<object[]> EdgePositionTriplets {
		//	get {
		//		var allEdgePositions = CubeGeometry.GetAllEdgePositions();
		//		foreach(var pos1 in allEdgePositions)
		//			foreach(var pos2 in allEdgePositions)
		//				if( !pos1.InSameSpace(pos2) )
		//					foreach(var pos3 in allEdgePositions)
		//						if( !pos3.InSameSpace(pos1) && !pos3.InSameSpace(pos2) )
		//							yield return new object[] { pos1, pos2, pos3 };
		//	}
		//}

		//[Theory]
		//[MemberData(nameof(EdgePositionTriplets))]
		//public void CanMove3EdgesToBottomCross( Edge startingPosition1, Edge startingPosition2, Edge startingPosition3 ) {
		//	var constraints = new CompoundConstraint();
		//	constraints.Add( new EdgeConstraint( startingPosition1, new Edge( Side.Front, Side.Down ) ) );
		//	constraints.Add( new EdgeConstraint( startingPosition2, new Edge( Side.Right, Side.Down ) ) );
		//	constraints.Add( new EdgeConstraint( startingPosition3, new Edge( Side.Back, Side.Down ) ) );

		//	Turn[] turns = Solver.GetStepsToAcheiveMatch( 6, constraints );
		//	System.Console.WriteLine(string.Join("",(IEnumerable<Turn>)turns));
		//}

		#endregion

		#region private static

		static IEnumerable<EdgeConstraint> CrossConstraints() {
			return new[] {
				new Edge( Side.Down, Side.Right ),
				new Edge( Side.Down, Side.Left ),
				new Edge( Side.Down, Side.Back ),
				new Edge( Side.Down, Side.Front )
			}.Select( x => EdgeConstraint.Stationary( x ) );
		}

		#endregion

	}

}
