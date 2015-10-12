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
      public abstract IEnumerable<Loc> GetPerimeter();

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
      List<Area> areas;

      public CombinedArea(Area a1, Area a2)
      {
         areas = new List<Area>() { a1, a2 };
      }

      public void Add(Area area)
      {
         areas.Add(area);
      }

      public override IEnumerable<Loc> GetPerimeter()
      {
         return areas.SelectMany(a => a.GetPerimeter()).Distinct();
      }

      public override IEnumerable<Loc> GetPointsInArea()
      {
         return areas.SelectMany(a => a.GetPointsInArea()).Distinct();
      }

      public override bool IsPointInArea(Loc point)
      {
         return areas.Any(a => a.IsPointInArea(point));
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

      public override IEnumerable<Loc> GetPerimeter()
      {
         int x1 = Rect.X;
         int y1 = Rect.Y;
         int x2 = x1 + Rect.Width;
         int y2 = y1 + Rect.Height;

         for (int x = x1; x < x2; x++)
         {
            yield return new Loc(x, y1);
         }
         for (int y = y1; y < y2; y++)
         {
            yield return new Loc(x2, y);
         }

         for (int x = x2; x > x1; x--)
         {
            yield return new Loc(x, y2);
         }
         for (int y = y2; y > y1; y--)
         {
            yield return new Loc(x1, y);
         }
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
         return (point - Center).LengthSquared <= (Radius + 1) * (Radius + 1);
      }

      public override IEnumerable<Loc> GetPointsInArea()
      {
         var rs = (Radius + 1) * (Radius + 1);

         var rect = new TerminalRect(-Radius - 1, -Radius - 1, Radius * 2 + 1, Radius * 2 + 1);
         foreach (var point in rect)
         {
            if ((point - Loc.Zero).LengthSquared <= rs)
            {
               yield return point + Center;
            }
         }
      }

      public override IEnumerable<Loc> GetPerimeter()
      {
         int x0 = Center.X;
         int y0 = Center.Y;
         int x = Radius;
         int y = 0;
         int decisionOver2 = 1 - x;   // Decision criterion divided by 2 evaluated at x=r, y=0

         while (y <= x)
         {
            yield return new Loc(x + x0, y + y0); // Octant 1
            yield return new Loc(y + x0, x + y0); // Octant 2
            yield return new Loc(-x + x0, y + y0); // Octant 4
            yield return new Loc(-y + x0, x + y0); // Octant 3
            yield return new Loc(-x + x0, -y + y0); // Octant 5
            yield return new Loc(-y + x0, -x + y0); // Octant 6
            yield return new Loc(x + x0, -y + y0); // Octant 8
            yield return new Loc(y + x0, -x + y0); // Octant 7
            y++;
            if (decisionOver2 <= 0)
            {
               decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
            }
            else
            {
               x--;
               decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
            }
         }
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
         return Start.GetLineTo(End).Contains(point);
      }

      public override IEnumerable<Loc> GetPointsInArea()
      {
         return Start.GetLineTo(End);
      }

      public override IEnumerable<Loc> GetPerimeter()
      {
         return Start.GetLineTo(End);
      }
   }
   public class PointsListArea : Area
   {
      public List<Loc> Points { get; set; }

      public override IEnumerable<Loc> GetPerimeter() => Points;

      public override IEnumerable<Loc> GetPointsInArea() => Points;

      public override bool IsPointInArea(Loc point) => Points.Contains(point);
   }
   public class ConeArea : Area
   {
      public Loc Origin { get; set; }
      public Vector Direction { get; set; }
      public int Length { get; set; }
      public int Width { get; set; }

      public override bool IsPointInArea(Loc point)
      {
         return false;
      }

      public override IEnumerable<Loc> GetPointsInArea()
      {
         yield return Origin;
      }

      public override IEnumerable<Loc> GetPerimeter()
      {
         yield return Origin;
      }
   }
}
