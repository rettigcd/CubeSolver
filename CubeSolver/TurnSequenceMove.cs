using AiSearch.OneSide;

namespace CubeSolver {
	class TurnSequenceMove : Move<Cube> {
		public TurnSequence _turns;
		public TurnSequenceMove( TurnSequence turns ) => _turns = turns;
		public string HumanReadable => _turns.ToString();
		public Cube GenerateChild(Cube cube) => cube.Apply( _turns );
	}

}
