using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortunesAlgorithm
{
    public class ScatterPlot
    {
        public readonly Rectangle rectangle;
        public readonly List<Point> points;

        ScatterPlot(Rectangle rectangle)
        {
            this.rectangle = rectangle;
            this.points = new List<Point>();
        }

        public ScatterPlot(Rectangle rectangle, IEnumerable<Point> points) : this(rectangle)
        {
            this.points = points.Where(p => Geometry.RectangleEnclosesPoint(rectangle, p)).ToList();
        }

        public ScatterPlot(Rectangle rectangle, int numberOfPoints) : this(rectangle, rectangle.RandomPointsInside(numberOfPoints)) { }

        public ScatterPlot(Rectangle rectangle, float meanRadius) : this(
            rectangle,
            (int)(rectangle.Area()/(meanRadius*meanRadius*Math.PI))
        ) { }

        public VoronoiDiagramBordered Voronoi()
        {
            return new VoronoiDiagramBordered(points, rectangle);
        }

        public ScatterPlot Smoothed()
        {
            return Voronoi().Smoothed();
        }

        public ScatterPlot Smoothed(int repetitions)
        {
            ScatterPlot accumulated = this;
            for (int i = 0; i < repetitions; i++)
                accumulated = accumulated.Smoothed();
            return accumulated;
        }
    }
}
