using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DraconicEngine.WPF
{
   public static class TermColorExtensions
   {
      public static Color ToSystemColor(this RogueColor color)
      {
         return Color.FromRgb(color.Red, color.Green, color.Blue);
         //switch (color)
         //{
         //   case TermColors.Black: return Colors.Black;
         //   case TermColors.White: return Colors.White;

         //   case TermColors.LightGray: return Color.FromRgb(192, 192, 192);
         //   case TermColors.Gray: return Color.FromRgb(128, 128, 128);
         //   case TermColors.DarkGray: return Color.FromRgb(48, 48, 48);

         //   case TermColors.LightRed:
         //   case TermColors.Pink:
         //      return Color.FromRgb(255, 160, 160);

         //   case TermColors.Red: return Color.FromRgb(220, 0, 0);
         //   case TermColors.DarkRed: return Color.FromRgb(100, 0, 0);

         //   case TermColors.LightOrange:
         //   case TermColors.Flesh:
         //      return Color.FromRgb(255, 200, 170);

         //   case TermColors.Orange: return Color.FromRgb(255, 128, 0);
         //   case TermColors.DarkOrange: return Color.FromRgb(128, 64, 0);

         //   case TermColors.LightGold: return Color.FromRgb(255, 230, 150);
         //   case TermColors.Gold: return Color.FromRgb(255, 192, 0);
         //   case TermColors.DarkGold: return Color.FromRgb(128, 96, 0);

         //   case TermColors.LightYellow: return Color.FromRgb(255, 255, 150);
         //   case TermColors.Yellow: return Color.FromRgb(255, 255, 0);
         //   case TermColors.DarkYellow: return Color.FromRgb(128, 128, 0);

         //   case TermColors.LightGreen: return Color.FromRgb(130, 255, 90);
         //   case TermColors.Green: return Color.FromRgb(0, 200, 0);
         //   case TermColors.DarkGreen: return Color.FromRgb(0, 100, 0);

         //   case TermColors.LightCyan: return Color.FromRgb(200, 255, 255);
         //   case TermColors.Cyan: return Color.FromRgb(0, 255, 255);
         //   case TermColors.DarkCyan: return Color.FromRgb(0, 128, 128);

         //   case TermColors.LightBlue: return Color.FromRgb(128, 160, 255);
         //   case TermColors.Blue: return Color.FromRgb(0, 64, 255);
         //   case TermColors.DarkBlue: return Color.FromRgb(0, 37, 168);

         //   case TermColors.LightPurple: return Color.FromRgb(200, 140, 255);
         //   case TermColors.Purple: return Color.FromRgb(128, 0, 255);
         //   case TermColors.DarkPurple: return Color.FromRgb(64, 0, 128);

         //   case TermColors.LightBrown: return Color.FromRgb(190, 150, 100);
         //   case TermColors.Brown: return Color.FromRgb(160, 110, 60);
         //   case TermColors.DarkBrown: return Color.FromRgb(100, 64, 32);

         //   //default: throw new UnexpectedEnumValueException(color);
         //   default: throw new ArgumentException("Unexpected enum value: " + color);
         //}
      }
   }

}
