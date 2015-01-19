using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
   /// <summary>
   /// Static class containing helper functions for dealing with <see cref="RogueColors"/> values.
   /// </summary>
   public static class RogueColors
   {
      public static readonly RogueColor LightRed = new RogueColor(255, 160, 160);
      public static readonly RogueColor Red = new RogueColor(220, 0, 0);
      public static readonly RogueColor DarkRed = new RogueColor(100, 0, 0);

      // Flame

      public static readonly RogueColor LightOrange = new RogueColor(255, 200, 170);
      public static readonly RogueColor Orange = new RogueColor(255, 128, 0);
      public static readonly RogueColor DarkOrange = new RogueColor(128, 64, 0);

      // Amber

      public static readonly RogueColor LightYellow = new RogueColor(255, 255, 150);
      public static readonly RogueColor Yellow = new RogueColor(255, 255, 0);
      public static readonly RogueColor DarkYellow = new RogueColor(128, 128, 0);

      // Lime

      // Chartreuse

      public static readonly RogueColor LightGreen = new RogueColor(130, 255, 90);
      public static readonly RogueColor Green = new RogueColor(0, 200, 0);
      public static readonly RogueColor DarkGreen = new RogueColor(0, 100, 0);

      // Sea

      // Turquoise

      public static readonly RogueColor LightCyan = new RogueColor(200, 255, 255);
      public static readonly RogueColor Cyan = new RogueColor(0, 255, 255);
      public static readonly RogueColor DarkCyan = new RogueColor(0, 128, 128);

      // Sky

      // Azure

      public static readonly RogueColor LightBlue = new RogueColor(128, 160, 255);
      public static readonly RogueColor Blue = new RogueColor(0, 64, 255);
      public static readonly RogueColor DarkBlue = new RogueColor(0, 37, 168);

      // Han

      public static readonly RogueColor LightViolet = new RogueColor(159, 63, 255);
      public static readonly RogueColor Violet = new RogueColor(127, 0, 255);

      public static readonly RogueColor LightPurple = new RogueColor(200, 140, 255);
      public static readonly RogueColor Purple = new RogueColor(128, 0, 255);
      public static readonly RogueColor DarkPurple = new RogueColor(64, 0, 128);

      // Fuchsia

      // Magenta

      // Crimson

      // Metals
      public static readonly RogueColor LightGold = new RogueColor(255, 230, 150);
      public static readonly RogueColor Gold = new RogueColor(255, 192, 0);
      public static readonly RogueColor DarkGold = new RogueColor(128, 96, 0);

      // Misc
      public static readonly RogueColor Flesh = new RogueColor(255, 200, 170);
      public static readonly RogueColor Pink = new RogueColor(255, 160, 160);

      // Grey and Sepia
      public static readonly RogueColor LightGray = new RogueColor(192, 192, 192);
      public static readonly RogueColor Gray = new RogueColor(128, 128, 128);
      public static readonly RogueColor DarkGray = new RogueColor(48, 48, 48);

      public static readonly RogueColor LightBrown = new RogueColor(190, 150, 100);
      public static readonly RogueColor Brown = new RogueColor(160, 110, 60);
      public static readonly RogueColor DarkBrown = new RogueColor(100, 64, 32);

      public static readonly RogueColor Black = new RogueColor();
      public static readonly RogueColor White = new RogueColor(255, 255, 255);

      public static RogueColor FromName(string name)
      {
         switch (name)
         {
            case "Black": return RogueColors.Black;
            case "White": return RogueColors.White;

            case "LightGray": return RogueColors.LightGray;
            case "Gray": return RogueColors.Gray;
            case "DarkGray": return RogueColors.DarkGray;

            case "LightRed": return RogueColors.LightRed;
            case "Red": return RogueColors.Red;
            case "DarkRed": return RogueColors.DarkRed;

            case "Pink": return RogueColors.Pink;

            case "LightOrange": return RogueColors.LightOrange;
            case "Orange": return RogueColors.Orange;
            case "DarkOrange": return RogueColors.DarkOrange;

            case "Flesh": return RogueColors.Flesh;

            case "LightGold": return RogueColors.LightGold;
            case "Gold": return RogueColors.Gold;
            case "DarkGold": return RogueColors.DarkGold;

            case "LightYellow": return RogueColors.LightYellow;
            case "Yellow": return RogueColors.Yellow;
            case "DarkYellow": return RogueColors.DarkYellow;

            case "LightGreen": return RogueColors.LightGreen;
            case "Green": return RogueColors.Green;
            case "DarkGreen": return RogueColors.DarkGreen;

            case "LightCyan": return RogueColors.LightCyan;
            case "Cyan": return RogueColors.Cyan;
            case "DarkCyan": return RogueColors.DarkCyan;

            case "LightBlue": return RogueColors.LightBlue;
            case "Blue": return RogueColors.Blue;
            case "DarkBlue": return RogueColors.DarkBlue;

            case "LightPurple": return RogueColors.LightPurple;
            case "Purple": return RogueColors.Purple;
            case "DarkPurple": return RogueColors.DarkPurple;

            case "LightBrown": return RogueColors.LightBrown;
            case "Brown": return RogueColors.Brown;
            case "DarkBrown": return RogueColors.DarkBrown;

            default: throw new ArgumentException("Unexpected name: " + name);
         }
      }

      public static RogueColor FromEscapeChar(char c)
      {
         switch (c)
         {
            case 'k': return RogueColors.DarkGray;
            case 'K': return RogueColors.Black;

            case 'm': return RogueColors.Gray; // "m"edium

            case 'w': return RogueColors.White;
            case 'W': return RogueColors.LightGray;

            case 'r': return RogueColors.Red;
            case 'R': return RogueColors.DarkRed;

            case 'o': return RogueColors.Orange;
            case 'O': return RogueColors.DarkOrange;

            case 'l': return RogueColors.Gold;
            case 'L': return RogueColors.DarkGold;

            case 'y': return RogueColors.Yellow;
            case 'Y': return RogueColors.DarkYellow;

            case 'g': return RogueColors.Green;
            case 'G': return RogueColors.DarkGreen;

            case 'c': return RogueColors.Cyan;
            case 'C': return RogueColors.DarkCyan;

            case 'b': return RogueColors.Blue;
            case 'B': return RogueColors.DarkBlue;

            case 'p': return RogueColors.Purple;
            case 'P': return RogueColors.DarkPurple;

            case 'f': return RogueColors.Flesh;
            case 'F': return RogueColors.Brown;

            default: return RogueColors.White;
         }
      }
   }
}
