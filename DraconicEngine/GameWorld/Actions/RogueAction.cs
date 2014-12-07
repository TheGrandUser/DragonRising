using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Prelude;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameStates;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DraconicEngine.GameWorld.Actions
{
   public abstract class RogueAction
   {
      public abstract void Do(Entity executer);
  
      class NoOpAction : RogueAction
      {
         public override void Do(Entity executer) { }
         public NoOpAction() { }
      }

      static readonly RogueAction abort = new NoOpAction();
      static readonly RogueAction idle = new NoOpAction();
      public static RogueAction Abort => abort;
      public static RogueAction Idle => idle;
   }

}