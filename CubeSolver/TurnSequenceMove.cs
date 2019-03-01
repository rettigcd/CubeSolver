using AiSearch.OneSide;

namespace CubeSolver {


	/// <summary>
	/// An AI 'move' that wraps a single cube 'Move' ie sequence-of-turns
	/// </summary>
	class TurnSequenceMove : Move<Cube> {
		public TurnSequence _sequence;
		public TurnSequenceMove( TurnSequence turns ) => _sequence = turns;
		public string HumanReadable => _sequence.ToString();
		public Cube GenerateChild(Cube cube) => cube.Apply( _sequence );
	}

}
