using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortunesAlgorithm
{
    internal static class Geometry
    {
        internal static VoronoiCellUnorganised PointInRectangleAsVoronoiCell(Point point, Rectangle rectangle)
        {
            EnsureRectangleContainsPoint(rectangle, point);
            VoronoiCellUnorganised cell = new VoronoiCellUnorganised(point);
            foreach (Point edge in MirrorPointsFormingRectangle(point, rectangle))
                cell.AddBorder(edge);
            return cell;
        }
        
        public static bool RectangleEnclosesPoint(Rectangle rectangle, Point point)
        {
            return point.x> rectangle.topLeft.x                 && point.x< rectangle.bottomRight.x                 && point.y< rectangle.topLeft.y                 && point.y> rectangle.bottomRight.y;
        }

        public static IEnumerable<Point> MirrorPointsFormingRectangle(Point point, Rectangle rectangle)
        {
            EnsureRectangleContainsPoint(rectangle, point);
            yield return new Point(2 * rectangle.Right() - point.x, point.y);
            yield return new Point(point.x, 2 * rectangle.Top() - point.y);
            yield return new Point(2 * rectangle.Left() - point.x, point.y);
            yield return new Point(point.x, 2 * rectangle.Bottom() - point.y);
        }

        static void EnsureRectangleContainsPoint(Rectangle rectangle, Point point)
        {
            if (!RectangleEnclosesPoint(rectangle, point))
                throw new ArgumentOutOfRangeException("Rectangle doesn't contain point");
        }

        public static bool Colinear(Point a, Point b, Point c)
        {
            return new PerpendicularBisector(a, b).Line().Vector().CrossProduct(new PerpendicularBisector(a, c).Line().Vector()).z == 0;
        }

        public static Point CircleCentre(Point a, Point b, Point c)
        {
            return new PerpendicularBisector(a, b).Line().Intersect(new PerpendicularBisector(a, c).Line());
        }
    }
}