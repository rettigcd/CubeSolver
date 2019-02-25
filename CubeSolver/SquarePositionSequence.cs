
namespace CubeSolver {

	public partial class Cube {
	
		// A sequence of Sticker positions that the stickers advance through (with wrap around)
		class SquarePositionSequence {

			public Tx[] _tx;

			public SquarePositionSequence(SquarePos[] pos) {
				_tx = BuildTx( pos );
			}

			static public Tx[] BuildTx( SquarePos[] pos ) {
				return new[] {
					new Tx{ To=pos[0].Index, From=pos[3].Index },
					new Tx{ To=pos[1].Index, From=pos[0].Index },
					new Tx{ To=pos[2].Index, From=pos[1].Index },
					new Tx{ To=pos[3].Index, From=pos[2].Index },
				};
			}

			public void Advance( Side[] original, Side[] stickers ) {
				foreach(var t in _tx)
					t.Advance(original,stickers);
			}

			public void Retreat( Side[] original, Side[] stickers ) {
				foreach(var t in _tx)
					t.Retreat(original,stickers);
			}

		}

	}

	public struct Tx {
		public int From;
		public int To;
		public void Advance( Side[] src, Side[] dst ) { dst[To] = src[From]; }
		public void Retreat( Side[] src, Side[] dst ) { dst[From] = src[To]; }
	}

}
