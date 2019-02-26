using System;
using System.Linq;

namespace CubeSolver {

	static class CubeGeometry {

		static public readonly Side[] AllSides = new[] { Side.Up,Side.Front,Side.Left,Side.Down,Side.Back,Side.Right };

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

		static public Side[] GetClockwiseAdjacentFaces( Side face ) {
			switch(face) {
				case Side.Up:		return new [] {Side.Back,Side.Right,Side.Front,Side.Left };
				case Side.Front:	return new [] {Side.Up,Side.Right,Side.Down,Side.Left };
				case Side.Down:		return new [] {Side.Front,Side.Right,Side.Back,Side.Left };
				case Side.Back:		return new [] {Side.Up,Side.Left,Side.Down,Side.Right };
				case Side.Left:		return new [] {Side.Up,Side.Front,Side.Down,Side.Back };
				case Side.Right:	return new [] {Side.Up,Side.Back,Side.Down,Side.Front };
				default: throw new ArgumentException("invalid side");
			}
		}

	}


}
