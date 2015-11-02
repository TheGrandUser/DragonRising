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
using DragonRising.GameWorld.Components;
using DraconicEngine;
using DragonRising.GameWorld;
namespace DragonRising.Plans.Queries
{
   public class AffectAllInRangeQuery : IFromLocationQuery
   {
      int range;
      ImmutableList<IEntityEffect> effects;
      IEntityFilter entityFilter;

      public AffectAllInRangeQuery(int range, IEntityFilter entityFilter, params IEntityEffect[] effects)
      {
         if (effects.Length == 0)
         {
            throw new ArgumentException("nextPlans can not be empty");
         }
         this.range = range;
         this.entityFilter = entityFilter;
         this.effects = effects.ToImmutableList();
      }
      
      public Option<Area> GetArea() => new CirlceArea(range, Loc.Zero);
      public Option<Area> GetArea(Loc origin) => new CirlceArea(range, origin);

      public IEnumerable<Fact> GetFacts(Entity user, Loc loc, Scene scene)
      {
         var entities = World.Current.Scene.EntityStore.Entities.Where(e => Loc.IsDistanceWithin(e.GetLocation(), e.Location, range));

         if (this.entityFilter != null)
         {
            entities = entities.Where(e => entityFilter.Matches(user, e));
         }
         entities = entities.ToList();

         var effectFacts = effects.SelectMany(effect => entities.SelectMany(e => effect.GetFacts(user, e, scene)));

         //var queryFacts = queries.SelectMany(query => query.GetFacts(user, user.Location));
         
         return effectFacts;
      }
   }
}