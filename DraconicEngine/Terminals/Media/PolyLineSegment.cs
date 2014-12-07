using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class PolyLineSegment : PathSegment
    {
        private List<Point> points;
        public PolyLineSegment()
        {
            this.points = new List<Point>();
        }

        public PolyLineSegment(IEnumerable<Point> points, bool isStroked)
        {
            this.points = points.ToList();
            this.IsStroked = isStroked;
        }

        public PolyLineSegment(bool isStroked, params Point[] points)
        {
            this.points = points.ToList();
            this.IsStroked = isStroked;
        }

        public List<Point> Points { get { return points; } }
    }
}
