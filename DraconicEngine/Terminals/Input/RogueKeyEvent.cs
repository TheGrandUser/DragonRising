using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;

namespace DraconicEngine.Input
{
   public class RogueKeyEvent
   {
      public RogueKey Key { get; set; }
      public RogueModifierKeys Modifiers { get; set; }
      public bool IsShiftDown { get { return Modifiers.HasFlag(RogueModifierKeys.Shift); } }
      public bool IsControlDown { get { return Modifiers.HasFlag(RogueModifierKeys.Control); } }
      public bool IsAltDown { get { return Modifiers.HasFlag(RogueModifierKeys.Alt); } }
   }

   public static class RogueKeyExtensions
   {
      public static bool IsStandardMovementKey(this RogueKey key)
      {
         return key == RogueKey.Up || key == RogueKey.NumPad8 ||
            key == RogueKey.Down || key == RogueKey.NumPad2 ||
            key == RogueKey.Left || key == RogueKey.NumPad4 ||
            key == RogueKey.Right || key == RogueKey.NumPad6 ||
            key == RogueKey.NumPad1 ||
            key == RogueKey.NumPad3 ||
            key == RogueKey.NumPad7 ||
            key == RogueKey.NumPad9;
      }

      public static int ToCycle(this RogueKey key)
      {
         switch (key)
         {
            case RogueKey.Up:
            case RogueKey.Right:
            case RogueKey.NumPad8:
            case RogueKey.NumPad6:
               return 1;

            case RogueKey.Down:
            case RogueKey.Left:
            case RogueKey.NumPad2:
            case RogueKey.NumPad4:
               return -1;
         }
         return 0;
      }

      public static Vector ToMovementVec(this RogueKey key)
      {
         switch (key)
         {
            case RogueKey.Up:
            case RogueKey.NumPad8:
               return new Vector(0, -1);
            case RogueKey.Down:
            case RogueKey.NumPad2:
               return new Vector(0, 1);
            case RogueKey.Left:
            case RogueKey.NumPad4:
               return new Vector(-1, 0);
            case RogueKey.Right:
            case RogueKey.NumPad6:
               return new Vector(1, 0);
            case RogueKey.NumPad1:
               return new Vector(-1, 1);
            case RogueKey.NumPad3:
               return new Vector(1, 1);
            case RogueKey.NumPad7:
               return new Vector(-1, -1);
            case RogueKey.NumPad9:
               return new Vector(1, -1);
            default:
               return Vector.Zero;
         }
      }

      public static Direction ToDirection(this RogueKey key)
      {
         switch (key)
         {
            case RogueKey.Up:
            case RogueKey.NumPad8:
               return Direction.North;
            case RogueKey.Down:
            case RogueKey.NumPad2:
               return Direction.South;
            case RogueKey.Left:
            case RogueKey.NumPad4:
               return Direction.West;
            case RogueKey.Right:
            case RogueKey.NumPad6:
               return Direction.East;
            case RogueKey.NumPad1:
               return Direction.Southwest;
            case RogueKey.NumPad3:
               return Direction.Southeast;
            case RogueKey.NumPad7:
               return Direction.Northwest;
            case RogueKey.NumPad9:
               return Direction.Northeast;
            default:
               return Direction.None;
         }
      }
   }
}