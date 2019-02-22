using System;
using System.Linq;

namespace CubeSolver {

	[Flags]
	public enum Side {
		Top     = 0x01,
		Bottom  = 0x02,
		Front   = 0x04,
		Back    = 0x08,
		Left    = 0x10,
		Right   = 0x20
	};

}
