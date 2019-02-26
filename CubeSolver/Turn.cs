using System;
using System.Collections.Generic;

namespace CubeSolver {

	public partial class Turn {

		#region static 

		static Dictionary<Side,MoveSequence> _clockwiseTurnGroupCache;        // holds 6 CW moves
		static Dictionary<Side,MoveSequence> _counterclockwiseTurnGroupCache; // holds 6 CCW moves

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

		static Side ParseSideSymbol(string s) {
			switch(s[0]) {
				case 'U': return Side.Up;
				case 'D': return Side.Down;
				case 'L': return Side.Left;
				case 'R': return Side.Right;
				case 'F': return Side.Front;
				case 'B': return Side.Back;
				default: throw new ArgumentException($"Invalid Side {s}.");
			}
		}

		static Direction ParseDirection(string s) {
			switch(s.Length) {
				case 1: return Direction.Clockwise;
				case 2: return Direction.CounterClockwise;
				default: throw new ArgumentException("This lazy parsing method assumes 1-charager strings are CW and 2-character strings are CCW");
			}
		}

		static Turn() {
			_clockwiseTurnGroupCache = new Dictionary<Side, MoveSequence>();
			_counterclockwiseTurnGroupCache = new Dictionary<Side, MoveSequence>();
			foreach(var side in CubeGeometry.AllSides) {
				var sequence = new ClockwiseSequenceGroup(side);
				_clockwiseTurnGroupCache.Add(side,sequence);
				_counterclockwiseTurnGroupCache.Add(side,sequence.Reverse());
			}
		}

		static public Turn Parse(string s) =>new Turn( ParseSideSymbol(s), ParseDirection(s) );

		#endregion

		public Side Side { get; private set; }
		public Direction Direction { get; private set; }

		#region constructor

		public Turn(Side side, Direction direction) {
			this.Side = side;
			this.Direction = direction;
		}

		#endregion

		public MoveSequence GetMoveSequence() => Direction == Direction.Clockwise 
			? _clockwiseTurnGroupCache[Side] 
			: _counterclockwiseTurnGroupCache[Side];

		public override string ToString() => Direction == Direction.Clockwise ? SideSymbol : SideSymbol + "'";

		string SideSymbol => GetSideSymbol( Side );

	}

}
