using System;
using System.Collections.Generic;

namespace FortunesAlgorithm
{
	public class Line
	{
		public float x;
		public float y;
		public float z;

		public Line(Vector3 v) {
			if (v.x == 0 && v.y == 0)
				throw new ArgumentException ("For a line defined as Vector3, x and y cannot both be 0.");
			x = v.x;
			y = v.y;
			z = v.z;
		}

		public Line (float x, float y, float z) : this(new Vector3(x, y, z)) {
		}

		public Line(Point a, Point b) {
			Line result = a.LineWith (b);
			x = result.x;
			y = result.y;
			z = result.z;
		}

		public Line PerpendicularThroughPoint(Point that) {
			return new Line (ParallelThroughOrigin().Vector ().CrossProduct (that.Vector()));
		}

		public Point Intersect(Line that) {
			return new Point (this.Vector ().CrossProduct (that.Vector ()));
		}

		Line ParallelThroughOrigin() {
			return new Line (x, y, 0);
		}

		public Vector3 Vector() {
			return new Vector3(x, y, z);
		}

		public override int GetHashCode ()
		{
			if (x != 0) {
				return Hash.Pair (y / x, z / x);
			}
			return Hash.Pair (x / y, z / y);
		}

		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			Line that = (Line)obj;
			return (this.Vector ().Colinear (that.Vector ()));
		}

		public override string ToString ()
		{
			return string.Format ("Line({0})", Vector());
		}
	}
}

