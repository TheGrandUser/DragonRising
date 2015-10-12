using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.Views;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using IFromLocationTargeter = DraconicEngine.RulesSystem.IFromLocationTargeter<DragonRising.Scene>;
using IFromLocationQuery = DraconicEngine.RulesSystem.IFromLocationQuery<DragonRising.Scene>;
using TargetResult = DraconicEngine.RulesSystem.TargetResult<DragonRising.Scene>;
using LocationTargetResult = DraconicEngine.RulesSystem.LocationTargetResult<DragonRising.Scene>;
using ILocationEffect = DraconicEngine.RulesSystem.ILocationEffect<DragonRising.Scene>;
using IToLocationTargeter = DraconicEngine.RulesSystem.IToLocationTargeter<DragonRising.Scene>;

namespace DragonRising.Plans.Targeters
{
   public class LocationInRangeTargeter : IFromLocationTargeter, IToLocationTargeter
   {
      public ImmutableList<IFromLocationTargeter> Targeters { get; }
      public ImmutableList<IFromLocationQuery> Queries { get; }
      public ImmutableList<ILocationEffect> Effects { get; }

      public LocationInRangeTargeter(
         SelectionRange range,
         IEnumerable<IFromLocationTargeter> targeters,
         IEnumerable<IFromLocationQuery> queries,
         IEnumerable<ILocationEffect> effects)
      {
         Range = range;
         this.Targeters = targeters.ToImmutableList();
         this.Queries = queries.ToImmutableList();
         this.Effects = effects.ToImmutableList();
      }

      public SelectionRange Range { get; }

      public static ILocationInRangeTargeterBuilder Build(SelectionRange range)
      {
         return new LocationInRangeTargeterBuilder(range);
      }

      public async Task<Option<TargetResult>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var area = Area.Combine(this.Queries.SelectMany(q => q.GetArea().AsEnumerable()));

         var location = await PlayerController.SelectTargetLocation(origin, "Select an adjacent creature", Range, sceneView, area);

         if (location.HasValue)
         {
            var newPath = path.Push(origin);
            var childResults = await Targeter.HandleChildTargetersAsync(
               this.Targeters,
               t => t.GetPlayerTargetingAsync(sceneView, location.Value, newPath));

            var result = childResults.Map(rs => (TargetResult)new LocationTargetResult(location.Value, this, rs));

            return result;
         }
         else
         {
            return None;
         }
      }

      class LocationInRangeTargeterBuilder : ILocationInRangeTargeterBuilder
      {
         SelectionRange range;
         List<IFromLocationTargeter> targeters = new List<IFromLocationTargeter>();
         List<IFromLocationQuery> queries = new List<IFromLocationQuery>();
         List<ILocationEffect> effects = new List<ILocationEffect>();

         public LocationInRangeTargeterBuilder(SelectionRange range)
         {
            this.range = range;
         }

         public ILocationInRangeTargeterBuilder Add(params IFromLocationTargeter[] targeters)
         {
            this.targeters.AddRange(targeters);

            return this;
         }

         public ILocationInRangeTargeterBuilder Add(params IFromLocationQuery[] queries)
         {
            this.queries.AddRange(queries);

            return this;
         }

         public ILocationInRangeTargeterBuilder Add(params ILocationEffect[] effects)
         {
            this.effects.AddRange(effects);

            return this;
         }

         public LocationInRangeTargeter Finish()
         {
            return new LocationInRangeTargeter(range,
               targeters,
               queries,
               effects);
         }
      }

   }

   public interface ILocationInRangeTargeterBuilder
   {
      ILocationInRangeTargeterBuilder Add(params IFromLocationTargeter[] targeters);
      ILocationInRangeTargeterBuilder Add(params IFromLocationQuery[] queries);
      ILocationInRangeTargeterBuilder Add(params ILocationEffect[] effects);

      LocationInRangeTargeter Finish();
   }

}
