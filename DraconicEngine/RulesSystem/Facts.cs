using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.RulesSystem
{
   public abstract class Fact
   {
      public static implicit operator ImmutableList<Fact>(Fact self)
      {
         return ImmutableList.Create(self);
      }
   }

   public sealed class FactInterupted<TFact> : Fact
   {
      public FactInterupted(TFact fact)
      {
         Fact = fact;
      }

      public TFact Fact { get; }
   }

   public abstract class ActionTaken : Fact
   {
      public ActionTaken()
      {

      }

      class NoOpAction : ActionTaken
      {
         public NoOpAction() { }
      }

      static readonly ActionTaken abort = new NoOpAction();
      static readonly ActionTaken idle = new NoOpAction();
      public static ActionTaken Abort => abort;
      public static ActionTaken Idle => idle;
   }
}
