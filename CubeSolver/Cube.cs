using System;
using System.Linq;

namespace CubeSolver {

	/// <summary>
	/// Tracks the location of all48 movable sticker positions/colors.
	/// </summary>
	public class Cube : IEquatable<Cube> {

		#region static 

		static readonly Side[] _solvedStickers;

		static Cube() {

			_solvedStickers = new Side[48];
			foreach(var face in CubeGeometry.AllSides)
				foreach(var pos in MovablePosition.GetMovablePositionsForSide(face))
					_solvedStickers[pos.Index] = face;

		}

		#endregion

		#region constructor

		public Cube() {
			_stickers = new Side[48];
			Array.Copy(_solvedStickers,_stickers,48);
		}

		/// <remarks>private because I like the .Clone() api butter than a copy constructor</remarks>
		Cube( Cube src ) {
			_stickers = new Side[48];
			Array.Copy( src._stickers, _stickers, 48 );
		}

		#endregion

		public Side this[MovablePosition pos] {
			get { return _stickers[pos.Index]; }
			set { _stickers[pos.Index] = value; }
		}

		public Cube Apply( IHaveMoveSequence turn ) => ApplyMoveSequence( turn.GetMoveSequence() );

		public Cube ApplyMoveSequence( StickerMoveGroup moveSequence ) {
			var child = Clone();
			moveSequence.Advance( _stickers, child._stickers );
			return child;
		}

		public bool IsSolved => _stickers.SequenceEqual( _solvedStickers );

		public Cube Clone() => new Cube(this);

		#region override Equals

		// not overriding GetHashCode because I don't think we will use this as a dictionary key

		public bool Equals( Cube other ) {
			return !Object.ReferenceEquals(other,null)
				&& _stickers.SequenceEqual(other._stickers);
		}

		public override bool Equals( object obj ) {
			return base.Equals( obj as Cube );
		}

		#endregion

		#region private

		Side[] _stickers;

		#endregion

	}


}
