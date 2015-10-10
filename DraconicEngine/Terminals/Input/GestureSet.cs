using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameViews;
using DraconicEngine;

namespace DraconicEngine.Input
{
   public class RogueKeyGesture
   {
      public RogueKey Key { get; private set; }
      public RogueModifierKeys Modifiers { get; private set; }

      public RogueKeyGesture(RogueKey key, RogueModifierKeys modifiers)
      {
         this.Key = key;
         this.Modifiers = modifiers;
      }

      public bool Matches(RogueKeyEvent rogueKeyEvent)
      {
         return this.Key == rogueKeyEvent.Key &&
            this.Modifiers == rogueKeyEvent.Modifiers;
      }
   }

   public class RogueMouseGesture
   {
      public RogueMouseAction MouseAction { get; }
      public RogueModifierKeys Modifiers { get; }

      public RogueMouseGesture(RogueMouseAction mouseAction, RogueModifierKeys modifiers)
      {
         this.MouseAction = mouseAction;
         this.Modifiers = modifiers;
      }

      public bool Matches(RogueMouseGesture gesture)
      {
         if (gesture == null)
         {
            return false;
         }

         return this.MouseAction == gesture.MouseAction && this.Modifiers == gesture.Modifiers;
      }
   }

   public enum RogueMouseAction
   {
      None = 0,
      LeftClick = 1,
      RightClick = 2,
      MiddleClick = 3,
      WheelClick = 4,
      LeftDoubleClick = 5,
      RightDoubleClick = 6,
      MiddleDoubleClick = 7,
      Movement = 8,
      WheelMove = 9,
   }

   public class GestureSet
   {
      public GestureSet(ImmutableList<RogueKeyGesture> keyGestures, RogueMouseGesture mouseGesture)
      {
         KeyGestures = keyGestures;
         MouseGesture = mouseGesture;
      }

      public ImmutableList<RogueKeyGesture> KeyGestures { get; set; }

      public RogueMouseGesture MouseGesture { get; set; }


      public static GestureSet Create(RogueKey key)
      {
         return new GestureSet(ImmutableList.Create(new RogueKeyGesture(key, RogueModifierKeys.None)), null);
      }

      public static GestureSet Create(RogueKey key, RogueModifierKeys modifiers)
      {
         return new GestureSet(ImmutableList.Create(new RogueKeyGesture(key, modifiers)), null);
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
