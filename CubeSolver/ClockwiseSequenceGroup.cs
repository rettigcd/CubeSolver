using System;
using System.Collections.Generic;

namespace CubeSolver {

	public partial class Cube {

		// used by Cube to manage moving the sticket positions forward (cw) and backwars(ccw)
		class ClockwiseSequenceGroup {

			#region static

			static public Tx[] MakeFaceEdges( Side face, Side[] adj ) {
				SquarePos[] faceEdges = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					faceEdges[i] = SquarePos.Get( face, adj[i] );
				return SquarePositionSequence.BuildTx(faceEdges);
			}

			static public Tx[] MakeEdges( Side face, Side[] adj ) {
				SquarePos[] edges = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					edges[i] = SquarePos.Get( adj[i], face );
				return SquarePositionSequence.BuildTx(edges);
			}

			static public Tx[] MakeFaceCorners( Side face, Side[] adj ) {
				SquarePos[] faceCorners = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					faceCorners[i] = SquarePos.Get( face, adj[i] | adj[(i + 1) % 4] );
				return SquarePositionSequence.BuildTx(faceCorners);
			}

			static public Tx[] MakeRightCorners( Side face, Side[] adj ) {
				SquarePos[] rightCorners = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					rightCorners[i] = SquarePos.Get( adj[i], face | adj[(i + 1) % 4] );
				return SquarePositionSequence.BuildTx(rightCorners);
			}

			static public Tx[] MakeLeftCorners( Side face, Side[] adj ) {
				SquarePos[] leftCorners = new SquarePos[4];
				for( int i = 0; i < 4; ++i )
					leftCorners[i] = SquarePos.Get( adj[i], face | adj[(i + 3) % 4] );
				return SquarePositionSequence.BuildTx(leftCorners);
			}

			#endregion

			public ClockwiseSequenceGroup( Side face ) {

				Side[] clockwiseAdjacent = GetClockwiseAdjacent( face );

				_stickerMoves = new List<Tx>();
				_stickerMoves.AddRange( MakeLeftCorners ( face, clockwiseAdjacent ) );
				_stickerMoves.AddRange( MakeRightCorners( face, clockwiseAdjacent ) );
				_stickerMoves.AddRange( MakeFaceCorners ( face, clockwiseAdjacent ) );
				_stickerMoves.AddRange( MakeEdges       ( face, clockwiseAdjacent ) );
				_stickerMoves.AddRange( MakeFaceEdges   ( face, clockwiseAdjacent ) );

			}

			// contains a list of moves that have to be made to implement this Turn/move
			// Facilititates compressing multiple moves into a single 'composite' move (but I haven't written the code that calculates that yet)
			// Can use Composite moves to apply a sequence of moves and skip directly to the end without doing the intermediary steps
			// ! any Tx in a composite move where the from and to are the same, can be removed.
			List<Tx> _stickerMoves;

			static Side[] GetClockwiseAdjacent( Side face ) {
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

			public void Advance(Side[] original, Side[] stickers) {
				foreach(var tx in _stickerMoves )
					tx.Advance(original, stickers);
			}

			public void Retreat(Side[] original, Side[] stickers) {
				foreach(var tx in _stickerMoves )
					tx.Retreat(original, stickers);
			}

		}

	}

}
