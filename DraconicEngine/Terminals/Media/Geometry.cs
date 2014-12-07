using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public abstract class Geometry
    {
        public Rect Bounds { get; protected set; }

        public Transform Transform { get; set; }

        public Geometry()
        {
            this.Bounds = Rect.Empty;
            this.Transform = Transform.Identity;
        }
    }
}
