using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public enum GeometryCombineMode
    {
        Union, // A || B
        Intersection, // A && B
        Exclude, // A && !B
        Xor, // (A && !B) || (!A && B)
    }

    public sealed class CombinedGeometry : Geometry
    {
        public CombinedGeometry()
        {
        }

        public CombinedGeometry(Geometry geometry1, Geometry geometry2)
        {
            this.Geometry1 = geometry1;
            this.Geometry2 = geometry2;
        }

        public CombinedGeometry(Geometry geometry1, Geometry geometry2, GeometryCombineMode geometryCombineMode)
        {
            this.Geometry1 = geometry1;
            this.Geometry2 = geometry2;
        }

        public Geometry Geometry1 { get; set; }

        public Geometry Geometry2 { get; set; }

        public GeometryCombineMode GeometryCombineMode { get; set; }
    }
}
