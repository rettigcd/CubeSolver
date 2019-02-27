using System;

namespace CubeSolver {

	public partial class Turn {

		static class TurnText {
			static public string ToSideSymbol(Side side) {
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

			static public Side ParseSideSymbol(char k) {
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

			static public Rotation ParseDirection(char k) {
				switch(k) {
					case '\'': return Rotation.CounterClockwise;
					case '2': return Rotation.Twice;
					default: return Rotation.Clockwise; // in case the next character in a series is passed in.
				}
			}

			static public string ToDirectionString(Rotation d) {
				switch(d) {
					case Rotation.Clockwise: return string.Empty;
					case Rotation.CounterClockwise: return "'";
					case Rotation.Twice: return "2";
					default: throw new ArgumentException(nameof(d));
				}
			}

		}
	}
}
