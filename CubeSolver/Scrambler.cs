using System;

namespace CubeSolver {

	public class Scrambler {

		public Cube Scramble(Cube cube) {
			Random rnd = new Random();

			for(int i=0;i<50;++i)
				cube = cube.Apply(
					new Turn(CubeGeometry.AllSides[rnd.Next(6)],
					rnd.Next(2)==0 ? Rotation.Clockwise:Rotation.CounterClockwise
				));
			return cube;
		}
	}

}
