using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class PolyQuadraticBezierSegment : PathSegment
    {
        private List<Point> points;
        public PolyQuadraticBezierSegment()
        {
            this.points = new List<Point>();
        }

        public PolyQuadraticBezierSegment(IEnumerable<Point> points, bool isStroked)
        {
            this.points = points.ToList();
            this.IsStroked = isStroked;
        }

        public List<Point> Points { get { return points; } }
    }
}
