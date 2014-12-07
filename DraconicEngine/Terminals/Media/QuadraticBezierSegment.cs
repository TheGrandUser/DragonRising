using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class QuadraticBezierSegment : PathSegment
    {
        public QuadraticBezierSegment()
        {
        }

        public QuadraticBezierSegment(Point point1, Point point2, bool isStroked)
        {
            this.Point1 = point1;
            this.Point2 = point2;
            this.IsStroked = isStroked;
        }

        public Point Point1 { get; set; }

        public Point Point2 { get; set; }
    }
}
