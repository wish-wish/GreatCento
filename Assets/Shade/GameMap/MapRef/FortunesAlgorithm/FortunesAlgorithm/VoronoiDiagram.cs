using System;
using System.Collections.Generic;
using System.Linq;
using Structures;

namespace FortunesAlgorithm
{
	public class VoronoiDiagram
	{
		internal Dictionary<Point, VoronoiCellUnorganised> cells;

		internal VoronoiDiagram(IEnumerable<Point> points)
		{
            cells = new FortunesAlgorithm().WithPoints(points);
		}
        
        internal List<VoronoiCellUnorganised> UnorganisedCells()
        {
            return cells.Values.ToList();
        }

        public IEnumerable<Point> Points()
        {
            return UnorganisedCells().Select(c => c.Site());
        }

        public IEnumerable<VoronoiCell> Cells()
        {
            return cells.Values.Select(c => c.Organised());
        }
	}

    public class VoronoiDiagramBordered : VoronoiDiagram
    {
        Rectangle enclosingRegion;

        public VoronoiDiagramBordered(IEnumerable<Point> points, Rectangle enclosingRegion) : base(points.Where(p => Geometry.RectangleEnclosesPoint(enclosingRegion, p)))
        {
            this.enclosingRegion = enclosingRegion;
            foreach (VoronoiCellUnorganised cell in cells.Values)
                foreach (Point p in Geometry.MirrorPointsFormingRectangle(cell.Site(), this.enclosingRegion))
                    cell.AddBorder(p);
        }

        public ScatterPlot Smoothed()
        {
            IEnumerable<Point> newPoints = cells.Values.Select(c => c.Organised().Centroid());
            return new ScatterPlot(this.enclosingRegion, newPoints);
        }
    }
}

