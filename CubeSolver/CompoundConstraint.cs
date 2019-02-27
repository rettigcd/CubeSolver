using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {

	/// <summary>
	/// IsMatch if all its parts are a match.
	/// </summary>
	public class CompoundConstraint : CubeConstraint {

		#region constructors

		public CompoundConstraint() {
			_parts = new List<CubeConstraint>();
		}

		public CompoundConstraint(IEnumerable<CubeConstraint> parts) {
			_parts = parts.ToList();
		}

		public CompoundConstraint(params CubeConstraint[] parts) {
			_parts = parts.ToList();
		}

		#endregion

		public void Add( CubeConstraint constraint ) => _parts.Add(constraint);

		public void AddRange( IEnumerable<CubeConstraint> constraints ) => _parts.AddRange(constraints);

		public bool IsMatch( Cube cube ) => _parts.All(p=>p.IsMatch(cube));

		List<CubeConstraint> _parts;

	}

}
