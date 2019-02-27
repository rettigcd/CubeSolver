using System;
using System.Collections.Generic;

namespace CubeSolver {

	public partial class Turn :IHaveMoveSequence {

		#region groups

		// ##GROUP maybe goes in a group/collections with AllEdges,AllLocations,AllCorners,etc...
		static public Turn[] BuildAllTurns() {
			// Build all possible single turns
			var allPossibleTurns = new List<Turn>();
			foreach(var side in CubeGeometry.AllSides) {
				allPossibleTurns.Add( new Turn( side, Rotation.Clockwise ) );
				allPossibleTurns.Add( new Turn( side, Rotation.CounterClockwise ) );
				allPossibleTurns.Add( new Turn( side, Rotation.Twice ) );
			}
			return allPossibleTurns.ToArray();
		}

		#endregion

		#region static 

		static Dictionary<Side,StickerMoveGroup> _clockwiseTurnGroupCache;        // holds 6 CW moves
		static Dictionary<Side,StickerMoveGroup> _counterclockwiseTurnGroupCache; // holds 6 CCW moves
		static Dictionary<Side,StickerMoveGroup> _twiceTurnGroupCache; // holds 6 CCW moves

		static Turn() {
			_clockwiseTurnGroupCache = new Dictionary<Side, StickerMoveGroup>();
			_counterclockwiseTurnGroupCache = new Dictionary<Side, StickerMoveGroup>();
			_twiceTurnGroupCache = new Dictionary<Side, StickerMoveGroup>();
			foreach(var side in CubeGeometry.AllSides) {
				var sequence = new ClockwiseSequenceGroup(side);
				_clockwiseTurnGroupCache.Add(side,sequence);
				_counterclockwiseTurnGroupCache.Add(side,sequence.Reverse());
				_twiceTurnGroupCache.Add(side, StickerMoveGroup.CalculateMultiMoveSequence( new[] { sequence, sequence } ) );
			}

		}

		#endregion

		public Side Side { get; private set; }
		public Rotation Direction { get; private set; }

		#region constructor

		public Turn(Side side, Rotation direction) {
			this.Side = side;
			this.Direction = direction;
		}

		#endregion

		public StickerMoveGroup GetMoveSequence() {
			switch(Direction) {
				case Rotation.Clockwise: return _clockwiseTurnGroupCache[Side];
				case Rotation.CounterClockwise: return _counterclockwiseTurnGroupCache[Side];
				case Rotation.Twice: return _twiceTurnGroupCache[Side];
				default: throw new ArgumentException(nameof(Direction));
			}
		}

		public override string ToString() => TurnText.ToSideSymbol( Side ) + TurnText.ToDirectionString(Direction);
		static public Turn Parse(string s, int index=0) =>new Turn( TurnText.ParseSideSymbol(s[index]), index+1<s.Length ? TurnText.ParseDirection(s[index+1]) : Rotation.Clockwise );

		#region GetHashcode / equal

		public override int GetHashCode() {
			return Side.GetHashCode() *17 + Direction.GetHashCode();
		}

		public override bool Equals( object obj ) {
			Turn other = obj as Turn;
			return !Object.ReferenceEquals(other,null)
				&& Side == other.Side
				&& Direction == other.Direction;
		}

		#endregion

	}

}
