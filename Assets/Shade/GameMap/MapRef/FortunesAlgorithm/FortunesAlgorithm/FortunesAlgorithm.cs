using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortunesAlgorithm
{
    internal class FortunesAlgorithm
    {
        Dictionary<Point, VoronoiCellUnorganised> cells;

        internal Dictionary<Point, VoronoiCellUnorganised> WithPoints(IEnumerable<Point> points)
        {
            if (points.Count() == 0)
                throw new System.ArgumentException("No points provided");

            cells = new Dictionary<Point, VoronoiCellUnorganised>();

            // We'll start at the top of the field and work down.

            // Initialise the beachline with all points having most Y coordinate.
            // There'll often be only one of these, but calculating their interactions 
            // with no background beachline is more difficult so it helps to include them all at once.

            HashSet<Point> distinctPoints = new HashSet<Point>(points);
            List<Point> highestPoints = FindHighestPoints(distinctPoints).OrderBy(p => p.x).ToList();
            float mostY = highestPoints.First().y;
            IEnumerable<Point> otherPoints = distinctPoints.Where(p => p.y< mostY);

            List<BeachSection> initialBeachSections = new List<BeachSection>();

            for (int i = 0; i < highestPoints.Count; i++)
            {
                Point left = null;
                Point right = null;
                Point focus = highestPoints[i];
                cells[focus] = new VoronoiCellUnorganised(focus);
                if (i > 0)
                {
                    left = highestPoints[i - 1];
                    cells[focus].AddBorder(left);
                }

                if (i + 1 < highestPoints.Count)
                {
                    right = highestPoints[i + 1];
                    cells[focus].AddBorder(right);
                }

                BeachSection bs = new BeachSection(focus, left, right);
                initialBeachSections.Add(bs);
            }

            BeachLine beachLine = new BeachLine(initialBeachSections, otherPoints);

            // For the remaining points that don't lie on the initial beachline, handle them in descending order
            // of their y coordinate.

            while (beachLine.HasMoreEvents())
            {

                IEventPoint eventPoint = beachLine.PopEvent();

                if (eventPoint.EventType() == "Site")
                {

                    Point site = eventPoint.Point();

                    BeachSection containingBeachSection = beachLine.BeachSectionContainingPoint(site);

                    VoronoiCellUnorganised cell = new VoronoiCellUnorganised(site);
                    cells[site] = cell;
                    AddBorderBetweenCells(site, containingBeachSection.focus);

                    beachLine.SplitBeachSection(site);

                }
                else // EventType = "Intersect"
                { 

                    IntersectEventPoint intersectEventPoint = (IntersectEventPoint)eventPoint;

                    BeachSection consumedBeachSection = intersectEventPoint.consumedBeachSection;

                    AddBorderBetweenCells(consumedBeachSection.leftBoundary, consumedBeachSection.rightBoundary);

                    beachLine.ConsumeBeachSection(intersectEventPoint);
                }
            }
            return cells;
        }

        void AddBorderBetweenCells(Point a, Point b)
        {
            cells[a].AddBorder(b);
            cells[b].AddBorder(a);
        }

        HashSet<Point> FindHighestPoints(HashSet<Point> points)
        {
            HashSet<Point> highestPoints = new HashSet<Point>();
            float mostY = points.First().y;
            foreach (Point point in points)
            {
                if (point.y== mostY)
                    highestPoints.Add(point);
                else if (point.y> mostY)
                {
                    highestPoints = new HashSet<Point> { point };
                    mostY = point.y;
                }
            }
            return highestPoints;
        }
    }
}
