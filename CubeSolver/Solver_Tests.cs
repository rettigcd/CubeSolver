using System.Collections.Generic;
using System.Linq;
using Xunit;

// create 2-turn, dis allow duplicate turning
// turn sequence
// remove repeat

namespace CubeSolver {

	public class Solver_Tests {

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

		}

		[Theory]
		[InlineData("DFR")]
		[InlineData("DFRB")]
		[InlineData("RL'FFR'L")]
		[InlineData("RLUDDL'RRBDBBU")]
		[InlineData("B2")]
		public void CanSolveCross(string mixItUpMove) {

			var mixItUpTurns = TurnSequence.Parse(mixItUpMove);
			var cube = new Cube().Apply(mixItUpTurns);

			// When: get cross solution and apply it
			var solution = Solver.GetCrossSolution( cube );
			cube = cube.Apply(solution);

			// Then: cross is solved
			Assert.True( EdgeConstraint.Stationary(new Edge(Side.Front,Side.Down) ).IsMatch(cube) );
			Assert.True( EdgeConstraint.Stationary(new Edge(Side.Right,Side.Down) ).IsMatch(cube) );
			Assert.True( EdgeConstraint.Stationary(new Edge(Side.Back,Side.Down) ).IsMatch(cube) );
			Assert.True( EdgeConstraint.Stationary(new Edge(Side.Left,Side.Down) ).IsMatch(cube) );
		}


		[Fact]
		public void Ftl_NotImplemented() {
			Assert.Throws<System.NotImplementedException>(()=>Solver.PlaceFtlPair(new Cube()));
		}

		[Fact]
		public void MustSolveCrossBeforePlacingFtl() {
			var nonSolvedCrossCube = new Cube().Apply(Turn.Parse("R"));
			Assert.Throws<System.InvalidOperationException>(()=>Solver.PlaceFtlPair(nonSolvedCrossCube));
		}

		[Theory]
		[InlineData("URU'R'")] 
		[InlineData("U'F'UF")]
		[InlineData("RUR'")]
		[InlineData("F'U'F")]
		public void CanPlace_SimpleFtl1(string knowSolveMoves) {

			// Given
			var cube = new Cube().Apply( TurnSequence.Parse( knowSolveMoves ).Reverse() );
			// And
			FtlPair pair = new FtlPair(Side.Front,Side.Right);

			// When
			var solution = Solver.PlaceFtlPairDirectly( pair, cube );
			var result = cube.Apply(solution);

			// Then
			Assert.True( new CompoundConstraint( 
				EdgeConstraint.Stationary(pair.Edge),
				CornerConstraint.Stationary(pair.Corner) 
			).IsMatch(result) );
			Assert.True( Solver.CrossConstraint.IsMatch(result) );
		}

		[Theory]
		// from: https://solvethecube.com/algorithms
		// Basic Case 
		[InlineData("URU'R'")]
		[InlineData("U'F'UF")]
		[InlineData("RUR'")]
		[InlineData("F'U'F")]
		// top layer, white on the side
		[InlineData("U'RU'R'URUR'")]
		[InlineData("UF'UFU'F'U'F")]
		[InlineData("U'RUR'URUR'")]
		[InlineData("UF'U'FU'F'U'F")]
		[InlineData("UF'U2FU'RUR'")]  // d R'U2R d' RUR'
		[InlineData("U'RU2R'UF'U'F")] // U'RU2R'dR'U'R
		[InlineData("RU'R'U2F'U'F")]  // RU'R'UdR'U'R
		[InlineData("F'UFU'U'RUR'")]  // F'UFU'd'FUF'
		[InlineData("UF'U2FUF'U2F")]
		[InlineData("U'RU2R'U'RU2R'")]
		[InlineData("UF'U'FUF'U2F")]
		[InlineData("U'RUR'U'RU2R'")]
		// Corner pointing up, edge in top row
		[InlineData("RU2R'U'RUR'")]
		[InlineData("F'U2FUF'U'F")]
		[InlineData("URU2R'URU'R'")]
		[InlineData("U'F'U2FU'F'UF")]
		[InlineData("U2RUR'URU'R'")]
		[InlineData("U2F'U'FU'F'UF")]
		[InlineData("RUR'U2RUR'U'RUR'")]
		[InlineData("F'U'FU2F'U'FUF'U'F")] // y'R'U'RUUR'U'RUR'U'R
		// Corner in top row, edge in middle 
		[InlineData("UF'UFUF'U2F")]
		[InlineData("U'RU'R'U'RU2R'")]
		[InlineData("UF'U'FU'RUR'")]	// UF'U'Fd'FUF'
		[InlineData("U'RUR'UF'U'F")]	// U'RUR'dR'U'R
		[InlineData("RU'R'UF'UF")]		// RU'R'dR'UR
		[InlineData("RUR'U'RUR'U'RUR'")]
		// corner in bottom, edge in top
		[InlineData("URU'R'U'F'UF")]
		[InlineData("U'F'UFURU'R'")]
		[InlineData("F'UFU'F'UF")]
		[InlineData("RU'R'URU'R'")]
		[InlineData("RUR'U'RUR'")]
		[InlineData("F'U'FUF'U'F")]
		// corner in bottom, edge in middle
		[InlineData("RU'R'URU2R'URU'R'")]
		[InlineData("RU'R'U'RUR'U'RU2R'")]
		[InlineData("RUR'U'RU'R'U2F'U'F")] // RUR'U'RU'R'UdR'U'R
		[InlineData("RU'R'UF'U'FU'F'U'F")] // RU'R'dR'U'RU'R'U'R
		[InlineData("RU'R'UF'U2FUF'U2F")]  // RU'R'dR'U2RUR'U2R
		public void F2L(string knownSolution ) {
			var solveTurns = TurnSequence.Parse( knownSolution );
			var cube = new Cube().Apply( solveTurns.Reverse() );
			Assert.True( cube.Apply( solveTurns ).IsSolved, "not solved" );

			var pair = new FtlPair( Side.Front, Side.Right );

			var prepSolution = Solver.FtlSolution( pair, cube );
			cube=cube.Apply( prepSolution );

			Assert_F2LSolved( cube );
			Assert_CrossSolved( cube );

		}

		static void Assert_CrossSolved( Cube cube ) {
			Assert.True( Solver.CrossConstraint.IsMatch( cube ), "cross not solved" );
		}

		private static void Assert_F2LSolved( Cube cube ) {
			Assert.True( new FtlPair( Side.Front, Side.Right ).Stationary.IsMatch( cube ),"FrontRight F2L not solved" );
			Assert.True( new FtlPair( Side.Right, Side.Back ).Stationary.IsMatch( cube ),"BackRight F2L not solved"  );
			Assert.True( new FtlPair( Side.Back, Side.Left ).Stationary.IsMatch( cube ),"BackLeft F2L not solved"  );
			Assert.True( new FtlPair( Side.Left, Side.Front ).Stationary.IsMatch( cube ), "FrontLeft F2L not solved" );
		}

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
