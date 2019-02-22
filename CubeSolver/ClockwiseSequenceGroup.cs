using System;
using System.Collections.Generic;

namespace CubeSolver {

	public partial class Cube {

		// used by Cube to manage moving the sticket positions forward (cw) and backwars(ccw)
		class ClockwiseSequenceGroup {

			#region static

			static public SquarePositionSequence MakeFaceEdges( Side face, Side[] adj ) {
				SquarePos[] faceEdges = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					faceEdges[i] = SquarePos.Get( face, adj[i] );
				return new SquarePositionSequence(faceEdges);
			}

			static public SquarePositionSequence MakeEdges( Side face, Side[] adj ) {
				SquarePos[] edges = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					edges[i] = SquarePos.Get( adj[i], face );
				return new SquarePositionSequence(edges);
			}

			static public SquarePositionSequence MakeFaceCorners( Side face, Side[] adj ) {
				SquarePos[] faceCorners = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					faceCorners[i] = SquarePos.Get( face, adj[i] | adj[(i + 1) % 4] );
				return new SquarePositionSequence(faceCorners);
			}

			static public SquarePositionSequence MakeRightCorners( Side face, Side[] adj ) {
				SquarePos[] rightCorners = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					rightCorners[i] = SquarePos.Get( adj[i], face | adj[(i + 1) % 4] );
				return new SquarePositionSequence(rightCorners);
			}

			static public SquarePositionSequence MakeLeftCorners( Side face, Side[] adj ) {
				SquarePos[] leftCorners = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					leftCorners[i] = SquarePos.Get( adj[i], face | adj[(i + 3) % 4] );
				return new SquarePositionSequence(leftCorners);
			}

			#endregion

			public ClockwiseSequenceGroup( Side face ) {

				Side[] clockwiseAdjacent = GetClockwiseAdjacent( face );

				this._sequences = new [] {
					MakeLeftCorners( face, clockwiseAdjacent ),
					MakeRightCorners( face, clockwiseAdjacent ),
					MakeFaceCorners( face, clockwiseAdjacent ),
					MakeEdges( face, clockwiseAdjacent ),
					MakeFaceEdges( face, clockwiseAdjacent ),
				};
			}

			static Side[] GetClockwiseAdjacent( Side face ) {
				switch(face) {
					case Side.Top:		return new [] {Side.Back,Side.Right,Side.Front,Side.Left };
					case Side.Front:	return new [] {Side.Top,Side.Right,Side.Bottom,Side.Left };
					case Side.Bottom:	return new [] {Side.Front,Side.Right,Side.Back,Side.Left };
					case Side.Back:		return new [] {Side.Top,Side.Left,Side.Bottom,Side.Right };
					case Side.Left:		return new [] {Side.Top,Side.Front,Side.Bottom,Side.Back };
					case Side.Right:	return new [] {Side.Top,Side.Back,Side.Bottom,Side.Front };
					default: throw new ArgumentException("invalid side");
				}
			}

			public void Advance(Dictionary<SquarePos,Side> stickers) {
				foreach(var sequence in _sequences)
					sequence.Advance(stickers);
			}

			public void Retreat(Dictionary<SquarePos,Side> stickers) {
				foreach(var sequence in _sequences)
					sequence.Retreat(stickers);
			}

			SquarePositionSequence[] _sequences;

		}

	}

}
