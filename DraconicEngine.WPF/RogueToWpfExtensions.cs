using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DraconicEngine.Input;

namespace DraconicEngine.WPF
{
   public static class RogueToWpfExtensions
   {
      public static Key ToWpfKey(this RogueKey key)
      {
         return (Key)key;
      }

      public static RogueKey ToRogueKey(this Key key)
      {
         return (RogueKey)key;
      }

      public static RogueKeyEvent ToRogueKeyEvent(this KeyEventArgs args)
      {
         return new RogueKeyEvent()
         {
            Key = args.Key.ToRogueKey(),
            Modifiers = (RogueModifierKeys)Keyboard.Modifiers,
         };
      }

      public static Loc ToVec(this Loc point)
      {
         return new Loc((int)point.X, (int)point.Y);
      }
   }
}