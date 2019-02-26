using System.Linq;
using Xunit;

namespace CubeSolver {

	public class CubeGeometry_Tests {

		public static object[][] AllSides = {
			new object[] { Side.Back },
			new object[] { Side.Front },
			new object[] { Side.Left },
			new object[] { Side.Right },
			new object[] { Side.Up },
			new object[] { Side.Down },
		};

		[Theory]
		[MemberData(nameof(AllSides))]
		public void SideHas9Squares(Side side) {
			var squares = MovablePosition.GetMovablePositionsForSide(side);
			Assert.Equal(8, squares.Length);

			// Center
			Assert.Equal(0, squares.Count( MovablePosition.IsCenter ) );

			// Edges
			MovablePosition[] edges = squares.Where( MovablePosition.IsEdge ).ToArray();
			Assert.Equal(4, edges.Length );
			var adjacents = CubeGeometry.AdjacentSidesOf(side);
			foreach(var adjacent in adjacents)
				Assert.Contains(MovablePosition.Get(side,adjacent), edges);

			// Corners
			MovablePosition[] corners = squares.Where( MovablePosition.IsCorner ).ToArray();
			Assert.Equal(4, corners.Length );
			for(int i=0;i<4;++i)
				Assert.Contains(MovablePosition.Get(side,adjacents[i]|adjacents[(i+1)%4]), corners);
		}

		[Fact]
		public void CanFindAllAdjacentSides() {
			Assert.Equal(6, CubeGeometry.AllSides.Length);
			foreach(var side in CubeGeometry.AllSides)
				Assert_CanFindClockwiseAdjacentSidesOf( side );
		}

		void Assert_CanFindClockwiseAdjacentSidesOf( Side side ) {
			var adjacents = CubeGeometry.AdjacentSidesOf( side );
			Assert.Equal( 4, adjacents.Length );
			var opposite = CubeGeometry.OppositeSideOf( side );
			foreach( var adjacent in adjacents )
				Assert.True( adjacent != side && adjacent != opposite );

			Assert_AdjacentSidesListGoAroundFaceInOrder( adjacents );
		}

		void Assert_AdjacentSidesListGoAroundFaceInOrder( Side[] adjacents ) {
			OppositeSidesOfCube( adjacents[0], adjacents[2] );
			OppositeSidesOfCube( adjacents[1], adjacents[3] );
		}

		[Theory]
		[InlineData(Side.Up, Side.Down)]
		[InlineData(Side.Left, Side.Right)]
		[InlineData(Side.Front, Side.Back)]
		public void OppositeSidesOfCube(Side s1, Side s2) {
			Assert.Equal( s1, CubeGeometry.OppositeSideOf(s2) );
			Assert.Equal( s2, CubeGeometry.OppositeSideOf(s1) );
		}

		[Fact]
		public void InitialCube_SideIsSolved() {
			var cube = new Cube();
			Assert_IsSolved( cube );
		}

		static void Assert_IsSolved( Cube cube ) {
			foreach( var side in CubeGeometry.AllSides )
				Assert_SideIsSolved( side, cube );

			Assert.True( cube.IsSolved );
		}

		static void Assert_SideIsSolved( Side side, Cube cube ) {
			var positions = MovablePosition.GetMovablePositionsForSide( side );
			foreach( var pos in positions )
				Assert.Equal( side, cube[pos] );
		}

		[Fact]
		public void TurningFaceClockwise_Moves1RowOfStickersOnAdjacentSides() {
			var cube = new Cube();
			Assert_IsSolved( cube );

			cube = cube.ApplyTurn(Turn.Parse("U") );

			Assert.Equal(Side.Right,cube[MovablePosition.Get(Side.Front,Side.Up)]);
			Assert.Equal(Side.Right,cube[MovablePosition.Get(Side.Front,Side.Up|Side.Left)]);
			Assert.Equal(Side.Right,cube[MovablePosition.Get(Side.Front,Side.Up|Side.Right)]);

			Assert.Equal(Side.Back,cube[MovablePosition.Get(Side.Right,Side.Up)]);
			Assert.Equal(Side.Back,cube[MovablePosition.Get(Side.Right,Side.Up|Side.Front)]);
			Assert.Equal(Side.Back,cube[MovablePosition.Get(Side.Right,Side.Up|Side.Back)]);

			Assert.Equal(Side.Left,cube[MovablePosition.Get(Side.Back,Side.Up)]);
			Assert.Equal(Side.Left,cube[MovablePosition.Get(Side.Back,Side.Up|Side.Right)]);
			Assert.Equal(Side.Left,cube[MovablePosition.Get(Side.Back,Side.Up|Side.Left)]);

			Assert.Equal(Side.Front,cube[MovablePosition.Get(Side.Left,Side.Up)]);
			Assert.Equal(Side.Front,cube[MovablePosition.Get(Side.Left,Side.Up|Side.Back)]);
			Assert.Equal(Side.Front,cube[MovablePosition.Get(Side.Left,Side.Up|Side.Front)]);

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
				cube = cube.ApplyTurn(new Turn(side,Direction.Clockwise));
				Assert.False( cube.IsSolved );
			}

			// 4th turn resolves it
			cube = cube.ApplyTurn(new Turn(side,Direction.Clockwise));
			Assert.True( cube.IsSolved );

		}

		[Theory]
		[MemberData(nameof(AllSides))]
		public void TurningASideForwardAndBackReturnsToOriginalPosition(Side side) {

			// Initially solved
			var cube = new Cube();
			Assert.True( cube.IsSolved );
			
			// turn -> unsolved
			cube = cube.ApplyTurn(new Turn(side,Direction.Clockwise));
			Assert.False( cube.IsSolved );

			// unturn -> resolved
			cube = cube.ApplyTurn(new Turn(side,Direction.CounterClockwise));
			Assert.True( cube.IsSolved );

		}

		[Theory]
		[MemberData(nameof(AllSides))]
		public void TurningSideOfScrambledCube(Side side) {

			// Initially solved
			var cube = new Cube(); new Scrambler().Scramble( cube );
			var orig = cube.Clone();
			
			// turn -> unsolved
			cube = cube.ApplyTurn(new Turn(side,Direction.Clockwise));
			Assert.False( orig.Equals(cube) );

			// unturn -> resolved
			cube = cube.ApplyTurn(new Turn(side,Direction.CounterClockwise));
			Assert.True( orig.Equals(cube) );

		}

	}

}
