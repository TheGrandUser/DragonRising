using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameStates;
using DraconicEngine;
using DraconicEngine.Terminals.Input;

namespace DraconicEngine.Input
{
   static class PackageBuilder
   {
      public static PackageMaker Create(RogueCommand value)
      {
         return new StaticPackage(value);
      }

      public static PackageMaker Create(Func<RogueCommand> value)
      {
         return new DynamicPackage(value, null, null);
      }

      public static PackageMaker Create(Func<RogueKeyGesture, RogueCommand> keyValue)
      {
         return new DynamicPackage(null, keyValue, null);
      }

      public static PackageMaker1D Create1D(RogueCommand package, Func<RogueKeyGesture, int> keyValue)
      {
         return new StaticPackage1D(package, keyValue);
      }

      public static PackageMaker1D Create1D<TValue>(TValue value, Func<RogueKeyGesture, int> keyValue)
      {
         return new StaticPackage1D(new ValueCommand<TValue>(value), keyValue);
      }

      public static PackageMaker1D Create1D(Func<RogueCommand> makePackage, Func<RogueKeyGesture, int> keyValue)
      {
         return new DynamicPackage1D(makePackage, null, null, keyValue);
      }

      public static PackageMaker2D Create2D(RogueCommand package)
      {
         return new StaticPackage2D(package);
      }

      public static PackageMaker2D Create2D<TValue>(TValue value)
      {
         return new StaticPackage2D(new ValueCommand<TValue>(value));
      }

      public static PackageMaker2D Create2D(
         Func<Loc?, Vector, RogueCommand> getCommandFunc,
         Func<RogueKeyGesture, Vector> directionFromKeyFunc)
      {
         return new DynamicPackage2D(getCommandFunc, directionFromKeyFunc);
      }
   }

   abstract class PackageMaker
   {
      public abstract RogueCommand GetPackage();
      public virtual RogueCommand GetPackage(RogueKeyGesture keyGesture) { return GetPackage(); }
      public virtual RogueCommand GetPackage(RogueMouseGesture mouseGesture) { return GetPackage(); }
   }

   abstract class PackageMaker1D
   {
      public abstract RogueCommand GetPackage(RogueKeyGesture keyGesture);
      public abstract RogueCommand GetPackage(int delta);

      public abstract int GetValue1D(RogueKeyGesture keyGesture);
   }

   abstract class PackageMaker2D
   {
      public abstract RogueCommand GetPackage(Loc? mouseMove, Vector delta);

      public abstract Vector GetValue2D(RogueKeyGesture keyGesture);
   }

   class StaticPackage : PackageMaker
   {
      RogueCommand command;
      public override RogueCommand GetPackage() { return command; }

      public StaticPackage(RogueCommand value)
      {
         this.command = value;
      }
   }

   class StaticPackage1D : PackageMaker1D
   {
      RogueCommand pacakge;
      private Func<RogueKeyGesture, int> keyValue;
      public StaticPackage1D(RogueCommand package, Func<RogueKeyGesture, int> keyValue)
      {
         this.pacakge = package;
         this.keyValue = keyValue;
      }

      public override int GetValue1D(RogueKeyGesture keyGesture)
      {
         return keyValue(keyGesture);
      }

      public override RogueCommand GetPackage(RogueKeyGesture keyGesture) { return pacakge; }

      public override RogueCommand GetPackage(int delta) { return pacakge; }
   }

   class StaticPackage2D : PackageMaker2D
   {
      RogueCommand value;

      public StaticPackage2D(RogueCommand value)
      {
         this.value = value;
      }

      public override Vector GetValue2D(RogueKeyGesture keyGesture)
      {
         return Vector.Zero;
      }

      public override RogueCommand GetPackage(Loc? mouseMove, Vector delta) { return value; }
   }

   class DynamicPackage : PackageMaker
   {
      Func<RogueCommand> func;
      Func<RogueKeyGesture, RogueCommand> keyFunc;
      Func<RogueMouseGesture, RogueCommand> mouseFunc;

      public DynamicPackage(
         Func<RogueCommand> value,
         Func<RogueKeyGesture, RogueCommand> keyFunc,
         Func<RogueMouseGesture, RogueCommand> mouseFunc)
      {
         if (func == null && keyFunc == null && mouseFunc == null)
         {
            throw new ArgumentNullException("func", "func, keyFunc, and mouseFunc can not all be null");
         }
         this.func = value;
         this.keyFunc = keyFunc;
         this.mouseFunc = mouseFunc;
      }

      public override RogueCommand GetPackage() { return func(); }

      public override RogueCommand GetPackage(RogueKeyGesture keyGesture)
      {
         return keyFunc != null ? keyFunc(keyGesture) : GetPackage();
      }

      public override RogueCommand GetPackage(RogueMouseGesture mouseGesture)
      {
         return mouseFunc != null ? mouseFunc(mouseGesture) : GetPackage();
      }
   }

   class DynamicPackage1D : PackageMaker1D
   {
      Func<RogueCommand> func;
      Func<int, RogueCommand> wheelFunc;
      Func<RogueKeyGesture, RogueCommand> keyFunc;
      Func<RogueKeyGesture, int> valueFunc;

      public DynamicPackage1D(
         Func<RogueCommand> value,
         Func<int, RogueCommand> wheelFunc,
         Func<RogueKeyGesture, RogueCommand> keyFunc,
         Func<RogueKeyGesture, int> valueFunc)
      {
         if (func == null && keyFunc == null && valueFunc == null)
         {
            throw new ArgumentNullException("func", "func, keyFunc, and mouseFunc can not all be null");
         }
         if (valueFunc == null)
         {
         }
         this.func = value;
         this.wheelFunc = wheelFunc;
         this.keyFunc = keyFunc;
         this.valueFunc = valueFunc;
      }

      public override RogueCommand GetPackage(RogueKeyGesture keyGesture)
      {
         return keyFunc != null ? keyFunc(keyGesture) : func();
      }

      public override int GetValue1D(RogueKeyGesture keyGesture) { return valueFunc(keyGesture); }

      public override RogueCommand GetPackage(int delta)
      {
         return wheelFunc != null ? wheelFunc(delta) : func();
      }
   }

   class DynamicPackage2D : PackageMaker2D
   {
      Func<Loc?, Vector, RogueCommand> moveFunc;
      Func<RogueKeyGesture, Vector> directionFromKeyFunc;

      public DynamicPackage2D(
         Func<Loc?, Vector, RogueCommand> getCommand,
         Func<RogueKeyGesture, Vector> directionFromKeyFunc)
      {
         if (getCommand == null)
         {
            throw new ArgumentNullException(nameof(getCommand));
         }

         if (directionFromKeyFunc == null)
         {
            throw new ArgumentNullException(nameof(directionFromKeyFunc));
         }
         this.moveFunc = getCommand;
         this.directionFromKeyFunc = directionFromKeyFunc;
      }

      public override RogueCommand GetPackage(Loc? value, Vector delta) => moveFunc(value, delta);

      public override Vector GetValue2D(RogueKeyGesture keyGesture) => directionFromKeyFunc(keyGesture);
   }
}