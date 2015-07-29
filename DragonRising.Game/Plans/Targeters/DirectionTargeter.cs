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
   public class DirectionTargeter : ILocationBasedTargeter
   {
      DirectionLimit limits;
      private readonly ImmutableArray<IDirectionBasedAreaSelector> queries;
      private readonly ImmutableArray<IDirectionBasedTargetter> targeters;

      public DirectionTargeter(
         DirectionLimit limits,
         IEnumerable<IDirectionBasedAreaSelector> areaSelectors,
         IEnumerable<IDirectionBasedTargetter> targeters)
      {
         this.limits = limits;
         this.queries = areaSelectors.ToImmutableArray();
         this.targeters = targeters.ToImmutableArray();
      }

      public IEnumerable<IQuery> Queries => queries;
      public IEnumerable<ITargeter> Targeters => targeters;
      public IEnumerable<IEffect> Effects => Enumerable.Empty<IEffect>();

      public async Task<Option<TargetResult>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var area = Area.Combine(this.queries.SelectMany(q => q.GetArea()).ToImmutableList());

         var direction = await PlayerController.SelectDirection(origin, "Select a direction", limits);

         if (direction.HasValue)
         {
            var childResults = await Targeter.HandleChildTargetersAsync(
               this.targeters,
               t => t.GetPlayerTargetingAsync(direction.Value, origin, path));

            var result = childResults.Match(
               Some: rs => Some<TargetResult>(new DirectionTargetResult(direction.Value, origin, this, rs)),
               None: () => None);

            return result;
         }

         return None;
      }
   }
}