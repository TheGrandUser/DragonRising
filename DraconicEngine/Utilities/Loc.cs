using Newtonsoft.Json;
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
   [JsonObject(MemberSerialization.OptIn)]
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
      /// <param name="a">First Loc.</param>
      /// <param name="b">Second Loc.</param>
      /// <param name="distance">Maximum distance between them.</param>
      /// <returns><c>true</c> if the distance between <c>a</c> and <c>b</c> is less than or equal to <c>distance</c>.</returns>
      public static bool IsDistanceWithin(Loc a, Loc b, int distance)
      {
         Vector offset = a - b;

         return offset.LengthSquared <= (distance * distance);
      }

      /// <summary>
      /// Initializes a new instance of Loc with the given coordinates.
      /// </summary>
      /// <param name="x">X coordinate.</param>
      /// <param name="y">Y coordinate.</param>
      public Loc(int x, int y)
      {
         X = x;
         Y = y;
      }

      [JsonProperty]
      public int X;
      [JsonProperty]
      public int Y;
      
      /// <summary>
      /// Converts this Loc to a human-readable string.
      /// </summary>
      /// <returns>A string representation of the Loc.</returns>
      public override string ToString()
      {
         return X.ToString() + ", " + Y.ToString();
      }

      /// <summary>
      /// Specifies whether this Loc contains the same coordinates as the specified Object. 
      /// </summary>
      /// <param name="obj">Object to compare to.</param>
      /// <returns><c>true</c> if <c>object</c> is a Loc with the same coordinates.</returns>
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
      /// Returns a hash code for this Loc.
      /// </summary>
      /// <returns>An integer value that specifies a hash value for this Loc.</returns>
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
      /// Returns a new Loc whose coordinates are the coordinates of this Loc
      /// with the given values added. This Loc is not modified.
      /// </summary>
      /// <param name="x">Distance to offset the X coordinate.</param>
      /// <param name="y">Distance to offset the Y coordinate.</param>
      /// <returns>A new Loc offset by the given coordinates.</returns>
      public Loc Offset(int x, int y)
      {
         return new Loc(X + x, Y + y);
      }

      /// <summary>
      /// Returns a new Loc whose coordinates are the coordinates of this Loc
      /// with the given value added to the X coordinate. This Loc is not modified.
      /// </summary>
      /// <param name="offset">Distance to offset the X coordinate.</param>
      /// <returns>A new Loc offset by the given X coordinate.</returns>
      public Loc OffsetX(int offset)
      {
         return new Loc(X + offset, Y);
      }

      /// <summary>
      /// Returns a new Loc whose coordinates are the coordinates of this Loc
      /// with the given value added to the Y coordinate. This Loc is not modified.
      /// </summary>
      /// <param name="offset">Distance to offset the Y coordinate.</param>
      /// <returns>A new Loc offset by the given Y coordinate.</returns>
      public Loc OffsetY(int offset)
      {
         return new Loc(X, Y + offset);
      }
      
      #region IEquatable<Loc> Members

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
