
namespace CubeSolver {

	public partial class Turn {

		// used by Turn to represent (sticker) Move group ofr a particular turn. (cw) and backwars(ccw)
		class ClockwiseSequenceGroup : MoveSequence {

			public ClockwiseSequenceGroup( Side face ) {

				_face = face;
				_clockwiseAdjacent = CubeGeometry.GetClockwiseAdjacentFaces( face );

				Append4PositionSequence( MakeLeftCorners() );
				Append4PositionSequence( MakeRightCorners() );
				Append4PositionSequence( MakeFaceCorners() );
				Append4PositionSequence( MakeEdges() );
				Append4PositionSequence( MakeFaceEdges() );

			}

			#region private methods

			void Append4PositionSequence( MovablePosition[] pos ) {
				_stickerMoves.Add( new Tx{ To=pos[0].Index, From=pos[3].Index } );
				_stickerMoves.Add( new Tx{ To=pos[1].Index, From=pos[0].Index } );
				_stickerMoves.Add( new Tx{ To=pos[2].Index, From=pos[1].Index } );
				_stickerMoves.Add( new Tx{ To=pos[3].Index, From=pos[2].Index } );
			}

			MovablePosition[] MakeFaceEdges() {
				MovablePosition[] faceEdges = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					faceEdges[i] = MovablePosition.Get( _face, _clockwiseAdjacent[i] );
				return faceEdges;
			}

			MovablePosition[] MakeEdges() {
				MovablePosition[] edges = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					edges[i] = MovablePosition.Get( _clockwiseAdjacent[i], _face );
				return edges;
			}

			MovablePosition[] MakeFaceCorners() {
				MovablePosition[] faceCorners = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					faceCorners[i] = MovablePosition.Get( _face, _clockwiseAdjacent[i] | _clockwiseAdjacent[(i + 1) % 4] );
				return faceCorners;
			}

			MovablePosition[] MakeRightCorners() {
				MovablePosition[] rightCorners = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					rightCorners[i] = MovablePosition.Get( _clockwiseAdjacent[i], _face | _clockwiseAdjacent[(i + 1) % 4] );
				return rightCorners;
			}

			MovablePosition[] MakeLeftCorners() {
				MovablePosition[] leftCorners = new MovablePosition[4];
				for( int i = 0; i < 4; ++i )
					leftCorners[i] = MovablePosition.Get( _clockwiseAdjacent[i], _face | _clockwiseAdjacent[(i + 3) % 4] );
				return leftCorners;
			}

			#endregion

			#region private fields

			Side _face;
			Side[] _clockwiseAdjacent;

			#endregion

		}

	}


}
