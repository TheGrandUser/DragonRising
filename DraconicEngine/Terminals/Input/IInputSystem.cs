using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.Terminals.Input;
using DraconicEngine.Terminals.Input.Commands;

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
      public RogueMouseAction MouseAction { get; private set; }
      public RogueModifierKeys Modifiers { get; private set; }

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

   public class InputResult
   {
      public RogueCommand Command { get; private set; }

      public InputResult(RogueCommand value)
      {
         this.Command = value;
      }

      public InputResult1D As1D()
      {
         return this as InputResult1D;
      }

      public InputResult2D As2D()
      {
         return this as InputResult2D;
      }
   }

   public class InputResult1D : InputResult
   {
      public int Delta { get; private set; }

      public InputResult1D(RogueCommand value, int delta)
         : base(value)
      {
         this.Delta = delta;
      }
   }

   public class InputResult2D : InputResult
   {
      public Loc? Point { get; }
      public Vector Delta { get; }

      public InputResult2D(RogueCommand value, Loc? point, Vector delta)
         : base(value)
      {
         this.Point = point;
         this.Delta = delta;
      }
   }

   public interface IInputSystem
   {
      Task<InputResult> GetCommandAsync(IEnumerable<CommandGesture> gestures);

      Task<RogueKeyEvent> GetKeyPressAsync();
      bool IsKeyPressed(RogueKey key);
   }

   public static class InputSystem
   {
      static IInputSystem current;

      public static IInputSystem Current { get { return current; } }
      public static void SetCurrent(IInputSystem inputSystem)
      {
         InputSystem.current = inputSystem;
      }
   }
}