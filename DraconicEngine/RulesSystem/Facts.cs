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

   [Flags]
   public enum FactResults
   {
      Nothing,
      Succesful,
      SuperSucessful,
      Unsuccessful
   }
   public sealed class FactInterupted<TFact> : Fact
   {
      public FactInterupted(TFact fact, string reason)
      {
         Fact = fact;
         Reason = reason;
      }

      public TFact Fact { get; }
      public string Reason { get; }
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
