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
		[InlineData("RUR'U'")][InlineData("F'U'FU")]	// Case 1 - this is interesting, there are 2 solutions with 4 moves
		[InlineData("RU'R'")][InlineData("F'UF")]		// Case 2
		// case 3 - hide corner than join -> case 1
		// case 4 - Corner already in slot and needs brought out and transitioned into case 1 or 2
		// case 5 - white is facing up, 8 different things, 4 if we reduce by symetery, 
			// one solutions hides the edge so corner can join -> case 1
		// case 6 - pair joined wrong way, separate into case 2
		public void CanPlace_SimpleFtl1(string messUpMoves) {

			// Given
			var cube = new Cube().Apply(TurnSequence.Parse(messUpMoves));
			// And
			var edge = new Edge(Side.Front,Side.Right);
			var corner = new Corner(Side.Down,Side.Front,Side.Right);

			// When
			var solution = Solver.PlaceFtlPair( new CompoundConstraint(
				Solver.FindEdgeAndSolveIt  (cube, edge ),
				Solver.FindCornerAndSolveIt(cube, corner)
			));
			var result = cube.Apply(solution);

			// Then
			Assert.True( new CompoundConstraint( EdgeConstraint.Stationary(edge),CornerConstraint.Stationary(corner) ).IsMatch(result) );
			Assert.True( Solver.CrossConstraint.IsMatch(result) );
		}

		/* this test checks if solver can solve place F2L from anywhere.  It can't - yet
		[Fact]
		public void CanFindAndPlace_Ftl1() {
			var cube = new Scrambler().Scramble(new Cube());
			var crossSolution = Solver.GetCrossSolution(cube);
			cube.Apply(crossSolution);

			var ftlSolution = Solver.PlaceFtlPair( new CompoundConstraint(
				Solver.FindEdgeAndSolveIt  (cube, new Edge  (Side.Front,Side.Right) ),
				Solver.FindCornerAndSolveIt(cube, new Corner(Side.Down,Side.Front,Side.Right))
			));
			var result = cube.Apply(ftlSolution);
		}
		*/


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
