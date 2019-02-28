using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {

	public class TurnSequence : IHaveMoveSequence {

		static public TurnSequence Parse(string s ) {
			var items = new List<Turn>();
			for(int i = 0; i<s.Length; ++i) {
				var turn = Turn.Parse(s,i);
				items.Add( turn );
				if( turn.Direction != Rotation.Clockwise ) ++i;
			}
			return new TurnSequence( items.ToArray() );
		}

		public TurnSequence(Turn[] turns ) {
			_turns = CleanUpSequence( turns );
		}

		public TurnSequence Reverse() => new TurnSequence(_turns.Reverse().Select(x=>x.Reverse()).ToArray());

		public StickerMoveGroup GetMoveSequence() {
			return StickerMoveGroup.CalculateMultiMoveSequence( _turns.Select(turn=>turn.GetMoveSequence() ) );
		}

		public override string ToString() => string.Join("",(IEnumerable<Turn>)_turns);

		#region private methods

		static Turn[] CleanUpSequence( IEnumerable<Turn> src ) {
			List<Turn> orig = src.ToList();
			var result = CleanUpSequenceOnePass(orig);
			// make multiple passes so that void created by removing moves
			// allows to moves on either side of the void to join together and have themselves possibly annihilate
			while(result.Count != orig.Count) {
				orig = result;
				result = CleanUpSequenceOnePass(orig);
			}
			return result.ToArray();
		}

		static List<Turn> CleanUpSequenceOnePass( List<Turn> src ) {
			var result = new List<Turn>();

			Side side = (Side)0;
			Rotation direction = Rotation.None;

			foreach(var item in src) {
				if(item.Side!=side) {
					// push current side
					if(direction!=Rotation.None) result.Add(new Turn(side,direction));

					// init new current
					side = item.Side;
					direction = item.Direction;
				} else {
					// side repeat
					direction = direction.Add( item.Direction );
				}
			}
			if(direction!=Rotation.None) result.Add(new Turn(side,direction));

			return result;
		}

		#endregion

		#region private fields

		public Turn[] _turns;

		#endregion

	}

}
