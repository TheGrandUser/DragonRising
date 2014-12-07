using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class LineSegment : PathSegment
    {
        public LineSegment()
        {
        }

        public LineSegment(Point point, bool isStroked)
        {
            this.Point = point;
            this.IsStroked = isStroked;
        }

        public Point Point { get; set; }
    }
}
