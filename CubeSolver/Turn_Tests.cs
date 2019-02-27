using Xunit;

namespace CubeSolver {

	public class Turn_Tests {

		public static object[][] AllSides = {
			new object[] { Side.Back },
			new object[] { Side.Front },
			new object[] { Side.Left },
			new object[] { Side.Right },
			new object[] { Side.Up },
			new object[] { Side.Down },
		};
	
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
		public void TurningASideTwiceTwice_ReSolvesIt(Side side) {

			// Initially solved
			var cube = new Cube();
			Assert.True( cube.IsSolved );
			
			// turn -> unsolved
			Turn turn = new Turn(side,Direction.Twice);
			var x = turn.GetMoveSequence();
			cube = cube.ApplyTurn( turn );
			Assert.False( cube.IsSolved );

			// unturn -> resolved
			cube = cube.ApplyTurn( turn );
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

		[Fact]
		public void InitialCube_SideIsSolved() {
			var cube = new Cube();
			Assert_IsSolved( cube );
		}

		[Theory]
		[InlineData("")]
		[InlineData("F")]
		[InlineData("FB'")]
		[InlineData("U'L")]
		[InlineData("UDLRBF")]
		public void ParseSeriesRoundTrip(string expected) {
			var turns = TurnSequence.Parse(expected);
			string actual = turns.ToString();
			Assert.Equal(expected,actual);
		}

		[Theory]
		[InlineData("ULRB'BR'L'D","UD")]
		[InlineData("UFF'D","UD")]
		public void CancellingTurns_RemovedFromSequence(string orig, string final) {
			var move = TurnSequence.Parse(orig);
			string s = move.ToString();
			Assert.Equal(final,s);
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
		public void ToStringParseRoundTrip_AllPossibleTurns() {
			foreach(var turn in Turn.AllPossibleTurns) {
				string text = turn.ToString();
				var copy = Turn.Parse(text);
				Assert.Equal(turn,copy);
			}
		}

	}

}
