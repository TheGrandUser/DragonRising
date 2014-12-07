using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
   /// <summary>
   /// A 2D integer vector class. Similar to Point but not dependent on System.Drawing and much more
   /// feature-rich.
   /// </summary>
   //[Serializable]
   public struct Loc : IEquatable<Loc>
   {
      /// <summary>
      /// Gets the zero vector.
      /// </summary>
      public static readonly Loc Zero = new Loc(0, 0);

      /// <summary>
      /// Gets the unit vector.
      /// </summary>
      public static readonly Loc One = new Loc(1, 1);

      #region Operators

      public static bool operator ==(Loc v1, Loc v2)
      {
         return v1.Equals(v2);
      }

      public static bool operator !=(Loc v1, Loc v2)
      {
         return !v1.Equals(v2);
      }

      public static Loc operator +(Loc v1, Loc v2)
      {
         return new Loc(v1.X + v2.X, v1.Y + v2.Y);
      }

      public static Loc operator +(Loc v1, Vector v2)
      {
         return new Loc(v1.X + v2.X, v1.Y + v2.Y);
      }

      public static Loc operator +(Vector v1, Loc v2)
      {
         return new Loc(v1.X + v2.X, v1.Y + v2.Y);
      }

      public static Loc operator +(Loc v1, int i2)
      {
         return new Loc(v1.X + i2, v1.Y + i2);
      }

      public static Loc operator +(int i1, Loc v2)
      {
         return new Loc(i1 + v2.X, i1 + v2.Y);
      }

      public static Vector operator -(Loc v1, Loc v2)
      {
         return new Vector(v1.X - v2.X, v1.Y - v2.Y);
      }

      public static Loc operator -(Loc v1, Vector v2)
      {
         return new Loc(v1.X - v2.X, v1.Y - v2.Y);
      }

      public static Loc operator -(Vector v1, Loc v2)
      {
         return new Loc(v1.X - v2.X, v1.Y - v2.Y);
      }

      public static Loc operator -(Loc v1, int i2)
      {
         return new Loc(v1.X - i2, v1.Y - i2);
      }

      public static Loc operator -(int i1, Loc v2)
      {
         return new Loc(i1 - v2.X, i1 - v2.Y);
      }

      public static Loc operator *(Loc v1, int i2)
      {
         return new Loc(v1.X * i2, v1.Y * i2);
      }

      public static Loc operator *(int i1, Loc v2)
      {
         return new Loc(i1 * v2.X, i1 * v2.Y);
      }

      public static Loc operator /(Loc v1, int i2)
      {
         return new Loc(v1.X / i2, v1.Y / i2);
      }

      #endregion

      /// <summary>
      /// Gets whether the distance between the two given <see cref="Loc">Vecs</see> is within
      /// the given distance.
      /// </summary>
      /// <param name="a">First Vec.</param>
      /// <param name="b">Second Vec.</param>
      /// <param name="distance">Maximum distance between them.</param>
      /// <returns><c>true</c> if the distance between <c>a</c> and <c>b</c> is less than or equal to <c>distance</c>.</returns>
      public static bool IsDistanceWithin(Loc a, Loc b, int distance)
      {
         Vector offset = a - b;

         return offset.LengthSquared <= (distance * distance);
      }

      /// <summary>
      /// Initializes a new instance of Vec with the given coordinates.
      /// </summary>
      /// <param name="x">X coordinate.</param>
      /// <param name="y">Y coordinate.</param>
      public Loc(int x, int y)
      {
         X = x;
         Y = y;
      }

      public int X;
      public int Y;

      /// <summary>
      /// Gets the area of a rectangle with opposite corners at (0, 0) and this Vec.
      /// </summary>
      public int Area { get { return X * Y; } }

      /// <summary>
      /// Gets the absolute magnitude of the Vec squared.
      /// </summary>
      public int LengthSquared { get { return (X * X) + (Y * Y); } }

      /// <summary>
      /// Gets the absolute magnitude of the Vec.
      /// </summary>
      public double Length { get { return Math.Sqrt(LengthSquared); } }

      /// <summary>
      /// Gets the rook length of the Vec, which is the number of squares a rook on a chessboard
      /// would need to move from (0, 0) to reach the endpoint of the Vec. Also known as
      /// Manhattan or taxicab distance.
      /// </summary>
      public int RookLength { get { return Math.Abs(X) + Math.Abs(Y); } }

      /// <summary>
      /// Gets the king length of the Vec, which is the number of squares a king on a chessboard
      /// would need to move from (0, 0) to reach the endpoint of the Vec. Also known as
      /// Chebyshev distance.
      /// </summary>
      public int KingLength { get { return Math.Max(Math.Abs(X), Math.Abs(Y)); } }

      /// <summary>
      /// Converts this Vec to a human-readable string.
      /// </summary>
      /// <returns>A string representation of the Vec.</returns>
      public override string ToString()
      {
         return X.ToString() + ", " + Y.ToString();
      }

      /// <summary>
      /// Specifies whether this Vec contains the same coordinates as the specified Object. 
      /// </summary>
      /// <param name="obj">Object to compare to.</param>
      /// <returns><c>true</c> if <c>object</c> is a Vec with the same coordinates.</returns>
      public override bool Equals(object obj)
      {
         if (obj is Loc)
         {
            return Equals((Loc)obj);
         }
         else
         {
            return false;
         }
      }

      /// <summary>
      /// Returns a hash code for this Vec.
      /// </summary>
      /// <returns>An integer value that specifies a hash value for this Vec.</returns>
      public override int GetHashCode()
      {
         return ToString().GetHashCode();
      }

      /// <summary>
      /// Gets whether the given vector is within a rectangle
      /// from (0,0) to this vector (half-inclusive).
      /// </summary>
      public bool Contains(Loc vec)
      {
         if (vec.X < 0) return false;
         if (vec.X >= X) return false;
         if (vec.Y < 0) return false;
         if (vec.Y >= Y) return false;

         return true;
      }

      public bool IsAdjacentTo(Loc other)
      {
         // not adjacent to the exact same position
         if (this == other) return false;

         Vector offset = this - other;

         return (Math.Abs(offset.X) <= 1) && (Math.Abs(offset.Y) <= 1);
      }

      /// <summary>
      /// Returns a new Vec whose coordinates are the coordinates of this Vec
      /// with the given values added. This Vec is not modified.
      /// </summary>
      /// <param name="x">Distance to offset the X coordinate.</param>
      /// <param name="y">Distance to offset the Y coordinate.</param>
      /// <returns>A new Vec offset by the given coordinates.</returns>
      public Loc Offset(int x, int y)
      {
         return new Loc(X + x, Y + y);
      }

      /// <summary>
      /// Returns a new Vec whose coordinates are the coordinates of this Vec
      /// with the given value added to the X coordinate. This Vec is not modified.
      /// </summary>
      /// <param name="offset">Distance to offset the X coordinate.</param>
      /// <returns>A new Vec offset by the given X coordinate.</returns>
      public Loc OffsetX(int offset)
      {
         return new Loc(X + offset, Y);
      }

      /// <summary>
      /// Returns a new Vec whose coordinates are the coordinates of this Vec
      /// with the given value added to the Y coordinate. This Vec is not modified.
      /// </summary>
      /// <param name="offset">Distance to offset the Y coordinate.</param>
      /// <returns>A new Vec offset by the given Y coordinate.</returns>
      public Loc OffsetY(int offset)
      {
         return new Loc(X, Y + offset);
      }

      /// <summary>
      /// Returns a new Vec whose coordinates are the coordinates of this Vec
      /// with the given function applied.
      /// </summary>
      public Loc Each(Func<int, int> function)
      {
         if (function == null) throw new ArgumentNullException("function");

         return new Loc(function(X), function(Y));
      }

      #region IEquatable<Vec> Members

      public bool Equals(Loc other)
      {
         return X.Equals(other.X) && Y.Equals(other.Y);
      }

      #endregion

      public IEnumerable<Loc> GetLineTo(Loc point)
      {
         var x0 = this.X;
         var y0 = this.Y;
         var x1 = point.X;
         var y1 = point.Y;

         var dx = Math.Abs(x0 - x1);
         var dy = Math.Abs(y0 - y1);
         var sx = x0 < x1 ? 1 : -1;
         var sy = y0 < y1 ? 1 : -1;
         var err = dx - dy;

         while (true)
         {
            yield return new Loc(x0, y0);

            if (x0 == x1 && y0 == y1) break;
            var e2 = 2 * err;
            if (e2 > -dy)
            {
               err = err - dy;
               x0 = x0 + sx;
            }
            if (x0 == x1 && y0 == y1)
            {
               yield return new Loc(x0, y0);
               break;
            }
            if (e2 < dx)
            {
               err = err + dx;
               y0 = y0 + sy;
            }
         }
      }
   }
}
