using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine;
using static LanguageExt.Prelude;

namespace DragonRising.Plans.Targeters
{
   public class DirectionTargeter : IFromLocationTargeter<Scene>, IToDirectionTargeter<Scene>
   {
      DirectionLimit limits;
      public ImmutableList<IAreaFromDirectionQuery<Scene>> Queries { get; }
      public ImmutableList<IFromDirectionTargetter<Scene>> Targeters { get; }

      public DirectionTargeter(
         DirectionLimit limits,
         IEnumerable<IAreaFromDirectionQuery<Scene>> areaSelectors,
         IEnumerable<IFromDirectionTargetter<Scene>> targeters)
      {
         this.limits = limits;
         this.Queries = areaSelectors.ToImmutableList();
         this.Targeters = targeters.ToImmutableList();
      }
      
      public async Task<Option<TargetResult<Scene>>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var area = Area.Combine(this.Queries.SelectMany(q => q.GetArea().AsEnumerable()).ToImmutableList());

         var direction = await PlayerController.SelectDirection(origin, "Select a direction", limits);

         if (direction.HasValue)
         {
            var childResults = await Targeter.HandleChildTargetersAsync(
               this.Targeters,
               t => t.GetPlayerTargetingAsync(direction.Value, origin, path));

            var result = childResults.Map(rs => (TargetResult<Scene>)new DirectionTargetResult<Scene>(direction.Value, origin, this, rs));

            return result;
         }

         return None;
      }
   }
}