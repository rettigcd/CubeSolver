//using System.Collections.Generic;
//using System.Linq;
//using Xunit;

//namespace CubeSolver {

//	// private so tests don't show up
//	public class DataGenerating_Tests {

//		public static IEnumerable<object[]> EdgePositions {
//			get {
//				return CubeGeometry.AllEdgePositions
//					.Select( e => new object[] { e } )
//					.ToArray();
//			}
//		}

//		public static IEnumerable<object[]> EdgePositionPairs {
//			get {
//				var allEdgePositions = CubeGeometry.AllEdgePositions;
//				foreach(var pos1 in allEdgePositions)
//					foreach(var pos2 in allEdgePositions)
//						if( !pos1.InSameSpace(pos2) )
//							yield return new object[] { pos1, pos2};
//			}
//		}

//		[Theory]
//		[MemberData(nameof(EdgePositions))]
//		public void CanMoveEdgeToBottomCross( Edge startingPosition ) {
//			var destination = new Edge( Side.Front, Side.Down );

//			var turns = Solver.GetStepsToAcheiveMatch( 6, new EdgeConstraint( startingPosition, destination ) );

//			System.IO.File.AppendAllText("C:\\bob.txt", $"{turns._turns.Length}\t{startingPosition}\t{turns}\r\n" );
//		}

//		[Theory]
//		[MemberData(nameof(EdgePositions))]
//		public void CanMoveEdgeToBottomCross_WithoutDistubingCross( Edge startingPosition ) {
//			var destination = new Edge( Side.Front, Side.Down );

//			var otherBottomEdges = new [] {
//				new Edge( Side.Down, Side.Right ),
//				new Edge( Side.Down, Side.Left ),
//				new Edge( Side.Down, Side.Back )
//			};

//			var bottomEdgeStationaryConstraints = otherBottomEdges
//				.Where( e=>!e.InSameSpace(startingPosition) )
//				.Select( x=>EdgeConstraint.Stationary(x) );

//			var constraints = new CompoundConstraint();
//			constraints.AddRange( bottomEdgeStationaryConstraints );
//			constraints.Add( new EdgeConstraint( startingPosition, destination ) );

//			var turns = Solver.GetStepsToAcheiveMatch( 6, constraints );
//			System.IO.File.AppendAllText("C:\\bob.txt", $"{turns._turns.Length}\t{startingPosition}\t{turns}\r\n" );

//			Assert.True( turns._turns.Length <= 6 );
//		}

//		[Theory]
//		[MemberData(nameof(EdgePositionPairs))]
//		public void CanMove_First2Edges_ToBottomCross( Edge startingPosition1, Edge startingPosition2 ) {
//			var constraints = new CompoundConstraint();
//			constraints.Add( new EdgeConstraint( startingPosition1, new Edge( Side.Front, Side.Down ) ) );
//			constraints.Add( new EdgeConstraint( startingPosition2, new Edge( Side.Back, Side.Down ) ) );

//			var turns = Solver.GetStepsToAcheiveMatch( 6, constraints );
//			System.IO.File.AppendAllText("C:\\bob.txt", $"{turns._turns.Length}\t{startingPosition1}\t{startingPosition2}\t{turns}\r\n" );
//		}

//		[Theory]
//		[MemberData(nameof(EdgePositionPairs))]
//		public void CanMove_Last2Edges_ToBottomCross( Edge startingPosition1, Edge startingPosition2 ) {

//			const string filename = "C:\\bob.txt";

//			Edge fixed1 = new Edge( Side.Left, Side.Down );
//			Edge fixed2 = new Edge( Side.Back, Side.Down );

//			if(    startingPosition1.InSameSpace(fixed1)
//				|| startingPosition2.InSameSpace(fixed1)
//				|| startingPosition1.InSameSpace(fixed2)
//				|| startingPosition2.InSameSpace(fixed2)
//			) {
//				System.IO.File.AppendAllText(filename, "-----\r\n" );
//				return;
//			}

//			var constraints = new CompoundConstraint();
//			constraints.Add( new EdgeConstraint( startingPosition1, new Edge( Side.Front, Side.Down ) ) );
//			constraints.Add( new EdgeConstraint( startingPosition2, new Edge( Side.Right, Side.Down ) ) );

//			constraints.Add( EdgeConstraint.Stationary( fixed1 ) );
//			constraints.Add( EdgeConstraint.Stationary( fixed2 ) );

//			try {
//				var turns = Solver.GetStepsToAcheiveMatch( 6, constraints );
//				System.IO.File.AppendAllText(filename, $"{turns._turns.Length}\t{startingPosition1}\t{startingPosition2}\t{turns}\r\n" );
//			} catch(MoveNotFoundExcpetion) {
//				System.IO.File.AppendAllText(filename, "-- No move found. -- \r\n" );
//			}
//		}

//	}

//}
