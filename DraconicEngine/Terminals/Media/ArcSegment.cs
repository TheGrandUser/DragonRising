using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public enum SweepDirection
    {
        Clockwise,
        CounterClockwise
    }

    public sealed class ArcSegment : PathSegment
    {
        public bool IsLargeArc { get; set; }

        public Point Point { get; set; }

        public double RotationAngle { get; set; }

        public Size Size { get; private set; }

        public SweepDirection SweepDireciton { get; set; }

        public ArcSegment()
        {
        }

        public ArcSegment(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked)
        {
            this.Point = point;
            this.Size = size;
            this.RotationAngle = rotationAngle;
            this.IsLargeArc = isLargeArc;
            this.SweepDireciton = sweepDirection;
            this.IsStroked = IsStroked;
        }
    }
}
