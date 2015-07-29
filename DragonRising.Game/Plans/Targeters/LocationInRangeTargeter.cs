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

namespace DragonRising.Plans.Targeters
{
   public class LocationInRangeTargeter : ILocationBasedTargeter
   {
      private readonly ImmutableArray<ILocationBasedTargeter> targeters;
      private readonly ImmutableArray<ILocationBasedQuery> queries;
      private readonly ImmutableArray<IEffect> effects;

      public LocationInRangeTargeter(
         SelectionRange range,
         IEnumerable<ILocationBasedTargeter> targeters,
         IEnumerable<ILocationBasedQuery> queries,
         IEnumerable<IEffect> effects)
      {
         Range = range;
         this.targeters = targeters.ToImmutableArray();
         this.queries = queries.ToImmutableArray();
         this.effects = effects.ToImmutableArray();
      }

      public IEnumerable<ITargeter> Targeters => targeters;
      public IEnumerable<IQuery> Queries => queries;
      public IEnumerable<IEffect> Effects => effects;

      public SelectionRange Range { get; }

      public static ILocationInRangeTargeterBuilder Build(SelectionRange range)
      {
         return new LocationInRangeTargeterBuilder(range);
      }

      public async Task<Option<TargetResult>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var area = Area.Combine(this.queries.SelectMany(q => q.GetArea()));

         var location = await PlayerController.SelectTargetLocation(origin, "Select an adjacent creature", Range, sceneView, area);

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

      class LocationInRangeTargeterBuilder : ILocationInRangeTargeterBuilder
      {
         SelectionRange range;
         List<ILocationBasedTargeter> targeters = new List<ILocationBasedTargeter>();
         List<ILocationBasedQuery> queries = new List<ILocationBasedQuery>();
         List<IEffect> effects = new List<IEffect>();

         public LocationInRangeTargeterBuilder(SelectionRange range)
         {
            this.range = range;
         }

         public ILocationInRangeTargeterBuilder Add(params ILocationBasedTargeter[] targeters)
         {
            this.targeters.AddRange(targeters);

            return this;
         }

         public ILocationInRangeTargeterBuilder Add(params ILocationBasedQuery[] queries)
         {
            this.queries.AddRange(queries);

            return this;
         }

         public ILocationInRangeTargeterBuilder Add(params IEffect[] effects)
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
      ILocationInRangeTargeterBuilder Add(params ILocationBasedTargeter[] targeters);
      ILocationInRangeTargeterBuilder Add(params ILocationBasedQuery[] queries);
      ILocationInRangeTargeterBuilder Add(params IEffect[] effects);

      LocationInRangeTargeter Finish();
   }

}
