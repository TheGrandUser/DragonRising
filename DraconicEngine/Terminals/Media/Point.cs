using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public struct Point
    {
        double x, y;

        public double X { get { return x; } }
        public double Y { get { return y; } }

        public static Point Zero { get { return new Point(); } }

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
