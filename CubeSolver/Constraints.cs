using System.Linq;

namespace CubeSolver {
	static class Constraints {

		#region constraints

		static public CubeConstraint DontMoveOtherSlotConstraint( FtlPair movingSlot) {

			var allSlots = new FtlPair[]{
				new FtlPair(Side.Right, Side.Back),
				new FtlPair(Side.Back, Side.Left),
				new FtlPair(Side.Left, Side.Front),
				new FtlPair(Side.Left, Side.Front)
			};
			
			var slotsToNotMove = allSlots
				.Where(x=>!x.Edge.InSameSpace(movingSlot.Edge));

			return new CompoundConstraint( slotsToNotMove.Select(x=>x.Stationary) );

		}

		static public CompoundConstraint MovePairDirectlyToSlot(FtlPair pair, Cube cube) {
			return new CompoundConstraint(
				Solver.FindEdgeAndSolveIt( cube, pair.Edge ),
				Solver.FindCornerAndSolveIt( cube, pair.Corner )
			);
		}

		static public Edge[] CrossEdges = new[] {
			new Edge( Side.Front, Side.Down ),
			new Edge( Side.Right, Side.Down ),
			new Edge( Side.Back, Side.Down ),
			new Edge( Side.Left, Side.Down ),
		};

		static readonly public CompoundConstraint CrossConstraint =
			new CompoundConstraint( CrossEdges.Select(x=>EdgeConstraint.Stationary(x)) );

		static public void VerifyConstraint( Cube cube, CubeConstraint constraint, string msg ) {
			if( !constraint.IsMatch( cube ) ) throw new System.InvalidOperationException( msg );
		}

		#endregion

	}

}
