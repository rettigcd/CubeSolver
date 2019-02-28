using System.Collections.Generic;
using System.Linq;

namespace CubeSolver {
	/// <summary>
	/// IsMatch if all its parts are a match.
	/// </summary>
	public class OptionalConstraint : CubeConstraint {

		#region constructors

		public OptionalConstraint() {
			_parts = new List<CubeConstraint>();
		}

		public OptionalConstraint(IEnumerable<CubeConstraint> parts) {
			_parts = parts.ToList();
		}

		public OptionalConstraint(params CubeConstraint[] parts) {
			_parts = parts.ToList();
		}

		#endregion

		public void Add( CubeConstraint constraint ) => _parts.Add(constraint);

		public void AddRange( IEnumerable<CubeConstraint> constraints ) => _parts.AddRange(constraints);

		public bool IsMatch( Cube cube ) {
			if( _parts.Count == 0) throw new System.InvalidOperationException("no part");
			return _parts.Any(p=>p.IsMatch(cube));
		}

		List<CubeConstraint> _parts;

	}


}
