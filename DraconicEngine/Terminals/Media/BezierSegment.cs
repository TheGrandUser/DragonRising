using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class BezierSegment : PathSegment
    {
        public BezierSegment()
        {
        }

        public BezierSegment(Point point1, Point point2, Point point3, bool isStroked)
        {
            this.Point1 = point1;
            this.Point2 = point2;
            this.Point3 = point3;
            this.IsStroked = isStroked;
        }

        public Point Point1 { get; set; }

        public Point Point2 { get; set; }

        public Point Point3 { get; set; }
    }
}
