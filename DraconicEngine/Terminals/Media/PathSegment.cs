using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public abstract class PathSegment
    {
        public bool IsSmoothJoin { get; set; }
        public bool IsStroked { get; set; }
    }
}
