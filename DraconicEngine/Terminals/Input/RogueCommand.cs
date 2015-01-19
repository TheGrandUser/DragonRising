using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.Actions.Requirements;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using System.Diagnostics.Contracts;

namespace DraconicEngine.Terminals.Input
{
   public abstract class RogueCommand
   {

   }

   public static class RogueCommands
   {
      static readonly NoOpCommand wait = new NoOpCommand();
      public static RogueCommand Wait { get { return wait; } }

      static readonly NoOpCommand noOp = new NoOpCommand();
      public static RogueCommand NoOp { get { return noOp; } }

      static readonly NoOpCommand quit = new NoOpCommand();
      public static RogueCommand Quit { get { return quit; } }

      class NoOpCommand : RogueCommand { }
   }

   public class ValueCommand<T> : RogueCommand
   {
      public T Value { get; }

      public ValueCommand(T value)
      {
         this.Value = value;
      }
   }

   public abstract class AsyncCommand : RogueCommand
   {
      public abstract Task Do();
   }

   public class DelegateCommand : RogueCommand
   {
      Action action;

      public DelegateCommand(Action action)
      {
         Contract.Requires(action != null);
         this.action = action;
      }

      public void Do() => action();
   }
}
