using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Rules
{
   public class RulesManager : IRulesManager
   {
      MultiValueDictionary<Type, IRule> rules = MultiValueDictionary<Type, IRule>.Create(
         () => new SortedSet<IRule>(Comparer<IRule>.Create((x, y) => x.Priority - y.Priority)));

      public void ProcessFacts(IEnumerable<Fact> actions)
      {
         Queue<Fact> factsToProcess = new Queue<Fact>(actions);
         while (factsToProcess.Count > 0)
         {
            var fact = factsToProcess.Dequeue();

            if (rules.ContainsKey(fact.GetType()))
            {
               var rules = this.rules[fact.GetType()];

               foreach (var rule in rules.Where(r => !r.UseFilter || r.Filter(fact)))
               {
                  var result = rule.Do(fact);

                  foreach (var newFact in result.Facts)
                  {
                     factsToProcess.Enqueue(newFact);
                  }
                  if (result.Interupt)
                  {
                     break;
                  }
               }
            }
         }
      }

      public void AddRule<TFact>(IRule<TFact> rule)
         where TFact : Fact
      {
         rules.Add(typeof(TFact), rule);
      }
   }

   public interface IRulesManager
   {
      void AddRule<TFact>(IRule<TFact> rule) where TFact : Fact;

      void ProcessFacts(IEnumerable<Fact> gameEvent);
   }
}