namespace CubeSolver {
	static class RotationMath {

		static public Rotation Add(this Rotation d1,Rotation d2) {
			int i1 = (int)d1, i2 = (int)d2;
			return (Rotation)((i1+i2)%4);
		}

		static public Rotation Reverse(this Rotation d1 ) {
			return (Rotation)(4-((int)d1));
		}
	}

}
