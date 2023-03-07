using System;

namespace FortunesAlgorithm
{
    internal class SiteEventPoint : IEventPoint {

		Point point;

		public SiteEventPoint(Point point) {
			this.point = point;
		}

		public Point Point ()
		{
			return point;
		}

		public string EventType ()
		{
			return "Site";
		}
	}


}

