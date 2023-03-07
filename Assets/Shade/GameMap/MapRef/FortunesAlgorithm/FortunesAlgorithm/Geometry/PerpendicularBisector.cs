using System;
using System.Collections.Generic;

namespace FortunesAlgorithm
{
	public class PerpendicularBisector
	{
		Point a;
		Point b;

		public PerpendicularBisector (Point a, Point b)
		{
			this.a = a;
			this.b = b;
		}

		public Line Line() {
			return Intersection ().PerpendicularThroughPoint (Midpoint ());
		}

		public override int GetHashCode ()
		{
			return Hash.PairReversible (a, b);
		}

		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			PerpendicularBisector that = (PerpendicularBisector)obj;
			return Line ().Equals (that.Line ());
		}

		Point Midpoint() {
			return a.MidpointWith (b);
		}

		Line Intersection() {
			return new Line (a, b);
		}
	}
}

