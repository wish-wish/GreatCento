using System;
using System.Collections.Generic;

namespace FortunesAlgorithm
{
    internal class BeachSection : IComparable
	{
		public Point focus;
		public Point leftBoundary;
		public Point rightBoundary;

		public BeachSection (Point focus, Point leftBoundary, Point rightBoundary)
		{
			this.focus = focus;
			this.leftBoundary = leftBoundary;
			this.rightBoundary = rightBoundary;
		}

		public static BeachSection FirstSection(Point focus) {
			return new BeachSection(focus, null, null);
		}
		public static BeachSection LeftSection(Point focus, Point rightBoundary) {
			return new BeachSection (focus, null, rightBoundary);
		}
		public static BeachSection RightSection(Point focus, Point leftBoundary) {
			return new BeachSection (focus, leftBoundary, null);
		}

		public bool IsLeftmost() {
			return leftBoundary == null;
		}

		public bool IsRightmost() {
			return rightBoundary == null;
		}

		public bool IsFullBeachLine() {
			return IsLeftmost () && IsRightmost ();
		}

		public int CompareTo(Object obj) {
			if (obj is BeachSection) {
				BeachSection that = (BeachSection)obj;
				return CompareTo (that);
			}
			throw new ArgumentException (String.Format ("Can't compare objects of type BeachSection and {0}", obj.GetType()));
		}

		public override bool Equals(Object obj) {
			if (obj == null || GetType () != obj.GetType ())
				return false;

			BeachSection that = (BeachSection)obj;
			return this.focus == that.focus && this.leftBoundary == that.leftBoundary && this.rightBoundary == that.rightBoundary;
		}

		public override int GetHashCode() {
			return Hash.Triple (leftBoundary, rightBoundary, focus);
		}

		public override string ToString ()
		{
			return String.Format("BS({0}, {1}, {2})", leftBoundary, focus, rightBoundary);
		}

		public int CompareTo(Point site) {
			if (PointIsLeftOfLeftEdge (site))
				return 1;
			if (PointIsRightOfRightEdge (site))
				return -1;
			return 0;
		}

		bool PointIsLeftOfLeftEdge(Point site) {
			if (IsLeftmost())
				return false;
			if (focus.y == leftBoundary.y )
				return site.x < (focus.x + leftBoundary.x ) / 2;
			Line directrix = new Line (0, 1, -site.y );
			float xIntercept = focus.LineWith (leftBoundary).Intersect (directrix).x ;
			if (leftBoundary.y > focus.y ) {
				if (site.x >= xIntercept)
					return false;
				return Geometry.CircleCentre (site, focus, leftBoundary).x > site.x ;
			}
			if (site.x <= xIntercept)
				return true;
			return Geometry.CircleCentre (site, focus, leftBoundary).x > site.x ;
		}

		bool PointIsRightOfRightEdge(Point site) {
            Point siteFlippedHorizontally = new Point(-site.x, site.y);
			return FlippedHorizontally().PointIsLeftOfLeftEdge (siteFlippedHorizontally);
		}

		public int CompareTo(BeachSection that) {
			if (this.Equals(that))
				return 0;
			// Eliminate all cases with null boundaries
			if (this.IsFullBeachLine () || that.IsFullBeachLine())
				throw new InvalidOperationException ("Can't compare full beach line to another beach section");
			if (this.IsLeftmost () || that.IsRightmost ())
				return -1;
			if (this.IsRightmost () || that.IsLeftmost ())
				return 1;
			if (this.focus.y == that.focus.y ) {
                int focusComparison = this.focus.x .CompareTo (that.focus.x );
                if (focusComparison != 0)
                    return focusComparison;
                int edgeXComparison = this.leftBoundary.x.CompareTo(that.leftBoundary.x);
                if (edgeXComparison != 0)
                    return edgeXComparison;
                return -this.leftBoundary.y.CompareTo(that.leftBoundary.y);
            }
			// Both sections have a left and a right edge and they're not the same. 
			// Use a separate function for the details, which can be tail recursive.
			return CompareProperBeachSectionToProperBeachSection (that);
		}

		int CompareProperBeachSectionToProperBeachSection(BeachSection that) {
			if (this.focus.y< that.focus.y)
				return -that.CompareProperBeachSectionToProperBeachSection (this);
			if (this.focus.x > that.focus.x ) // We want to assume that 'this' focus is to the left of 'that' focus, so if that's not true then flip the situation.
				return -FlippedHorizontally ().CompareProperBeachSectionToProperBeachSection (that.FlippedHorizontally ());
			if (this.leftBoundary.y >= this.focus.y) // If our edge point is above both foci
				return -1;
            if (this.leftBoundary.x>= that.focus.x) // If our left edge is to the right of the lower focus
                return 1;
            if (this.leftBoundary.y <= that.focus.y ) // If our edge point is below both foci, or level with the lower
				return this.leftBoundary.x.CompareTo(that.focus.x);
			// Final case, if our perpendicular bisector of the two foci crosses the vertical line through our lower focus at a lower point than
			// our perpendicular bisector of our 'left' edge and our lower focus does...
			Line twoFociBisector = this.focus.PerpendicularBisector(that.focus);
			Line edgeAndLowerFocusBisector = this.leftBoundary.PerpendicularBisector (that.focus);
			Line verticalLineThroughLowerFocus = new Line (1, 0, -that.focus.x);
			Point twoFociBisectorVerticalIntersect = twoFociBisector.Intersect (verticalLineThroughLowerFocus);
			Point edgeAndLowerFocusBisectorVerticalIntersect = edgeAndLowerFocusBisector.Intersect (verticalLineThroughLowerFocus);
			return twoFociBisectorVerticalIntersect.y .CompareTo (edgeAndLowerFocusBisectorVerticalIntersect.y );
			throw new ApplicationException (String.Format ("Couldn't resolve comparison of beach sections {0} and {1}", this, that));
		}

		BeachSection FlippedHorizontally() { // Just completely mirror all the x points through the line x=0, to solve similar situations.
			Point flippedFocus = new Point (-focus.x , focus.y);
            Point flippedLeftBoundary = null;
            Point flippedRightBoundary = null;
            if (rightBoundary != null)
			    flippedLeftBoundary = new Point(-rightBoundary.x, rightBoundary.y);
            if (leftBoundary != null)
			    flippedRightBoundary = new Point(-leftBoundary.x, leftBoundary.y);
			return new BeachSection (flippedFocus, flippedLeftBoundary, flippedRightBoundary);
		}

        // 'Closed' refers to whether our parabola will be engulfed by those on either side of it as the sweep line progresses downwards.
        // If it won't, we'll never produce an IntersectEventPoint from this beach section.
        public bool Closed()
        {
            return new ConvexPolygon(new List<Point> { focus, leftBoundary, rightBoundary }).NextVertex(focus) == leftBoundary;
        }
	}
}

