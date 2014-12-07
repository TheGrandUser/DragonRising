using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class RectangleGeometry : Geometry
    {
        public Rect Rect { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }

        public RectangleGeometry()
        {
            this.Rect = Rect.Empty;
            this.Bounds = Rect;
        }

        public RectangleGeometry(Rect rect)
        {
            this.Rect = rect;
            this.Bounds = rect;
        }

        public RectangleGeometry(Rect rect, double radiusX, double radiusY)
        {
            this.Rect = rect;
            this.RadiusX = radiusX;
            this.RadiusY = radiusY;
            this.Bounds = rect;
        }
    }
}
