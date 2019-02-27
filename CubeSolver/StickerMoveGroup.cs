using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {

	public class StickerMoveGroup {

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

		public StickerMoveGroup Twice() {
			int[] cur = new int[48];
			int[] dst = new int[48];

			for(int i=0;i<48;++i) cur[i] = i; // init

			for(int i=0;i<2;++i) {
				System.Array.Copy(cur,dst,48);
				foreach(var tx in _stickerMoves)
					dst[tx.To] = cur[tx.From];

				var tmp = cur; cur = dst; dst = tmp; // swap cur/dst
			}

			List<Tx> resultMoves = new List<Tx>();
			for(int i=0;i<48;++i) {
				if(cur[i]!=i)
					resultMoves.Add(new CubeSolver.Tx { From=cur[i], To=i });
			}
			return new StickerMoveGroup(resultMoves);
		}

		// contains a list of moves that have to be made to implement this Turn/move
		// Facilititates compressing multiple moves into a single 'composite' move (but I haven't written the code that calculates that yet)
		// Can use Composite moves to apply a sequence of moves and skip directly to the end without doing the intermediary steps
		// ! any Tx in a composite move where the from and to are the same, can be removed.
		protected List<Tx> _stickerMoves;

	}


}
