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
	/// Leaving a branching factor of 9,6,6,6 and reducing the search tree depth to 1..4
	/// </summary>
	class SlotTurnGenerator : NodeMoveGenerator<Cube> {
		FtlSlot _slot;
		TurnSequenceMove[] _turns;
		public SlotTurnGenerator(FtlSlot slot) {

			// Alternative to explicitly listing 9 valid moves
			// Have solver calculate subset of moves that don't violate constraints

			Turn u0 = new Turn(Side.Up,Rotation.Clockwise);
			Turn u1 = new Turn(Side.Up,Rotation.CounterClockwise);
			Turn u2 = new Turn(Side.Up,Rotation.Twice);
			Turn leftOfSlotUp = new Turn(slot.LeftOf,Rotation.CounterClockwise);
			Turn leftOfSlotDown = new Turn(slot.LeftOf,Rotation.Clockwise);
			Turn rightOfSlotUp = new Turn(slot.RightOf,Rotation.Clockwise);
			Turn rightOfSlotDown = new Turn(slot.RightOf,Rotation.CounterClockwise);

			_slot = slot;
			_turns = new TurnSequence[] {
				new TurnSequence( u0 ),
				new TurnSequence( u1 ),
				new TurnSequence( u2 ),
				new TurnSequence( rightOfSlotUp, u0, rightOfSlotDown),
				new TurnSequence( rightOfSlotUp, u1, rightOfSlotDown),
				new TurnSequence( rightOfSlotUp, u2, rightOfSlotDown),
				new TurnSequence( leftOfSlotUp, u0, leftOfSlotDown),
				new TurnSequence( leftOfSlotUp, u1, leftOfSlotDown),
				new TurnSequence( leftOfSlotUp, u2, leftOfSlotDown),
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
