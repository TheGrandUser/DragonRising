using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameStates;
using DraconicEngine;

namespace DraconicEngine.Input
{
   public class GestureSet
   {
      public GestureSet(ImmutableList<RogueKeyGesture> keyGestures, RogueMouseGesture mouseGesture)
      {
         this.KeyGestures = keyGestures;
         this.MouseGesture = mouseGesture;
      }

      public ImmutableList<RogueKeyGesture> KeyGestures { get; set; }

      public RogueMouseGesture MouseGesture { get; set; }


      public static GestureSet Create(RogueKey key)
      {
         return new GestureSet(ImmutableList.Create(new RogueKeyGesture(key, RogueModifierKeys.None)), null);
      }

      public static GestureSet Create(RogueMouseAction mouseAction, params RogueKey[] keys)
      {
         ImmutableList<RogueKeyGesture> keyGestures = ImmutableList.CreateRange(keys.Select(k => new RogueKeyGesture(k, RogueModifierKeys.None)));

         return new GestureSet(
            keyGestures,
            new RogueMouseGesture(mouseAction, RogueModifierKeys.None));
      }

      public static GestureSet Create(RogueKey firstKey, params RogueKey[] keys)
      {
         return new GestureSet(ImmutableList.Create(new RogueKeyGesture(firstKey, RogueModifierKeys.None))
            .AddRange(keys.Select(k => new RogueKeyGesture(k, RogueModifierKeys.None))), null);
      }

      public static GestureSet CreateMouseMoveAnd8WayMove()
      {
         return Create(RogueMouseAction.Movement,
            RogueKey.Up, RogueKey.NumPad8,
            RogueKey.Down, RogueKey.NumPad2,
            RogueKey.Left, RogueKey.NumPad4,
            RogueKey.Right, RogueKey.NumPad6,
            RogueKey.NumPad1,
            RogueKey.NumPad3,
            RogueKey.NumPad7,
            RogueKey.NumPad9);
      }

      public static GestureSet Create8WayMove()
      {
         return Create(
            RogueKey.Up, RogueKey.NumPad8,
            RogueKey.Down, RogueKey.NumPad2,
            RogueKey.Left, RogueKey.NumPad4,
            RogueKey.Right, RogueKey.NumPad6,
            RogueKey.NumPad1,
            RogueKey.NumPad3,
            RogueKey.NumPad7,
            RogueKey.NumPad9);
      }

      public static GestureSet Create4WayMove()
      {
         return Create(RogueKey.Up, RogueKey.Down, RogueKey.Left, RogueKey.Right, RogueKey.NumPad2, RogueKey.NumPad4, RogueKey.NumPad6, RogueKey.NumPad8);
      }
   }
}
