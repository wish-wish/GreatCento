using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortunesAlgorithm
{
    public class Rectangle
    {
        public Point topLeft;
        public Point bottomRight;
        public static Random rng;

        public Rectangle(Point topLeft, Point bottomRight)
        {
            rng = new Random();
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }

        public float Right() { return this.bottomRight.x; }
        public float Top() { return this.topLeft.y; }
        public float Left() { return this.topLeft.x; }
        public float Bottom() { return this.bottomRight.y; }
        public float Area()
        {
            return (Right() - Left()) * (Top() - Bottom());
        }

        public IEnumerable<Point> RandomPointsInside(int pointCount)
        {
            for (int i = 0; i < pointCount; i++)
                yield return RandomPointInside();
        }

        public Point RandomPointInside()
        {
            float xDist = 0f;
            float yDist = 0f;
            
            while (xDist <= 0 || xDist >= 1)
                xDist = (float)rng.NextDouble();
            while (yDist <= 0 || yDist >= 1)
                yDist = (float)rng.NextDouble();
            return new Point(topLeft.x + xDist * (bottomRight.x - topLeft.x), bottomRight.y + yDist * (topLeft.y - bottomRight.y));
        }

        
    }
}
