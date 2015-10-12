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
   class AdjacentLocationTargeter : IFromLocationTargeter<Scene>, IToLocationTargeter<Scene>
   {
      public ImmutableList<IFromLocationTargeter<Scene>> Targeters { get; }
      public ImmutableList<IFromLocationQuery<Scene>> Queries { get; }
      public ImmutableList<ILocationEffect<Scene>> Effects { get; }

      public AdjacentLocationTargeter(
         ImmutableList<ILocationEffect<Scene>> effects,
         ImmutableList<IFromLocationTargeter<Scene>> targeters,
         ImmutableList<IFromLocationQuery<Scene>> queries)
      {
         this.Targeters = targeters;
         this.Queries = queries;
         this.Effects = effects;
      }

      public async Task<Option<TargetResult<Scene>>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var range = new SelectionRange(1, RangeLimits.None);

         var area = Area.Combine(this.Queries.SelectMany(q => q.GetArea().AsEnumerable()));

         var location = await PlayerController.SelectTargetLocation(origin, "Select an adjacent creature", range, sceneView, area);

         if (location.HasValue)
         {
            var newPath = path.Push(origin);

            var childResults = await Targeter.HandleChildTargetersAsync(
               this.Targeters,
               t => t.GetPlayerTargetingAsync(sceneView, location.Value, newPath));

            var result = childResults.Map(rs => (TargetResult<Scene>)new LocationTargetResult<Scene>(location.Value, this, rs));

            return result;
         }
         else
         {
            return None;
         }
      }
   }
}
