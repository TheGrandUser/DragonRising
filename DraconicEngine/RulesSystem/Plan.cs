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
   public interface IPlan
   {
      IEnumerable<ITargeter> Targeters { get; }
      IEnumerable<IQuery> Queries { get; }
      IEnumerable<IEffect> Effects { get; }
   }

   public class FinalizedPlan
   {
      private ImmutableDictionary<ITargeter, TargetResult> targetResults;

      private ImmutableList<IEffect> effects;
      private ImmutableList<ILocationBasedQuery> queries;
      
      public FinalizedPlan(IEnumerable<TargetResult> targetResults, IEnumerable<ILocationBasedQuery> queries, IEnumerable<IEffect> effects)
      {
         this.queries = queries.ToImmutableList();
         this.effects = effects.ToImmutableList();
         this.targetResults = targetResults.ToImmutableDictionary(tr => tr.Targeter);
      }

      public ImmutableDictionary<ITargeter, TargetResult> TargetResults => targetResults;

      public ImmutableList<Fact> GetFacts(Entity user)
      {
         var effectFacts = this.effects.SelectMany(effect =>
            effect.Match(
               entity: e => e.GetFacts(user, user),
               loc: e => e.GetFacts(user, user.Location),
               area: e => e.GetFacts(user, new RectArea(user.Location, user.Location))));

         var queryFacts = queries.SelectMany(query => query.GetFacts(user, user.Location));


         return effectFacts.Concat(queryFacts).ToImmutableList();
      }
   }
}
