using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
   /// <summary>
   /// Identifies a color that can be used in a terminal. An enumeration of
   /// fixed colors is used here to avoid making DraconicEngine.Core dependent on
   /// System.Drawing or some other assembly that provides a color type.
   /// </summary>
   public struct RogueColor : IEquatable<RogueColor>
   {
      byte r, g, b;

      public byte Red { get { return r; } }
      public byte Green { get { return g; } }
      public byte Blue { get { return b; } }

      public RogueColor(byte red, byte green, byte blue)
      {
         this.r = red;
         this.g = green;
         this.b = blue;
      }

      public RogueColor(int color)
      {
         r = (byte)((color & 0x00FF0000) >> 16);
         g = (byte)((color & 0x0000FF00) >> 8);
         b = (byte)(color & 0x000000FF);
      }

      public static bool operator ==(RogueColor c1, RogueColor c2)
      {
         return c1.r == c2.r &&
            c1.g == c2.g &&
            c1.b == c2.b;
      }

      public static bool operator !=(RogueColor c1, RogueColor c2)
      {
         return c1.r != c2.r ||
            c1.g != c2.g ||
            c1.b != c2.b;
      }

      public override int GetHashCode()
      {
         return r ^ g ^ b;
      }

      public override bool Equals(object obj)
      {
         if (obj is RogueColor)
         {
            var c2 = (RogueColor)obj;

            return this.r == c2.r &&
            this.g == c2.g &&
            this.b == c2.b;
         }

         return false;
      }

      public bool Equals(RogueColor other)
      {
         return this.r == other.r &&
            this.g == other.g &&
            this.b == other.b;
      }

      public override string ToString()
      {
         return r.ToString("x2") + g.ToString("x2") + b.ToString("x2");
      }

      public int ToInt32() => (r << 16) | (g << 8) | b;

      public static RogueColor? TryParse(string str)
      {
         int value;
         if (int.TryParse(str, out value))
         {
            return new RogueColor(value);
         }
         else if (int.TryParse(str, NumberStyles.HexNumber, null, out value))
         {
            return new RogueColor(value);
         }
         else if (str.Contains(','))
         {
            var parts = str.Split(',');
            if (parts.Length != 3)
            {
               return null;
            }
            byte r, g, b;
            if (!byte.TryParse(parts[0], out r) || !byte.TryParse(parts[0], NumberStyles.HexNumber, null, out r))
            {
               return null;
            }
            if (!byte.TryParse(parts[1], out g) || !byte.TryParse(parts[1], NumberStyles.HexNumber, null, out g))
            {
               return null;
            }
            if (!byte.TryParse(parts[1], out b) || !byte.TryParse(parts[1], NumberStyles.HexNumber, null, out b))
            {
               return null;
            }

            return new RogueColor(r, g, b);
         }

         return null;
      }
   }
}
