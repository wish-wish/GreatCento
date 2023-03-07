using System;
using System.Collections.Generic;
using System.Linq;

namespace FortunesAlgorithm
{
    public abstract class IVoronoiCell
    {
        protected Point site;

        public IVoronoiCell(Point site)
        {
            this.site = site;
        }

        public Point Site()
        {
            return site;
        }

        internal abstract IEnumerable<Point> Borders();
    }

	internal class VoronoiCellUnorganised : IVoronoiCell
    {

        protected List<Point> borderSites;

        public VoronoiCellUnorganised (Point site) : base(site)
		{
			this.borderSites = new List<Point> ();
		}

		public void AddBorder(Point border) {
			borderSites.Add (border);
		}

        public VoronoiCell Organised()
        {
            return new VoronoiCell(site, borderSites);
        }

        internal override IEnumerable<Point> Borders()
        {
            return borderSites;
        }
}

    public class VoronoiCell : IVoronoiCell
    {
        ConvexPolygon borderOrdering;

        public VoronoiCell(Point site, IEnumerable<Point> borderSites) : base(site)
        {
            if (borderSites.Count() == 0)
                throw new System.ArgumentException("No borders provided");
            borderOrdering = new ConvexPolygon(borderSites);
            RemoveEclipsedBorders();
        }

        private void RemoveEclipsedBorders()
        {
            Point candidateBorder = borderOrdering.AnyVertex();
            int initialBorders = Borders().Count();
            int excludedBorders = 0;
            int streak = 0;
            while (streak + excludedBorders + 1 < initialBorders)
            {
                if (EclipsedByNeighbours(candidateBorder))
                {
                    Point nextCandidate = borderOrdering.PreviousVertex(candidateBorder);
                    borderOrdering.Remove(candidateBorder);
                    candidateBorder = nextCandidate;
                    streak = Math.Max(streak - 1, 0);
                    excludedBorders++;
                } else
                {
                    candidateBorder = borderOrdering.NextVertex(candidateBorder);
                    streak++;
                }
            }
            
        }

        private bool EclipsedByNeighbours(Point border)
        {
            if (Geometry.Colinear(site, borderOrdering.NextVertex(border), borderOrdering.PreviousVertex(border)))
                return false;
            Point neighbourIntersect = Geometry.CircleCentre(site, borderOrdering.NextVertex(border), borderOrdering.PreviousVertex(border));
            return border.DistanceFrom(neighbourIntersect) >= site.DistanceFrom(neighbourIntersect);
        }

        internal override IEnumerable<Point> Borders()
        {
            return borderOrdering.AllPointsInOrder();
        }

        public IEnumerable<Point> Vertices()
        {
            List<Line> borders = borderOrdering.AllPointsInOrder().Select(p => p.PerpendicularBisector(site)).ToList();
            for (int i = 0; i < borders.Count(); i++ )
            {
                if (i + 1 < borders.Count())
                    yield return borders[i].Intersect(borders[i + 1]);
                else
                    yield return borders[i].Intersect(borders[0]);
            }
        }

        ConvexPolygon Polygon()
        {
            return new ConvexPolygon(Vertices());
        }

        public Point Centroid()
        {
            return Polygon().Centroid();
        }
        
    }
}

