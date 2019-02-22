using System;
using System.Linq;
using Xunit;

namespace CubeSolver {

	public class Solver_Tests {

		public static object[][] AllSides = {
			new object[] { Side.Back },
			new object[] { Side.Front },
			new object[] { Side.Left },
			new object[] { Side.Right },
			new object[] { Side.Top },
			new object[] { Side.Bottom },
		};

		[Theory]
		[MemberData(nameof(AllSides))]
		public void SideHas9Squares(Side side) {
			var squares = Cube.GetSquarePositionsForSide(side);
			Assert.Equal(9, squares.Length);

			// Center
			Assert.Equal(1, squares.Count( SquarePos.IsCenter ) );

			// Edges
			SquarePos[] edges = squares.Where( SquarePos.IsEdge ).ToArray();
			Assert.Equal(4, edges.Length );
			var adjacents = Cube.AdjacentSidesOf(side);
			foreach(var adjacent in adjacents)
				Assert.Contains(SquarePos.Get(side,adjacent), edges);

			// Corners
			SquarePos[] corners = squares.Where( SquarePos.IsCorner ).ToArray();
			Assert.Equal(4, corners.Length );
			for(int i=0;i<4;++i)
				Assert.Contains(SquarePos.Get(side,adjacents[i]|adjacents[(i+1)%4]), corners);
		}

		[Fact]
		public void CanFindAllAdjacentSides() {
			Assert.Equal(6, Cube.AllSides.Length);
			foreach(var side in Cube.AllSides)
				Assert_CanFindClockwiseAdjacentSidesOf( side );
		}

		void Assert_CanFindClockwiseAdjacentSidesOf( Side side ) {
			var adjacents = Cube.AdjacentSidesOf( side );
			Assert.Equal( 4, adjacents.Length );
			var opposite = Cube.OppositeSideOf( side );
			foreach( var adjacent in adjacents )
				Assert.True( adjacent != side && adjacent != opposite );

			Assert_AdjacentSidesListGoAroundFaceInOrder( adjacents );
		}

		void Assert_AdjacentSidesListGoAroundFaceInOrder( Side[] adjacents ) {
			OppositeSidesOfCube( adjacents[0], adjacents[2] );
			OppositeSidesOfCube( adjacents[1], adjacents[3] );
		}

		[Theory]
		[InlineData(Side.Top, Side.Bottom)]
		[InlineData(Side.Left, Side.Right)]
		[InlineData(Side.Front, Side.Back)]
		public void OppositeSidesOfCube(Side s1, Side s2) {
			Assert.Equal( s1, Cube.OppositeSideOf(s2) );
			Assert.Equal( s2, Cube.OppositeSideOf(s1) );
		}

		[Fact]
		public void InitialCube_SideIsSolved() {
			var cube = new Cube();
			Assert_IsSolved( cube );
		}

		static void Assert_IsSolved( Cube cube ) {
			foreach( var side in Cube.AllSides )
				Assert_SideIsSolved( side, cube );

			Assert.True( cube.IsSolved );
		}

		static void Assert_SideIsSolved( Side side, Cube cube ) {
			var positions = Cube.GetSquarePositionsForSide( side );
			foreach( var pos in positions )
				Assert.Equal( side, cube[pos] );
		}

		[Fact]
		public void TurningFaceClockwise_Moves1RowOfStickersOnAdjacentSides() {
			var cube = new Cube();
			Assert_IsSolved( cube );

			cube.Turn(new Turn(Side.Top, Direction.Clockwise) );

			Assert.Equal(Side.Right,cube[SquarePos.Get(Side.Front,Side.Top)]);
			Assert.Equal(Side.Right,cube[SquarePos.Get(Side.Front,Side.Top|Side.Left)]);
			Assert.Equal(Side.Right,cube[SquarePos.Get(Side.Front,Side.Top|Side.Right)]);

			Assert.Equal(Side.Back,cube[SquarePos.Get(Side.Right,Side.Top)]);
			Assert.Equal(Side.Back,cube[SquarePos.Get(Side.Right,Side.Top|Side.Front)]);
			Assert.Equal(Side.Back,cube[SquarePos.Get(Side.Right,Side.Top|Side.Back)]);

			Assert.Equal(Side.Left,cube[SquarePos.Get(Side.Back,Side.Top)]);
			Assert.Equal(Side.Left,cube[SquarePos.Get(Side.Back,Side.Top|Side.Right)]);
			Assert.Equal(Side.Left,cube[SquarePos.Get(Side.Back,Side.Top|Side.Left)]);

			Assert.Equal(Side.Front,cube[SquarePos.Get(Side.Left,Side.Top)]);
			Assert.Equal(Side.Front,cube[SquarePos.Get(Side.Left,Side.Top|Side.Back)]);
			Assert.Equal(Side.Front,cube[SquarePos.Get(Side.Left,Side.Top|Side.Front)]);

			Assert.False( cube.IsSolved );

		}

		[Theory]
		[MemberData(nameof(AllSides))]
		public void TurningASide4TimesResolvesIt(Side side) {

			// Initially solved
			var cube = new Cube();
			Assert.True( cube.IsSolved );
			
			// 3 turns, not solved
			for(int i=0;i<3;++i) {
				cube.Turn(new Turn(side,Direction.Clockwise));
				Assert.False( cube.IsSolved );
			}

			// 4th turn resolves it
			cube.Turn(new Turn(side,Direction.Clockwise));
			Assert.True( cube.IsSolved );

		}

		[Theory]
		[MemberData(nameof(AllSides))]
		public void TurningASideForwardAndBackReturnsToOriginalPosition(Side side) {

			// Initially solved
			var cube = new Cube();
			Assert.True( cube.IsSolved );
			
			// turn -> unsolved
			cube.Turn(new Turn(side,Direction.Clockwise));
			Assert.False( cube.IsSolved );

			// unturn -> resolved
			cube.Turn(new Turn(side,Direction.CounterClockwise));
			Assert.True( cube.IsSolved );

		}

		[Theory]
		[MemberData(nameof(AllSides))]
		public void TurningSideOfScrambledCube(Side side) {

			// Initially solved
			var cube = new Cube(); new Scrambler().Scramble( cube );
			var orig = cube.Clone();
			
			// turn -> unsolved
			cube.Turn(new Turn(side,Direction.Clockwise));
			Assert.False( orig.Equals(cube) );

			// unturn -> resolved
			cube.Turn(new Turn(side,Direction.CounterClockwise));
			Assert.True( orig.Equals(cube) );

		}

	}

}
