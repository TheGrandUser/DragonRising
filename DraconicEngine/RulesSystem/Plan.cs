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
   public class FinalizedPlan<TContext>
   {
      private ImmutableList<TargetResult<TContext>> targetResults;
      private ImmutableList<IEntityEffect<TContext>> effects;
      private ImmutableList<IFromLocationQuery<TContext>> queries;

      public FinalizedPlan(
         IEnumerable<TargetResult<TContext>> targetResults,
         IEnumerable<IFromLocationQuery<TContext>> queries,
         IEnumerable<IEntityEffect<TContext>> effects)
      {
         this.queries = queries.ToImmutableList();
         this.effects = effects.ToImmutableList();
         this.targetResults = targetResults.ToImmutableList();
      }

      public ImmutableList<Fact> GetFacts(Entity user, TContext context)
      {
         var targetFacts = targetResults.SelectMany(tr => tr.GetFacts(user, context));
         var queryFacts = queries.SelectMany(query => query.GetFacts(user, user.Location, context));
         var effectFacts = effects.SelectMany(effect => effect.GetFacts(user, user, context));

         return targetFacts.Concat(effectFacts).Concat(queryFacts).ToImmutableList();
      }
   }
}
