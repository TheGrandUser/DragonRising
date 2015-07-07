using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
   /// <summary>
   /// A 2D integer rectangle class. Similar to Rectangle, but not dependent on System.Drawing
   /// and much more feature-rich.
   /// </summary>
   [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
   public struct TerminalRect : IEquatable<TerminalRect>, IEnumerable<Loc>
   {
      /// <summary>
      /// Gets the empty rectangle.
      /// </summary>
      public readonly static TerminalRect Empty;

      /// <summary>
      /// Creates a new rectangle a single row in height, as wide as the given size.
      /// </summary>
      /// <param name="size">The width of the rectangle.</param>
      /// <returns>The new rectangle.</returns>
      public static TerminalRect Row(int size) => new TerminalRect(0, 0, size, 1);

      /// <summary>
      /// Creates a new rectangle a single row in height, as wide as the given size,
      /// starting at the given top-left corner.
      /// </summary>
      /// <param name="x">The left edge of the rectangle.</param>
      /// <param name="y">The top of the rectangle.</param>
      /// <param name="size">The width of the rectangle.</param>
      /// <returns>The new rectangle.</returns>
      public static TerminalRect Row(int x, int y, int size) => new TerminalRect(x, y, size, 1);

      /// <summary>
      /// Creates a new rectangle a single row in height, as wide as the given size,
      /// starting at the given top-left corner.
      /// </summary>
      /// <param name="pos">The top-left corner of the rectangle.</param>
      /// <returns>The new rectangle.</returns>
      public static TerminalRect Row(Loc pos, int size) => new TerminalRect(pos.X, pos.Y, size, 1);

      /// <summary>
      /// Creates a new rectangle a single column in width, as tall as the given size.
      /// </summary>
      /// <param name="size">The height of the rectangle.</param>
      /// <returns>The new rectangle.</returns>
      public static TerminalRect Column(int size) => new TerminalRect(0, 0, 1, size);

      /// <summary>
      /// Creates a new rectangle a single column in width, as tall as the given size,
      /// starting at the given top-left corner.
      /// </summary>
      /// <param name="x">The left edge of the rectangle.</param>
      /// <param name="y">The top of the rectangle.</param>
      /// <param name="size">The height of the rectangle.</param>
      /// <returns>The new rectangle.</returns>
      public static TerminalRect Column(int x, int y, int size) => new TerminalRect(x, y, 1, size);

      /// <summary>
      /// Creates a new rectangle a single column in width, as tall as the given size,
      /// starting at the given top-left corner.
      /// </summary>
      /// <param name="pos">The top-left corner of the rectangle.</param>
      /// <param name="size">The height of the rectangle.</param>
      /// <returns>The new rectangle.</returns>
      public static TerminalRect Column(Loc pos, int size) => new TerminalRect(pos.X, pos.Y, 1, size);

      /// <summary>
      /// Creates a new rectangle that is the intersection of the two given rectangles.
      /// </summary>
      /// <example><code>
      /// .----------.
      /// | a        |
      /// | .--------+----.
      /// | | result |  b |
      /// | |        |    |
      /// '-+--------'    |
      ///   |             |
      ///   '-------------'
      /// </code></example>
      /// <param name="a">The first rectangle.</param>
      /// <param name="b">The rectangle to intersect it with.</param>
      /// <returns></returns>
      public static TerminalRect Intersection(TerminalRect a, TerminalRect b)
      {
         int left = Math.Max(a.Left, b.Left);
         int right = Math.Min(a.Right, b.Right);
         int top = Math.Max(a.Top, b.Top);
         int bottom = Math.Min(a.Bottom, b.Bottom);

         int width = Math.Max(0, right - left);
         int height = Math.Max(0, bottom - top);

         return new TerminalRect(left, top, width, height);
      }

      public static bool Intersects(TerminalRect a, TerminalRect b) => a.Left <= b.Right && a.Right >= b.Left &&
                                                                        a.Top <= b.Bottom && a.Bottom >= b.Top;

      public static TerminalRect CenterIn(TerminalRect toCenter, TerminalRect main) =>
         new TerminalRect(main.Position + ((main.Size - toCenter.Size) / 2), toCenter.Size);

      #region Operators

      public static bool operator ==(TerminalRect r1, TerminalRect r2) =>  r1.Equals(r2);

      public static bool operator !=(TerminalRect r1, TerminalRect r2) => !r1.Equals(r2);

      public static TerminalRect operator +(TerminalRect r1, Loc v2) => new TerminalRect(r1.Position + v2, r1.Size);

      public static TerminalRect operator +(Loc v1, TerminalRect r2) =>  new TerminalRect(r2.Position + v1, r2.Size);

      public static TerminalRect operator -(TerminalRect r1, Vector v2) => new TerminalRect(r1.Position - v2, r1.Size);

      #endregion

      public Loc Position => mPos;
      public Vector Size => mSize;

      public int X => mPos.X;
      public int Y => mPos.Y;
      public int Width => mSize.X; 
      public int Height => mSize.Y;

      public int Left => X;
      public int Top => Y;
      public int Right => X + Width;
      public int Bottom => Y + Height;

      public Loc TopLeft => new Loc(Left, Top);
      public Loc TopRight => new Loc(Right, Top);
      public Loc BottomLeft => new Loc(Left, Bottom);
      public Loc BottomRight => new Loc(Right, Bottom);

      public Loc Center => new Loc((Left + Right) / 2, (Top + Bottom) / 2);

      public int Area => mSize.X * mSize.Y;

      public TerminalRect(Loc pos, Vector size)
      {
         mPos = pos;
         mSize = size;
      }

      public TerminalRect(Vector size)
      {
         this.mPos = Loc.Zero;
         this.mSize = size;
      }

      public TerminalRect(int x, int y, int width, int height)
      {
         this.mPos = new Loc(x, y);
         this.mSize = new Vector(width, height);
      }

      public TerminalRect(Loc pos, int width, int height)
      {
         this.mPos = pos;
         this.mSize = new Vector(width, height);
      }

      public TerminalRect(int width, int height)
      {
         this.mPos = Loc.Zero;
         this.mSize = new Vector(width, height);
      }

      public TerminalRect(int x, int y, Vector size)
      {
         this.mPos = new Loc(x, y);
         this.mSize = size;
      }

      public override string ToString()
      {
         return string.Format("({0}):({1})", mPos, mSize);
      }

      public override bool Equals(object obj)
      {
         if (obj is TerminalRect) return Equals((TerminalRect)obj);

         return base.Equals(obj);
      }

      public override int GetHashCode()
      {
         return mPos.GetHashCode() + mSize.GetHashCode();
      }

      public TerminalRect Offset(Loc pos, Vector size)
      {
         return new TerminalRect(mPos + pos, mSize + size);
      }

      public TerminalRect Offset(int x, int y, int width, int height)
      {
         return Offset(new Loc(x, y), new Vector(width, height));
      }

      public TerminalRect Inflate(int distance)
      {
         return new TerminalRect(mPos.Offset(-distance, -distance), mSize.Offset(distance * 2, distance * 2));
      }

      public bool Contains(Loc pos)
      {
         if (pos.X < mPos.X) return false;
         if (pos.X >= mPos.X + mSize.X) return false;
         if (pos.Y < mPos.Y) return false;
         if (pos.Y >= mPos.Y + mSize.Y) return false;

         return true;
      }

      public bool Contains(TerminalRect rect)
      {
         // all sides must be within
         if (rect.Left < Left) return false;
         if (rect.Right > Right) return false;
         if (rect.Top < Top) return false;
         if (rect.Bottom > Bottom) return false;

         return true;
      }

      public bool Overlaps(TerminalRect rect)
      {
         // fail if they do not overlap on any axis
         if (Left > rect.Right) return false;
         if (Right < rect.Left) return false;
         if (Top > rect.Bottom) return false;
         if (Bottom < rect.Top) return false;

         // then they must overlap
         return true;
      }

      public TerminalRect Intersection(TerminalRect rect)
      {
         return Intersection(this, rect);
      }

      public bool Intersects(TerminalRect rect)
      {
         return Intersects(this, rect);
      }

      public TerminalRect CenterIn(TerminalRect rect)
      {
         return CenterIn(this, rect);
      }

      public IEnumerable<Loc> Trace()
      {
         if ((Width > 1) && (Height > 1))
         {
            // trace all four sides
            foreach (Loc top in Row(TopLeft, Width - 1)) yield return top;
            foreach (Loc right in Column(TopRight.OffsetX(-1), Height - 1)) yield return right;
            foreach (Loc bottom in Row(Width - 1)) yield return BottomRight.Offset(-1, -1) - (Vector)bottom;
            foreach (Loc left in Column(Height - 1)) yield return BottomLeft.OffsetY(-1) - (Vector)left;
         }
         else if ((Width > 1) && (Height == 1))
         {
            // a single row
            foreach (Loc pos in Row(TopLeft, Width)) yield return pos;
         }
         else if ((Height >= 1) && (Width == 1))
         {
            // a single column, or one unit
            foreach (Loc pos in Column(TopLeft, Height)) yield return pos;
         }

         // otherwise, the rect doesn't have a positive size, so there's nothing to trace
      }

      #region IEquatable<Rect> Members

      public bool Equals(TerminalRect other)
      {
         return mPos.Equals(other.mPos) && mSize.Equals(other.mSize);
      }

      #endregion

      #region IEnumerable<Vec> Members

      public IEnumerator<Loc> GetEnumerator()
      {
         if (mSize.X < 0) throw new ArgumentOutOfRangeException("Cannot enumerate a Rectangle with a negative width.");
         if (mSize.Y < 0) throw new ArgumentOutOfRangeException("Cannot enumerate a Rectangle with a negative height.");

         for (int y = mPos.Y; y < mPos.Y + mSize.Y; y++)
         {
            for (int x = mPos.X; x < mPos.X + mSize.X; x++)
            {
               yield return new Loc(x, y);
            }
         }
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

      #endregion

      [JsonProperty]
      private Loc mPos;
      [JsonProperty]
      private Vector mSize;
   }
}
