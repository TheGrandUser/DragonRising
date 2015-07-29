using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.Terminals.Input;
using System.Threading;

namespace DraconicEngine.Input
{
   public class InputResult
   {
      public RogueCommand Command { get; private set; }

      public InputResult(RogueCommand value) { Command = value; }

      public InputResult1D As1D() => this as InputResult1D;

      public InputResult2D As2D() => this as InputResult2D;
   }

   public class InputResult1D : InputResult
   {
      public int Delta { get; private set; }

      public InputResult1D(RogueCommand value, int delta) : base(value) { Delta = delta; }
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
      Task<InputResult> GetCommandAsync(IEnumerable<CommandGesture> gestures, CancellationToken cancelToken);

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