using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class PolyBezierSegment : PathSegment
    {
        public PolyBezierSegment()
        {
            Points = new List<Point>();
        }

        public PolyBezierSegment(IEnumerable<Point> points, bool isStroked)
        {
            Points = points.ToList();
            this.IsStroked = isStroked;
        }

        public List<Point> Points { get; }
    }
}
