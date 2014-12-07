using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Media
{
    public enum FillRule
    {
        EvenOdd,
        Nonzero,
    }

    public sealed class PathGeometry : Geometry
    {

        List<PathFigure> figures;

        public List<PathFigure> Figures { get { return figures; } }
        public FillRule FillRule { get; set; }

        public PathGeometry()
        {
        }

        public PathGeometry(IEnumerable<PathFigure> figures)
        {
            this.figures = figures.ToList();
        }

        public PathGeometry(params PathFigure[] figures)
        {
            this.figures = figures.ToList();
        }
    }
}
