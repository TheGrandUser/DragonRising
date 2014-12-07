using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class PathFigure
    {
        List<PathSegment> segments;

        public List<PathSegment> Segments { get { return segments; } }
        public bool IsClosed { get; set; }
        public bool IsFilled { get; set; }
        public Point StartPoint { get; set; }

        public PathFigure()
        {
        }

        public PathFigure(Point start, IEnumerable<PathSegment> segments, bool closed)
        {
            this.StartPoint = start;
            this.segments = segments.ToList();
            this.IsClosed = closed;
        }

        public PathFigure(Point start, bool closed, params PathSegment[] segments)
        {
            this.StartPoint = start;
            this.segments = segments.ToList();
            this.IsClosed = closed;
        }
    }
}
