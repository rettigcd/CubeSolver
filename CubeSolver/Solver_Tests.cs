using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CubeSolver {

	public class Solver_Tests {

		[Fact]
		public void CanSolveCrossAndFirst2Layers() {
			var scrambled = new Scrambler().Scramble(new Cube());

			var crossSolution = Solver.GetCrossSolution(scrambled);
			var crossSolved = scrambled.Apply( crossSolution );

			var ftlSolution = Solver.PlaceFtlPairs( crossSolved );
			var ftlSolved = crossSolved.Apply(ftlSolution);

			int i = 0;

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
		public void MustSolveCrossBeforePlacingFtl() {
			var nonSolvedCrossCube = new Cube().Apply(Turn.Parse("R"));
			var slot = new FtlSlot( Side.Front, Side.Right );
			Assert.Throws<System.InvalidOperationException>(()=> Solver.PlaceSingleFtlPairFromTop( slot, nonSolvedCrossCube ) );
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

			var pair = new FtlSlot( Side.Front, Side.Right );

			var prepSolution = Solver.PlaceSingleFtlPairFromTop( pair, cube );
			cube=cube.Apply( prepSolution );

			Assert_F2LSolved( cube );
			Assert_CrossSolved( cube );

		}

		static void Assert_CrossSolved( Cube cube ) {
			Assert.True( Constraints.CrossConstraint.IsMatch( cube ), "cross not solved" );
		}

		static void Assert_F2LSolved( Cube cube ) {
			Assert.True( new FtlSlot( Side.Front, Side.Right ).Home.Stationary.IsMatch( cube ),"FrontRight F2L not solved" );
			Assert.True( new FtlSlot( Side.Right, Side.Back ).Home.Stationary.IsMatch( cube ),"BackRight F2L not solved"  );
			Assert.True( new FtlSlot( Side.Back, Side.Left ).Home.Stationary.IsMatch( cube ),"BackLeft F2L not solved"  );
			Assert.True( new FtlSlot( Side.Left, Side.Front ).Home.Stationary.IsMatch( cube ), "FrontLeft F2L not solved" );
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
