
namespace CubeSolver {

	// Advances a ticker from From to To.
	public struct Tx {
		public int From;
		public int To;
		public void Advance( Side[] src, Side[] dst ) { dst[To] = src[From]; }
		public Tx Reverse() => new Tx { From = To, To = From};
	}

}
