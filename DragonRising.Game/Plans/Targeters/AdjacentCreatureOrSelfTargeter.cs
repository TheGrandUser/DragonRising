using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using System.Collections.Immutable;
using DragonRising.GameWorld.Components;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.Views;
using DraconicEngine;

namespace DragonRising.Plans.Targeters
{
   class AdjacentCreatureOrSelfTargeter : ILocationBasedTargeter
   {
      ImmutableList<IEffect> effects;
      ImmutableList<ILocationBasedTargeter> targeters;
      ImmutableList<ILocationBasedQuery> queries;

      public AdjacentCreatureOrSelfTargeter(
         ImmutableList<IEffect> effects,
         ImmutableList<ILocationBasedTargeter> targeters,
         ImmutableList<ILocationBasedQuery> queries)
      {
         this.effects = effects;
         this.targeters = targeters;
         this.queries = queries;
      }

      public IEnumerable<IQuery> Queries => queries;
      public IEnumerable<IEffect> Effects => effects;
      public IEnumerable<ITargeter> Targeters => targeters;

      public async Task<Option<TargetResult>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var range = new SelectionRange(1, RangeLimits.None);
         
         var area = Area.Combine(this.queries.SelectMany(q => q.GetArea().AsEnumerable()));

         var location = await PlayerController.SelectTargetLocation(origin, "Select an adjacent creature", range, sceneView, area);

         if (location.HasValue)
         {
            var newPath = path.Push(origin);

            var childResults = await Targeter.HandleChildTargetersAsync(
               this.targeters,
               t => t.GetPlayerTargetingAsync(sceneView, location.Value, newPath));

            var result = childResults.Match(
               Some: rs => Some<TargetResult>(new LocationTargetResult(location.Value, this, rs)),
               None: () => None);

            return result;
         }
         else
         {
            return None;
         }
      }
   }
}
