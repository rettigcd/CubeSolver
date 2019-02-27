using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {

	public class TurnSequence {

		public TurnSequence(Turn[] turns ) {
			var items = turns.ToList();
			int i=0;
			while(i<items.Count-1) {
				while( CancelEachOtherOut( items[i], items[i+1] )) {
					items.RemoveAt(i);
					items.RemoveAt(i);
					if(i>0) --i;
				}
				++i;
			}
			_turns = items.ToArray();

		}
		static bool CancelEachOtherOut( Turn t1, Turn t2 ) {
			return t1.Side == t2.Side 
				&& DirectionsCancel(t1.Direction,t2.Direction);
		}
		static bool DirectionsCancel(Direction d1,Direction d2 ) {
			switch(d1) {
				case Direction.Clockwise: return d2 == Direction.CounterClockwise;
				case Direction.CounterClockwise: return d2 == Direction.Clockwise;
				default: return false;
			}
		}

		static public TurnSequence Parse(string s ) {
			var items = new List<Turn>();
			for(int i = 0; i<s.Length; ++i) {
				var turn = Turn.Parse(s,i);
				items.Add( turn );
				if( turn.Direction != Direction.Clockwise ) ++i;
			}
			return new TurnSequence( items.ToArray() );
		}

		public Cube TurnCube( Cube cube ) {
			foreach(var turn in _turns)
				cube = cube.ApplyTurn(turn);
			return cube;
		}

		public override string ToString() => string.Join("",(IEnumerable<Turn>)_turns);

		public Turn[] _turns;
	}

}
