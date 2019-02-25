using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {

	public partial class Cube : IEquatable<Cube> {

		#region static 

		static Dictionary<Side,ClockwiseSequenceGroup> _clockwiseTurnGroups;

		static readonly Side[] _solvedStickers;

		static public readonly Side[] AllSides = new[] { Side.Up,Side.Front,Side.Left,Side.Down,Side.Back,Side.Right };

		static Cube() {

			// Init Solved
			_solvedStickers = new Side[54];
			foreach(var face in AllSides)
				foreach(var pos in Cube.GetSquarePositionsForSide(face))
					_solvedStickers[pos.Index] = face;

			// Init Turn-Group mappings
			_clockwiseTurnGroups = new Dictionary<Side, ClockwiseSequenceGroup>();
			foreach(var side in AllSides)
				_clockwiseTurnGroups.Add(side,new ClockwiseSequenceGroup(side));
		}

		static public SquarePos[] GetSquarePositionsForSide( Side side ) {

			Side[] adjacents = AdjacentSidesOf( side );

			return new[] {
				SquarePos.Get(side,(Side)0),
				SquarePos.Get(side,adjacents[0]),
				SquarePos.Get(side,adjacents[1]),
				SquarePos.Get(side,adjacents[2]),
				SquarePos.Get(side,adjacents[3]),
				SquarePos.Get(side,adjacents[0]|adjacents[1]),
				SquarePos.Get(side,adjacents[1]|adjacents[2]),
				SquarePos.Get(side,adjacents[2]|adjacents[3]),
				SquarePos.Get(side,adjacents[3]|adjacents[0]),
			};
		}

		static public Side[] AdjacentSidesOf( Side side ) {
			Side opposite = OppositeSideOf( side );
			return AllSides
				.Where(s => s!=side && s!=opposite )
				.ToArray();
		}

		static public Side OppositeSideOf( Side side ) {
			switch(side) {
				case Side.Back: return Side.Front;
				case Side.Front: return Side.Back;
				case Side.Left: return Side.Right;
				case Side.Right: return Side.Left;
				case Side.Up: return Side.Down;
				case Side.Down: return Side.Up;
				default: throw new ArgumentException($"Invalid value for {nameof(side)}:{side}");
			}
		}

		#endregion

		public Cube() {
			_stickers = new Side[54];
			Array.Copy(_solvedStickers,_stickers,54);
		}

		/// <remarks>private because I like the .Clone() api butter than a copy constructor</remarks>
		Cube( Cube src ) {
			_stickers = new Side[54];
			Array.Copy( src._stickers, _stickers, 54 );
		}

		public Side this[SquarePos pos] {
			get { return _stickers[pos.Index]; }
			set { _stickers[pos.Index] = value; }
		}

		public Cube ApplyTurn( Turn turn ) {
			var child = Clone();

			var grp = _clockwiseTurnGroups[turn.Side];
			if(turn.Direction == Direction.Clockwise)
				grp.Advance( _stickers, child._stickers );
			else 
				grp.Retreat( _stickers, child._stickers );

			return child;
		}

		public bool IsSolved => _stickers.SequenceEqual(_solvedStickers);

		public Cube Clone() => new Cube(this);

		#region override 

		// not overriding GetHashCode because I don't think we will use this as a dictionary key

		public bool Equals( Cube other ) {
			return !Object.ReferenceEquals(other,null)
				&& _stickers.SequenceEqual(other._stickers);
		}

		public override bool Equals( object obj ) {
			return base.Equals( obj as Cube );
		}

		#endregion

		Side[] _stickers;

	}


}
