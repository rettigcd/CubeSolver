using System;
using System.Linq;

namespace CubeSolver {

	[Flags]
	public enum Side {
		Up     = 0x01,
		Down  = 0x02,
		Front   = 0x04,
		Back    = 0x08,
		Left    = 0x10,
		Right   = 0x20
	};

}
