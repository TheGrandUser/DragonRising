using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameViews;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.Effects;

namespace DraconicEngine.GameWorld.Actions
{
   public abstract class RogueAction
   {
      class NoOpAction : RogueAction
      {
         public NoOpAction() { }
      }

      static readonly RogueAction abort = new NoOpAction();
      static readonly RogueAction idle = new NoOpAction();
      public static RogueAction Abort => abort;
      public static RogueAction Idle => idle;
   }

   public interface IRule
   {

   }

   public interface IActionRule<TAction> : IRule
      where TAction : RogueAction
   {
      void Apply(TAction action);
   }
   public interface IActionRule<TAction, TEffect> : IRule
      where TAction : RogueAction
      where TEffect : IEffect
   {
      TEffect Apply(TAction action);
   }
}