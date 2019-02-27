using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {

	public class MovablePosition : IEquatable<MovablePosition> {

		// there are 48 moveable positions, no more, no less  (centers don't move)
		static Dictionary<int,MovablePosition> _positionLookupByFaceCoordHash = new Dictionary<int, MovablePosition>();

		public static bool IsCenter(MovablePosition pos) => pos._coord == (Side)0;
		public static bool IsEdge(MovablePosition pos) => CubeGeometry.AllSides.Contains( pos._coord );
		public static bool IsCorner(MovablePosition pos) => !IsCenter(pos) && !IsEdge(pos);

		static public MovablePosition[] GetMovablePositionsForSide( Side side ) {

			Side[] adjacents = CubeGeometry.GetClockwiseAdjacentFaces( side );

			return new[] {
				MovablePosition.Get(side,adjacents[0]),
				MovablePosition.Get(side,adjacents[1]),
				MovablePosition.Get(side,adjacents[2]),
				MovablePosition.Get(side,adjacents[3]),
				MovablePosition.Get(side,adjacents[0]|adjacents[1]),
				MovablePosition.Get(side,adjacents[1]|adjacents[2]),
				MovablePosition.Get(side,adjacents[2]|adjacents[3]),
				MovablePosition.Get(side,adjacents[3]|adjacents[0]),
			};
		}

		static public MovablePosition Get( Side face, Side coord ) {

			if( coord == default(Side))
				throw new Exception("center!");

			int hash = CalcHash(face,coord);

			lock(locker){ // make this part thread safe so unit tests can run in parrallel

				// Find Existing
				if( _positionLookupByFaceCoordHash.ContainsKey(hash) ) return _positionLookupByFaceCoordHash[hash];

				// Add new
				if( _positionLookupByFaceCoordHash.Count >= 48 ) throw new Exception("Houston we have a problem. We should never try to add more than 48 positions.");
				var pos = new MovablePosition( face, coord, hash, _positionLookupByFaceCoordHash.Count);
				_positionLookupByFaceCoordHash.Add(hash,pos);
				return pos;
			}
		}

		static object locker = new object(); 

		MovablePosition(Side face, Side coord, int hash, int index) {
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

		public override bool Equals( object obj ) => Equals( obj as MovablePosition );

		public bool Equals( MovablePosition other ) {
			return !Object.ReferenceEquals(other,null) 
				&& _coord == other._coord
				&& Face == other.Face;
		}

		#endregion
	}

}
