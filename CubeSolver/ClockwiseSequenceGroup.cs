using System;
using System.Collections.Generic;

namespace CubeSolver {

	public partial class Turn {

		// used by Turn to represent (sticker) Move group ofr a particular turn. (cw) and backwars(ccw)
		class ClockwiseSequenceGroup : MoveSequence {

			#region static

			static void Append4PositionSequence( MovablePosition[] pos, List<Tx> items ) {
				items.Add( new Tx{ To=pos[0].Index, From=pos[3].Index } );
				items.Add( new Tx{ To=pos[1].Index, From=pos[0].Index } );
				items.Add( new Tx{ To=pos[2].Index, From=pos[1].Index } );
				items.Add( new Tx{ To=pos[3].Index, From=pos[2].Index } );
			}

			static MovablePosition[] MakeFaceEdges( Side face, Side[] adj ) {
				MovablePosition[] faceEdges = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					faceEdges[i] = MovablePosition.Get( face, adj[i] );
				return faceEdges;
			}

			static MovablePosition[] MakeEdges( Side face, Side[] adj ) {
				MovablePosition[] edges = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					edges[i] = MovablePosition.Get( adj[i], face );
				return edges;
			}

			static MovablePosition[] MakeFaceCorners( Side face, Side[] adj ) {
				MovablePosition[] faceCorners = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					faceCorners[i] = MovablePosition.Get( face, adj[i] | adj[(i + 1) % 4] );
				return faceCorners;
			}

			static MovablePosition[] MakeRightCorners( Side face, Side[] adj ) {
				MovablePosition[] rightCorners = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					rightCorners[i] = MovablePosition.Get( adj[i], face | adj[(i + 1) % 4] );
				return rightCorners;
			}

			static MovablePosition[] MakeLeftCorners( Side face, Side[] adj ) {
				MovablePosition[] leftCorners = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					leftCorners[i] = MovablePosition.Get( adj[i], face | adj[(i + 3) % 4] );
				return leftCorners;
			}

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

			#endregion

			public ClockwiseSequenceGroup( Side face ) {

				Side[] clockwiseAdjacent = GetClockwiseAdjacent( face );

				Append4PositionSequence( MakeLeftCorners ( face, clockwiseAdjacent), _stickerMoves );
				Append4PositionSequence( MakeRightCorners( face, clockwiseAdjacent), _stickerMoves );
				Append4PositionSequence( MakeFaceCorners ( face, clockwiseAdjacent), _stickerMoves );
				Append4PositionSequence( MakeEdges       ( face, clockwiseAdjacent), _stickerMoves );
				Append4PositionSequence( MakeFaceEdges   ( face, clockwiseAdjacent), _stickerMoves );

			}

		}

	}


}
