
namespace CubeSolver {

	/// <summary>
	/// base predicate for determining if the Cube meets an individual constraint
	/// Generally, an EdgeConstraint or a CornerConstraint
	/// </summary>
	public interface CubeConstraint {
		bool IsMatch( Cube cube );
	}

}
