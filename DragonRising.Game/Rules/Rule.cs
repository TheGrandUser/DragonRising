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
using DraconicEngine;

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

      public RuleResult Do(Fact fact, Scene scene) => Do((TFact)fact, scene);

      public abstract RuleResult Do(TFact gameEvent, Scene scene);

      public virtual int Priority => 100;

      public Type FactType => typeof(TFact);
   }
}
