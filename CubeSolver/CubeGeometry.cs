using System;
using System.Collections.Generic;

namespace CubeSolver {

	static class CubeGeometry {

		// these are in a special order of -x,x,-y,y,-z,z
		static public readonly Side[] AllSides = new[] { Side.Left, Side.Right, Side.Front, Side.Back, Side.Down, Side.Up, };

		static public Side OppositeSideOf( Side side ) {
			int index = Array.IndexOf(AllSides,side);
			return AllSides[index ^ 1];
		}

		static public Side[] GetClockwiseAdjacentFaces( Side face ) {
			// math to do generic cross product is too hard
			// just hard code them
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

		static Edge[] _allEdgePositions = null;
		static public Edge[] AllEdgePositions {
			get {
				if( _allEdgePositions == null ) {
					List<Edge> edges = new List<Edge>();
					foreach( var face in CubeGeometry.AllSides )
						foreach( var adj in CubeGeometry.GetClockwiseAdjacentFaces( face ) )
							edges.Add( new Edge( face, adj ) );
					_allEdgePositions = edges.ToArray();
				}
				return _allEdgePositions;
			}
		}

		static Corner[] _allCornerPositions = null;
		static public Corner[] AllCornerPositions {
			get {
				if( _allCornerPositions == null ) {
					List<Corner> corners = new List<Corner>();
					foreach( var face in CubeGeometry.AllSides ) {
						Side[] adjs = CubeGeometry.GetClockwiseAdjacentFaces( face );
						corners.Add(new Corner(face,adjs[0],adjs[1]));
						corners.Add(new Corner(face,adjs[1],adjs[2]));
						corners.Add(new Corner(face,adjs[2],adjs[3]));
						corners.Add(new Corner(face,adjs[3],adjs[0]));
					}
					_allCornerPositions = corners.ToArray();
				}
				return _allCornerPositions;
			}
		}

	}


}
