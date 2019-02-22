using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {

	public class SquarePos : IEquatable<SquarePos> {

		// only have 54 positions, no more, no less
		static Dictionary<int,SquarePos> _indexLookup = new Dictionary<int, SquarePos>();

		public static bool IsCenter(SquarePos pos) => pos._coord == (Side)0;
		public static bool IsEdge(SquarePos pos) => Cube.AllSides.Contains( pos._coord );
		public static bool IsCorner(SquarePos pos) => !IsCenter(pos) && !IsEdge(pos);

		static public SquarePos Get( Side face, Side coord ) {

			int hash = CalcHash(face,coord);
			if( _indexLookup.ContainsKey(hash) ) return _indexLookup[hash];

			var pos = new SquarePos( face, coord, hash, _indexLookup.Count);
			_indexLookup.Add(hash,pos);
			return pos;
		}

		SquarePos(Side face, Side coord, int hash, int index) {
			Face =face;
			_coord = coord;
			_hash = hash;
			Index = index;
		}
		public int Index { get; private set; }
		int _hash;

		public Side Face { get; private set; }

		/// <summary>
		/// lenth = 0 for center, 
		/// length = 1 for edge, 
		/// length = 2 for corner
		/// </summary>
		Side _coord;

		#region Hash / Equals

		public override string ToString() => Face+":["+string.Join(",",_coord)+"]";

		public override int GetHashCode() => _hash;

		static int CalcHash(Side face, Side coord) =>face.GetHashCode() * 947 + coord.GetHashCode();

		public override bool Equals( object obj ) => Equals( obj as SquarePos );

		public bool Equals( SquarePos other ) {
			return !Object.ReferenceEquals(other,null) 
				&& _coord == other._coord
				&& Face == other.Face;
		}

		#endregion
	}

}
