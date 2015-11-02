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
   public class DirectionTargeter : IFromLocationTargeter, IToDirectionTargeter
   {
      DirectionLimit limits;
      public ImmutableList<IAreaFromDirectionQuery> Queries { get; }
      public ImmutableList<IFromDirectionTargetter> Targeters { get; }

      public DirectionTargeter(
         DirectionLimit limits,
         IEnumerable<IAreaFromDirectionQuery> areaSelectors,
         IEnumerable<IFromDirectionTargetter> targeters)
      {
         this.limits = limits;
         this.Queries = areaSelectors.ToImmutableList();
         this.Targeters = targeters.ToImmutableList();
      }
      
      public async Task<Option<TargetResult>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var area = Area.Combine(this.Queries.SelectMany(q => q.GetArea().AsEnumerable()).ToImmutableList());

         var direction = await PlayerController.SelectDirection(origin, "Select a direction", limits);

         if (direction.HasValue)
         {
            var childResults = await Targeter.HandleChildTargetersAsync(
               this.Targeters,
               t => t.GetPlayerTargetingAsync(direction.Value, origin, path));

            var result = childResults.Map(rs => (TargetResult)new DirectionTargetResult(direction.Value, origin, this, rs));

            return result;
         }

         return None;
      }
   }
}