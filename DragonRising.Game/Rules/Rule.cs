using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Practices.Prism.PubSubEvents;
using DraconicEngine.RulesSystem;

namespace DragonRising.Rules
{
   public static class Rule
   {
   }
   
   public abstract class Rule<TFact> : IRule<TFact>
      where TFact : Fact
   {
      protected virtual bool Filter(TFact fact) => true;

      public bool Filter(Fact fact) => Filter((TFact)fact);
      public virtual bool UseFilter => false;

      public RuleResult Do(Fact fact)
      {
         var @event = (TFact)fact;

         return Do(@event);
      }

      public abstract RuleResult Do(TFact gameEvent);

      public virtual int Priority => 100;

      public Type FactType => typeof(TFact);
   }
}
