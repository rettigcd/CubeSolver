using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {

	public class StickerMoveGroup {

		public static StickerMoveGroup CalculateMultiMoveSequence( IEnumerable<StickerMoveGroup> moves ) {
			// allocate space to track changes
			int[] cur = new int[48];
			int[] dst = new int[48]; // holds values while in transit

			for( int i = 0; i < 48; ++i ) cur[i] = i; // init

			foreach( var move in moves ) {
				// apply move to curr
				System.Array.Copy( cur, dst, 48 ); // for moves
				foreach( var tx in move._stickerMoves )
					dst[tx.To] = cur[tx.From];
				var tmp = dst; dst = cur; cur = tmp;
			}

			var stickerMoves = Enumerable.Range(0,48)
				.Where(i=>cur[i]!=i) // stickers moved
				.Select(i=>new Tx { From = cur[i], To = i } );
			return new StickerMoveGroup( stickerMoves );

		}

		#region constructors

		public StickerMoveGroup(IEnumerable<Tx> tx) {
			_stickerMoves = tx.ToList();
		}

		protected StickerMoveGroup() {
			_stickerMoves = new List<Tx>();
		}

		#endregion

		public void Advance(Side[] original, Side[] stickers) {
			foreach(var tx in _stickerMoves )
				tx.Advance(original, stickers);
		}

		public StickerMoveGroup Reverse() => new StickerMoveGroup(_stickerMoves.Select(x=>x.Reverse()).ToList() );

		// contains a list of moves that have to be made to implement this Turn/move
		// Facilititates compressing multiple moves into a single 'composite' move (but I haven't written the code that calculates that yet)
		// Can use Composite moves to apply a sequence of moves and skip directly to the end without doing the intermediary steps
		// ! any Tx in a composite move where the from and to are the same, can be removed.
		protected List<Tx> _stickerMoves;

	}


}
