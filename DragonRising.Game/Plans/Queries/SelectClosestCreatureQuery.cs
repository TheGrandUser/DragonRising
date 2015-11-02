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
   public class SelectClosestCreatureQuery : IFromLocationQuery
   {
      int? maxRange;
      ImmutableList<IFromLocationQuery> queries;
      ImmutableList<IEntityEffect> effects;
      IEntityFilter entityFilter;

      public SelectClosestCreatureQuery(int? maxRange, IEntityEffect effect, IEntityFilter entityFilter)
         : this(maxRange, entityFilter, Enumerable.Empty<IFromLocationQuery>(), EnumerableEx.Return(effect))
      {
      }

      public SelectClosestCreatureQuery(int? maxRange, IEntityFilter entityFilter,
         IEnumerable<IFromLocationQuery> queries, IEnumerable<IEntityEffect> effects)
      {
         this.maxRange = maxRange;
         this.queries = queries.ToImmutableList();
         this.effects = effects.ToImmutableList();
         this.entityFilter = entityFilter;
      }
      
      public bool IsArea => false;

      public IEnumerable<Fact> GetFacts(Entity user, Loc loc, Scene scene)
      {
         var closestEntity = World.Current.Scene.ClosestEntity(user, maxRange, entityFilter);
         
         return closestEntity.Match(
            e =>
            {
               var effectFacts = effects.SelectMany(effect => effect.GetFacts(user, e, scene));
               
               var queryFacts = queries.SelectMany(q => q.GetFacts(user, e.Location, scene));

               return effectFacts.Concat(queryFacts);
            },
            () => ImmutableList<Fact>.Empty);
      }

      public Option<Area> GetArea() => None;

      public Option<Area> GetArea(Loc origin) => None;
   }
}
