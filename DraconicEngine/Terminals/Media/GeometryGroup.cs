using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public sealed class GeometryGroup : Geometry
    {
        List<Geometry> children = new List<Geometry>();

        public GeometryGroup()
        {
        }

        public List<Geometry> Children { get { return children; } }

        public FillRule FillRule { get; set; }
    }
}
