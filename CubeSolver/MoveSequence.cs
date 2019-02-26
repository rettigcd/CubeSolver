using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {

	public class MoveSequence {

		#region constructors

		public MoveSequence(IEnumerable<Tx> tx) {
			_stickerMoves = tx.ToList();
		}

		protected MoveSequence() {
			_stickerMoves = new List<Tx>();
		}

		#endregion

		public void Advance(Side[] original, Side[] stickers) {
			foreach(var tx in _stickerMoves )
				tx.Advance(original, stickers);
		}

		public MoveSequence Reverse() => new MoveSequence(_stickerMoves.Select(x=>x.Reverse()).ToList() );

		// contains a list of moves that have to be made to implement this Turn/move
		// Facilititates compressing multiple moves into a single 'composite' move (but I haven't written the code that calculates that yet)
		// Can use Composite moves to apply a sequence of moves and skip directly to the end without doing the intermediary steps
		// ! any Tx in a composite move where the from and to are the same, can be removed.
		protected List<Tx> _stickerMoves;

	}


}
