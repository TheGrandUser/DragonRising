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
      MultiValueDictionary<Type, IRule<Scene>> rules = MultiValueDictionary<Type, IRule<Scene>>.Create(
         () => new SortedSet<IRule<Scene>>(Comparer<IRule<Scene>>.Create((x, y) => x.Priority - y.Priority)));

      public void ProcessFacts(IEnumerable<Fact> actions, Scene scene)
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
                  var result = rule.Do(fact, scene);

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

      public void AddRule<TFact>(IRule<TFact, Scene> rule)
         where TFact : Fact
      {
         rules.Add(typeof(TFact), rule);
      }
   }

   public interface IRulesManager
   {
      void AddRule<TFact>(IRule<TFact, Scene> rule) where TFact : Fact;

      void ProcessFacts(IEnumerable<Fact> gameEvent, Scene scene);
   }
}