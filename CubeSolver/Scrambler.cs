using System;
using System.Linq;
using Xunit;

namespace CubeSolver {

	public class Scrambler {
		public void Scramble(Cube cube) {
			Random rnd = new Random();

			for(int i=0;i<50;++i)
				cube = cube.ApplyTurn(
					new Turn(Cube.AllSides[rnd.Next(6)],
					rnd.Next(2)==0 ? Direction.Clockwise:Direction.CounterClockwise
				));
		}
	}

}
