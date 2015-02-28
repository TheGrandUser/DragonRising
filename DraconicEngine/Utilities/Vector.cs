using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DraconicEngine
{
   /// <summary>
   /// A 2D integer Vector class. Similar to Vector but not dependent on System.Drawing and much more
   /// feature-rich.
   /// </summary>
   //[Serializable]
   [JsonObject(MemberSerialization.OptIn)]
   public struct Vector : IEquatable<Vector>
   {
      /// <summary>
      /// Gets the zero Vector.
      /// </summary>
      public static readonly Vector Zero = new Vector(0, 0);

      /// <summary>
      /// Gets the unit Vector.
      /// </summary>
      public static readonly Vector One = new Vector(1, 1);

      #region Operators

      public static bool operator ==(Vector v1, Vector v2)
      {
         return v1.Equals(v2);
      }

      public static bool operator !=(Vector v1, Vector v2)
      {
         return !v1.Equals(v2);
      }

      public static Vector operator +(Vector v1, Vector v2)
      {
         return new Vector(v1.X + v2.X, v1.Y + v2.Y);
      }

      public static Vector operator +(Vector v1, int i2)
      {
         return new Vector(v1.X + i2, v1.Y + i2);
      }

      public static Vector operator +(int i1, Vector v2)
      {
         return new Vector(i1 + v2.X, i1 + v2.Y);
      }

      public static Vector operator -(Vector v1, Vector v2)
      {
         return new Vector(v1.X - v2.X, v1.Y - v2.Y);
      }

      public static Vector operator -(Vector v1, int i2)
      {
         return new Vector(v1.X - i2, v1.Y - i2);
      }

      public static Vector operator -(int i1, Vector v2)
      {
         return new Vector(i1 - v2.X, i1 - v2.Y);
      }

      public static Vector operator *(Vector v1, int i2)
      {
         return new Vector(v1.X * i2, v1.Y * i2);
      }

      public static Vector operator *(int i1, Vector v2)
      {
         return new Vector(i1 * v2.X, i1 * v2.Y);
      }

      public static Vector operator /(Vector v1, int i2)
      {
         return new Vector(v1.X / i2, v1.Y / i2);
      }

      public Direction ToDirection()
      {
         if(this.KingLength == 0)
         {
            return Direction.None;
         }
         if (this.KingLength == 1)
         {
            if (this.X > 0) // east
            {
               if (this.Y > 0)
               {
                  return Direction.Southeast;
               }
               else if (this.Y < 0)
               {
                  return Direction.Northeast;
               }
               else
               {
                  return Direction.East;
               }
            }
            else if (this.X < 0) // west
            {
               if (this.Y > 0)
               {
                  return Direction.Southwest;
               }
               else if (this.Y < 0)
               {
                  return Direction.Northwest;
               }
               else
               {
                  return Direction.West;
               }
            }
            else if (this.Y > 0)
            {
               return Direction.South;
            }
            else
            {
               return Direction.North;
            }
         }

         var pathVec = this.PathFindAttempts().FirstOrDefault();
         Debug.Assert(pathVec.KingLength == 1);
         return pathVec.ToDirection();
      }

      public static explicit operator Loc(Vector v)
      {
         return new Loc(v.X, v.Y);
      }

      public static explicit operator Vector(Loc v)
      {
         return new Vector(v.X, v.Y);
      }

      #endregion

      /// <summary>
      /// Gets whether the distance between the two given <see cref="Vector">Vecs</see> is within
      /// the given distance.
      /// </summary>
      /// <param name="a">First Vec.</param>
      /// <param name="b">Second Vec.</param>
      /// <param name="distance">Maximum distance between them.</param>
      /// <returns><c>true</c> if the distance between <c>a</c> and <c>b</c> is less than or equal to <c>distance</c>.</returns>
      public static bool IsDistanceWithin(Vector a, Vector b, int distance)
      {
         Vector offset = a - b;

         return offset.LengthSquared <= (distance * distance);
      }

      /// <summary>
      /// Initializes a new instance of Vec with the given coordinates.
      /// </summary>
      /// <param name="x">X coordinate.</param>
      /// <param name="y">Y coordinate.</param>
      public Vector(int x, int y)
      {
         X = x;
         Y = y;
      }

      [JsonProperty]
      public int X;
      [JsonProperty]
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
      /// would need to move from (0, 0) to reach the endVector of the Vec. Also known as
      /// Manhattan or taxicab distance.
      /// </summary>
      public int RookLength { get { return Math.Abs(X) + Math.Abs(Y); } }

      /// <summary>
      /// Gets the king length of the Vec, which is the number of squares a king on a chessboard
      /// would need to move from (0, 0) to reach the endVector of the Vec. Also known as
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
         if (obj is Vector)
         {
            return Equals((Vector)obj);
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
      /// Gets whether the given Vector is within a rectangle
      /// from (0,0) to this Vector (half-inclusive).
      /// </summary>
      public bool Contains(Loc vec)
      {
         if (vec.X < 0) return false;
         if (vec.X >= X) return false;
         if (vec.Y < 0) return false;
         if (vec.Y >= Y) return false;

         return true;
      }

      public bool IsAdjacentTo(Vector other)
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
      public Vector Offset(int x, int y)
      {
         return new Vector(X + x, Y + y);
      }

      /// <summary>
      /// Returns a new Vec whose coordinates are the coordinates of this Vec
      /// with the given value added to the X coordinate. This Vec is not modified.
      /// </summary>
      /// <param name="offset">Distance to offset the X coordinate.</param>
      /// <returns>A new Vec offset by the given X coordinate.</returns>
      public Vector OffsetX(int offset)
      {
         return new Vector(X + offset, Y);
      }

      /// <summary>
      /// Returns a new Vec whose coordinates are the coordinates of this Vec
      /// with the given value added to the Y coordinate. This Vec is not modified.
      /// </summary>
      /// <param name="offset">Distance to offset the Y coordinate.</param>
      /// <returns>A new Vec offset by the given Y coordinate.</returns>
      public Vector OffsetY(int offset)
      {
         return new Vector(X, Y + offset);
      }

      /// <summary>
      /// Returns a new Vec whose coordinates are the coordinates of this Vec
      /// with the given function applied.
      /// </summary>
      public Vector Each(Func<int, int> function)
      {
         if (function == null) throw new ArgumentNullException("function");

         return new Vector(function(X), function(Y));
      }

      #region IEquatable<Vec> Members

      public bool Equals(Vector other)
      {
         return X.Equals(other.X) && Y.Equals(other.Y);
      }

      #endregion

      public IEnumerable<Vector> GetLineTo(Vector Vector)
      {
         var x0 = this.X;
         var y0 = this.Y;
         var x1 = Vector.X;
         var y1 = Vector.Y;

         var dx = Math.Abs(x0 - x1);
         var dy = Math.Abs(y0 - y1);
         var sx = x0 < x1 ? 1 : -1;
         var sy = y0 < y1 ? 1 : -1;
         var err = dx - dy;

         while (true)
         {
            yield return new Vector(x0, y0);

            if (x0 == x1 && y0 == y1) break;
            var e2 = 2 * err;
            if (e2 > -dy)
            {
               err = err - dy;
               x0 = x0 + sx;
            }
            if (x0 == x1 && y0 == y1)
            {
               yield return new Vector(x0, y0);
               break;
            }
            if (e2 < dx)
            {
               err = err + dx;
               y0 = y0 + sy;
            }
         }
      }

      public static Vector FromDirection(Direction direction)
      {
         switch (direction)
         {
            case Direction.East:
               return new Vector(1, 0);
            case Direction.Southeast:
               return new Vector(1, 1);
            case Direction.South:
               return new Vector(0, 1);
            case Direction.Southwest:
               return new Vector(-1, 1);
            case Direction.West:
               return new Vector(-1, 0);
            case Direction.Northwest:
               return new Vector(-1, -1);
            case Direction.North:
               return new Vector(0, -1);
            case Direction.Northeast:
               return new Vector(1, -1);
            default:
               return Vector.Zero;
         }
      }

      public static Vector? TryFromDirection(Direction direction)
      {
         switch (direction)
         {
            case Direction.East:
               return new Vector(1, 0);
            case Direction.Southeast:
               return new Vector(1, -1);
            case Direction.South:
               return new Vector(0, -1);
            case Direction.Southwest:
               return new Vector(-1, -1);
            case Direction.West:
               return new Vector(-1, 0);
            case Direction.Northwest:
               return new Vector(-1, 1);
            case Direction.North:
               return new Vector(0, 1);
            case Direction.Northeast:
               return new Vector(1, 1);
            default:
               return null;
         }
      }


      public IEnumerable<Vector> PathFindAttempts()
      {
         if(this == Vector.Zero)
         {
            return Enumerable.Empty<Vector>();
         }

         var absX = Abs(X);
         var absY = Abs(Y);

         Vector main;
         List<Vector> secondaries = new List<Vector>();
         List<Vector> tertiaries = new List<Vector>();

         if (absX > absY)
         {
            main = new Vector(Sign(X), 0);
            if (absY > 0)
            {
               secondaries.Add(new Vector(Sign(X), Sign(Y)));
               tertiaries.Add(new Vector(0, Sign(Y)));
            }
            else
            {
               secondaries.Add(new Vector(Sign(X), 1));
               secondaries.Add(new Vector(Sign(X), -1));
               tertiaries.Add(new Vector(0, 1));
               tertiaries.Add(new Vector(0, -1));
            }
         }
         else if (absY > absX)
         {
            main = new Vector(0, Sign(Y));
            if (absX > 0)
            {
               secondaries.Add(new Vector(Sign(X), Sign(Y)));
               tertiaries.Add(new Vector(Sign(X), 0));
            }
            else
            {
               secondaries.Add(new Vector(1, Sign(Y)));
               secondaries.Add(new Vector(-1, Sign(Y)));
               tertiaries.Add(new Vector(1, 0));
               tertiaries.Add(new Vector(-1, 0));
            }
         }
         else
         {
            main = new Vector(Sign(X), Sign(Y));
            secondaries.Add(new Vector(Sign(X), 0));
            secondaries.Add(new Vector(0, Sign(Y)));
         }

         var r = RogueGame.Current.GameRandom;

         return EnumerableEx.Return(main).Concat(secondaries.OrderBy(_ => r.Next())).Concat(tertiaries.OrderBy(_ => r.Next()));
      }
   }
}
