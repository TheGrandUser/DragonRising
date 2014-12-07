using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class LineGeometry : Geometry
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public LineGeometry()
        {
            this.StartPoint = Point.Zero;
            this.EndPoint = Point.Zero;
        }

        public LineGeometry(Point startPoint, Point endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            this.Bounds = new Rect(startPoint, endPoint);
        }
    }
}
