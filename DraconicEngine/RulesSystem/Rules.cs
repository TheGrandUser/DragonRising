using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.RulesSystem
{
   public interface IRule
   {
      Type FactType { get; }
      bool Filter(Fact gameEvent);
      bool UseFilter { get; }
      int Priority { get; }
      RuleResult Do(Fact gameEvent);
   }
   
   public interface IRule<TFact> : IRule
      where TFact : Fact
   {
      RuleResult Do(TFact gameEvent);
   }

   public class RuleResult
   {
      public RuleResult(ImmutableList<Fact> facts)
      {
         Facts = facts;
      }

      public RuleResult(ImmutableList<Fact> facts, bool interupt)
      {
         Facts = facts;
         Interupt = interupt;
      }

      public RuleResult(params Fact[] gameEvents)
      {
         Facts = gameEvents.ToImmutableList();
      }

      public ImmutableList<Fact> Facts { get; }

      public static RuleResult Empty { get; } = new RuleResult(ImmutableList<Fact>.Empty);
      public bool Interupt { get; } = false;

      public static implicit operator RuleResult(Fact facts)
      {
         return new RuleResult(facts);
      }
      
      public static implicit operator RuleResult(ImmutableList<Fact> facts)
      {
         return new RuleResult(facts);
      }
   }
}
