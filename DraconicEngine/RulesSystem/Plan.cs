using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using System.Collections.Immutable;
using DraconicEngine.EntitySystem;

namespace DraconicEngine.RulesSystem
{
   public class FinalizedPlan
   {
      private ImmutableList<TargetResult> targetResults;
      private ImmutableList<IEntityEffect> effects;
      private ImmutableList<IFromLocationQuery> queries;

      public FinalizedPlan(
         IEnumerable<TargetResult> targetResults,
         IEnumerable<IFromLocationQuery> queries,
         IEnumerable<IEntityEffect> effects)
      {
         this.queries = queries.ToImmutableList();
         this.effects = effects.ToImmutableList();
         this.targetResults = targetResults.ToImmutableList();
      }

      public ImmutableList<Fact> GetFacts(Entity user, Scene context)
      {
         var targetFacts = targetResults.SelectMany(tr => tr.GetFacts(user, context));
         var queryFacts = queries.SelectMany(query => query.GetFacts(user, user.Location, context));
         var effectFacts = effects.SelectMany(effect => effect.GetFacts(user, user, context));

         return targetFacts.Concat(effectFacts).Concat(queryFacts).ToImmutableList();
      }
   }
}
