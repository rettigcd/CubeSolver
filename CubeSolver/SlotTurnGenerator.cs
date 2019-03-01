using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;

namespace CubeSolver {

	/// <summary>
	/// While solving the F2L, we can limit the possible moves to: 
	/// a) moving the top: CW,CCW,twice
	/// b) R,(a),R'
	/// c) F',(a),F
	/// 
	/// And throw away the 3 moves that repeat the last sequence
	/// 
	/// Leaving a branching factor of 9,6,6,6,6,6... and reducing the depth by 2 for all turns including Rightish and Leftish sides.
	/// </summary>
	class SlotTurnGenerator : NodeMoveGenerator<Cube> {
		FtlPair _slot;
		TurnSequenceMove[] _turns;
		public SlotTurnGenerator(FtlPair slot) {

			// TODO:
			// Have solver calculate subset of moves that don't violate constraints
			// Make SlotTurnGenerator work for any slot

			if( slot.Edge.InSameSpace(new Edge(Side.Front,Side.Right))==false)
				throw new System.NotImplementedException("only implemented for front right");

			_slot = slot;
			_turns = new TurnSequence[] {
				TurnSequence.Parse("U"),
				TurnSequence.Parse("U'"),
				TurnSequence.Parse("U2"),
				TurnSequence.Parse("RUR'"),
				TurnSequence.Parse("RU'R'"),
				TurnSequence.Parse("RU2R'"),
				TurnSequence.Parse("F'UF"),
				TurnSequence.Parse("F'U'F"),
				TurnSequence.Parse("F'U2F"),
			}.Select(x=>new TurnSequenceMove(x))
			.ToArray();
		}

		public IEnumerable<Move<Cube>> GetMoves(Node<Cube> node){
			if(node.Move == null)
				return _turns;

			Side previousTurnSide = ((TurnSequenceMove)node.Move)._sequence._turns[0].Side;
			return _turns
				.Where(t=>t._sequence._turns[0].Side != previousTurnSide);
		}
	}


}
