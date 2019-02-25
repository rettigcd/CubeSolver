using System;
using System.Collections.Generic;
using System.Linq;
using AiSearch.OneSide;
using Xunit;

namespace CubeSolver {

	public interface CubeMatch {
		bool IsMatch( Cube cube );
	}

}
