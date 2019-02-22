using System.Collections.Generic;

namespace CubeSolver {

	public partial class Cube {
	
		// A sequence of Sticker positions that the stickers advance through (with wrap around)
		class SquarePositionSequence {

			SquarePos[] _pos;
			public SquarePositionSequence(SquarePos[] pos) { _pos = pos; }

			public void Advance( Dictionary<SquarePos,Side> _stickers ) {
				Side temp = _stickers[_pos[3]];
				for(int i=3;i>=1;--i)
					_stickers[_pos[i]] = _stickers[_pos[i-1]];
				_stickers[_pos[0]] = temp;
			}

			public void Retreat( Dictionary<SquarePos,Side> stickers ) {
				Side temp = stickers[_pos[0]];
				for(int i=1;i<4;++i)
					stickers[_pos[i-1]] = stickers[_pos[i]];
				stickers[_pos[3]] = temp;
			}

		}

	}

}
