using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace DraconicEngine
{
   public abstract class Area
   {
      public abstract bool IsPointInArea(Loc point);
      public abstract IEnumerable<Loc> GetPointsInArea();
      public abstract IEnumerable<Tuple<Loc, Edges>> GetPerimeter();

      public static Option<Area> Combine(IEnumerable<Area> areas)
      {
         Area finalArea = null;

         foreach (var area in areas)
         {
            if (finalArea == null)
            {
               finalArea = area;
            }
            else if (finalArea is CombinedArea)
            {
               var combined = (CombinedArea)finalArea;
               combined.Add(area);
            }
            else
            {
               finalArea = new CombinedArea(finalArea, area);
            }
         }

         return Optional(finalArea);
      }
   }

   public class CombinedArea : Area
   {
      public CombinedArea(Area a1, Area a2)
      {

      }

      public void Add(Area area)
      {

      }

      public override IEnumerable<Tuple<Loc, Edges>> GetPerimeter()
      {
         throw new NotImplementedException();
      }

      public override IEnumerable<Loc> GetPointsInArea()
      {
         throw new NotImplementedException();
      }

      public override bool IsPointInArea(Loc point)
      {
         throw new NotImplementedException();
      }
   }

   public class RectArea : Area
   {
      TerminalRect Rect { get; }

      public RectArea(TerminalRect rect)
      {
         Rect = rect;
      }

      public RectArea(Loc point1, Loc point2)
      {
         var maxPoint = new Loc(Math.Max(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
         var minPoint = new Loc(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));

         Rect = new TerminalRect(minPoint, maxPoint - minPoint);
      }

      public override bool IsPointInArea(Loc point) => Rect.Contains(point);

      public override IEnumerable<Loc> GetPointsInArea() => Rect;

      public override IEnumerable<Tuple<Loc, Edges>> GetPerimeter()
      {
         throw new NotImplementedException();
      }
   }
   public class CirlceArea : Area
   {
      public CirlceArea(int radius, Loc center)
      {
         Radius = radius;
         Center = center;
      }
      public int Radius { get; }
      public Loc Center { get; }

      public override bool IsPointInArea(Loc point)
      {
         throw new NotImplementedException();
      }

      public override IEnumerable<Loc> GetPointsInArea()
      {
         throw new NotImplementedException();
      }

      public override IEnumerable<Tuple<Loc, Edges>> GetPerimeter()
      {
         throw new NotImplementedException();
      }
   }
   public class LineArea : Area
   {
      public LineArea(Loc start, Loc end)
      {
         Start = start;
         End = end;
      }

      public Loc Start { get; }
      public Loc End { get; }

      public override bool IsPointInArea(Loc point)
      {
         throw new NotImplementedException();
      }

      public override IEnumerable<Loc> GetPointsInArea()
      {
         throw new NotImplementedException();
      }

      public override IEnumerable<Tuple<Loc, Edges>> GetPerimeter()
      {
         throw new NotImplementedException();
      }
   }
   public class PointsListArea : Area
   {
      public List<Loc> Points { get; set; }

      public override IEnumerable<Tuple<Loc, Edges>> GetPerimeter()
      {
         throw new NotImplementedException();
      }

      public override IEnumerable<Loc> GetPointsInArea()
      {
         throw new NotImplementedException();
      }

      public override bool IsPointInArea(Loc point)
      {
         throw new NotImplementedException();
      }
   }
   public class ConeArea : Area
   {
      public Loc Origin { get; set; }
      public Vector Direction { get; set; }
      public int Length { get; set; }
      public int Width { get; set; }

      public override bool IsPointInArea(Loc point)
      {
         throw new NotImplementedException();
      }

      public override IEnumerable<Loc> GetPointsInArea()
      {
         throw new NotImplementedException();
      }

      public override IEnumerable<Tuple<Loc, Edges>> GetPerimeter()
      {
         throw new NotImplementedException();
      }
   }
}
