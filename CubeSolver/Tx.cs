
namespace CubeSolver {

	// Advances a sticker from From to To.
	public struct Tx {
		public int From;
		public int To;
		public void Advance( Side[] stickerSource, Side[] stickerDestination ) { stickerDestination[To] = stickerSource[From]; }
		public Tx Reverse() => new Tx { From = To, To = From};
		public override string ToString() => $"{From}=>{To}";
	}

}
