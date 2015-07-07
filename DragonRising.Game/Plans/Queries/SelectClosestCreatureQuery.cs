using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using DragonRising.Commands.Requirements;
using LanguageExt;
using System.Collections.Immutable;
using System.Diagnostics;
using DraconicEngine;
using static LanguageExt.Prelude;
using DragonRising.GameWorld;

namespace DragonRising.Plans.Queries
{
   class SelectClosestCreatureQuery : ILocationBasedQuery
   {
      int? maxRange;
      ImmutableList<ILocationBasedQuery> queries;
      ImmutableList<IEffect> effects;
      IEntityFilter entityFilter;

      public SelectClosestCreatureQuery(int? maxRange, IEffect effect, IEntityFilter entityFilter)
         : this(maxRange, entityFilter, Enumerable.Empty<ILocationBasedQuery>(), EnumerableEx.Return(effect))
      {
      }

      public SelectClosestCreatureQuery(int? maxRange, IEntityFilter entityFilter,
         IEnumerable<ILocationBasedQuery> queries, IEnumerable<IEffect> effects)
      {
         this.maxRange = maxRange;
         this.queries = queries.ToImmutableList();
         this.effects = effects.ToImmutableList();
         this.entityFilter = entityFilter;
      }

      public IEnumerable<IQuery> Queries => queries;

      public IEnumerable<IEffect> Effects => effects;
      
      public bool IsArea => false;

      public IEnumerable<Fact> GetFacts(Entity user, Loc loc)
      {
         var closestEntity = World.Current.Scene.ClosestEntity(user, maxRange, entityFilter);
         
         return closestEntity.Match(
            e =>
            {
               var effectFacts = effects.SelectMany(effect =>
                  effect.Match(
                     entity: ee => ee.GetFacts(user, e),
                     loc: le => le.GetFacts(user, e.Location),
                     area: ae => ae.GetFacts(user, new RectArea(e.Location, e.Location))));

               var queryFacts = queries.SelectMany(q => q.GetFacts(user, e.Location));

               return effectFacts.Concat(queryFacts);
            },
            () => ImmutableList<Fact>.Empty);
      }

      public Option<Area> GetArea() => None;

      public Option<Area> GetArea(Loc origin) => None;
   }
}
