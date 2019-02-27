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
			var adjacents = CubeGeometry.GetClockwiseAdjacentFaces(side);
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
			var adjacents = CubeGeometry.GetClockwiseAdjacentFaces( side );
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

	}

}
