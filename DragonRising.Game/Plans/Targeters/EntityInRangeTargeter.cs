using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine;
using DragonRising.Views;
using static LanguageExt.Prelude;
using DragonRising.GameWorld;

namespace DragonRising.Plans.Targeters
{
   public class EntityInRangeTargeter : ILocationBasedTargeter
   {
      private readonly ImmutableArray<ILocationBasedTargeter> targeters;
      private readonly ImmutableArray<ILocationBasedQuery> queries;
      private readonly ImmutableArray<IEffect> effects;

      public EntityInRangeTargeter(
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

      public async Task<Option<TargetResult>> GetPlayerTargetingAsync(Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var playingState = MyPlayingScreen.Current;

         var range = new SelectionRange(1, RangeLimits.None);

         var area = Area.Combine(this.queries.SelectMany(q => q.GetArea()));

         var location = await PlayerController.SelectTargetEntity(
            origin,range,
            c=> c != World.Current.Player, playingState.SceneView, area);
         
         return await location.Match(Some: async creature =>
         {
            var newPath = path.Push(origin);

            var childResults = await Targeter.HandleChildTargetersAsync(
               this.targeters,
               t => t.GetPlayerTargetingAsync(creature.Location, newPath));

            var result = childResults.Match(
               Some: rs => Some<TargetResult>(new EntityTargetResult(creature, this, rs)),
               None: () => None);

            return result;
         },
         None: () => Task.FromResult<Option<TargetResult>>(None));
      }
   }

}
