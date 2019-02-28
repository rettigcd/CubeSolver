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
		// https://en.wikibooks.org/wiki/How_to_Solve_the_Rubik%27s_Cube/CFOP#First_two_layers_(F2L)
		// case 1a
		[InlineData("URU'R'")] 
		[InlineData("URU'R'U")]
		[InlineData("URU'R'UU")]
		[InlineData("URU'R'UUU")]
		// case 1b
		[InlineData("U'F'UF")]
		[InlineData("U'F'UFU")]
		[InlineData("U'F'UFUU")]
		[InlineData("U'F'UFUUU")]
		// case 2a
		[InlineData("RUR'")]
		[InlineData("RUR'U")]
		[InlineData("RUR'UU")]
		[InlineData("RUR'UUU")]
		// case 2b
		[InlineData("F'U'F")]
		[InlineData("F'U'FU")]
		[InlineData("F'U'FUU")]
		[InlineData("F'U'FUUU")]
		// case 3 - hide corner then join -> case 1
		// case 4 - Corner already in slot and needs brought out and transitioned into case 1 or 2
		// case 5 - white is facing up, 8 different things, 4 if we reduce by symetery, 
		// case 6 - pair joined wrong way, separate into case 2
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
		[InlineData("URU'R'",0)] // case 1a
		[InlineData("U'F'UF",0)] // case 1b
		[InlineData("RUR'",0)]   // case 2a
		[InlineData("F'U'F",0)]  // case 2b
		[InlineData("U2URU'R'",1)]
		[InlineData("U2U'F'UF",1)]// fail
		[InlineData("U2RUR'",1)] // fail
		[InlineData("U2F'U'F",1)]
		public void CanPrepF2L(string knownSolution, int expectedPrepMoves) {
			var cube = new Cube().Apply(TurnSequence.Parse(knownSolution).Reverse());

			var pair = new FtlPair(Side.Front,Side.Right);

			var prepSolution = Solver.PrepareToPlaceFtlPairDirectly(pair,cube);
			Assert.Equal(expectedPrepMoves,prepSolution._turns.Length);
			cube = cube.Apply( prepSolution );

			// verify there is now a direct solution
			var finalSolution = Solver.PrepareToPlaceFtlPairDirectly(pair,cube);
			Assert.True(finalSolution._turns.Length <= 4);
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

	public class FtlPair {

		public FtlPair(Side leftishSide, Side rightishSide ) {
			Leftish = leftishSide;
			Rightish = rightishSide;
		}
		public Side Leftish{ get; set; }
		public Side Rightish{ get; set; }
		public Edge Edge => new Edge(Leftish,Rightish);
		public Corner Corner => new Corner(Side.Down,Leftish,Rightish);
	}


}
