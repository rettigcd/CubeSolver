using System.Collections.Generic;
using AiSearch.OneSide;

namespace CubeSolver {

	/// <summary>
	/// An Ai 'move' that wraps a single cube 'Turn'
	/// </summary>
	class TurnMove : Move<Cube> {
		public Turn _turn;
		public TurnMove( Turn turn ) { _turn = turn; }
		public string HumanReadable => _turn.ToString();
		public Cube GenerateChild( Cube state ) => state.ApplyTurn( _turn );

	}

}
