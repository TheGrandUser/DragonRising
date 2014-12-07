using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class EllipseGeometry : Geometry
    {
        public Point Center { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }

        public EllipseGeometry()
        {
        }

        public EllipseGeometry(Rect rect)
        {
            this.Center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            this.RadiusX = rect.Width / 2;
            this.RadiusY = rect.Height / 2;
        }

        public EllipseGeometry(Point center, double radiusX, double radiusY)
        {
            this.Center = center;
            this.RadiusX = radiusX;
            this.RadiusY = radiusY;
        }
    }
}
