using System.Linq;

namespace CubeSolver {

	static class Constraints {

		#region constraints

		static public readonly FtlSlot[] AllFtlSlots = new FtlSlot[]{
			new FtlSlot(Side.Right, Side.Back),
			new FtlSlot(Side.Back, Side.Left),
			new FtlSlot(Side.Left, Side.Front),
			new FtlSlot(Side.Left, Side.Front)
		};

		static public Edge[] CrossEdges = new[] {
			new Edge( Side.Front, Side.Down ),
			new Edge( Side.Right, Side.Down ),
			new Edge( Side.Back, Side.Down ),
			new Edge( Side.Left, Side.Down ),
		};

		static public CubeConstraint OtherSlotsConstraint( FtlSlot movingSlot) {
			
			var slotsToNotMove = AllFtlSlots
				.Where(x=>!x.Home.Edge.InSameSpace(movingSlot.Home.Edge));

			return new CompoundConstraint( slotsToNotMove.Select(x=>x.Home.Stationary) );

		}

		static readonly public CompoundConstraint CrossConstraint =
			new CompoundConstraint( CrossEdges.Select(x=>EdgeConstraint.Stationary(x)) );

		static public void VerifyConstraint( Cube cube, CubeConstraint constraint, string msg ) {
			if( !constraint.IsMatch( cube ) ) throw new System.InvalidOperationException( msg );
		}

		#endregion

	}

}
