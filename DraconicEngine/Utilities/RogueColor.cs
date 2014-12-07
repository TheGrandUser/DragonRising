using System;
using System.Collections.Generic;
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
    }
}
