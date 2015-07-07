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
      public ImmutableList<RogueKeyGesture> KeyGestures { get; protected set; }
      public RogueMouseGesture MouseGesture { get; protected set; }

      protected CommandGesture(GestureSet gestureSet)
      {
         this.KeyGestures = gestureSet.KeyGestures;
         this.MouseGesture = gestureSet.MouseGesture;
      }

   }

   public static class CommandGestureFactory
   {
      public static CommandGesture1D Create1D<TValue>(TValue value, Func<RogueKeyGesture, int> keyValue, GestureSet gestureSet)
      {
         return new CommandGesture1D(PackageBuilder.Create1D(value, keyValue), gestureSet);
      }
      public static CommandGesture2D CreateMouseKey2D(RogueCommand command)
      {
         
         return new CommandGesture2D(PackageBuilder.Create2D(
            (loc, delta) => command,
            k => k.Key.IsEightWayMovementKey() ? k.Key.ToMovementVec() : Vector.Zero),
            GestureSet.CreateMouseMoveAnd8WayMove());
      }

      public static CommandGesture2D CreateMouseKey2D(Func<Loc?, Vector, RogueCommand> generateCommand)
      {
         return new CommandGesture2D(PackageBuilder.Create2D(
            generateCommand,
            k => k.Key.IsEightWayMovementKey() ? k.Key.ToMovementVec() : Vector.Zero),
            GestureSet.CreateMouseMoveAnd8WayMove());
      }

      public static CommandGesture2D CreateMouseKey2D<T>(T value)
      {
         var command = new ValueCommand<T>(value);
         return new CommandGesture2D(PackageBuilder.Create2D(
            (loc, delta) => command,
            k => k.Key.IsEightWayMovementKey() ? k.Key.ToMovementVec() : Vector.Zero),
            GestureSet.CreateMouseMoveAnd8WayMove());
      }

      public static CommandGesture2D CreateMousePointer(Func<Loc?, Vector, RogueCommand> mouseMoveFunc)
      {
         var packageMaker = PackageBuilder.Create2D(mouseMoveFunc, key => Vector.Zero);
         return new CommandGesture2D(packageMaker, GestureSet.Create(RogueMouseAction.Movement));
      }
      public static CommandGesture2D CreateMousePointer<TValue>(TValue value)
      {
         var packageMaker = PackageBuilder.Create2D(value);
         return new CommandGesture2D(packageMaker, GestureSet.Create(RogueMouseAction.Movement));
      }

      public static CommandGesture Create(RogueCommand value, GestureSet gestureSet)
      {
         return new CommandGestureSingle(PackageBuilder.Create(value), gestureSet);
      }

      public static CommandGesture CreateGesture<TValue>(TValue value, GestureSet gestureSet)
      {
         return new CommandGestureSingle(PackageBuilder.Create(new ValueCommand<TValue>(value)), gestureSet);
      }

      public static CommandGesture Create(RogueCommand value, RogueKey key)
      {
         return new CommandGestureSingle(PackageBuilder.Create(value), GestureSet.Create(key));
      }

      public static CommandGesture Create(Func<RogueCommand> value, GestureSet gestureSet)
      {
         return new CommandGestureSingle(PackageBuilder.Create(value), gestureSet);
      }

      public static CommandGesture Create(Func<RogueKeyGesture, RogueCommand> keyValue, GestureSet gestureSet)
      {
         return new CommandGestureSingle(PackageBuilder.Create(keyValue), gestureSet);
      }

      public static CommandGesture CreateCommand(Func<RogueKeyGesture, RogueCommand> actionSelector, GestureSet gestureSet)
      {
         return new CommandGestureSingle(PackageBuilder.Create(actionSelector), gestureSet);
      }
   }


   public class CommandGestureSingle : CommandGesture
   {
      PackageMaker packageMaker;

      internal CommandGestureSingle(PackageMaker packageMaker, GestureSet gestureSet)
         : base(gestureSet)
      {
         this.packageMaker = packageMaker;
      }

      public RogueCommand GetPackage(RogueKeyGesture keyGesture)
      {
         return this.packageMaker.GetPackage(keyGesture);
      }

      public RogueCommand GetPackage(RogueMouseGesture mouseGesture)
      {
         return this.packageMaker.GetPackage(mouseGesture);
      }
   }

   public class CommandGesture1D : CommandGesture
   {
      PackageMaker1D packageMaker;

      internal CommandGesture1D(PackageMaker1D packageMaker, GestureSet gestureSet)
         : base(gestureSet)
      {
         this.packageMaker = packageMaker;
      }

      public RogueCommand GetPackage(int delta)
      {
         return this.packageMaker.GetPackage(delta);
      }

      public RogueCommand GetPackage(RogueKeyGesture keyGesture)
      {
         return this.packageMaker.GetPackage(keyGesture);
      }

      public int GetValue(RogueKeyGesture keyGesture)
      {
         return this.packageMaker.GetValue1D(keyGesture);
      }
   }


   public class CommandGesture2D : CommandGesture
   {
      PackageMaker2D packageMaker;

      internal CommandGesture2D(PackageMaker2D packageMaker, GestureSet gestureSet)
         : base(gestureSet)
      {
         this.packageMaker = packageMaker;
      }

      public RogueCommand GetPackage(Loc? point, Vector delta)
      {
         return packageMaker.GetPackage(point, delta);
      }

      public Vector GetDirectionFromKeys(RogueKeyGesture keyGesture)
      {
         return this.packageMaker.GetValue2D(keyGesture);
      }
   }
}