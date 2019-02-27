using System;
using System.Collections.Generic;

namespace CubeSolver {

	public partial class Turn {

		#region static 

		static readonly public Turn[] AllPossibleMoves;

		static Dictionary<Side,StickerMoveGroup> _clockwiseTurnGroupCache;        // holds 6 CW moves
		static Dictionary<Side,StickerMoveGroup> _counterclockwiseTurnGroupCache; // holds 6 CCW moves

		static Turn[] BuildAllTurns() {
			// Build all possible single turns
			var allPossibleTurns = new List<Turn>();
			foreach(var side in CubeGeometry.AllSides) {
				allPossibleTurns.Add( new Turn( side, Direction.Clockwise ) );
				allPossibleTurns.Add( new Turn( side, Direction.CounterClockwise ) );
			}
			return allPossibleTurns.ToArray();
		}

		static string GetSideSymbol(Side side) {
			switch(side) {
				case Side.Up:    return "U";
				case Side.Down:  return "D";
				case Side.Left:  return "L";
				case Side.Right: return "R";
				case Side.Front: return "F";
				case Side.Back:  return "B";
				default: throw new ArgumentException($"Invalid Side {side}.");
			}
		}

		static Side ParseSideSymbol(char k) {
			switch(k) {
				case 'U': return Side.Up;
				case 'D': return Side.Down;
				case 'L': return Side.Left;
				case 'R': return Side.Right;
				case 'F': return Side.Front;
				case 'B': return Side.Back;
				default: throw new ArgumentException($"Invalid Side {k}.");
			}
		}

		static Direction ParseDirection(char k) {
			switch(k) {
				case '\'': return Direction.CounterClockwise;
				//case '2': return Direction.;
				default: return Direction.Clockwise; // in case the next character in a series is passed in.
			}
		}

		static public Turn Parse(string s, int index=0) =>new Turn( 
			ParseSideSymbol(s[index]), 
			index+1<s.Length ? ParseDirection(s[index+1]) : Direction.Clockwise
		);

		static Turn() {
			_clockwiseTurnGroupCache = new Dictionary<Side, StickerMoveGroup>();
			_counterclockwiseTurnGroupCache = new Dictionary<Side, StickerMoveGroup>();
			foreach(var side in CubeGeometry.AllSides) {
				var sequence = new ClockwiseSequenceGroup(side);
				_clockwiseTurnGroupCache.Add(side,sequence);
				_counterclockwiseTurnGroupCache.Add(side,sequence.Reverse());
			}

			AllPossibleMoves = BuildAllTurns();
		}

		#endregion

		public Side Side { get; private set; }
		public Direction Direction { get; private set; }

		#region constructor

		public Turn(Side side, Direction direction) {
			this.Side = side;
			this.Direction = direction;
		}

		#endregion

		public StickerMoveGroup GetMoveSequence() => Direction == Direction.Clockwise 
			? _clockwiseTurnGroupCache[Side] 
			: _counterclockwiseTurnGroupCache[Side];

		public override string ToString() => Direction == Direction.Clockwise ? SideSymbol : SideSymbol + "'";

		string SideSymbol => GetSideSymbol( Side );

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
