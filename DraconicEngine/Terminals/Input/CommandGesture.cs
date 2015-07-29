using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameViews;
using DraconicEngine;
using DraconicEngine.RulesSystem;
using DraconicEngine.Terminals.Input;

namespace DraconicEngine.Input
{
   public abstract class CommandGesture
   {
      protected CommandGesture(GestureSet gestureSet) { GestureSet = gestureSet; }
      public GestureSet GestureSet { get; }
   }

   public class CommandGestureSingle : CommandGesture
   {
      public CommandGestureSingle(RogueCommand command, GestureSet gestureSet) : base(gestureSet) { Command = command; }
      public RogueCommand Command { get; }
   }

   public class CommandGesture1D : CommandGesture
   {
      Func<int, RogueCommand> getValue;
      Func<RogueKeyGesture, int> keyToDelta;

      internal CommandGesture1D(
         Func<int, RogueCommand> getValue,
         Func<RogueKeyGesture, int> keyToDelta,
         GestureSet gestureSet) : base(gestureSet)
      {
         if (getValue == null)
         {
            throw new ArgumentNullException(nameof(getValue));
         }
         this.getValue = getValue;
         this.keyToDelta = keyToDelta;
      }

      public int KeyToDelta(RogueKeyGesture keyGesture) => keyToDelta?.Invoke(keyGesture) ?? 0;
      public RogueCommand GetValue(int delta) => getValue(delta);
   }

   public class CommandGesture2D : CommandGesture
   {
      Func<Loc?, Vector, RogueCommand> getValue;
      Func<RogueKeyGesture, Vector> keyToDelta;

      internal CommandGesture2D(
         Func<Loc?, Vector, RogueCommand> getValue,
         Func<RogueKeyGesture, Vector> keyToDelta,
         GestureSet gestureSet) : base(gestureSet)
      {
         if (getValue == null)
         {
            throw new ArgumentNullException(nameof(getValue));
         }
         this.getValue = getValue;
         this.keyToDelta = keyToDelta;
      }

      public Vector KeyToDelta(RogueKeyGesture keyGesture) => keyToDelta?.Invoke(keyGesture) ?? Vector.Zero;
      public RogueCommand GetValue(Loc? value, Vector delta) => getValue(value, delta);
   }

   public static class CommandGestureFactory
   {
      public static CommandGesture CreateGesture<TValue>(TValue value, GestureSet gestureSet) => new CommandGestureSingle(new ValueCommand<TValue>(value), gestureSet);
      public static CommandGesture Create(RogueCommand value, RogueKey key, RogueModifierKeys modifiers = RogueModifierKeys.None) => new CommandGestureSingle(value, GestureSet.Create(key, modifiers));

      public static CommandGesture1D Create1D<TValue>(TValue value, Func<RogueKeyGesture, int> keyValue, GestureSet gestureSet)
      {
         return new CommandGesture1D(_ => new ValueCommand<TValue>(value), keyValue, gestureSet);
      }

      public static CommandGesture2D CreateEightWay(Func<Loc?, Vector, RogueCommand> commandMaker)
      {
         return new CommandGesture2D(
            commandMaker,
            k => k.Key.IsEightWayMovementKey() ? k.Key.ToMovementVec() : Vector.Zero,
            GestureSet.CreateMouseMoveAnd8WayMove());
      }
      public static CommandGesture2D CreateEightWay<T>(T value) => CreateEightWay((loc, delta) => new ValueCommand<T>(value));

      public static CommandGesture2D CreateMousePointer(Func<Loc?, Vector, RogueCommand> mouseMoveFunc)
      {
         return new CommandGesture2D(mouseMoveFunc, key => Vector.Zero, GestureSet.Create(RogueMouseAction.Movement));
      }
      public static CommandGesture2D CreateMousePointer<TValue>(TValue value) => CreateMousePointer(new ValueCommand<TValue>(value));
   }
}