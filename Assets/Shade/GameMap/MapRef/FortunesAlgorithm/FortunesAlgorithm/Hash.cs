using System;

namespace FortunesAlgorithm
{
    internal static class Hash
	{
		public static int Pair(object a, object b) {
			unchecked {
				int ah = a.GetHashCode ();
				int bh = b.GetHashCode ();
				return ((ah << 5) + ah) ^ bh;
			}
		}

		public static int PairReversible(object a, object b) {
			unchecked {
				return a.GetHashCode () + b.GetHashCode ();
			}
		}

		public static int Triple(object a, object b, object c) {
			return Pair (Pair (a, b), c);
		}

		public static int TripleSet(object a, object b, object c) {
			unchecked {
				return a.GetHashCode () + b.GetHashCode () + c.GetHashCode ();
			}
		}
	}
}

